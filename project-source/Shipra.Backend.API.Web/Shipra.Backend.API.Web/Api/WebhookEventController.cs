using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.WebhookEventFeature.Command.CreateClientWebhookEvent;
using Shipra.Backend.API.Application.Features.WebhookEventFeature.Command.DeleteWebhookEvent;
using Shipra.Backend.API.Application.Features.WebhookEventFeature.Command.UpdateClientWebhookEvent;
using Shipra.Backend.API.Application.Features.WebhookEventFeature.Query.GetAllClientWebhookEvent;
using Shipra.Backend.API.Application.Features.WebhookEventFeature.Query.GetAllWebhookEventLookups;
using Shipra.Backend.API.Application.Features.WebhookEventFeature.Query.GetClientWebhookEventById;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class WebhookEventController : BaseApiController
{
  public WebhookEventController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region carrier

  #region command
  [HttpPost("CreateClientWebhookEvent")]
  public async Task<ActionResult> CreateClientWebhookEvent([FromBody] CreateClientWebhookEventCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateClientWebhookEvent")]
  public async Task<ActionResult> UpdateClientWebhookEvent([FromBody] UpdateClientWebhookEventCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  
  [HttpGet("DeleteWebhookEvent")]
  public async Task<ActionResult> DeleteWebhookEvent([FromQuery] DeleteWebhookEventCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  #endregion

  #region query
 
  [HttpPost("GetAllWebhookEventLog")]
  public async Task<ActionResult> GetAllWebhookEventLog([FromBody] GetAllWebhookEventLogByClientQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetClientWebhookEventById")]
  public async Task<ActionResult> GetClientWebhookEventById([FromQuery] GetClientWebhookEventByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetAllWebhookEventLookup")]
  public async Task<ActionResult> GetAllWebhookEventLookup(CancellationToken cancellationToken = default)
  {
    GetAllWebhookEventLookupQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllClientWebhookEvents")]
  public async Task<ActionResult> GetAllClientWebhookEvents(CancellationToken cancellationToken = default)
  {
    GetAllClientWebhookEventQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  #endregion
  #endregion 
}
