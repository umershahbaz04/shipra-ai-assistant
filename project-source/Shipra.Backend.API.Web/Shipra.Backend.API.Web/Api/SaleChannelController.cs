using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.CreateSaleChannelConfig;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.DeleteSaleChannelConfig;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.UpdateSaleChannelConfig;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelByLookupIdForSelection;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelConfig;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelLookupForSelection;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetAllSaleChannelsByStoreId;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelByStoreIdForSelection;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigById;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigByKey;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelConfigForUpdateById;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSaleChannelLookupById;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSalePersonConfig;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelInventorySyncProcessor;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelOrderPostProcessor;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelProductPostProcessor;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelOrderPreProcessor;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelProductPreProcessor;
using Shipra.Backend.API.Application.Features.ShopifyFeature.Command.DeleteShopifyConfig;

namespace Shipra.Backend.API.Web.Api;
[Authorize]

public class SaleChannelController : BaseApiController
{
  public SaleChannelController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region Command
  [HttpPost("CreateSaleChannelConfig")]
  public async Task<ActionResult> CreateSaleChannelConfig([FromBody] CreateSaleChannelConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateSaleChannelConfig")]
  public async Task<ActionResult> UpdateSaleChannelConfig([FromBody] UpdateSaleChannelConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteSaleChannelConfig")]
  public async Task<ActionResult> DeleteSaleChannelConfig([FromBody] DeleteSaleChannelConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("DeleteShopifyConfig")]
  public async Task<ActionResult> DeleteShopifyConfig([FromBody] DeleteShopifyConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  #endregion

  #region Query
  [HttpPost("GetAllSaleChannelConfig")]
  public async Task<ActionResult> GetAllSaleChannelConfig([FromBody] GetAllSaleChannelConfigQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllSaleChannelLookupForSelection")]
  public async Task<ActionResult> GetAllSaleChannelLookupForSelection([FromQuery] GetAllSaleChannelLookupForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetSaleChannelConfigById")]
  public async Task<ActionResult> GetSaleChannelConfigById([FromQuery] GetSaleChannelConfigByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetSaleChannelConfigForUpdateById")]
  public async Task<ActionResult> GetSaleChannelConfigForUpdateById([FromQuery] GetSaleChannelConfigForUpdateByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetSaleChannelLookupById")]
  public async Task<ActionResult> GetSaleChannelLookupById([FromQuery] GetSaleChannelLookupByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetSaleChannelByStoreIdForSelection")]
  public async Task<ActionResult> GetSaleChannelByStoreIdForSelection([FromQuery] GetSaleChannelByStoreIdForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("SaleChannelOrderPreProcessor")]
  public async Task<ActionResult> SaleChannelOrderPreProcessor([FromBody] SaleChannelOrderPreProcessorCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("SaleChannelProductPreProcessor")]
  public async Task<ActionResult> SaleChannelProductPreProcessor([FromBody] SaleChannelProductPreProcessorCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("SaleChannelOrderPostProcessor")]
  public async Task<ActionResult> SaleChannelOrderPostProcessor([FromBody] SaleChannelOrderPostProcessorCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("SaleChannelProductPostProcessor")]
  public async Task<ActionResult> SaleChannelProductPostProcessor([FromBody] SaleChannelProductPostProcessorCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("SaleChannelInventorySync")]
  public async Task<ActionResult> SaleChannelInventorySync([FromBody] SaleChannelInventorySyncProcessorCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllSaleChannelByLookupIdForSelection")]
  public async Task<ActionResult> GetAllSaleChannelByLookupIdForSelection([FromQuery] GetAllSaleChannelByLookupIdForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllSaleChannelsByStoreId")]
  public async Task<ActionResult> GetAllSalePersons([FromQuery] GetAllSaleChannelsByStoreIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetSalePersonConfig")]
  public async Task<ActionResult> GetSalePersonConfig(CancellationToken cancellationToken = default)
  {
    GetSalePersonConfigQuery request = new GetSalePersonConfigQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetSaleChannelConfigByKey")]
  public async Task<ActionResult> GetSaleChannelConfigByKey([FromQuery] GetSaleChannelConfigByKeyQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
