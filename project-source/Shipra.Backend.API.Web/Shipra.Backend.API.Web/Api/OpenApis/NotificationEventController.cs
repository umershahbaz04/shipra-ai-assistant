using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.CommonFeature.Command.HandleNotificationAndEvent;

namespace Shipra.Backend.API.Web.Api.OpenApis;

public class NotificationEventController : BaseApiController
{
  public NotificationEventController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }


  #region command 
  [HttpPost("HandleNotificationAndEvent")]
  public async Task<ActionResult> HandleNotificationAndEvent([FromBody] HandleNotificationAndEventCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

}
