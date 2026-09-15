using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Commands.CreateCarrierReturnReport;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.ExcelExportReturnReportById;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetOrderDetailByUplaodReturnReportFile;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetOrderDetailForReturnReport;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetReturnReportByReturnReportId;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetReturnReportSampleFile;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetReturnRerports;
using Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetShipmentsByReturnReportId;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReturnReportController : BaseApiController
{
  public ReturnReportController(IServiceProvider serviceProvider) : base(serviceProvider)
  {

  }
  #region command

  [HttpPost("CreateCarrierReturnReport")]
  public async Task<ActionResult> CreateCarrierReturnReport([FromBody] CreateCarrierReturnReportCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  #endregion

  #region query 
  [HttpGet("GetReturnReportSampleFile")]
  public async Task<ActionResult> GetReturnReportSampleFile(CancellationToken cancellationToken = default)
  {
    GetReturnReportSampleFileQuery request = new GetReturnReportSampleFileQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetOrderDetailByUplaodReturnReportFile")]
  public async Task<ActionResult> GetOrderDetailByUplaodReturnReportFile([FromForm] GetOrderDetailByUplaodReturnReportFileQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetOrderDetailForReturnReport")]
  public async Task<ActionResult> GetOrderDetailForReturnReport([FromBody] GetOrderDetailForReturnReportQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetReturnRerports")]
  public async Task<ActionResult> GetReturnRerports([FromBody] GetReturnRerportsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetShipmentsByReturnReportId")]
  public async Task<ActionResult> GetShipmentsByReturnReportId([FromQuery] GetShipmentsByReturnReportIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetReturnReportByReturnReportId")]
  public async Task<ActionResult> GetReturnReportByReturnReportId([FromQuery] GetReturnReportByReturnReportIdQuery request, CancellationToken cancellationToken = default)
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
  [HttpGet("ExcelExportReturnReportById")]
  public async Task<ActionResult> ExcelExportReturnReportById([FromQuery] ExcelExportReturnReportByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("ReturnReport"));
  }
  #endregion

}
