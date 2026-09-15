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

namespace Shipra.Backend.API.Application.Features.InventorySalesFeatures.Query.GetPDFInventorySalesSummary;
public class GetPDFInventorySalesSummaryQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? ProductStationIds { get; set; }
  public string? ProductSKUs { get; set; }
  public string? TrackingStatusID { get; set; }
  public string? saleChannelConfigIds { get; set; }
  public string? StoreIds { get; set; }
  public bool? IsFulfilled { get; set; }
  public bool? IsInTransit { get; set; }
  public string? RegionIds { get; set; }
}
public class GetPDFInventorySalesSummaryQueryHandler : RequestHandlerBase<GetPDFInventorySalesSummaryQuery, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IInventoryRepository _inventoryRepository;

  public GetPDFInventorySalesSummaryQueryHandler(IWebHostEnvironment webHostEnvironment, IInventoryRepository inventoryRepository, IServiceProvider serviceProvider, ILogger<GetPDFInventorySalesSummaryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _inventoryRepository = inventoryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPDFInventorySalesSummaryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var result = await _inventoryRepository.GetInventorySalesSummary(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request.ProductStationIds, request.ProductSKUs, request.TrackingStatusID, request.IsFulfilled, request.IsInTransit, request.RegionIds, request.saleChannelConfigIds, request.StoreIds!, _currentUser.ClientIdStr!);
      var castedList = (IEnumerable<dynamic>)result!.list;

      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"inventorysalessummary.html");

      //create directory for file upload
      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"inventorysalessummary_{Guid.NewGuid()}");
      bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(outputFolder);
      ///test
      ///
      string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, new Dictionary<string, object>(), "");


      StringBuilder sb = new StringBuilder();
      StringBuilder sbItems = new();
      int serialNo = 0;
      foreach (var data in castedList)
      {
        serialNo++;
        Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);

        var oitem = $@"<tr>
                          <td  class='bl bb'> {serialNo}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("storeName", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("stationName", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("fullFillmentStatus", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("productName", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("sKU", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("orderNo", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("quantity", replacements)}</td>
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
