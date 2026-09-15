using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.ActivateSMSProcess;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSActivate;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSLookupForSelection;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetSMSActivateById;
using Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Command.ActivateWhatsappProcess;
using Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Query;

namespace Shipra.Backend.API.Web.Api;
public class WhatsappController : BaseApiController
{
  public WhatsappController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region Command
  [HttpPost("ActivateWhatsappProcess")]
  public async Task<ActionResult> ActivateWhatsappProcess([FromBody] ActivateWhatsappProcessCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion



  #region Query
  [HttpPost("GetAllWhatsappActivate")]
  public async Task<ActionResult> GetAllWhatsappActivate([FromBody] GetAllWhatsappActivateQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetWhatsappActivateById")]
  public async Task<ActionResult> GetWhatsappActivateById([FromQuery] GetWhatsappActivateByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllWhatsappLookupForSelection")]
  public async Task<ActionResult> GetAllWhatsappLookupForSelection(CancellationToken cancellationToken = default)
  {
    GetAllWhatsappLookupForSelectionQuery request = new GetAllWhatsappLookupForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
