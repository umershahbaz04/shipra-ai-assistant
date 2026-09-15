using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCities;

namespace Shipra.Backend.API.Web.Api;

public class DistributionController : BaseApiController
{
  public DistributionController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  [HttpPost("GetAllCities")]
  public async Task<ActionResult> GetAllCities(CancellationToken cancellationToken = default)
  {
    var request = new GetAllCitiesQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
