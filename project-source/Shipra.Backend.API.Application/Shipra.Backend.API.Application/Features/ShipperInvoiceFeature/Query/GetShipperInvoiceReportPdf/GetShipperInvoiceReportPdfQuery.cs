using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers.Reporting;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;
using Stripe;

namespace Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperInvoiceReportPdf;
public class GetShipperInvoiceReportPdfQuery : IRequest<ServiceResultDTO>
{
  public int? ShipperInvoiceId { get; set; }
}
public class GetShipperInvoiceReportPdfQueryValidator : RequestHandlerBase<GetShipperInvoiceReportPdfQuery, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;
  private readonly IWebHostEnvironment _webHostEnvironment;

  public GetShipperInvoiceReportPdfQueryValidator(ISaleChannelConfigRepository saleChannelConfigRepository, IShipperInvoiceRepository shipperInvoiceRepository, IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider, ILogger<GetShipperInvoiceReportPdfQueryValidator> logger) : base(serviceProvider, logger)
  {
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _shipperInvoiceRepository = shipperInvoiceRepository;
    _webHostEnvironment = webHostEnvironment;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetShipperInvoiceReportPdfQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    DirectoryHelper directoryHelper = new DirectoryHelper();
    string outputFolder = string.Empty;

    try
    {
      // ✅ Safe current user reads
      var clientIdGuid = _currentUser?.ClientId?.Value;
      var clientIdStr = _currentUser?.ClientIdStr;

      if (clientIdGuid is null || string.IsNullOrWhiteSpace(clientIdStr))
      {
        serviceResult.CreateError("InvalidClient", new string[] { "ClientId missing from current user." });
        return serviceResult;
      }
      // ✅ Safe repo calls
      var allShipperInvoiceDetail =
          await _shipperInvoiceRepository.GetAllShipperInvoiceDetail(clientIdStr, request.ShipperInvoiceId)
          ?? new List<dynamic>(); // change type if needed

      var allShipperInvocieAdjustment =
          await _shipperInvoiceRepository.GetShipperInvoiceAdjustmentByShipperInvoiceId(request.ShipperInvoiceId, clientIdGuid.Value)
          ?? new List<ShipperInvoiceAdjustment>();

      // ✅ Paths
      string accessUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "PdfTemplates");
      string templatePath = Path.Combine(accessUploadsFolder, "shipperInvoice.html");

      if (!System.IO.File.Exists(templatePath))
      {
        serviceResult.CreateError("InvalidTemplate", new string[] { $"Template not found: {templatePath}" });
        return serviceResult;
      }
      outputFolder = Path.Combine(_webHostEnvironment.WebRootPath, $"OutSID_{Guid.NewGuid()}");
      directoryHelper.CheckDirectoryExistAndCreate(outputFolder);

      var uniqueFileName = Guid.NewGuid().ToString("N");
      string pdfFilePath = Path.Combine(outputFolder, $"{uniqueFileName}.pdf");

      // ✅ Build rows
      StringBuilder sbItems = new StringBuilder();
      StringBuilder sbAdjustmentItems = new StringBuilder();
      string? invoiceNo = string.Empty;
      string? createdOn = string.Empty;
  
      decimal totalCOD = 0m;

      int saleChannelConfigId = allShipperInvoiceDetail.FirstOrDefault()?.SaleChannelConfigId ?? 0;

      var employeeInfo = await _saleChannelConfigRepository.GetSaleChannelInforByConfigId(saleChannelConfigId, _currentUser!.ClientIdStr!);

      // ✅ Items table (use invoice detail list)
      if (allShipperInvoiceDetail.Any())
      {
        invoiceNo = allShipperInvoiceDetail.FirstOrDefault()?.InvoiceNo ?? "";

        //grandTotal = allShipperInvoiceDetail.FirstOrDefault()?.InvoiceTotal ?? 0;
        createdOn = allShipperInvoiceDetail.FirstOrDefault()?.CreatedOn.ToString("dd-MM-yyyy");

        int serialNo = 0;
        foreach (dynamic item in allShipperInvoiceDetail)
        {
          serialNo++;

          Dictionary<string, object> replacements;
          try
          {
            replacements = PDFDocumentGenerator.ConvertDynamicToDictionary(item)
                           ?? new Dictionary<string, object>();
          }
          catch
          {
            replacements = new Dictionary<string, object>();
          }

          var rateStr = Utils.GetValueFromDictionryByKey("rate", replacements) ?? "0";
          if (decimal.TryParse(rateStr, out var rate))
            totalCOD += rate;

          sbItems.Append($@"<tr>
                    <td class='bl bb'>{serialNo}</td>
                    <td class='bl bb'>{Utils.GetValueFromDictionryByKey("orderNo", replacements) ?? ""}</td>
                    <td class='bl bb'>{Utils.GetValueFromDictionryByKey("refNo", replacements) ?? ""}</td>
                    <td class='bl bb'>{Utils.GetValueFromDictionryByKey("customerName", replacements) ?? ""}</td>
                    <td class='bl bb'>{Utils.GetValueFromDictionryByKey("mobile1", replacements) ?? ""}</td>
                    <td class='bl bb'>{Utils.GetValueFromDictionryByKey("weight", replacements) ?? ""}</td>
                    <td class='bl br bb txtRight'>{rateStr}</td>
                </tr>");
                    //<td class='bl bb'>{Utils.GetValueFromDictionryByKey("carrierTrackingStatus", replacements) ?? ""}</td>
        }
      }
      else
      {
        sbItems.Append(@"<tr>
                <td colspan='10' class='bl br bb text-center'>No records found</td>
            </tr>");
      }

      // ✅ Adjustment table
      if (allShipperInvocieAdjustment.Any())
      {
        int serialNo = 0;
        foreach (var adj in allShipperInvocieAdjustment)
        {
          string trasType = Enum.IsDefined(typeof(EnumTransactionType), adj?.TransactionTypeId!)
                           ? Enum.GetName(typeof(EnumTransactionType), adj!.TransactionTypeId!)!
                           : "Unknown";
          serialNo++;
          sbAdjustmentItems.Append($@"<tr>
                    <td class='bl bb'>{serialNo}</td>
                    <td class='bl bb'>{trasType ?? ""}</td>
                    <td class='bl bb'>{(adj?.Amount ?? 0).ToString("0.##")}</td>
                    <td class='bl bb'>{adj?.Comment ?? ""}</td>
                </tr>");
        }
      }
      else
      {
        sbAdjustmentItems.Append(@"<tr>
                <td colspan='10' class='bl br bb text-center'>No records found</td>
            </tr>");
      }

      // ✅ Totals null-safe
      var adjustmentTableStyle = allShipperInvocieAdjustment.Count == 0 ? "d-none" : "";

      var totalDebit = allShipperInvocieAdjustment
          .Where(x => x.TransactionTypeId == (int)EnumTransactionType.Debit)
          .Sum(x => (decimal?)x.Amount) ?? 0m;

      var totalCredit = allShipperInvocieAdjustment
          .Where(x => x.TransactionTypeId == (int)EnumTransactionType.Credit)
          .Sum(x => (decimal?)x.Amount) ?? 0m;

      // ✅ Safe replace helper
      string Safe(string? v) => v ?? "";

      // ✅ Build HTML from template
      string modifiedHtmlContent =
          PDFDocumentGenerator.ReplacePlaceholdersInTemplate(templatePath, new Dictionary<string, object>(), "")
          ?? "";

      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportDatas}}", sbItems.ToString());
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{reportAdjustmentDatas}}", sbAdjustmentItems.ToString());
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{adjustmentTableStyle}}", Safe(adjustmentTableStyle));
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{contact}}", "");

      // These variables must exist in your method; keep them safe:
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{invoiceNo}}", Safe(invoiceNo?.ToString()));
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{date}}", DateTime.Now.ToString("dd-MM-yyyy"));
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{createdOn}}", Safe(createdOn));
      //modifiedHtmlContent = modifiedHtmlContent.Replace("{{companyLogo}}", Safe(logoUri));

      if (employeeInfo is not null)
      {
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerCode}}", employeeInfo.EmployeeCode);
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerName}}", employeeInfo.EmployeeName);
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerContact}}", employeeInfo.Mobile);
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerEmail}}", employeeInfo.Email);
        modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerAddress}}", employeeInfo.CountryName);
      }
      // ✅ ignoring clientInfo placeholders for now
      //modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerCode}}", "");
      //modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerName}}", "");
      //modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerContact}}", "");
      //modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerEmail}}", "");
      //modifiedHtmlContent = modifiedHtmlContent.Replace("{{customerAddress}}", "");
      decimal grandTotal =
                           totalCOD      // sum of all row rates (40)
                           + totalDebit  // 0
                           - totalCredit; // 0
      //modifiedHtmlContent = modifiedHtmlContent.Replace("{{contact}}", "+971 58 691 1359");
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{totalDebit}}", totalDebit.ToString("0.##"));
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{totalCredit}}", totalCredit.ToString("0.##"));
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{totalCOD}}", totalCOD.ToString("0.##"));
      modifiedHtmlContent = modifiedHtmlContent.Replace("{{grandTotal}}", grandTotal.ToString());

      // ✅ SAME AS YOUR WORKING REFERENCE
      PDFDocumentGenerator.ConvertHtmlToPdf(modifiedHtmlContent, pdfFilePath);

      var bytes = PDFDocumentGenerator.ReadAllBytes(pdfFilePath);
      return new ServiceResultDTO(bytes);
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
    finally
    {
      // ✅ Always cleanup even if exception happens
      if (!string.IsNullOrWhiteSpace(outputFolder))
      {
        try { directoryHelper.DeleteDirectroy(outputFolder); } catch { /* ignore cleanup errors */ }
      }
    }
  }
}
