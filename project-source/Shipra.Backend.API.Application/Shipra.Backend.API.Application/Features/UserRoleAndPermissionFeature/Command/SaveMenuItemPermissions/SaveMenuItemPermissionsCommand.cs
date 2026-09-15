using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Command.SaveMenuItemPermissions;

public class SaveMenuItemPermissionsCommand : IRequest<ServiceResultDTO>
{
  public long ClientUserRoleId { get; set; }
  public List<int> MenuItemIds { get; set; } = new();
  public List<int> MenuIds { get; set; } = new();
}

public class SaveMenuItemPermissionsCommandHandler : RequestHandlerBase<SaveMenuItemPermissionsCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public SaveMenuItemPermissionsCommandHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<SaveMenuItemPermissionsCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SaveMenuItemPermissionsCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      if (request.ClientUserRoleId <= 0)
        throw new Exception("ClientUserRoleId is required and must be greater than zero.");
      if (_currentUser.RoleId.GetValueOrDefault() == 0)
      {
        _currentUser.RoleId = 1;
      }
      bool isLoggedInUserAdmin = await _permissionRepository.IsAdminRole(_currentUser.RoleId.GetValueOrDefault(), _currentUser.ClientId!);

      if (!isLoggedInUserAdmin)
        throw new Exception("Only Admin or Super Admin users are allowed to modify permissions.");

      var data = await _permissionRepository.SaveMenuItemPermissions(
          _currentUser.ClientId!, 
          request.ClientUserRoleId, 
          request.MenuItemIds, 
          request.MenuIds,
          0
      );

      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Data = data,
        Message = "Permissions saved successfully"
      });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
