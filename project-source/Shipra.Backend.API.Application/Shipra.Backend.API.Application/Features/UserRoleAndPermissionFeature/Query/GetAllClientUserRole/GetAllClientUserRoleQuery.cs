using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.UserRoleAndPermissionFeature.Query.GetAllClientUserRole;
public class GetAllClientUserRoleQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllClientUserRoleQueryHandler : RequestHandlerBase<GetAllClientUserRoleQuery, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;

  public GetAllClientUserRoleQueryHandler(IPermissionRepository permissionRepository, IServiceProvider serviceProvider, ILogger<GetAllClientUserRoleQueryHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllClientUserRoleQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var allUserRoles = await _permissionRepository.GetAllClientUserRole(_currentUser.ClientId!);
      if (allUserRoles.Count == 0)
      {
        #region Permission lookups
        var allUserRoleList = await _permissionRepository.GetAllUserRole();
        foreach (var oUserRole in allUserRoleList)
        {
          ClientUserRole clientUserRole = ClientUserRole.Create(oUserRole.RoleName, oUserRole.RoleDescription, _currentUser.ClientId!, true);
          await _permissionRepository.CreateClientUserRole(clientUserRole);

          var allGroupPermission = await _permissionRepository.GetAllRolePermissionGroupDefaultsByRoleId(oUserRole.RoleId);
          foreach (var oRolePermissionGroup in allGroupPermission)
          {
            var oClientRolePermissionGroup = ClientRolePermissionGroup.Create(clientUserRole.ClientUserRoleId, oRolePermissionGroup.RolePermissionGroupId, _currentUser.ClientId!);
            await _permissionRepository.CreateClientRolePermissionGroup(oClientRolePermissionGroup);
          }
        }
        #endregion
      }

      var obj = ClientUserRole.AddDefault();
      allUserRoles?.Add(obj);
      var selectedList = allUserRoles!.Select(x => new { x.ClientUserRoleId, x.RoleName,x.RoleDescription,x.IsDefault }).OrderBy(x => x.ClientUserRoleId).ToList();
      serviceResult = new ServiceResultDTO(selectedList);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
