using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllCarrierWithServiceAndLocation;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllUpsSettingSelectionByType;

namespace Shipra.Backend.API.Web.Api.OpenApis;
public class CarrierSettingController : BaseApiController
{
  public CarrierSettingController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  [HttpGet("GetAllUpsSettingSelectionByType")]
  public async Task<ActionResult> GetAllUpsSettingSelectionByType([FromQuery] GetAllUpsSettingSelectionByTypeQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
