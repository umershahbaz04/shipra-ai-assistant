using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetAllCarrierPaymentSettlements;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderInvoiceByOrderNo;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.CreateShipperInvoice;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.CreateShipperInvoiceAdjustment;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.DeleteShipperInvoiceAdjustment;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.UpdateShipperInvoiceStatus;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.UpsertServiceRateGroup;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Commands.UpsertShipperRateWithSlabs;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.ExportShipperInvoiceDetailsToExcel;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllCarrierRateWithContractData;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllInvoiceStatus;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllServiceRateGroup;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllServiceRateGroupForSelection;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllShipperForGenerateInvocice;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllShipperInvoice;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllShipperInvoiceAdjustment;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetAllTransactionType;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetServiceRateGroupById;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperDraftOrderForInvoices;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperInvoiceAdjustmentById;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperInvoiceAdjustmentBySCId;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperInvoiceReportPdf;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperRateBySaleChannelConfig;
using Shipra.Backend.API.Application.Features.ShipperInvoiceFeature.Query.GetShipperRateBySaleChannelConfigGroupQuery;
using Shipra.Backend.API.Application.Features.WalletFeature.Query.ExcelExportAllTransaction;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class ShipperContractController : BaseApiController
{
  public ShipperContractController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("UpsertServiceRateGroup")]
  public async Task<ActionResult> UpsertServiceRateGroup([FromBody] UpsertServiceRateGroupCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpsertShipperRatesWithSlabs")]
  public async Task<ActionResult> UpsertShipperRatesWithSlabs([FromBody] UpsertShipperRatesWithSlabsCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("CreateInvoice")]
  public async Task<ActionResult> CreateInvoice([FromBody] CreateInvoiceCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateShipperInvoiceStatus")]
  public async Task<ActionResult> UpdateShipperInvoiceStatusCommand([FromBody] UpdateShipperInvoiceStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("AddUpdateShipperInvoiceAdjustment")]
  public async Task<ActionResult> AddUpdateShipperInvoiceAdjustment([FromBody] AddUpdateShipperInvoiceAdjustmentCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region Query
  [HttpPost("GetAllServiceRateGroup")]
  public async Task<ActionResult> GetAllServiceRateGroup([FromBody] GetAllServiceRateGroupQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllShipperRateWithContract")]
  public async Task<ActionResult> GetAllShipperRateWithContract([FromBody] GetAllCarrierRateWithContractDataQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetShipperRateBySaleChannelConfig")]
  public async Task<ActionResult> GetShipperRateBySaleChannelConfig([FromBody] GetShipperRateBySaleChannelConfigQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetShipperRateBySaleChannelConfigGroup")]
  public async Task<ActionResult> GetShipperRateBySaleChannelConfigGroup(CancellationToken cancellationToken = default)
  {
    var request = new GetShipperRateBySaleChannelConfigGroupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllShipperForGenerateInvoice")]
  public async Task<ActionResult> GetAllShipperForGenerateInvoice(CancellationToken cancellationToken = default)
  {
    var request = new GetShipperRateBySaleChannelConfigGroupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetShipperOrderForInvoices")]
  public async Task<ActionResult> GetShipperOrderForInvoices([FromBody] GetShipperOrderForInvoicesQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetShipperInvoiceAdjustmentById")]
  public async Task<ActionResult> GetShipperInvoiceAdjustmentById([FromBody] GetShipperInvoiceAdjustmentByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllShipperForGenerateInvocice")]
  public async Task<ActionResult> GetAllShipperForGenerateInvocice(CancellationToken cancellationToken = default)
  {
    GetAllShipperForGenerateInvociceQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetAllInvoiceStatus")]
  public async Task<ActionResult> GetAllInvoiceStatus(CancellationToken cancellationToken = default)
  {
    GetAllInvoiceStatusQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllTransactionType")]
  public async Task<ActionResult> GetAllTransactionType(CancellationToken cancellationToken = default)
  {
    GetAllTransactionTypeQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetAllServiceRateGroupForSelection")]
  public async Task<ActionResult> GetAllServiceRateGroupForSelection(CancellationToken cancellationToken = default)
  {
    GetAllServiceRateGroupForSelectionQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllShipperInvoice")]
  public async Task<ActionResult> GetAllShipperInvoice([FromBody]GetAllShipperInvoiceQuery request,CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllShipperInvoiceAdjustment")]
  public async Task<ActionResult> GetAllShipperInvoiceAdjustment([FromBody] GetAllShipperInvoiceAdjustmentQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("ExportShipperInvoiceDetailsToExcel")]
  public async Task<ActionResult> ExportShipperInvoiceDetailsToExcel([FromQuery] ExportShipperInvoiceDetailsToExcelQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File((byte[])response!.Result!.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("InvoiceDetails"));
  }

  [HttpPost("DeleteShipperInvoiceAdjustmentById")]
  public async Task<ActionResult> DeleteShipperInvoiceAdjustmentById([FromBody] DeleteShipperInvoiceAdjustmentCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpPost("GetShipperInvoiceAdjustmentBySCId")]
  public async Task<ActionResult> GetShipperInvoiceAdjustmentBySCId([FromBody] GetShipperInvoiceAdjustmentBySCIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetServiceRateGroupById")]
  public async Task<ActionResult> GetServiceRateGroupById([FromQuery] GetServiceRateGroupByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetShipperInvoiceReportPdf")]
  public async Task<ActionResult> GetShipperInvoiceReportPdf([FromBody] GetShipperInvoiceReportPdfQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("InvoicePdf");
    try
    {
      return File(result.Result!, PDFDocumentGenerator.ContentType, file);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
    return Ok(result);
  }
  #endregion

}
