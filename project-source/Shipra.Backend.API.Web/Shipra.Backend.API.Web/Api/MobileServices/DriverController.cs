using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientProfile;
using Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Query.GetAllClientOrderLabelLookupForSelection;
using Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllCarrierTrackingStatusLookup;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetAllDeliveryNoteForDriverById;
using Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteDetailForDriverById;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetAllowStatusUpdateOnDeliveryTask;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetCODClearedByDriver;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverAccountBalanceById;
using Shipra.Backend.API.Application.Features.DriverFeatures.Command.UpdateOrderStatusByDriver;
using Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetAllDriverCTSForSelection;
using Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetDriverProfile;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderPODFiles;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetMapApiKey;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderForDriverById;

namespace Shipra.Backend.API.Web.Api.MobileServices;
[Authorize]
public class DriverController : BaseApiController
{
  public DriverController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region query
  [HttpGet("GetDriverAccountBalanceById")]
  public async Task<ActionResult> GetAllDriverExpenseForMobile([FromQuery] GetDriverAccountBalanceByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllDeliveryNoteForDriverById")]
  public async Task<ActionResult> GetAllDeliveryNoteForDriverById([FromQuery] GetAllDeliveryNoteForDriverByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetDeliveryNoteDetailForDriverById")]
  public async Task<ActionResult> GetDeliveryNoteDetailForDriverById([FromBody] GetDeliveryNoteDetailForDriverByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetOrderForDriverById")]
  public async Task<ActionResult> GetOrderForDriverById([FromBody] GetOrderForDriverByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetClientProfile")]
  public async Task<ActionResult> GetClientProfile(CancellationToken cancellationToken = default)
  {
    GetClientProfileQuery request = new GetClientProfileQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetDriverProfile")]
  public async Task<ActionResult> GetDriverProfile(CancellationToken cancellationToken = default)
  {
    GetDriverProfileQuery request = new GetDriverProfileQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetCarrierTrackingStatusForDriver")]
  public async Task<ActionResult> GetCarrierTrackingStatusForDriver(CancellationToken cancellationToken = default)
  {
    GetAllDriverCTSForSelectionQuery request = new GetAllDriverCTSForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllowStatusUpdateOnDeliveryTask")]
  public async Task<ActionResult> GetAllowStatusUpdateOnDeliveryTask(CancellationToken cancellationToken = default)
  {
    GetAllowStatusUpdateOnDeliveryTaskQuery request = new GetAllowStatusUpdateOnDeliveryTaskQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region command
  [HttpPost("UploadOrderPODFiles")]
  public async Task<ActionResult> UploadOrderPODFiles([FromForm] CreateOrderPODFilesCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateOrderStatusByDriver")]
  public async Task<ActionResult> UpdateOrderStatusByDriver([FromBody] UpdateOrderStatusByDriverCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetCODClearedByDriver")]
  public async Task<ActionResult> GetCODClearedByDriver()
  {
    var response = await Mediator.Send(new GetCODClearedByDriverQuery(), default);
    return Ok(response);
  }
  [HttpGet("GetMapApiKey")]
  public async Task<ActionResult> GetMapApiKey()
  {
    var request = new GetMapApiKeyQuery();
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }
  [HttpGet("GetAllOrderLabels")]
  public async Task<ActionResult> GetAllOrderLabels(CancellationToken cancellationToken = default)
  {
    GetAllClientOrderLabelLookupForSelectionQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
