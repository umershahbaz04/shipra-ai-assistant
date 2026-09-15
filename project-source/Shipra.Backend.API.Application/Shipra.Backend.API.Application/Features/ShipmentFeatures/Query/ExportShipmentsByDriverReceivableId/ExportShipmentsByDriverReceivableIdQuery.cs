using System;
using System.Collections.Generic;
using Shipra.Backend.API.Core.DriverAggregate;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using FluentValidation;
using iText.Html2pdf;
using iText.Kernel.Events;
using iText.Kernel.Pdf;
using iText.Layout;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetPDFCarrierReturnReportById;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Services.Implementation;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.ExportShipmentsByDriverReceivableId;
public class ExportShipmentsByDriverReceivableIdQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? DriverReceivableId { get; set; }
}
public class ExportShipmentsByDriverReceivableIdQueryHandler : RequestHandlerBase<ExportShipmentsByDriverReceivableIdQuery, ServiceResultDTO>
{
  private readonly IBarcodeGenerate _barcodeGenerate;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IShipmentRepository _shipmentRepository;
  private readonly IDriverAccountRepository _driverAccountRepository;

  public ExportShipmentsByDriverReceivableIdQueryHandler(IBarcodeGenerate barcodeGenerate,IWebHostEnvironment webHostEnvironment, IShipmentRepository shipmentRepository, IDriverAccountRepository driverAccountRepository, IServiceProvider serviceProvider, ILogger<ExportShipmentsByDriverReceivableIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _barcodeGenerate = barcodeGenerate;
    _webHostEnvironment = webHostEnvironment;
    _shipmentRepository = shipmentRepository;
    _driverAccountRepository = driverAccountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ExportShipmentsByDriverReceivableIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      var result = await _shipmentRepository.GetAllShipmentsByDriverReceivableId(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request.DriverReceivableId, _currentUser.ClientIdStr!);
 
      var castedList = (IEnumerable<dynamic>)result!.list;

      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"driverReceiveable.html");

      //create directory for file upload
      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"driverReceiveable_{Guid.NewGuid()}");
      bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(outputFolder);
      ///test
      ///
      string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, new Dictionary<string, object>(), "");


      StringBuilder sb = new StringBuilder();
      StringBuilder sbItems = new();
      int serialNo = 0;
      decimal totalAmmount = 0;
      foreach (var data in castedList)
      {
        serialNo++;
        totalAmmount += data?.Amount;  
        Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);

        var oitem = $@"<tr>
                          <td  class='bl bb'> {serialNo}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("orderNo", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("storeName", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("stationName", replacements)}</td> 
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("customerName", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("mobile1", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("trackingStatus", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("remarks", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("discount", replacements)}</td>
                          <td  class='bl br bb'>{Utils.GetValueFromDictionryByKey("amount", replacements)}</td>                
            </tr>";
        sbItems.Append(oitem);
      }


      string path = Path.Combine(outputFolder, "GeneratedCRR");
      if (!Directory.Exists(path))
      {
        {
          Directory.CreateDirectory(path);
        }
      }
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{date}}", DateTime.Now.ToString("MM/dd/yyyy"));
      #region company logo
      var logoPath = ApplicationConstants.ShipraLogo;
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
      if (castedList.Count() > 0)
      {
        var obj = castedList.FirstOrDefault();
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{clientName}}", obj?.ClientName);

        #region report no  
        var returnReportNo = obj?.DriverReceivableNo;
        if (!string.IsNullOrEmpty(returnReportNo))
        {
          string reportNoBase64 = _barcodeGenerate.CreateBase64(returnReportNo);
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{receiveableNoBarcode}}", $"data:image/png;base64,{reportNoBase64}");
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{receiveableNo}}", returnReportNo);
        }
        #endregion
      }
      #endregion
      //actual content
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
          var driverReceivable = await _driverAccountRepository.GetDriverReceivableById(new DriverReceivableId(Guid.Parse(request.DriverReceivableId!)));
          decimal expense = driverReceivable?.Expense ?? 0;
          decimal totalAmountSaved = driverReceivable?.Total ?? 0;
          decimal cashSaved = driverReceivable?.Cash ?? 0;
          var footerContent = $"Total Amount: {totalAmountSaved} - Expense: {expense} = Net Amount: {cashSaved}";
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
public class ExportShipmentsByDriverReceivableIdQueryValidator : AbstractValidator<ExportShipmentsByDriverReceivableIdQuery>
{
  public ExportShipmentsByDriverReceivableIdQueryValidator()
  {
    RuleFor(x => x.DriverReceivableId).NotEmpty().NotNull();
  }
}
