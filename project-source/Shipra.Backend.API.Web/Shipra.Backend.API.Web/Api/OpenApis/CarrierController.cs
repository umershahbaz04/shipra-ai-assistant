using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.CustomBinder;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrier;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrierPickupLocation;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdateActiveCarrierClientSettingConfig;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdatePriceContractCarrier;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarrierPickupLocationForSelection;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarriersForAdmin;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetSettingConfigByActiveCarrierId;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.AssignCarrierManualAdmin;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.CreateUpdateShipraContractClientCarrier;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.deletepickLocation;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.DeleteShipraContractClientCarrier;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.GetAllShipraContractClientCarrier;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.UpdateCarrierBackgroundColor;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.ClientAddressFromAdmin;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllCarrierWithServiceAndLocation;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllDeliveryService;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierWebHookUrlByCarrierId;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierWithServiceAndLocationByCarrierId;
using Shipra.Backend.API.Application.Features.ContractCarrierFeature.Command.ActiveDeactiveShipraContractCarrier;
using Shipra.Backend.API.Application.Features.ContractCarrierFeature.Query.GetAllShipraContractCarrierWithServiceAndLocation;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.IntegrationToCarrier;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllCivilEntityExtendedByCarrier;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetCalculatedRateByCarrier;

namespace Shipra.Backend.API.Web.Api.OpenApis;

public class CarrierController : BaseApiController
{
  public CarrierController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region carrier

  #region command 
  //[HttpPost("DeleteCarrierWithServiceAndLocationById")]
  //public async Task<ActionResult> DeleteCarrierWithServiceAndLocationById([FromBody] DeleteCarrierCommand request, CancellationToken cancellationToken = default)
  //{
  //  var response = await Mediator.Send(request, cancellationToken);
  //  return Ok(response);
  //}
  #endregion

  #region query 
  [HttpPost("GetAllCarrierWithServiceAndLocation")]
  public async Task<ActionResult> GetAllCarrierWithServiceAndLocation([FromBody] GetAllCarrierWithServiceAndLocationQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetCarrierWithServiceAndLocationByCarrierId")]
  public async Task<ActionResult> GetCarrierWithServiceAndLocationByCarrierId([FromQuery] GetCarrierWithServiceAndLocationByCarrierIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllShipraContractCarrierWithServiceAndLocationForAdmin")]
  public async Task<ActionResult> GetAllShipraContractCarrierWithServiceAndLocationForAdmin([FromBody] GetAllShipraContractCarrierWithServiceAndLocationQuery request, CancellationToken cancellationToken = default)
  {
    request.IsForAdmin = true;
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllDeliveryService")]
  public async Task<ActionResult> GetAllDeliveryService(CancellationToken cancellationToken = default)
  {
    GetAllDeliveryServiceQuery request = new GetAllDeliveryServiceQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateCarrierBackgroundColor")]
  public async Task<ActionResult> UpdateCarrierBackgroundColor([FromBody] UpdateCarrierBackgroundColorCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAndCarrierWebHookUrlByCarrierId")]
  public async Task<ActionResult> GetAndCarrierWebHookUrlByCarrierId([FromQuery] GetCarrierWebHookUrlByCarrierIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateActiveContractCarrier")]
  public async Task<ActionResult> CreateActiveContractCarrier([FromBody] CreateActiveCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateShipraContractCarrier")]
  public async Task<ActionResult> UpdateShipraContractCarrier([FromBody] UpdateActiveCarrierClientSettingConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteShipraContractClientCarrier")]
  public async Task<ActionResult> DeleteShipraContractClientCarrier([FromBody] DeleteShipraContractClientCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetShipraContractCarrierById")]
  public async Task<ActionResult> GetShipraContractCarrierById([FromQuery] GetSettingConfigByActiveCarrierIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ActiveDeactiveShipraContractCarrier")]
  public async Task<ActionResult> ActiveDeactiveShipraContractCarrier([FromBody] ActiveDeactiveShipraContractCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  //UpdatePrice.....
  [HttpPost("UpdateContractCarrierPrice")]
  public async Task<ActionResult> UpdateContractCarrierPrice([FromBody] UpdatePriceForContractCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateUpdateShipraContractClientCarrier")]
  public async Task<ActionResult> CreateUpdateShipraContractClientCarrier([FromBody] CreateUpdateShipraContractClientCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllActiveCarriersForAdmin")]
  public async Task<ActionResult> GetAllActiveCarriersForAdmin([FromBody] GetActiveCarriersForAdminQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #endregion
  [HttpGet("GetAllActiveCarrierPickupLocationForSelectionForAdmin")]
  public async Task<ActionResult> GetActiveCarrierPickupLocationForSelection([FromQuery] GetActiveCarrierPickupLocationForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllShipraContractClientCarrier")]
  public async Task<ActionResult> GetAllShipraContractClientCarrier(CancellationToken cancellationToken = default)
  {
    GetAllShipraContractClientCarrierQuery request = new GetAllShipraContractClientCarrierQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllCivilEntityExtendedByCarrier")]
  public async Task<ActionResult> GetAllCivilEntityExtendedByCarrier([FromQuery] GetAllCivilEntityExtendedByCarrierQuery request, CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("AssignCarrierManualAdmin")]
  public async Task<ActionResult> AssignCarrierManualAdmin([FromBody] AssignCarrierManualAdminCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetClientRateByAdmin")]
  public async Task<ActionResult> GetClientRateByAdmin([FromBody] GetClientAddressFromAdminQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateActiveCarrierPickupLocationAdmin")]
  public async Task<ActionResult> CreateActiveCarrierPickupLocationAdmin([ModelBinder(BinderType = typeof(CreateActiveCarrierPickupLocationClientSideBinder))] CreateActiveCarrierPickupLocationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("AssignToCarrierAdmin")]
  public async Task<ActionResult> AssignToCarrierAdmin([FromBody] AssignToCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetCalculatedRateByCarrierAdmin")]
  public async Task<ActionResult> GetCalculatedRateByCarrier([FromBody] GetCalculatedRateByCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("deletepickLocationByIdAdmin")]
  public async Task<ActionResult> deletepickLocation([FromBody] DeletePickupLocationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
