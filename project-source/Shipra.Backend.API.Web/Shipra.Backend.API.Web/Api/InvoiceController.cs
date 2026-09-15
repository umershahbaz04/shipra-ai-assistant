using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.InvoiceFeature.Query.GetAllInvoiceHistory;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class InvoiceController : BaseApiController
{
  public InvoiceController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region invocie 
  #region query
  [HttpPost("GetAllInvoiceHistory")]
  public async Task<ActionResult> GetAllInvoiceHistory([FromBody] GetAllInvoiceHistoryQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #endregion
}
