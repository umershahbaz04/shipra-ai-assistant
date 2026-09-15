using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IDynamicPermissionService
{
  Task<List<PermissionUserResponseModel>> GetPermissionsByUserIdAsync(ClientId clientId, int roleId); 
  Task<List<PermissionUserResponseModel>> GetGivenPermissionsByUserIdAsync(ClientId clientId, int roleId); 
}
