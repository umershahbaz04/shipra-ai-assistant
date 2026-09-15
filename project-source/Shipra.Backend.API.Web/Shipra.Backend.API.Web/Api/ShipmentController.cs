using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetPDFCarrierReturnReportById;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.CreateShipmentGridColumn;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.DeleteShipmentGridColumn;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.MoveStatusFromOneTabToAnother;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.UpdateShipmentGridColumn;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Command.UpdateShipmentTabDisplayOrder;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.ExcelExportsShipments;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.ExportShipmentsByDriverReceivableId;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetAllCarrierShipmentPodFile;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetAllShipmentsByDriverReceivableId;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetAllShipmentsQuery;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetAllShipmentTabsCount;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetOrderInfoByOrderNoForPopUp;
using Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetSettingOperationDashboard;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Helpers.Reporting;

namespace Shipra.Backend.API.Web.Api;

[Authorize] 
public class ShipmentController : BaseApiController
{
  private readonly IWebHostEnvironment _webHostEnvironment;


  public ShipmentController(IServiceProvider serviceProvider, IWebHostEnvironment webHostEnvironment) : base(serviceProvider)
  {
    this._webHostEnvironment = webHostEnvironment;
  }
  #region command
  [HttpPost("CreateShipmentGridColumn")]
  public async Task<ActionResult> CreateShipmentGridColumn([FromBody] CreateShipmentGridColumnCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  } 
  [HttpPost("DeleteShipmentGridColumn")]
  public async Task<ActionResult> DeleteShipmentGridColumn([FromBody] DeleteShipmentGridColumnCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  }
  [HttpPost("UpdateShipmentTabDisplayOrder")]
  public async Task<ActionResult> CreateShipmentGridColumn([FromBody] UpdateShipmentTabDisplayOrderCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  }  
  [HttpPost("UpdateShipmentGridColumn")]
  public async Task<ActionResult> UpdateShipmentGridColumn([FromBody] UpdateShipmentGridColumnCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response); 
  } 
  [HttpPost("MoveDashboardStatusFromOneTabToAnother")]
  public async Task<ActionResult> CreateShipmentGridColumn([FromBody] MoveDashboardStatusFromOneTabToAnotherCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);

  }
  #endregion
  #region query
  [HttpPost("GetShipments")]
  public async Task<ActionResult> GetShipments([FromBody] GetAllShipmentsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetShipmentsForSalePerson")]
  public async Task<ActionResult> GetShipmentsForSalePerson([FromBody] GetAllShipmentsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpPost("GetAllShipmentsByDriverReceivableId")]
  public async Task<ActionResult> GetAllShipmentsByDriverReceivableId([FromBody] GetAllShipmentsByDriverReceivableIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetShipmentTabsCount")]
  public async Task<ActionResult> GetShipmentTabsCount([FromBody] GetAllShipmentTabsCountQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetShipmentTabsCountConfig")]
  public async Task<ActionResult> GetShipmentTabsCountConfig([FromQuery] GetShipmentTabsCountConfigQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetShipmentInfoByOrderNo")]
  public async Task<ActionResult> GetShipmentInfoByOrderNo([FromBody] GetOrderInfoByOrderNoForPopUpQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ExcelExportShipments")]
  public async Task<ActionResult> ExcelExportShipments([FromBody] ExcelExportShipmentsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return File(response!.Result?.Bytes!, ExcelExportHelper.ExcelContentType, ExcelExportHelper.GetExcelFileName("Shipments"));

  }  
  [HttpPost("GetAllCarrierShipmentPodFiles")]
  public async Task<ActionResult> GetAllCarrierShipmentPodFiles([FromBody] GetAllCarrierShipmentPodFileQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);

  }
  [HttpPost("ExportShipmentsByDriverReceivableId")]
  public async Task<ActionResult> ExportShipmentsByDriverReceivableId([FromBody] ExportShipmentsByDriverReceivableIdQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("DRO");
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
  [HttpPost("GetPDFCarrierReturnReportById")]
  public async Task<ActionResult> GetPDFCarrierReturnReportById([FromBody] GetPDFCarrierReturnReportByIdQuery request, CancellationToken cancellationToken = default)
  {
    var result = await Mediator.Send(request, cancellationToken);
    var file = DirectoryHelper.GetRandomNameForPdf("DRRO");
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
