using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.NotificationFeature.ContactUs;
using Shipra.Backend.API.Application.Features.NotificationFeature.NewsSubscription;
using Shipra.Backend.API.Application.Features.NotificationFeature.SendEmailToCustomer;

namespace Shipra.Backend.API.Web.Api;
[Route("api/[controller]")]
[ApiController] 
public class NotificationController : BaseApiController
{
  public NotificationController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  [HttpPost("ContactUs")]
  public async Task<ActionResult> ContactUs([FromBody] ContactUsCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("NewsSubscription")]
  public async Task<ActionResult> NewsSubscription([FromBody] NewsSubscriptionCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("SendNotificationToCustomer")]
  public async Task<ActionResult> SendNotificationToCustomer([FromBody] SendNotificationToCustomerCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
