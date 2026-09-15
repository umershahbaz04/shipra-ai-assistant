using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.CarrierPendingForReturnReportFeatures.Command.CreateCarrierPendingForReturn;
using Shipra.Backend.API.Application.Features.CarrierPendingForReturnReportFeatures.Query.GetAllCarrierPendingForReturnReport;

namespace Shipra.Backend.API.Web.Api;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CarrierPendingForReturnReportController : BaseApiController
{
  public CarrierPendingForReturnReportController(IServiceProvider serviceProvider) : base(serviceProvider)
  {

  }
  #region command

  [HttpPost("CreateCarrierPendingForReturn")]
  public async Task<ActionResult> CreateCarrierReturnReport([FromBody] CreateCarrierPendingForReturnCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  #endregion

  #region query 
 
  [HttpPost("GetAllPendingForReturnShipment")]
  public async Task<ActionResult> GetReturnRerports([FromBody] GetAllCarrierPendingForReturnReportQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
   
  #endregion

}
