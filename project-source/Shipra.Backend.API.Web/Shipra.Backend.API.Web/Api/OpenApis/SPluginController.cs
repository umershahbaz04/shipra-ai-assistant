using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.CreateShopifySaleChannelConfig;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.DeleteShopifySaleChannelConfig;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.UpdateShopifySaleChannelConfig;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetShopifySessionTokenByShop;
using Shipra.Backend.API.Application.Features.ShopifyFeature.Command.DeleteShopifyConfig;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoresForShopifySelection;

namespace Shipra.Backend.API.Web.Api.OpenApis;
public class SPluginController : BaseApiController
{
  public SPluginController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  [HttpPost("GetStoresForShopifySelection")]
  public async Task<ActionResult> GetStoresForShopifySelection([FromBody] GetStoresForShopifySelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }

  [HttpPost("CreateShopifySaleChannelConfig")]
  public async Task<ActionResult> CreateShopifySaleChannelConfig([FromBody] CreateShopifySaleChannelConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateShopifySaleChannelConfig")]
  public async Task<ActionResult> UpdateShopifySaleChannelConfig([FromBody] UpdateShopifySaleChannelConfigCommand request, CancellationToken cancellationToken = default)
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
  [HttpPost("DeleteShopifySaleChannelConfig")]
  public async Task<ActionResult> DeleteShopifySaleChannelConfig([FromBody] DeleteShopifySaleChannelConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpPost("GetShopifySessionTokenByShop")]
  public async Task<ActionResult> GetShopifySessionTokenByShop([FromBody] GetShopifySessionTokenByShopQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
