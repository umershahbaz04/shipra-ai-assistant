using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;
using Shipra.Backend.API.Application.Features.MetaFieldFeature.Query.GetAllEntityMetaFieldLookup;
using Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Command.ActivateWhatsappProcess;
using Shipra.Backend.API.Application.Features.WhatsappprocessFeatures.Query;
using Shipra.Backend.API.Application.Features.MetaFieldFeature.Query.GetMetaFieldsByOrderIds;

namespace Shipra.Backend.API.Web.Api;
public class MetaFieldController : BaseApiController
{
  public MetaFieldController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region Command
  [HttpPost("CreateUpdateClientMetaField")]
  public async Task<ActionResult> CreateUpdateClientMetaField([FromBody] CreateUpdateClientMetaFieldCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region Query
  [HttpGet("GetAllEntityMetaFieldLookup")]
  public async Task<ActionResult> GetAllEntityMetaFieldLookup(CancellationToken cancellationToken = default)
  {
    GetAllEntityMetaFieldLookupQuery request = new GetAllEntityMetaFieldLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  [HttpGet("GetMetaFieldByEntityId")]
  public async Task<ActionResult> GetMetaFieldByEntityId(string orderId, CancellationToken cancellationToken = default)
  {
    var request = new GetMetaFieldByEntityIdQuery
    {
      OrderId = orderId
    };
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetMetaFieldsByOrderIds")]
  public async Task<ActionResult> GetMetaFieldsByOrderIds([FromBody] GetMetaFieldsByOrderIdsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateMetaFieldForOrders")]
  public async Task<ActionResult> UpdateMetaFieldForOrders([FromBody] UpdateMetaFieldForOrdersCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
