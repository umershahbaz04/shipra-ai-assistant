using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.DocumentFeatures.Query.GetDocumentSettingForClient;
using Shipra.Backend.API.Application.Features.DocumentFeatures.Query.GetValidateDocumentSetting;
using Shipra.Backend.API.Application.Features.MiscFeatures.Command;
using Shipra.Backend.API.Application.Features.MiscFeatures.Query.GetMiscSetting;
using Shipra.Backend.API.Application.Features.TaxFeatures.Command.UpdateClientTax;
using Shipra.Backend.API.Application.Features.TaxFeatures.Query.GetClientTaxById;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class DocumentSettingController : BaseApiController
{
  public DocumentSettingController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command 
  [HttpPost("UpdateDocumentSetting")]
  public async Task<ActionResult> UpdateClientTax([FromBody] UpdateDocumentSettingCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region query 
  [HttpGet("GetDocumentSetting")]
  public async Task<ActionResult> GetMiscSetting(CancellationToken cancellationToken = default)
  {
    GetDocumentSettingQuery request = new GetDocumentSettingQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  [HttpGet("GetClientDocumentSetting")]
  public async Task<ActionResult> GetDocumentSettingForClient(CancellationToken cancellationToken = default)
  {
    GetDocumentSettingForClientQuery request = new GetDocumentSettingForClientQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  
  [HttpGet("GetValidateDocumentSetting")]
  public async Task<ActionResult> GetValidateDocumentSetting(CancellationToken cancellationToken = default)
  {
    GetValidateDocumentSettingQuery request = new GetValidateDocumentSettingQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  #endregion

}
