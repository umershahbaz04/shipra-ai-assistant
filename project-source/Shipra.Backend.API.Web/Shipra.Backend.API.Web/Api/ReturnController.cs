using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.ActiveDeactiveClientReturnReason;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.CreateClientReturnReason;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.CreateReturn;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Commands.UpdateReturnStatus;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllClientReturnReason;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllClientReturnReasonForSelection;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllOrderReturn;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllRefundTypeLookupForSelection;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetAllReturnStatusLookupForSelection;
using Shipra.Backend.API.Application.Features.ReturnFeatures.Query.GetClientReturnReasonId;

namespace Shipra.Backend.API.Web.Api;


[Authorize]
public class ReturnController : BaseApiController
{
  public ReturnController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command

  [HttpPost("CreateClientReturnReason")]
  public async Task<ActionResult> CreateClientReturnReason([FromBody] CreateClientReturnReasonCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateClientReturnReason")]
  public async Task<ActionResult> UpdateClientReturnReason([FromBody] UpdateClientReturnReasonCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ActiveDeactiveClientReturnReason")]
  public async Task<ActionResult> ActiveDeactiveClientReturnReason([FromBody] ActiveDeactiveClientReturnReasonCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateReturnStatus")]
  public async Task<ActionResult> UpdateReturnStatus([FromBody] UpdateReturnStatusCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion  
  #region query
  [HttpGet("GetClientReturnReasonId")]
  public async Task<ActionResult> GetClientReturnReasonId([FromQuery] GetClientReturnReasonIdQuery request, CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllClientReturnReason")]
  public async Task<ActionResult> GetAllClientReturnReason([FromBody] GetAllClientReturnReasonQuery request, CancellationToken cancellationToken = default)
  { 
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetAllOrderReturn")]
  public async Task<ActionResult> GetAllOrderReturn([FromBody] GetAllOrderReturnQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllRefundTypeForSelection")]
  public async Task<ActionResult> GetAllRefundTypeLookupForSelection(CancellationToken cancellationToken = default)
  {
    GetAllRefundTypeLookupForSelectionQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  
  [HttpGet("GetAllClientReturnReasonForSelection")]
  public async Task<ActionResult> GetAllClientReturnReasonForSelection(CancellationToken cancellationToken = default)
  {
    GetAllClientReturnReasonForSelectionQuery request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  [HttpGet("GetAllReturnStatusForSelection")]
  public async Task<ActionResult> GetAllReturnStatusForSelection(CancellationToken cancellationToken = default)
  {
    GetAllReturnStatusLookupForSelection request = new();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }


  #endregion

}




