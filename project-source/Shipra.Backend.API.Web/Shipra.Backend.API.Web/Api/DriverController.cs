using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.DriverFeatures.Command.CreateDriver;
using Shipra.Backend.API.Application.Features.DriverFeatures.Command.CreateDriverCTSSetting;
using Shipra.Backend.API.Application.Features.DriverFeatures.Command.DeleteDriver;
using Shipra.Backend.API.Application.Features.DriverFeatures.Command.DeleteDriverCTSSetting;
using Shipra.Backend.API.Application.Features.DriverFeatures.Command.UpdateDriver;
using Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetAllDriverCtssetting;
using Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetAllDrivers;
using Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetDriverById;
using Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetDriversForSelection;

namespace Shipra.Backend.API.Web.Api;

[Authorize]
public class DriverController : BaseApiController
{
  public DriverController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  #region command
  [HttpPost("CreateDriver")]
  public async Task<ActionResult> CreateDriver([FromBody] CreateDriverCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("UpdateDriver")]
  public async Task<ActionResult> UpdateDriver([FromBody] UpdateDriverCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteDriver")]
  public async Task<ActionResult> DeleteDriver([FromBody] DeleteDriverCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #region driver tracking status
  [HttpPost("CreateDriverCtssetting")]
  public async Task<ActionResult> CreateDriverCtssetting([FromBody] CreateDriverCTSSettingCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("DeleteDriverCtssetting")]
  public async Task<ActionResult> DeleteDriverCtssetting([FromBody] DeleteDriverCTSSettingCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
  #endregion

  #region query
  [HttpGet("GetDriversForSelection")]
  public async Task<ActionResult> GetDriversForSelection([FromQuery] GetDriversForSelectionQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("GetAllDrivers")]
  public async Task<ActionResult> GetAllDrivers([FromBody] GetAllDriversQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetDriverById")]
  public async Task<ActionResult> GetDriverById([FromQuery] GetDriverByIdQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  
  [HttpGet("GetAllDriverCtssetting")]
  public async Task<ActionResult> GetAllDriverCtssetting([FromQuery] GetAllDriverCTSSettingQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion
}
