using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Services.Implementation;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.ExportDriverReceivables;

public class ExportDriverReceivablesQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public List<Guid>? DriverIds { get; set; }
}

public class ExportDriverReceivablesQueryHandler : RequestHandlerBase<ExportDriverReceivablesQuery, ServiceResultDTO>
{
  private readonly IDriverAccountRepository _driverAccount;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public ExportDriverReceivablesQueryHandler(IDriverAccountRepository driverAccount, IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<ExportDriverReceivablesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverAccount = driverAccount;
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExportDriverReceivablesQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var result = await _driverAccount.GetAllIDriverReceivables(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientIdStr!, request.DriverIds);
      
      var castedList = (IEnumerable<dynamic>)(result?.list ?? Enumerable.Empty<dynamic>());

      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"driverReceivableSummary.html");

      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"driverReceivableSum_{Guid.NewGuid()}");
      bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(outputFolder);

      string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, new Dictionary<string, object>(), "");

      StringBuilder sb = new StringBuilder();
      StringBuilder sbItems = new();
      decimal totalAmmount = 0;
      decimal totalExpense = 0;
      decimal totalCash = 0;

      string driverInfo = "All";
      // If multiple drivers selected, show 'Multiple' or just string join their names. If one, show name and mobile.
      // We will derive from the castedList.
      var uniqueDrivers = castedList.Select(x => new { Name = (string?)x.DriverName ?? "", Mobile = (string?)x.Mobile ?? "" }).Distinct().ToList();
      if (uniqueDrivers.Count == 1)
      {
         driverInfo = $"{uniqueDrivers[0].Name} ({uniqueDrivers[0].Mobile})";
      }
      else if (uniqueDrivers.Count > 1 && request.DriverIds != null && request.DriverIds.Any())
      {
         driverInfo = "Multiple";
      }

      foreach (var data in castedList)
      {
        if (data == null) continue;
        totalAmmount += (decimal)(data?.Total ?? 0);
        totalExpense += (decimal)(data?.Expense ?? 0);
        totalCash += (decimal)(data?.Cash ?? 0);

        var statusName = (int)(data?.DriverPaidStatusId ?? 0) == 1 ? "Paid" : "Unpaid";
        
        var oitem = $@"<tr>
                          <td class='bl bb'>{data?.DriverReceivableNo}</td>
                          <td class='bl bb'>{data?.DriverName}</td>
                          <td class='bl bb'>{statusName}</td>
                          <td class='bl bb'>{(data?.ReceiveDate != null ? ((DateTime)data.ReceiveDate).ToString("MM/dd/yyyy") : "")}</td> 
                          <td class='bl bb'>{data?.TotalShipment}</td>
                          <td class='bl bb'>{data?.Total}</td>
                          <td class='bl bb'>{data?.Expense}</td>
                          <td class='bl bb'>{data?.Cash}</td>
                          <td class='bl br bb'>{((DateTime)(data?.CreatedOn ?? DateTime.Now)).ToString("MM/dd/yyyy")}</td>                
            </tr>";
        sbItems.Append(oitem);
      }

      string path = Path.Combine(outputFolder, "GeneratedSum");
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }

      modifiedHtmlContent = modifiedHtmlContent.Replace("{{date}}", DateTime.Now.ToString("MM/dd/yyyy"));
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{driverInfo}}", driverInfo);
      
      var clientName = castedList.FirstOrDefault()?.ClientName ?? "";
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{clientName}}", clientName);
      
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportDatas}}", sbItems.ToString());
      
      sb.Append(modifiedHtmlContent);

      if (!string.IsNullOrEmpty(sb.ToString()))
      {
        var uniqueFileName = Guid.NewGuid().ToString();
        string pdfFilePath = Path.Combine(outputFolder, $"{uniqueFileName}.pdf");

        PdfWriter pdfWriter = new PdfWriter(pdfFilePath);
        PdfDocument pdfDoc = new PdfDocument(pdfWriter);
        using (Document document = new Document(pdfDoc, iText.Kernel.Geom.PageSize.A4))
        {
          document.SetMargins(10, 20, 20, 20);
          ConverterProperties converterProperties = new ConverterProperties();
          var footerContent = $"Total Amount: {totalAmmount} - Total Expense: {totalExpense} = Net Cash: {totalCash}";
          var pdfEventHandler = new PdfEventHandler(footerContent);
          pdfDoc.AddEventHandler(iText.Kernel.Events.PdfDocumentEvent.END_PAGE, pdfEventHandler);

          HtmlConverter.ConvertToPdf(sb.ToString(), pdfDoc, converterProperties);
        }
        byte[] bytes = PDFDocumentGenerator.ReadAllBytes(pdfFilePath);
        serviceResult = new ServiceResultDTO(bytes);
      }
      
      bool deleted = directoryHelper.DeleteDirectroy(outputFolder);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
