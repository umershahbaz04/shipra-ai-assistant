using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.ActivatePaymentProcess;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.CreateStripeWebhook;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.DeletePPActivate;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetAllPPActivate;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetAllPPLookup;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetPPActivateByPPActivateId;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetPPLookupById;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class PaymentProcessController : BaseApiController
{
  public PaymentProcessController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("ActivatePaymentProcess")]
  public async Task<ActionResult> ActivatePaymentProcess([FromBody] ActivatePaymentProcessCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("DeletePPActivate")]
  public async Task<ActionResult> DeletePPActivate([FromBody] DeletePPActivateCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("CreateStripeWebhook")]
  public async Task<ActionResult> CreateStripeWebhook([FromBody] CreateStripeWebhookCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region query
  [HttpPost("GetAllPPActivate")]
  public async Task<ActionResult> GetAllPPActivate([FromBody] GetAllPPActivateQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetPPActivateById")]
  public async Task<ActionResult> GetPPActivateById([FromQuery] GetPPActivateByPPActivateIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllPPLookupForSelection")]
  public async Task<ActionResult> GetAllPPLookupForSelection(CancellationToken cancellationToken = default)
  {
    GetAllPPLookupForSelectionQuery request = new GetAllPPLookupForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetPPLookupById")]
  public async Task<ActionResult> GetPPLookupById([FromQuery] GetPPLookupByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
