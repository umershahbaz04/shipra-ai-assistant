using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.SaveMenuItemPermissions;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.CheckAllPermissionActionExist;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientUserRole;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllMenuItemPermissions;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetMenuItemPermissions;

namespace Shipra.Backend.API.Web.Api.OpenApis;

public class PermissionController : BaseApiController
{
  public PermissionController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }
  [HttpPost("CheckAllPermissionActionExist")]
  public async Task<ActionResult> CheckAllPermissionActionExist([FromBody] CheckAllPermissionActionExistQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllClientUserRoleForAdmin")]
  public async Task<ActionResult> GetAllClientUserRoleForAdmin(CancellationToken cancellationToken = default)
  {
    GetAllClientUserRoleQuery request = new GetAllClientUserRoleQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetMenuItemPermissionsForAdmin")]
  public async Task<ActionResult> GetMenuItemPermissionsForAdmin([FromQuery] GetMenuItemPermissionsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("SaveMenuItemPermissionsForAdmin")]
  public async Task<ActionResult> SaveMenuItemPermissionsForAdmin([FromBody] SaveMenuItemPermissionsCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);

  }
  [HttpGet("GetAllMenuItemPermissionsForAdmin")]
  public async Task<ActionResult> GetAllMenuItemPermissionsForAdmin([FromQuery] GetAllMenuItemPermissionsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
