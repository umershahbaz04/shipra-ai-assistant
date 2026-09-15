using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Common.Security;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.AddUpdateClientRolePermissionGroup;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.CreateClientUserRole;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.CreatePermissionGroup;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.UpdateControllerAndActionsRecord;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.UpdatePermissionAction;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientMenuByRoleId;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientRolePermissionGroup;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientUserRole;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllPermissionActionForSelection;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllPermissionGroupLookup;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.SaveMenuItemPermissions;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetMenuItemPermissions;
using Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllMenuItemPermissions;
using Shipra.Backend.API.Web.Common;

namespace Shipra.Backend.API.Web.Api;
[Authorize]
public class PermissionController : BaseApiController
{
  public PermissionController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  #region command
  [HttpPost("CreateClientUserRole")]
  public async Task<ActionResult> CreateClientUserRole([FromBody] CreateClientUserRoleCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpPost("AddUpdateClientRolePermissionGroup")]
  public async Task<ActionResult> AddUpdateClientRolePermissionGroup([FromBody] AddUpdateClientRolePermissionGroupCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("UpdatePermissionAction")]
  public async Task<ActionResult> UpdatePermissionAction([FromBody] UpdatePermissionActionCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }



  [HttpPost("CreatePermissionGroup")]
  public async Task<ActionResult> CreatePermissionGroup([FromBody] CreatePermissionGroupCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  #endregion

  [HttpGet("GetAllClientUserRole")]
  public async Task<ActionResult> GetAllClientUserRole(CancellationToken cancellationToken = default)
  {
    GetAllClientUserRoleQuery request = new GetAllClientUserRoleQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
  [HttpGet("GetAllClientRolePermissionGroup")]
  public async Task<ActionResult> CheckAllPermissionActionExist([FromQuery] GetAllClientRolePermissionGroupQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }  
  [HttpGet("GetAllClientMenuByRoleId")]
  public async Task<ActionResult> GetAllClientMenuByRoleId(CancellationToken cancellationToken = default)
  {
    GetAllClientMenuByRoleIdQuery request = new GetAllClientMenuByRoleIdQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllPermissionActionForSelection")]
  public async Task<ActionResult> GetAllPermissionActionForSelection(CancellationToken cancellationToken = default)
  {
    GetAllPermissionActionForSelectionQuery request = new GetAllPermissionActionForSelectionQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllPermissionGroupLookup")]
  public async Task<ActionResult> GetAllPermissionGroupLookup(CancellationToken cancellationToken = default)
  {
    GetAllPermissionGroupLookupQuery request = new GetAllPermissionGroupLookupQuery();
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("UpdateControllerAndActionsRecord")]
  public async Task<ActionResult> UpdateControllerAndActionsRecord(CancellationToken cancellationToken = default)
  {
    var mvcControllers = ControllerScanner.GetMvcControllersAndActions();
    UpdateControllerAndActionsRecordCommand request = new();
    request.mvcControllers = mvcControllers;
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("SaveMenuItemPermissions")]
  public async Task<ActionResult> SaveMenuItemPermissions([FromBody] SaveMenuItemPermissionsCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetMenuItemPermissions")]
  public async Task<ActionResult> GetMenuItemPermissions([FromQuery] GetMenuItemPermissionsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("GetAllMenuItemPermissions")]
  public async Task<ActionResult> GetAllMenuItemPermissions([FromQuery] GetAllMenuItemPermissionsQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
