using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Command.DeleteNotificationConfig;
using Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Command.UpdateNotificationConfig;
using Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Query.GetNotificationConfigByTypeId;
using Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Query.GetNotificationEvents;
using Shipra.Backend.API.Application.Features.NotificationConfigFeatures.Query.GetNotificationTypes;
using Shipra.Backend.API.Application.Features.NotificationFeatures.Command.CreaetNotificationConfig;

namespace Shipra.Backend.API.Web.Api;

[Authorize]
public class NotificationConfigController : BaseApiController
{
  public NotificationConfigController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  [HttpPost("CreaetNotificationConfig")]
  public async Task<ActionResult> CreaetNotificationConfig([FromBody] CreaetNotificationConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateNotificationConfig")]
  public async Task<ActionResult> UpdateNotificationConfig([FromBody] UpdateNotificationConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteNotificationConfig")]
  public async Task<ActionResult> DeleteNotificationConfig([FromBody] DeleteNotificationConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }


  #region query 
  [HttpGet("GetNotificationEvents")]
  public async Task<ActionResult> GetNotificationEvents(CancellationToken cancellationToken = default)
  {
    GetNotificationEventsQuery request = new GetNotificationEventsQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  [HttpGet("GetNotificationTypes")]
  public async Task<ActionResult> GetNotificationTypes(CancellationToken cancellationToken = default)
  {
    GetNotificationTypesQuery request = new GetNotificationTypesQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetNotificationConfigByTypeId")]
  public async Task<ActionResult> GetNotificationConfigByTypeId([FromQuery] GetNotificationConfigByTypeIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 

  #endregion
}
