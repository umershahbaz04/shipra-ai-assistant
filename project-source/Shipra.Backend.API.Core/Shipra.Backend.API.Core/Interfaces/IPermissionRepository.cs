using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IPermissionRepository
{
  Task<List<PermissionUserResponseModel>> GetPermissionsByUserIdAsync(ClientId clientId, int roleId);
  Task<List<PermissionUserResponseModel>> GetGivenPermissionsByUserIdAsync(ClientId clientId, int roleId);
  Task<List<PermissionAction>> GetAllPermissionActions();
  Task<PermissionAction?> GetPermissionActionByIdAsync(string? controllerName, string? actionName);
  Task<bool> CreatePermissionActions(List<PermissionAction> permissions);
  Task<bool> UpdatePermissionAction(PermissionAction oPermissionAction);
  Task<bool> CreatePermissionGroupAsync(PermissionGroupLookup permissionGroupLookup);


  #region lookups 
  Task<dynamic> GetAllClientRolePermissionGroup(ClientId clientId,int clientRoleId);
  Task<ClientUserRole> CreateClientUserRole(ClientUserRole clientUserRole);
  Task<List<ClientUserRole>> GetAllClientUserRole(ClientId clientId);
  Task<bool> CreateClientRolePermissionGroup(ClientRolePermissionGroup clientRolePermissionGroup);

  Task<bool> DeleteClientRolePermissionGroup(ClientRolePermissionGroup clientRolePermissionGroup);


  Task<List<PermissionAction>> GetAllPermissionActionsByPermissionGroupId(int rolePermissionGroupId);
  Task<List<UserRoleLookup>> GetAllUserRole();
  Task<List<RolePermissionGroupDefault>> GetAllRolePermissionGroupDefaultsByRoleId(int roleId);
  Task<ClientRolePermissionGroup?> GetClientRolePermissionGroupById(int clientRolePgid);

  #endregion
  #region menu
  Task<dynamic> GetAllClientMenuByRoleId(ClientId clientId, int roleId);
  Task<PermissionAction> GetPermissionActionBy(string controller, string action);
  #endregion
  Task<ClientUserRole?> GetClientUserRoleByName(string salePerson, ClientId clientId); 
  Task<ClientUserRole?> GetClientUserRoleById(int clientUserRoleId, ClientId clientId);

  Task<List<PermissionAction>> GetAllPermissionActionForSelection();
  Task<List<PermissionGroupLookup>> GetAllPermissionGroupLookup();
  Task<List<ClientMenuPermissionResponseModel>> GetMenuItemPermissions(ClientId clientId, long clientUserRoleId);
  Task<List<ClientMenuPermissionResponseModel>> GetAllMenuItemPermissions(ClientId clientId, long clientUserRoleId);
  Task<bool> SaveMenuItemPermissions(ClientId clientId, long clientUserRoleId, List<int> menuItemIds, List<int> menuIds, int employeeId);
  Task<bool> IsAdminRole(long clientUserRoleId, ClientId clientId);
}
