using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.OrderBoxFeature.Command.CreateClientOrderBox;
using Shipra.Backend.API.Application.Features.OrderBoxFeature.Command.EnableDisabpleClientOrderBox;
using Shipra.Backend.API.Application.Features.OrderBoxFeature.Command.UpdateClientOrderBox;
using Shipra.Backend.API.Application.Features.OrderBoxFeature.Query.GetAllClientOrderBox;
using Shipra.Backend.API.Application.Features.OrderBoxFeature.Query.GetClientOrderBoxById;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class OrderBoxController : BaseApiController
{
  public OrderBoxController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region carrier

  #region command
  [HttpPost("CreateClientOrderBox")]
  public async Task<ActionResult> CreateClientOrderBox([FromBody] CreateClientOrderBoxCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateClientOrderBox")]
  public async Task<ActionResult> UpdateCarrier([FromBody] UpdateClientOrderBoxCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("EnableDisableClientOrderBox")]
  public async Task<ActionResult> DeleteCarrierById([FromBody] EnableDisableClientOrderBoxCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  #endregion

  #region query
  [HttpGet("GetAllClientOrderBox")]
  public async Task<ActionResult> GetAllClientOrderBox(CancellationToken cancellationToken = default)
  {
    GetAllClientOrderBoxQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  
  [HttpGet("GetClientOrderBoxById")]
  public async Task<ActionResult> GetCarrierById([FromQuery] GetClientOrderBoxByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
   
  #endregion
  #endregion 
}
