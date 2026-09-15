using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.StoreFeatures.Query.GetStoresForShopifySelection;
using Shipra.Backend.API.Application.Features.WalletFeature.Command.UpdateWalletWithTotalProcessingLinkData;

namespace Shipra.Backend.API.Web.Api.OpenApis; 
public class TotalProcessingController : BaseApiController
{
  public TotalProcessingController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  [HttpPost("UpdateWallet")]
  public async Task<ActionResult> UpdateWallet([FromBody] UpdateWalletWithTotalProcessingLinkDataCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, default);
    return Ok(response);
  }
}
