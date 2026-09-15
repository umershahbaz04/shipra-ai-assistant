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
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetReturnReportByReturnReportId;
public class GetReturnReportByReturnReportIdQuery : IRequest<ServiceResultDTO>
{
  public string? CarrierRrid { get; set; }
}
public class GetReturnReportByReturnReportIdQueryHandler : RequestHandlerBase<GetReturnReportByReturnReportIdQuery, ServiceResultDTO>
{
  private readonly IBarcodeGenerate _barcodeGenerate;
  private readonly ICarrierReturnReport _carrierReturnReport;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public GetReturnReportByReturnReportIdQueryHandler(IBarcodeGenerate barcodeGenerate,ICarrierReturnReport carrierReturnReport, IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<GetReturnReportByReturnReportIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _barcodeGenerate = barcodeGenerate;
    _carrierReturnReport = carrierReturnReport;
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetReturnReportByReturnReportIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _carrierReturnReport.GetShipmentsByReturnReportId(request.CarrierRrid!, _currentUser.ClientIdStr!);
      CarrierReturnReport returnReport = await _carrierReturnReport.GetCarrierReturnRerport(new CarrierRRId(new Guid(request.CarrierRrid!)));


      var castedList = (IEnumerable<dynamic>)result.list!;

      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"returnreport.html");

      //create directory for file upload
      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"Outreturnreport_{Guid.NewGuid()}");
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
                          <td>{Utils.GetValueFromDictionryByKey("orderNo", replacements)}</td>
                          <td>{Utils.GetValueFromDictionryByKey("carrierTrackingNo", replacements)}</td>
                          <td>{Utils.GetValueFromDictionryByKey("storeName", replacements)}</td>
                          <td>{Utils.GetValueFromDictionryByKey("description", replacements)}</td>
                          <td>{Utils.GetValueFromDictionryByKey("amount", replacements)}</td>
                          <td>Canceled</td>
            </tr>";
        sbItems.Append(oitem);
      }

      string path = Path.Combine(outputFolder, "GeneratedCRR");
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      } 
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{printDate}}", DateTime.Now.ToString());
      string base64Barcode = _barcodeGenerate.CreateBase64(returnReport.ReturnReportNo!, 500, 100);
 
      var carrierName = result.list![0].CarrierName.ToString();
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierName}}", carrierName);
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportNo}}", returnReport.ReturnReportNo!);
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportCreatedDate}}", returnReport.CreatedOn?.ToString("dd-MM-yyyy"));

      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportNoBarcode}}", $"data:image/png;base64,{base64Barcode}");

      #region company logo
      var logoPath = ApplicationConstants.ShipraLogo;
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
      #endregion
      //actual content
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{returnReports}}", sbItems.ToString());
      if (castedList.Count() > 0)
      {
        var obj = castedList.FirstOrDefault();
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{clientName}}", obj?.ClientName);
      }

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
