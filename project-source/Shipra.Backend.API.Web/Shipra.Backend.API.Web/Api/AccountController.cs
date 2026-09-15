using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.AccountFeature.Commands.CreateCarrierPaymentSettlement;
using Shipra.Backend.API.Application.Features.AccountFeature.Commands.CreateCpsettlementPopFile;
using Shipra.Backend.API.Application.Features.AccountFeature.Commands.DeleteCarrierPaymentSettlement;
using Shipra.Backend.API.Application.Features.AccountFeature.Commands.DeleteCpsettlementPopFile;
using Shipra.Backend.API.Application.Features.AccountFeature.Commands.MarkCarrierSettlementPaid;
using Shipra.Backend.API.Application.Features.AccountFeature.Commands.MarkCarrierSettlementUnPaid;
using Shipra.Backend.API.Application.Features.AccountFeature.Commands.UpdateAmountReceived;
using Shipra.Backend.API.Application.Features.AccountFeature.Commands.UplaodCarrierSettlementFile;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.ExcelExportCarrierSettlementById;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.ExcelExportCodPending;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetAllCarrierPaymentSettlements;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetAllCarrierWithCodPending;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetAllCODPendings;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetAllCpsettlementPopFiles;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetCarrierSettlementSamplefile;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetCodSettlementReportPdf;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetPDFCODPendings;
using Shipra.Backend.API.Application.Features.AccountFeature.Query.GetShipmentsBySettlementId;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;


[Authorize]
public class AccountController : BaseApiController
{
  public AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("UplaodCarrierSettlementFile")]
  public async Task<ActionResult> UplaodCarrierSettlementFile([FromForm] UplaodCarrierSettlementFileCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("CreateCarrierPaymentSettlement")]
  public async Task<ActionResult> CreateCarrierPaymentSettlement([FromBody] CreateCarrierPaymentSettlementCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteCarrierPaymentSettlement")]
  public async Task<ActionResult> DeleteCarrierPaymentSettlement([FromBody] DeleteCarrierPaymentSettlementCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateAmountReceived")]
  public async Task<ActionResult> UpdateAmountReceived([FromBody] UpdateAmountReceivedCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("MarkCarrierSettlementPaid")]
  public async Task<ActionResult> MarkCarrierSettlementPaid([FromBody] MarkCarrierSettlementPaidCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("MarkCarrierSettlementUnPaid")]
  public async Task<ActionResult> MarkCarrierSettlementUnPaid([FromBody] MarkCarrierSettlementUnPaidCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpPost("GetAllCarrierWithCodPending")]
  public async Task<ActionResult> GetAllCarrierWithCodPending([FromBody] GetAllCarrierWithCodPendingQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region query
  [HttpPost("GetAllCarrierPaymentSettlements")]
  public async Task<ActionResult> GetAllCarrierPaymentSettlements([FromBody] GetAllCarrierPaymentSettlementsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetCarrierSettlementSamplefile")]
  public async Task<ActionResult> GetCarrierSettlementSamplefile(CancellationToken cancellationToken = default)
  {
    GetCarrierSettlementSamplefileQuery request = new GetCarrierSettlementSamplefileQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetShipmentsBySettlementId")]
  public async Task<ActionResult> GetShipmentsBySettlementId([FromQuery] GetShipmentsBySettlementIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("ExcelExportCarrierSettlementById")]
  public async Task<ActionResult> ExcelExportCarrierSettlementById([FromQuery] ExcelExportCarrierSettlementByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("CarrierSettlement"));
  }
  [HttpGet("GetCodSettlementReportPdf")]
  public async Task<ActionResult> GetCodSettlementReportPdf([FromQuery] GetCodSettlementReportPdfQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);

    var file = DirectoryHelper.GetRandomNameForPdf("CPC");
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
  #region CODPendings
  [HttpPost("GetAllCODPendings")]
  public async Task<ActionResult> GetAllCODPendings([FromBody] GetAllCODPendingsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ExcelExportCodPending")]
  public async Task<ActionResult> ExcelExportCodPending([FromBody] ExcelExportCodPendingQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("CodPending"));
  }
  [HttpPost("GetPDFCODPendings")]
  public async Task<ActionResult> GetPDFCODPendings([FromBody] GetPDFCODPendingsQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);

    var file = DirectoryHelper.GetRandomNameForPdf("CRR");
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
  #endregion
  #region payment file
  #region command
  [HttpPost("CreateCpsettlementPopFile")]
  public async Task<ActionResult> CreateCpsettlementPopFile([FromForm] CreateCpsettlementPopFileCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteCpsettlementPopFile")]
  public async Task<ActionResult> DeleteCpsettlementPopFile([FromBody] DeleteCpsettlementPopFileCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  #endregion
  #region query
  [HttpPost("GetAllCpsettlementPopFilesById")]
  public async Task<ActionResult> GetAllCpsettlementPopFiles([FromBody] GetAllCpsettlementPopFilesQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #endregion
}
