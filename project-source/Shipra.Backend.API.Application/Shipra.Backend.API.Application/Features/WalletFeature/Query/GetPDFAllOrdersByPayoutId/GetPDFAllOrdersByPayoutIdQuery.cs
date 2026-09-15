using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Query.GetPDFAllOrdersByPayoutId;
public class GetPDFAllOrdersByPayoutIdQuery : IRequest<ServiceResultDTO>
{
  public string? PayoutId { get; set; }
}
public class GetPDFAllOrdersByPayoutIdQueryHandler : RequestHandlerBase<GetPDFAllOrdersByPayoutIdQuery, ServiceResultDTO>
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IWalletRepository _walletRepository;

  public GetPDFAllOrdersByPayoutIdQueryHandler(IWebHostEnvironment webHostEnvironment, IWalletRepository walletRepository, IServiceProvider serviceProvider, ILogger<GetPDFAllOrdersByPayoutIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _webHostEnvironment = webHostEnvironment;
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPDFAllOrdersByPayoutIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var oResponseModel = await _walletRepository.GetAllOrdersByPayoutId(null, null, 0, 1000000, null, 0, "desc", _currentUser.ClientIdStr!, request.PayoutId);

      // Create directory for the file upload
      DirectoryHelper directoryHelper = new DirectoryHelper();
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, $"payoutOrderPdf.html");
      string outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"OutpayoutOrderPdf");
      bool isCreated = directoryHelper.CheckDirectoryExistAndCreate(outputFolder);
      var uniqueFileName = Guid.NewGuid().ToString();
      string pdfFilePath = Path.Combine(outputFolder, $"{uniqueFileName}.pdf");

      StringBuilder sbItems = new StringBuilder();
      if (oResponseModel.list != null)
      {
        decimal TotalCOD = 0;
        var response = oResponseModel.list;
        Dictionary<string, object> replacements = new Dictionary<string, object>();

        int serialNo = 0;
        foreach (dynamic item in response)
        {
          TotalCOD += 0;
          serialNo++;
          replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(item);

          var oitem = $@"<tr>
                            <td class='bl bb'> {serialNo}</td>
                            <td class='bl bb'>{Utils.GetValueFromDictionryByKey("orderNo", replacements)} </td> 
                            <td class='bl bb'>{Utils.GetValueFromDictionryByKey("customerName", replacements)}</td>
                            <td class='bl bb'>{Utils.GetValueFromDictionryByKey("mobile1", replacements)}</td>  
                            <td class='bl bb'>{Utils.GetValueFromDictionryByKey("remarks", replacements)}</td>
                            <td class='bl bb'>{Utils.GetValueFromDictionryByKey("createdOn", replacements)}</td>
                            <td class='bl br bb'>{Utils.GetValueFromDictionryByKey("amount", replacements)}</td>
                          </tr>";

          sbItems.Append(oitem);
        }
      }
      else
      {
        // Add a row indicating no records found
        sbItems.Append($@"<tr>
                            <td colspan='10' class='bl br bb text-center'>No records found</td>
                          </tr>");
      }

      // Generate PDF with header and table content
      Dictionary<string, object> emptyReplacements = new Dictionary<string, object>();
      string modifiedHtmlContent = PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, emptyReplacements, "");
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportDatas}}", sbItems.ToString());
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{date}}", DateTime.Now.ToString("dd-MM-yyyy"));
      var payout = await _walletRepository.GetPayoutById(request.PayoutId, _currentUser.ClientId!);
      if (payout is not null)
      {
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{totalAmount}}", payout.Amount.ToString()); 
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{serviceCharges}}", payout.ServiceCharges.GetValueOrDefault().ToString()); 
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{transactionCharges}}", payout.TransactionCharges.GetValueOrDefault().ToString());
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{netTotal}}", payout.Outstanding.GetValueOrDefault().ToString());
      }
      PDFDocumentGenerator.ConvertHtmlToPdf(modifiedHtmlContent, pdfFilePath);

      byte[] bytes = PDFDocumentGenerator.ReadAllBytes(pdfFilePath);
      serviceResult = new ServiceResultDTO(bytes);

      // Cleanup temporary files
      directoryHelper.DeleteDirectroy(outputFolder);

      return serviceResult;

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
