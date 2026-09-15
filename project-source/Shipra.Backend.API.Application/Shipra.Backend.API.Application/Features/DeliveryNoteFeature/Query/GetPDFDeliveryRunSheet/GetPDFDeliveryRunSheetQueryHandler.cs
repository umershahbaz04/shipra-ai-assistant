using System.Text;
using iText.Html2pdf;
using iText.Kernel.Events;
using iText.Kernel.Pdf;
using iText.Layout;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Application.Services.Implementation;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetPDFRunSheet;
public class GetPDFDeliveryRunSheetQueryHandler : RequestHandlerBase<GetPDFDeliveryRunSheetQuery, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly IBarcodeGenerate _barcodeGenerate;
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public GetPDFDeliveryRunSheetQueryHandler(IConfigRepository configRepository, IBarcodeGenerate barcodeGenerate, IWebHostEnvironment webHostEnvironment, IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<GetPDFDeliveryRunSheetQueryHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _barcodeGenerate = barcodeGenerate;
    _webHostEnvironment = webHostEnvironment;
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPDFDeliveryRunSheetQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _deliveryNoteRepository.GetRunSheetInfoByDeliveryNoteId(request.DeliveryNoteId!, _currentUser.ClientIdStr!);
      decimal TotalCOD = 0;
      if (data.TotalCount > 0)
      {

        DirectoryHelper directoryHelper = new DirectoryHelper();
        string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
        string templatePath = Path.Combine(accessUploadsFolder, $"runsheet.html");

        //create directory for file upload
        string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"RunSheet_{Guid.NewGuid()}");
        directoryHelper.CheckDirectoryExistAndCreate(outputFolder);
        StringBuilder sb = new StringBuilder();
        StringBuilder sbItems = new StringBuilder();
        Dictionary<string, object> replacements = new Dictionary<string, object>();
        int serialNo = 0;
        foreach (var item in data.list)
        {
          TotalCOD += item.COD;
          serialNo++;
          replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(item);



          #region Order Barcode
          var base64 = _barcodeGenerate.CreateBase64(item.OrderNo, 100, 500);


          #endregion

          var oitem = $@"<tr>
                          <td  class='bl bb'> {serialNo}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("orderNo", replacements)} {Utils.GetValueFromDictionryByKey("trackingNo", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("saleChannelName", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("customer", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("address", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("description", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("remarks", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("paymentMethod", replacements)}</td>
                          <td  class='bl bb'>{Utils.GetValueFromDictionryByKey("cOD", replacements)}</td>
                        <td  class='bl br bb'><img style='width:100px; height:50px;' src='data:image/png;base64,{base64}'/></td>                   
            </tr>";
          sbItems.Append(oitem);

        }

        string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, replacements, "");

        #region company logo
        var logoPath = ApplicationConstants.ShipraLogo;
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", logoPath);
        if (data.TotalCount > 0)
        {
          var obj = data.list[0];
          modifiedHtmlContent = modifiedHtmlContent.Replace("{{clientName}}", obj?.ClientName);
        }
        #endregion
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportDatas}}", sbItems.ToString());
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{date}}", DateTime.Now.ToString("dd-MM-yyyy"));
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{shipmentCount}}", serialNo.ToString());
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{totalCOD}}", TotalCOD.ToString());

        sb.Append(modifiedHtmlContent);
        if (!string.IsNullOrEmpty(sb.ToString()))
        {
          var uniqueFileName = Guid.NewGuid().ToString();

          string pdfFilePath = Path.Combine(outputFolder, $"{uniqueFileName}.pdf");

          PdfWriter pdfWriter = new PdfWriter(pdfFilePath);
          PdfDocument pdfDoc = new PdfDocument(pdfWriter);
          using (Document document = new Document(pdfDoc, iText.Kernel.Geom.PageSize.A4))
          {
            document.SetMargins(5, 5, 5, 5);
            ConverterProperties converterProperties = new ConverterProperties();
            var footerContent = $"Total COD: {TotalCOD}";
            var pdfEventHandler = new PdfEventHandler(footerContent);
            pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, pdfEventHandler);

            HtmlConverter.ConvertToPdf(sb.ToString(), pdfDoc, converterProperties);
          }
          byte[] bytes = PDFDocumentGenerator.ReadAllBytes(pdfFilePath);
          serviceResult = new ServiceResultDTO(bytes);

        }
        bool deleted = directoryHelper.DeleteDirectroy(outputFolder);
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  public static string ImageToBase64(string imagePath)
  {
    byte[] imageBytes = File.ReadAllBytes(imagePath);
    string base64String = Convert.ToBase64String(imageBytes);
    return base64String;
  }
}
