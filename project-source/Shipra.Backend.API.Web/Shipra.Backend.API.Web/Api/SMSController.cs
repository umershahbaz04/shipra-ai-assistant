using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.SmsFeature.Command.SendSms;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.ActivateSMSProcess;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.DeleteSMSActivate;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSActivate;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSLookupForSelection;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetSMSActivateById;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetSMSActivateForSelection;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class SMSController : BaseApiController
{
  public SMSController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("ActivateSmsProcess")]
  public async Task<ActionResult> ActivateSmsProcess([FromBody] ActivateSMSProcessCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("DeleteSMSActivate")]
  public async Task<ActionResult> DeleteSMSActivate([FromBody] DeleteSMSActivateCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region query
  [HttpPost("GetAllSMSActivate")]
  public async Task<ActionResult> GetAllSMSActivate([FromBody] GetAllSMSActivateQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  [HttpGet("GetAllSMSLookupForSelection")]
  public async Task<ActionResult> GetAllSMSLookupForSelection(CancellationToken cancellationToken = default)
  {
    GetAllSMSLookupForSelectionQuery request = new GetAllSMSLookupForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetSMSActivateById")]
  public async Task<ActionResult> GetSMSActivateById([FromQuery] GetSMSActivateByIdQuery request,CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("SendSms")]
  public async Task<ActionResult> SendSms(CancellationToken cancellationToken = default)
  {
    SendSmsCommand request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  
  [HttpGet("GetSMSActivateForSelection")]
  public async Task<ActionResult> GetSMSActivateForSelection(CancellationToken cancellationToken = default)
  {
    GetSMSActivateForSelectionQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

}
