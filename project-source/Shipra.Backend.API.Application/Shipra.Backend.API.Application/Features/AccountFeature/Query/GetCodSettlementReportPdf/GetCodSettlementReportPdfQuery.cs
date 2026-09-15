using System.Text;
using FluentValidation;
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
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Query.GetCodSettlementReportPdf;
public class GetCodSettlementReportPdfQuery : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; }
}
public class GetCodSettlementReportPdfQueryHandler : RequestHandlerBase<GetCodSettlementReportPdfQuery, ServiceResultDTO>
{
  private readonly IBarcodeGenerate _barcodeGenerate;
  private readonly IAccountRepository _accountRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public GetCodSettlementReportPdfQueryHandler(IBarcodeGenerate barcodeGenerate,IAccountRepository accountRepository, IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<GetCodSettlementReportPdfQueryHandler> logger) : base(serviceProvider, logger)
  {
    _barcodeGenerate = barcodeGenerate;
    _accountRepository = accountRepository;
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCodSettlementReportPdfQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var clientId = _currentUser.ClientId!.Value.ToString(); 
      var carrierPayment = await _accountRepository.GetCarrierPaymentSettlementById(new CarrierPaymentSettlementId(new Guid(request.CarrierPaymentSettlementId!)));


      dynamic result = await _accountRepository.GetShipmentsBySettlementId(request.CarrierPaymentSettlementId!, clientId);

      var castedList = (IEnumerable<dynamic>)result.list!;

      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"carrierPaymentSettlement.html");

      //create directory for file upload
      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"OutCPS_{Guid.NewGuid()}");
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
                    </tr>";
        sbItems.Append(oitem);
      }

      string path = Path.Combine(outputFolder, "GeneratedCPS");
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      } 
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{printDate}}", DateTime.Now.ToString());
      string barcodeBase64 = _barcodeGenerate.CreateBase64(carrierPayment?.PaymentRef!, 500, 100); 

      var carrierName = result.list![0].CarrierName.ToString();
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{carrierName}}", carrierName);
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportNo}}", carrierPayment?.PaymentRef!);
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportCreatedDate}}", carrierPayment?.CreatedOn?.ToString("dd-MM-yyyy"));

      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportNoBarcode}}", $"data:image/png;base64,{barcodeBase64}");

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
public class GetCodSettlementReportPdfQueryValidator : AbstractValidator<GetCodSettlementReportPdfQuery>
{
  public GetCodSettlementReportPdfQueryValidator()
  {
    RuleFor(x => x.CarrierPaymentSettlementId).NotNull().NotEmpty();
  }
}
