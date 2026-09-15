using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class DynamicPermissionAppService
{
  private readonly IDynamicPermissionService _dynamicPermissionService;

  public DynamicPermissionAppService(IDynamicPermissionService dynamicPermissionService)
  {
    _dynamicPermissionService = dynamicPermissionService;
  }

  public async Task<List<PermissionUserResponseModel>> GetPermissionsForUserAsync(ClientId clientId,int roleId)
  {
    return await _dynamicPermissionService.GetPermissionsByUserIdAsync(clientId,roleId);
  }
  public async Task<List<PermissionUserResponseModel>> GetGivenPermissionsByUserIdAsync(ClientId clientId,int roleId)
  {
    return await _dynamicPermissionService.GetGivenPermissionsByUserIdAsync(clientId,roleId);
  }
}
