using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Infrastructure.Services.Permissions;
public class DynamicPermissionService : IDynamicPermissionService
{
  private readonly IPermissionRepository _permissionRepository;

  public DynamicPermissionService(IPermissionRepository permissionRepository)
  {
    _permissionRepository = permissionRepository;
  }

  public async Task<List<PermissionUserResponseModel>> GetPermissionsByUserIdAsync(ClientId clientId, int roleId)
  {
    return await _permissionRepository.GetPermissionsByUserIdAsync(clientId, roleId); 
  } 
  public async Task<List<PermissionUserResponseModel>> GetGivenPermissionsByUserIdAsync(ClientId clientId, int roleId)
  {
    return await _permissionRepository.GetGivenPermissionsByUserIdAsync(clientId, roleId); 
  } 
}
