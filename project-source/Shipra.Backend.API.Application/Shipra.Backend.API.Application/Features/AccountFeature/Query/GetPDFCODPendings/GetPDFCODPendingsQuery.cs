using System.Text;
using iText.Html2pdf;
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
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.GetPDFCODPendings;
public class GetPDFCODPendingsQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? StoreId { get; set; }
  public string? CarrierIds { get; set; }
  public int? OrderTypeId { get; set; }
}
public class GetPDFCODPendingsQueryHandler : RequestHandlerBase<GetPDFCODPendingsQuery, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IOrderRepository _orderRepository;
  public GetPDFCODPendingsQueryHandler(IWebHostEnvironment webHostEnvironment, IOrderRepository
    orderRepository, IServiceProvider serviceProvider, ILogger<GetPDFCODPendingsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _orderRepository = orderRepository;

  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPDFCODPendingsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var clientId = _currentUser.ClientId!.Value.ToString();
      var result = await _orderRepository.GetAllCODPendings(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, clientId, request.StoreId, request.CarrierIds, request.OrderTypeId);


      var castedList = (IEnumerable<dynamic>)result.list;

      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"codpendings.html");

      //create directory for file upload
      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"Outcodpendings_{Guid.NewGuid()}");
      bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(outputFolder);
      ///test
      ///
      string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, new Dictionary<string, object>(), "");


      StringBuilder sb = new StringBuilder();
      StringBuilder sbItems = new();
      foreach (var data in castedList)
      {
        Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);

        var oitem = $@"<tr>
                          <td class='bl bb'>{Utils.GetValueFromDictionryByKey("orderNo", replacements)}</td>
                          <td class='bb'>{Utils.GetValueFromDictionryByKey("carrierTrackingNo", replacements)}</td>
                          <td  class='bb'>{Utils.GetValueFromDictionryByKey("storeName", replacements)}</td>
                          <td  class='bb'>{Utils.GetValueFromDictionryByKey("description", replacements)}</td>
                          <td class='br bb'>{Utils.GetValueFromDictionryByKey("amount", replacements)}</td>
                        
            </tr>";
        sbItems.Append(oitem);
      }

      string path = Path.Combine(outputFolder, "GeneratedCRR");
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
      string filePath = Path.Combine(outputFolder, $@"GeneratedBarcode\{Guid.NewGuid()}.png");
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{date}}", DateTime.Now.ToString("MM/dd/yyyy"));

      #region company logo
      var logoPath = ApplicationConstants.ShipraLogo;
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
      if (castedList.Count() > 0)
      {
        var obj = castedList.FirstOrDefault();
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{clientName}}", obj?.ClientName);
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
