using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.ProductStationFeatures.Commands.ActiveProductStation;
using Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.CreateProductStation;
using Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.DeleteProductStation;
using Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.UpdateProductStation;
using Shipra.Backend.API.Application.Features.ProductStationtFeatures.Query.GetAllProductStations;
using Shipra.Backend.API.Application.Features.ProductStationtFeatures.Query.GetProductStationById;

namespace Shipra.Backend.API.Web.Api;

[Authorize]
public class ProductStationController : BaseApiController
{
  public ProductStationController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("CreateProductStation")]
  public async Task<ActionResult> CreateProductStation([FromBody] CreateProductStationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdateProductStation")]
  public async Task<ActionResult> UpdateProductStation([FromBody] UpdateProductStationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteProductStationById")]
  public async Task<ActionResult> DeleteProductStationById([FromBody] DeleteProductStationCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("ActiveProductStationById")]
  public async Task<ActionResult> ActiveProductStationById([FromBody] ActiveProductStationByIdCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  #region query

  [HttpGet("GetProductStationById")]
  public async Task<ActionResult> GetProductStationById([FromQuery] GetProductStationByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetProductStations")]
  public async Task<ActionResult> GetProductStations([FromBody] GetAllProductStationsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
