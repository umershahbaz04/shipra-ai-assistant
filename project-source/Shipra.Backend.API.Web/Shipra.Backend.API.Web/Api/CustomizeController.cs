using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.GetAccessTokenWithRefreshToken;
using Shipra.Backend.API.Application.Features.ClientFeatures.Commands.Login;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Query.GetOrderTrackingHistoryByOrderId;

namespace Shipra.Backend.API.Web.Api;
[Route("api")]

public class CustomizeController : BaseApiController
{
  public CustomizeController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region Usermanagment
  [HttpPost("Login")]
  public async Task<ActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAccessTokenWithRefreshToken")]
  public async Task<ActionResult> GetAccessTokenWithRefreshToken([FromBody] GetAccessTokenWithRefreshTokenCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region Order
  [Authorize]
  [HttpPost("CreateOrder")]
  public async Task<ActionResult> CreateOrder([FromBody] CreateOrderCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [Authorize]
  [HttpGet("GetOrderTrackingHistoryByOrderNo")]
  public async Task<ActionResult> GetOrderTrackingHistoryByOrderNo([FromQuery] GetOrderTrackingHistoryByOrderNoQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
