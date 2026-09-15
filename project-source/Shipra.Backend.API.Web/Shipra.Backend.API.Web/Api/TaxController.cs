using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.SmsFeature.Command.SendSms;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.ActivateSMSProcess;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Command.DeleteSMSActivate;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSActivate;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetAllSMSLookupForSelection;
using Shipra.Backend.API.Application.Features.SMSProcessFeature.Query.GetSMSActivateById;
using Shipra.Backend.API.Application.Features.TaxFeatures.Command.CreateClientTax;
using Shipra.Backend.API.Application.Features.TaxFeatures.Command.IsEnableTax;
using Shipra.Backend.API.Application.Features.TaxFeatures.Command.UpdateClientTax;
using Shipra.Backend.API.Application.Features.TaxFeatures.Query.GetAllClientTax;
using Shipra.Backend.API.Application.Features.TaxFeatures.Query.GetAllClientTaxForSelection;
using Shipra.Backend.API.Application.Features.TaxFeatures.Query.GetClientTaxById;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class TaxController : BaseApiController
{
  public TaxController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("CreateClientTax")]
  public async Task<ActionResult> CreateClientTax([FromBody] CreateClientTaxCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("EnableDisableTax")]
  public async Task<ActionResult> EnableDisableTax([FromBody] IsEnableTaxCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateClientTax")]
  public async Task<ActionResult> UpdateClientTax([FromBody] UpdateClientTaxCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region query
  [HttpPost("GetAllClientTax")]
  public async Task<ActionResult> GetAllClientTax([FromBody] GetAllClientTaxQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  [HttpPost("GetClientTaxById")]
  public async Task<ActionResult> GetClientTaxById([FromBody] GetClientTaxByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
   
  [HttpGet("GetAllTaxForSelection")]
  public async Task<ActionResult> GetAllTaxForSelection(CancellationToken cancellationToken = default)
  {
    GetAllClientTaxForSelectionQuery request = new GetAllClientTaxForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  } 
  #endregion

}
