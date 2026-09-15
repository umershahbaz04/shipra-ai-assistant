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

namespace Shipra.Backend.API.Application.Features.InventorySalesFeatures.Query.GetPDFInventorySales;
public class GetPDFInventorySalesQuery : IRequest<ServiceResultDTO>
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
public class GetPDFInventorySalesQueryHandler : RequestHandlerBase<GetPDFInventorySalesQuery, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IInventoryRepository _inventoryRepository;

  public GetPDFInventorySalesQueryHandler(IWebHostEnvironment webHostEnvironment, IInventoryRepository inventoryRepository
    , IServiceProvider serviceProvider, ILogger<GetPDFInventorySalesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _inventoryRepository = inventoryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPDFInventorySalesQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var result = await _inventoryRepository.GetAllInventorySales(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, request?.ProductStationIds, request?.ProductSKUs, request?.TrackingStatusID!, request?.IsFulfilled, request?.IsInTransit, request?.RegionIds, request!.saleChannelConfigIds, request.StoreIds!, _currentUser.ClientIdStr!);
      var castedList = (IEnumerable<dynamic>)result!.list;

      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"inventorysales.html");

      //create directory for file upload
      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"inventorysales_{Guid.NewGuid()}");
      bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(outputFolder);
      ///test
      ///
      string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, new Dictionary<string, object>(), "");


      StringBuilder sb = new StringBuilder();
      StringBuilder sbItems = new();
      int count = 0;
      foreach (var data in castedList)
      {
        count++;
        Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);

        var oitem = $@"<tr>
                          <td class='bl bb'>{count}</td>
                          <td  class='bb'>{Utils.GetValueFromDictionryByKey("orderNo", replacements)}</td>
                          <td  class='bb'>{Utils.GetValueFromDictionryByKey("stationName", replacements)}</td>
                          <td  class='bb  w-10'>{Utils.GetValueFromDictionryByKey("fullFillmentStatus", replacements)}</td>
                          <td  class='bb  w-5'>{Utils.GetValueFromDictionryByKey("quantity", replacements)}</td>
                          <td  class='bb  w-5'>{Utils.GetValueFromDictionryByKey("discount", replacements)}</td>
                          <td  class='br bb'>{Utils.GetValueFromDictionryByKey("price", replacements)}</td>  
            </tr>";
        sbItems.Append(oitem);
      }

      string path = Path.Combine(outputFolder, "GeneratedCRR");
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
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
