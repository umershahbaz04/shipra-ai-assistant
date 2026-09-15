using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Commands.CreateProductStationTransfer;
using Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Query.GetAllProductStationTransfer;
using Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Query.GetProductStationTransferById;
using Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Query.GetTotalNoOfProductStationTransferQuery;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class ProductStationTransferController : BaseApiController
{
  public ProductStationTransferController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("CreateProductStationTransfer")]
  public async Task<ActionResult> CreateProductStationTransfer([FromBody] CreateProductStationTransferCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #region Query
  [HttpPost("GetProductStationTransfer")]
  public async Task<ActionResult> GetProductStationTransfer([FromBody] GetAllProductStationTransferQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetProductStationTransferById")]
  public async Task<ActionResult> GetProductStationTransferById([FromQuery] GetProductStationTransferByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllTypeCountProductStationTransfer")]
  public async Task<ActionResult> GetAllTypeCountProductStationTransfer([FromBody] GetAllTypeCountProductStationTransferQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
