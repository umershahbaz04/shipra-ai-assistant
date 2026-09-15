using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.CustomBinder;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CheckDuplicationCarrierAliasByClient;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrier;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrierPickupLocation;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.DeleteActiveCarrier;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdateActiveCarrier;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdateActiveCarrierClientSettingConfig;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdateCarrierAliasByClient;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdatePriceContractCarrier;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarrierById;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarrierPickupLocationbyId;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarrierPickupLocationForSelection;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarriersForAdmin;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarriersForSelection;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetAllActiveCarrier;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetNextCarrierAliasWithCarrierId;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetSettingConfigByActiveCarrierId;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.CreateCarrier;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.DeleteCarrier;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.UpdateCarrier;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.UploadCarrierImage;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.Admin.GetAllCarriers;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllCarrier;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllCarrierForCreateOrderSelectionFilter;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllCarrierLocationsByCarrier;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierById;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierLocationByCarrierAndCountry;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierWebHookUrlByCarrierId;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetInputRequiredConfigById;
using Shipra.Backend.API.Application.Features.ContractCarrierFeature.Query.GetAllShipraContractCarrierWithServiceAndLocation;
using Shipra.Backend.API.Application.Features.ContractCarrierFeature.Query.GetShipraContractByShipraContractCarrierId;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UnAssignFromCarrier;
 
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.deletepickLocation;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.UpdatepickLocation;
 
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoreByIdQuery;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllClientRate;


namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class CarrierController : BaseApiController
{
  public CarrierController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  } 

  #region carrier

  #region command
  [HttpPost("CreateCarrier")]
  public async Task<ActionResult> CreateCarrier([FromBody] CreateCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateCarrier")]
  public async Task<ActionResult> UpdateCarrier([FromBody] UpdateCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteCarrierById")]
  public async Task<ActionResult> DeleteCarrierById([FromBody] DeleteCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UnAssignFromCarrier")]
  public async Task<ActionResult> UnAssignFromCarrier([FromBody] UnAssignFromCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region query
  [HttpPost("GetAllCarriersForAdmin")]
  public async Task<ActionResult> GetAllCarriersForAdmin([FromBody] GetAllCarriersQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetCarrierById")]
  public async Task<ActionResult> GetCarrierById([FromQuery] GetCarrierByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllCarriers")]
  public async Task<ActionResult> GetAllCarriers([FromBody] GetAllCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllShipraContractCarrierWithServiceAndLocation")]
  public async Task<ActionResult> GetAllShipraContractCarrierWithServiceAndLocation([FromBody] GetAllShipraContractCarrierWithServiceAndLocationQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetShipraContractByShipraContractCarrierId")]
  public async Task<ActionResult> GetShipraContractByShipraContractCarrierId([FromQuery] GetShipraContractByShipraContractCarrierIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetCarrierLocationByCarrier")]
  public async Task<ActionResult> GetCarrierLocationByCarrier([FromQuery] GetCarrierLocationByCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllCarrierLocationsByCarrier")]
  public async Task<ActionResult> GetAllCarrierLocationsByCarrier([FromQuery] GetAllCarrierLocationsByCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllClientRate")]
  public async Task<ActionResult> GetAllClientRate([FromBody] GetAllClientRateQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  #endregion
  #endregion
  #region active carrier

  #region command
  [HttpPost("CreateActiveCarrier")]
  public async Task<ActionResult> CreateActiveCarrier([FromBody] CreateActiveCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateActiveCarrier")]
  public async Task<ActionResult> UpdateActiveCarrier([FromBody] UpdateActiveCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateActiveCarrierClientSettingConfig")]
  public async Task<ActionResult> UpdateActiveCarrierClientSettingConfig([FromBody] UpdateActiveCarrierClientSettingConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteActiveCarrierById")]
  public async Task<ActionResult> DeleteActiveCarrier([FromBody] DeleteActiveCarrierCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateCarrierAlias")]
  public async Task<ActionResult> UpdateCarrierAlias([FromBody] UpdateCarrierAliasByClientCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateActiveCarrierPickupLocation")]
  public async Task<ActionResult> CreateActiveCarrierPickupLocation([ModelBinder(BinderType = typeof(CreateActiveCarrierPickupLocationClientSideBinder))] CreateActiveCarrierPickupLocationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }


  [HttpPost("UploadCarrrierImage")]
  public async Task<ActionResult> UploadCarrrierImage([FromForm] UploadCarrierImageCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  #endregion

  #region query

  [HttpGet("GetActiveCarrierById")]
  public async Task<ActionResult> GetActiveCarrierById([FromQuery] GetActiveCarrierByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetSettingConfigByActiveCarrierId")]
  public async Task<ActionResult> GetSettingConfigByActiveCarrierId([FromBody] GetSettingConfigByActiveCarrierIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  //[Authorize]
  [HttpPost("GetAllActiveCarrier")]
  public async Task<ActionResult> GetAllActiveCarrier([FromBody] GetAllActiveCarrierQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllActiveCarrierForCreateOrderSelection")]
  public async Task<ActionResult> GetAllActiveCarrierForCreateOrderSelection([FromQuery] GetAllActiveCarrierForCreateOrderSelectionFilterQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetInputRequiredConfigById")]
  public async Task<ActionResult> GetInputRequiredConfigById([FromQuery] GetInputRequiredConfigByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetCarrierWebHookUrlByCarrierId")]
  public async Task<ActionResult> GetCarrierWebHookUrlByCarrierId([FromQuery] GetCarrierWebHookUrlByCarrierIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetActiveCarriersForSelection")]
  public async Task<ActionResult> GetActiveCarriersForSelection(CancellationToken cancellationToken = default)
  {
    GetActiveCarriersForSelectionQuery request = new GetActiveCarriersForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CheckDuplicationCarrierAlias")]
  public async Task<ActionResult> CheckDuplicationCarrierAlias([FromBody] CheckDuplicationCarrierAliasByClientCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetNextCarrierAliasWithCarrierId")]
  public async Task<ActionResult> GetNextCarrierAliasWithCarrierId([FromBody] GetNextCarrierAliasWithCarrierIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetActiveCarrierPickupLocationForSelection")]
  public async Task<ActionResult> GetActiveCarrierPickupLocationForSelection([FromQuery] GetActiveCarrierPickupLocationForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetActiveCarrierPickupLocationbyid")]
  public async Task<ActionResult> GetActiveCarrierPickupLocationbyid([FromQuery] GetActiveCarrierLocationbyIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  //[HttpGet("GetStoreById")]
  //public async Task<ActionResult> GetStoreById([FromQuery] GetStoreByIdQuery request, CancellationToken cancellationToken = default)
  //{
  //  var response = await Mediator.Send(request, cancellationToken);
  //  return Ok(response);
  //}
  #endregion
  #endregion
  [HttpPost("deletepickLocationById")]
  public async Task<ActionResult> deletepickLocation([FromBody] DeletePickupLocationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdatepickLocation")]
  public async Task<ActionResult> UpdatepickLocation([FromBody] UpdatepickLocationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
