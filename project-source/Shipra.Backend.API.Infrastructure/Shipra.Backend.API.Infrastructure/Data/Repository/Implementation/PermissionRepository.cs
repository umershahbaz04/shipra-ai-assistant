
using System.Dynamic;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;
using Shipra.Backend.API.Core.MenuAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class PermissionRepository : IPermissionRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public PermissionRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  public async Task<List<PermissionAction>> GetAllPermissionActions()
  {
    return await _context.PermissionActions.Where(p => p.Active.HasValue && p.Active.Value).ToListAsync();
  }
  public async Task<bool> CreatePermissionActions(List<PermissionAction> permissions)
  {
    var existingPermissions = await _context.PermissionActions
        .Select(x => new { x.ActionName, x.ControllerName })
        .ToListAsync();

    var permissionsToAdd = permissions
        .Where(p => !existingPermissions.Any(e =>
            e.ActionName == p.ActionName &&
            e.ControllerName == p.ControllerName))
        .ToList();

    if (!permissionsToAdd.Any())
      return false;

    await _context.PermissionActions.AddRangeAsync(permissionsToAdd);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<PermissionUserResponseModel>> GetPermissionsByUserIdAsync(ClientId clientId, int roleId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@ClientId", clientId.Value!.ToString());

      string query = $@"SELECT pa.ControllerName,
       pa.ActionName,
       ISNULL(pa.GroupAssigned,0) GroupAssigned,
       cur.RoleName,
       ISNULL(pgl.GroupName,'') AS GroupName,
       ISNULL(pgl.GroupParent,'') AS GroupParent
FROM dbo.ClientUserRole AS cur
    INNER JOIN dbo.ClientRolePermissionGroup AS crpg
        ON crpg.ClientUserRoleId = cur.ClientUserRoleId
           AND cur.ClientUserRoleId = {roleId} 
           AND crpg.Active = 1
           And (cur.ClientId = @ClientId)
    INNER JOIN dbo.PermissionGroupLookup AS pgl
        ON pgl.PermissionGroupId = crpg.PermissionGroupId
           AND pgl.Active = 1
    RIGHT JOIN dbo.PermissionAction AS pa
        ON pa.PermissionGroupId = crpg.PermissionGroupId
           AND pa.Active = 1 ";

      string whereStart = @$"WHERE ( 1=1  ";
      string whereEnd = ")";

      //if (clientId is not null)
      //{
      //  whereStart += "And (cur.ClientId = @ClientId) ";
      //}

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync<PermissionUserResponseModel>(queryData, dynamicParams);
      var dataList = data.ToList();
      //dynamic result = new ExpandoObject();
      //int totalCount = 0;
      //if (dataList.Count > 0)
      //{
      //  var firstRecord = dataList.FirstOrDefault();
      //  totalCount = firstRecord?.TotalCount; 
      //}

      //result.TotalCount = totalCount;
      //result.list = dataList;
      return dataList;
    }

  }
  public async Task<List<PermissionUserResponseModel>> GetGivenPermissionsByUserIdAsync(ClientId clientId, int roleId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@ClientId", clientId.Value!.ToString());
      dynamicParams.Add("@ClientRoleId", roleId);

      string query = $@"SELECT 
				pa.ControllerName,
				pa.ActionName,
				ISNULL(urpg.Active, 0) AS GroupAssigned,
				url.RoleName,
				ISNULL(pgl.GroupName, '') AS GroupName,
				ISNULL(pgl.GroupParent, '') AS GroupParent,
				 CASE 
									   WHEN urpg.PermissionGroupId IS NOT NULL THEN 1 
									   ELSE 0 
								   END AS HavePermission
FROM dbo.PermissionAction AS pa
LEFT JOIN dbo.PermissionGroupLookup AS pgl
				ON pgl.PermissionGroupId = pa.PermissionGroupId
				AND pgl.Active = 1
LEFT JOIN dbo.ClientRolePermissionGroup AS urpg
				ON urpg.PermissionGroupId = pa.PermissionGroupId
				AND urpg.ClientUserRoleId = @ClientRoleId
				AND urpg.Active = 1
LEFT JOIN dbo.ClientUserRole AS url
				ON url.ClientUserRoleId = urpg.ClientUserRoleId ";

      string whereStart = @$"WHERE ( 1=1 AND (pgl.GroupParent = pa.GroupParent OR pgl.GroupParent IS NULL) AND urpg.ClientUserRoleId = @ClientRoleId ";
      string whereEnd = ")";

      //if (clientId is not null)
      //{
      //  whereStart += "And (cur.ClientId = @ClientId) ";
      //}

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync<PermissionUserResponseModel>(queryData, dynamicParams);
      var dataList = data.ToList();
      //dynamic result = new ExpandoObject();
      //int totalCount = 0;
      //if (dataList.Count > 0)
      //{
      //  var firstRecord = dataList.FirstOrDefault();
      //  totalCount = firstRecord?.TotalCount; 
      //}

      //result.TotalCount = totalCount;
      //result.list = dataList;
      return dataList;
    }

  }

  public async Task<List<ClientUserRole>> GetAllClientUserRole(ClientId clientId)
  {
    return await _context.ClientUserRolees.Where(x => x.ClientId == clientId && !x.RoleName!.Contains("Super Admin")).ToListAsync();
  }

  public async Task<dynamic> GetAllClientRolePermissionGroup(ClientId clientId, int clientRoleId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT ISNULL(crpg.ClientRolePGId, 0) AS ClientRolePGId,
                               rd.PermissionGroupId,
                               rd.GroupName,
                               rd.GroupParent,
                               CASE
                                   WHEN crpg.ClientUserRoleId IS NOT NULL THEN
                                       1
                                   ELSE
                                       0
                               END AS HavePermission
                        FROM dbo.ClientRolePermissionGroup AS crpg
                            INNER JOIN dbo.ClientUserRole AS cur
                                ON cur.ClientUserRoleId = crpg.ClientUserRoleId AND cur.Active =1
                             AND cur.ClientUserRoleId = {clientRoleId} 
                            AND cur.ClientId = '{clientId.Value!.ToString()}' AND cur.Active =1
                            RIGHT JOIN
                            (
                                SELECT pgl.PermissionGroupId,
                                       pgl.GroupName,
                                       pgl.GroupParent
                                FROM dbo.PermissionGroupLookup AS pgl
                            ) AS rd
                                ON rd.PermissionGroupId = crpg.PermissionGroupId ";

      string whereStart = @$"WHERE (  1=1 ";
      string whereEnd = ")";

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      var dataList = data.ToList();

      var groupedResults = data.GroupBy(r => r.GroupParent)
                            .Select(group => new
                            {
                              GroupName = group.Key,
                              Items = group.Select(item => new
                              {
                                ClientRolePGId = item.ClientRolePGId,
                                PermissionGroupId = item.PermissionGroupId,
                                GroupName = item.GroupName,
                                GroupParent = item.GroupParent,
                                HavePermission = item.HavePermission
                              }).ToList()
                            }).ToList();
      return groupedResults;
    }

  }

  public Task<List<PermissionAction>> GetAllPermissionActionsByPermissionGroupId(int rolePermissionGroupId)
  {
    throw new NotImplementedException();
  }

  #region lookups
  public async Task<List<UserRoleLookup>> GetAllUserRole()
  {
    return await _context.UserRoleLookups.ToListAsync();
  }
  public async Task<List<RolePermissionGroupDefault>> GetAllRolePermissionGroupDefaultsByRoleId(int roleId)
  {
    return await _context.RolePermissionGroupDefaults.Where(x => x.RoleId == roleId).ToListAsync();
  }

  #endregion
  public async Task<ClientRolePermissionGroup?> GetClientRolePermissionGroupById(int clientRolePgid)
  {
    return await _context.ClientRolePermissionGroups.FirstOrDefaultAsync(x => x.ClientRolePgid == clientRolePgid)!;
  }
  public async Task<ClientUserRole> CreateClientUserRole(ClientUserRole clientUserRole)
  {
    var oClientUserRole = await _context.ClientUserRolees.FirstOrDefaultAsync(x => x.RoleName!.Trim().ToLower() == clientUserRole.RoleName!.Trim().ToLower() && x.ClientId == clientUserRole.ClientId);
    if (oClientUserRole is null)
    {
      await _context.ClientUserRolees.AddAsync(clientUserRole);
      _context.SaveChanges();
      return clientUserRole;
    }
    else
    {
      return oClientUserRole;
    }
  }
  public async Task<bool> CreateClientRolePermissionGroup(ClientRolePermissionGroup clientRolePermissionGroup)
  {
    var oClientRolePermissionGroup = await _context.ClientRolePermissionGroups.FirstOrDefaultAsync(x => x.ClientUserRoleId == clientRolePermissionGroup.ClientUserRoleId && x.ClientId! == clientRolePermissionGroup.ClientId && x.PermissionGroupId == clientRolePermissionGroup.PermissionGroupId);
    if (oClientRolePermissionGroup is null)
    {
      await _context.ClientRolePermissionGroups.AddAsync(clientRolePermissionGroup);
      return _context.SaveChanges() > 0;
    }
    else
    {
      return true;
    }
  }

  public async Task<bool> DeleteClientRolePermissionGroup(ClientRolePermissionGroup clientRolePermissionGroup)
  {
    _context.Remove(clientRolePermissionGroup);
    return await _context.SaveChangesAsync() > 0;
  }

  #region menu
  public async Task<dynamic> GetAllClientMenuByRoleId(ClientId clientId, int roleId)
  {
    bool isRequestedRoleAdmin = await IsAdminRole(roleId, clientId);

    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@ClientId", clientId.Value!.ToString());
      dynamicParams.Add("@ClientUserRoleId", roleId);
      dynamicParams.Add("@IsAdmin", isRequestedRoleAdmin ? 1 : 0);

      string query = @"SELECT 
                          m.MenuId,
                          m.MenuName,
                          m.IsCollapse,
                          m.TabBarTitle AS MenuTabBarTitle,
                          m.RoutePath AS MenuRoutePath,
                          mi.MenuItemName,
                          mi.TabBarTitle AS MenuItemTabBarTitle,
                          mi.RoutePath AS MenuItemRoutePath
                      FROM dbo.Menu m
                      LEFT JOIN dbo.MenuItem mi ON m.MenuId = mi.MenuId AND mi.Active = 1 AND ISNULL(mi.IsDeleted, 0) = 0
                      WHERE m.Active = 1 AND ISNULL(m.IsDeleted, 0) = 0
                        AND (
                            (mi.MenuItemId IS NULL AND EXISTS (
                                SELECT 1 
                                FROM dbo.MenuItemPermissionClient mipc 
                                WHERE mipc.MenuId = m.MenuId 
                                  AND mipc.ClientId = @ClientId 
                                  AND mipc.Active = 1
                                  AND (@IsAdmin = 1 OR mipc.ClientUserRoleId = @ClientUserRoleId)
                            ))
                            OR
                            (mi.MenuItemId IS NOT NULL AND EXISTS (
                                SELECT 1 
                                FROM dbo.MenuItemPermissionClient mipc 
                                WHERE mipc.MenuItemId = mi.MenuItemId 
                                  AND mipc.ClientId = @ClientId 
                                  AND mipc.Active = 1
                                  AND (@IsAdmin = 1 OR mipc.ClientUserRoleId = @ClientUserRoleId)
                            ))
                        )
                      ORDER BY m.DisplayOrder, mi.DisplayOrder";

      var data = await connection.QueryAsync<AllowedMenuQueryResult>(query, dynamicParams);
      var dataList = data.ToList();

      var groupedDataList = dataList
                            .GroupBy(crpg => new { crpg.MenuName, crpg.MenuId })
                            .Select(group =>
                            {
                              var menuItemsList = group
                                .Where(mgi => mgi.MenuItemName != null)
                                .Select(mgi => new AllowedMenuItemResponseModel
                                {
                                  MenuItemName = mgi.MenuItemName,
                                  MenuItemTabBarTitle = mgi.MenuItemTabBarTitle,
                                  RoutePath = mgi.MenuItemRoutePath,
                                  HasPermission = true
                                }).ToList();

                              return new AllowedMenuResponseModel
                              {
                                MenuName = group.Key.MenuName,
                                MenuId = group.Key.MenuId,
                                IsCollapse = group.First().IsCollapse,
                                MenuTabBarTitle = group.First().MenuTabBarTitle,
                                RoutePath = group.First().MenuRoutePath,
                                MenuItems = menuItemsList,
                                HasPermission = menuItemsList.Any() || group.First().MenuItemName == null
                              };
                            }).ToList();

      foreach (var item in groupedDataList)
      {
        item.MenuOtherRoutes = await GetAllMenuOtherRoutesByMenuId(item.MenuId);
      }

      return groupedDataList;
    }
  }

  private async Task<List<MenuOtherRouteDto>> GetAllMenuOtherRoutesByMenuId(int menuId)
  {
    var data = await _context.MenuOtherRoutes.Where(x => x.MenuId == menuId).ToListAsync();
    return data.Select(x => new MenuOtherRouteDto { TabBarTitle = x.TabBarTitle, RoutePath = x.RoutePath }).ToList();
  }
  public async Task<PermissionAction> GetPermissionActionBy(string controller, string action)
  {
    var data = await _context.PermissionActions.FirstOrDefaultAsync(x => x.ControllerName == controller && x.ActionName == action);
    return data!;
  }

  #endregion
  public async Task<ClientUserRole?> GetClientUserRoleByName(string roleName, ClientId clientId)
  {
    var userRole = await _context.ClientUserRolees.FirstOrDefaultAsync(x => x.RoleName!.Trim()!.ToLower() == roleName!.Trim()!.ToLower() && x.ClientId == clientId);
    return userRole;
  }
  public async Task<ClientUserRole?> GetClientUserRoleById(int clientUserRoleId, ClientId clientId)
  {
    var userRole = await _context.ClientUserRolees.FirstOrDefaultAsync(x => x.ClientUserRoleId  == clientUserRoleId && x.ClientId == clientId);
    return userRole;
  }

  public async Task<bool> IsAdminRole(long clientUserRoleId, ClientId clientId)
  {
    if (clientUserRoleId == 1) return true;

    var requestedRole = await _context.ClientUserRolees
        .FirstOrDefaultAsync(r => r.ClientUserRoleId == clientUserRoleId && r.ClientId == clientId);

    if (requestedRole != null)
    {
      string? roleName = requestedRole.RoleName?.Trim();
      if (!string.IsNullOrEmpty(roleName))
      {
        if (roleName.Equals("Super Admin", StringComparison.OrdinalIgnoreCase) ||
            roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }
      }
    }

    return false;
  }

  public async Task<List<PermissionAction>> GetAllPermissionActionForSelection()
  {
    return await _context.PermissionActions.ToListAsync();
  }
  public async Task<List<PermissionGroupLookup>> GetAllPermissionGroupLookup()
  {
    return await _context.PermissionGroupLookups.ToListAsync();
  }

  public async Task<PermissionAction?> GetPermissionActionByIdAsync(string? controllerName, string? actionName)
  {
    return await _context.PermissionActions.FirstOrDefaultAsync(p => p.ControllerName == controllerName && p.ActionName == actionName);
  }

  public async Task<bool> UpdatePermissionAction(PermissionAction permissionAction)
  {
    _context.PermissionActions.Update(permissionAction);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> CreatePermissionGroupAsync(PermissionGroupLookup permissionGroupLookup)
  {
    await _context.PermissionGroupLookups.AddAsync(permissionGroupLookup);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<ClientMenuPermissionResponseModel>> GetMenuItemPermissions(ClientId clientId, long clientUserRoleId)
  {
    //bool isRequestedRoleAdmin = await IsAdminRole(clientUserRoleId, clientId);

    // Single Dapper SQL Query with JOINs and EXISTS
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@ClientId", clientId.Value!.ToString());
      dynamicParams.Add("@ClientUserRoleId", clientUserRoleId);

      string query = @"SELECT 
                          m.MenuId,
                          m.MenuName,
                          m.MenuIcon,
                          m.Description AS MenuDescription,
                          m.DisplayOrder AS MenuDisplayOrder,
                          m.IsCollapse,
                          m.TabBarTitle AS MenuTabBarTitle,
                          m.RoutePath AS MenuRoutePath,
                          mi.MenuItemId,
                          mi.MenuItemName,
                          mi.MenuItemIcon,
                          mi.Description AS MenuItemDescription,
                          mi.RoutePath AS MenuItemRoutePath,
                          mi.TabBarTitle AS MenuItemTabBarTitle,
                          mi.DisplayOrder AS MenuItemDisplayOrder,
                          CAST(CASE WHEN mi.MenuItemId IS NOT NULL AND EXISTS (
                                        SELECT 1 
                                        FROM dbo.MenuItemPermissionClient mipc 
                                        WHERE mipc.MenuItemId = mi.MenuItemId 
                                          AND mipc.ClientId = @ClientId 
                                          AND mipc.Active = 1
                                          AND mipc.ClientUserRoleId = @ClientUserRoleId
                                   ) THEN 1 ELSE 0 END AS BIT) AS HasPermission,
                          CAST(CASE WHEN EXISTS (
                                        SELECT 1 
                                        FROM dbo.MenuItemPermissionClient mipc 
                                        WHERE mipc.MenuId = m.MenuId 
                                          AND mipc.ClientId = @ClientId 
                                          AND mipc.Active = 1
                                          AND mipc.ClientUserRoleId = @ClientUserRoleId
                                   ) THEN 1 ELSE 0 END AS BIT) AS MenuHasPermission
                      FROM dbo.Menu m
                      LEFT JOIN dbo.MenuItem mi ON m.MenuId = mi.MenuId AND mi.Active = 1 AND ISNULL(mi.IsDeleted, 0) = 0
                      WHERE m.Active = 1 AND ISNULL(m.IsDeleted, 0) = 0
                        AND EXISTS (
                            SELECT 1 
                            FROM dbo.MenuItemPermissionClient client_mipc 
                            WHERE client_mipc.ClientId = @ClientId 
                              AND client_mipc.Active = 1
                              AND (
                                  (mi.MenuItemId IS NULL AND client_mipc.MenuId = m.MenuId) OR
                                  (mi.MenuItemId IS NOT NULL AND client_mipc.MenuItemId = mi.MenuItemId)
                              )
                        )
                      ORDER BY m.DisplayOrder, mi.DisplayOrder";

      var data = await connection.QueryAsync<FlatMenuPermissionResult>(query, dynamicParams);
      var flatList = data.ToList();

      // Group flat query results in memory to build the hierarchy
      var result = flatList
          .GroupBy(row => new
          {
            row.MenuId,
            row.MenuName,
            row.MenuIcon,
            Description = row.MenuDescription,
            DisplayOrder = row.MenuDisplayOrder,
            row.IsCollapse,
            TabBarTitle = row.MenuTabBarTitle,
            RoutePath = row.MenuRoutePath
          })
          .Select(group =>
          {
            var menuItemsList = group
                .Where(row => row.MenuItemId.HasValue)
                .Select(row => new ClientMenuItemPermissionModel
                {
                  MenuItemId = row.MenuItemId.GetValueOrDefault(),
                  MenuItemName = row.MenuItemName,
                  MenuItemIcon = row.MenuItemIcon,
                  Description = row.MenuItemDescription,
                  MenuId = group.Key.MenuId,
                  RoutePath = row.MenuItemRoutePath,
                  TabBarTitle = row.MenuItemTabBarTitle,
                  DisplayOrder = row.MenuItemDisplayOrder,
                  HasPermission = row.HasPermission
                })
                .ToList();

            return new ClientMenuPermissionResponseModel
            {
              MenuId = group.Key.MenuId,
              MenuName = group.Key.MenuName,
              MenuIcon = group.Key.MenuIcon,
              Description = group.Key.Description,
              DisplayOrder = group.Key.DisplayOrder,
              IsCollapse = group.Key.IsCollapse,
              TabBarTitle = group.Key.TabBarTitle,
              RoutePath = group.Key.RoutePath,
              MenuItems = menuItemsList,
              HasPermission = menuItemsList.Any()
                    ? menuItemsList.Any(mi => mi.HasPermission)
                    : group.First().MenuHasPermission
            };
          })
          .OrderBy(m => m.DisplayOrder)
          .ToList();

      return result;
    }
  }

  public async Task<List<ClientMenuPermissionResponseModel>> GetAllMenuItemPermissions(ClientId clientId, long clientUserRoleId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@ClientId", clientId.Value!.ToString());
      dynamicParams.Add("@ClientUserRoleId", clientUserRoleId);

      string query = @"SELECT 
                          m.MenuId,
                          m.MenuName,
                          m.MenuIcon,
                          m.Description AS MenuDescription,
                          m.DisplayOrder AS MenuDisplayOrder,
                          m.IsCollapse,
                          m.TabBarTitle AS MenuTabBarTitle,
                          m.RoutePath AS MenuRoutePath,
                          mi.MenuItemId,
                          mi.MenuItemName,
                          mi.MenuItemIcon,
                          mi.Description AS MenuItemDescription,
                          mi.RoutePath AS MenuItemRoutePath,
                          mi.TabBarTitle AS MenuItemTabBarTitle,
                          mi.DisplayOrder AS MenuItemDisplayOrder,
                          CAST(CASE WHEN mi.MenuItemId IS NOT NULL AND EXISTS (
                                        SELECT 1 
                                        FROM dbo.MenuItemPermissionClient mipc 
                                        WHERE mipc.MenuItemId = mi.MenuItemId 
                                          AND mipc.ClientId = @ClientId 
                                          AND mipc.Active = 1
                                          AND mipc.ClientUserRoleId = @ClientUserRoleId
                                   ) THEN 1 ELSE 0 END AS BIT) AS HasPermission,
                          CAST(CASE WHEN EXISTS (
                                        SELECT 1 
                                        FROM dbo.MenuItemPermissionClient mipc 
                                        WHERE mipc.MenuId = m.MenuId 
                                          AND mipc.ClientId = @ClientId 
                                          AND mipc.Active = 1
                                          AND mipc.ClientUserRoleId = @ClientUserRoleId
                                   ) THEN 1 ELSE 0 END AS BIT) AS MenuHasPermission
                      FROM dbo.Menu m
                      LEFT JOIN dbo.MenuItem mi ON m.MenuId = mi.MenuId AND mi.Active = 1 AND ISNULL(mi.IsDeleted, 0) = 0
                      WHERE m.Active = 1 AND ISNULL(m.IsDeleted, 0) = 0
                      ORDER BY m.DisplayOrder, mi.DisplayOrder";

      var data = await connection.QueryAsync<FlatMenuPermissionResult>(query, dynamicParams);
      var flatList = data.ToList();

      var result = flatList
          .GroupBy(row => new
          {
            row.MenuId,
            row.MenuName,
            row.MenuIcon,
            Description = row.MenuDescription,
            DisplayOrder = row.MenuDisplayOrder,
            row.IsCollapse,
            TabBarTitle = row.MenuTabBarTitle,
            RoutePath = row.MenuRoutePath
          })
          .Select(group =>
          {
            var menuItemsList = group
                .Where(row => row.MenuItemId.HasValue)
                .Select(row => new ClientMenuItemPermissionModel
                {
                  MenuItemId = row.MenuItemId.GetValueOrDefault(),
                  MenuItemName = row.MenuItemName,
                  MenuItemIcon = row.MenuItemIcon,
                  Description = row.MenuItemDescription,
                  MenuId = group.Key.MenuId,
                  RoutePath = row.MenuItemRoutePath,
                  TabBarTitle = row.MenuItemTabBarTitle,
                  DisplayOrder = row.MenuItemDisplayOrder,
                  HasPermission = row.HasPermission
                })
                .ToList();

            return new ClientMenuPermissionResponseModel
            {
              MenuId = group.Key.MenuId,
              MenuName = group.Key.MenuName,
              MenuIcon = group.Key.MenuIcon,
              Description = group.Key.Description,
              DisplayOrder = group.Key.DisplayOrder,
              IsCollapse = group.Key.IsCollapse,
              TabBarTitle = group.Key.TabBarTitle,
              RoutePath = group.Key.RoutePath,
              MenuItems = menuItemsList,
              HasPermission = menuItemsList.Any()
                    ? menuItemsList.Any(mi => mi.HasPermission)
                    : group.First().MenuHasPermission
            };
          })
          .OrderBy(m => m.DisplayOrder)
          .ToList();

      return result;
    }
  }

  public async Task<bool> SaveMenuItemPermissions(ClientId clientId, long clientUserRoleId, List<int> menuItemIds, List<int> menuIds, int employeeId)
  {
    // Get all existing permission records for this role and client
    var existingPermissions = await _context.MenuItemPermissionClients
        .Where(p => p.ClientUserRoleId == clientUserRoleId && p.ClientId == clientId)
        .ToListAsync();

    // 1. Physically delete permissions that are no longer selected (both menu items and menus)
    var toRemove = existingPermissions
        .Where(p => (p.MenuItemId.HasValue && !menuItemIds.Contains(p.MenuItemId.Value)) || 
                    (p.MenuId.HasValue && !menuIds.Contains(p.MenuId.Value)))
        .ToList();

    if (toRemove.Count > 0)
    {
      _context.MenuItemPermissionClients.RemoveRange(toRemove);
      await _context.SaveChangesAsync();
    }

    // 2. Add or update MenuItem permissions
    if (menuItemIds != null && menuItemIds.Count > 0)
    {
      var activeMenuItems = await _context.MenuItems
          .Where(mi => mi.Active == true && mi.IsDeleted != true)
          .ToListAsync();

      foreach (var menuItemId in menuItemIds)
      {
        var menuItem = activeMenuItems.FirstOrDefault(mi => mi.MenuItemId == menuItemId);
        if (menuItem == null) continue;

        var existing = existingPermissions.FirstOrDefault(p => p.MenuItemId == menuItemId);
        if (existing != null)
        {
          existing.UpdatePermission(menuItem.MenuItemName, menuItem.Description ?? "Assigned via API", employeeId, true);
        }
        else
        {
          var newPermission = MenuItemPermissionClient.Create(
              menuItem.MenuItemName,
              menuItemId,
              null,
              clientUserRoleId,
              menuItem.Description ?? "Assigned via API",
              clientId,
              employeeId,
              true
          );
          await _context.MenuItemPermissionClients.AddAsync(newPermission);
        }
      }
    }

    // 3. Add or update Menu permissions
    if (menuIds != null && menuIds.Count > 0)
    {
      var activeMenus = await _context.Menus
          .Where(m => m.Active == true && m.IsDeleted != true)
          .ToListAsync();

      foreach (var menuId in menuIds)
      {
        var menu = activeMenus.FirstOrDefault(m => m.MenuId == menuId);
        if (menu == null) continue;

        var existing = existingPermissions.FirstOrDefault(p => p.MenuId == menuId);
        if (existing != null)
        {
          existing.UpdatePermission(menu.MenuName, menu.Description ?? "Assigned via API", employeeId, true);
        }
        else
        {
          var newPermission = MenuItemPermissionClient.Create(
              menu.MenuName,
              null,
              menuId,
              clientUserRoleId,
              menu.Description ?? "Assigned via API",
              clientId,
              employeeId,
              true
          );
          await _context.MenuItemPermissionClients.AddAsync(newPermission);
        }
      }
    }

    return await _context.SaveChangesAsync() > 0;
  }
}
