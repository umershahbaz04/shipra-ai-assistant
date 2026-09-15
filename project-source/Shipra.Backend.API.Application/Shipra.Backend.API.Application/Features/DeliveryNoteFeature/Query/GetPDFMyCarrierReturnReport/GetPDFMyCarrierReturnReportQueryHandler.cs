using System.Text;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetPDFMyCarrierReturnReport;
public class GetPDFMyCarrierReturnReportQueryHandler : RequestHandlerBase<GetPDFMyCarrierReturnReportQuery, ServiceResultDTO>
{
  private readonly ICarrierReturnReport _carrierReturnReport;
  private readonly IClientRepository _clientRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IBarcodeGenerate _barcodeGenerate;

  public GetPDFMyCarrierReturnReportQueryHandler(ICarrierReturnReport carrierReturnReport, IClientRepository clientRepository, IWebHostEnvironment webHostEnvironment, IBarcodeGenerate barcodeGenerate, IServiceProvider serviceProvider, ILogger<GetPDFMyCarrierReturnReportQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierReturnReport = carrierReturnReport;
    _clientRepository = clientRepository;
    _webHostEnvironment = webHostEnvironment;
    _barcodeGenerate = barcodeGenerate;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPDFMyCarrierReturnReportQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var oClient = await _clientRepository.GetClientById(_currentUser.ClientId!);
      var result = await _carrierReturnReport.GetAllMyCarrierReturnReport(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, oClient?.DefaultCarrierId!, _currentUser.ClientIdStr!);

      var castedList = (IEnumerable<dynamic>)result.list!;

      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"mycarrierreturnreport.html");

      //create directory for file upload
      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"Outreturnreport_{Guid.NewGuid()}");
      bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(outputFolder);
      ///test
      ///
      string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, new Dictionary<string, object>(), "");


      StringBuilder sb = new StringBuilder();
      StringBuilder sbItems = new();
      int srNo = 0;
      foreach (var data in castedList)
      {
        srNo++;
        Dictionary<string, object> replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(data);
        string base64Barcode = _barcodeGenerate.CreateBase64(Utils.GetValueFromDictionryByKey("returnReportNo!", replacements), 500, 100);
        var date = "";
        if (replacements.ContainsKey("createdOn"))
        {
          DateTime.TryParse(Utils.GetValueFromDictionryByKey("createdOn", replacements!), out DateTime changedDate);
          date = changedDate.ToString("dd-MM-yyyy");
        }
        var oitem = $@"<tr>
                          <td>{srNo}</td>
                          <td>{Utils.GetValueFromDictionryByKey("returnReportNo", replacements)}</td>
                          <td>{Utils.GetValueFromDictionryByKey("totalOrders", replacements)}</td>
                           <td>{date!}</td>
                          <td  class='bl br bb'><img style='width:100px; height:34px;' src='data:image/png;base64,{base64Barcode}'/></td>  
            </tr>";
        sbItems.Append(oitem);
      }

      string path = Path.Combine(outputFolder, "GeneratedCRR");
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportCreatedDate}}", DateTime.Now.ToString());
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
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{returnReports}}", sbItems.ToString());

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
      return serviceResult;
    }
  }
}
