using System;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;

public class MenuItemPermissionClient : IAggregateRoot
{
  public MenuItemPermissionClient() { }

  public long PermissionId { get; private set; }
  public string? PermissionName { get; private set; }
  public int? MenuItemId { get; private set; }
  public int? MenuId { get; private set; }
  public long? ClientUserRoleId { get; private set; }
  public string? Description { get; private set; }
  public ClientId? ClientId { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public int? CreatedBy { get; private set; }
  public DateTime? ModifiedOn { get; private set; }
  public int? ModifiedBy { get; private set; }
  public bool? Active { get; private set; }

  public static MenuItemPermissionClient Create(string? permissionName, int? menuItemId, int? menuId, long? clientUserRoleId, string? description, ClientId? clientId, int? createdBy, bool? active = true)
  {
    return new MenuItemPermissionClient
    {
      PermissionName = permissionName,
      MenuItemId = menuItemId,
      MenuId = menuId,
      ClientUserRoleId = clientUserRoleId,
      Description = description,
      ClientId = clientId,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy,
      Active = active
    };
  }

  public void UpdatePermission(string? permissionName, string? description, int? modifiedBy, bool? active)
  {
    PermissionName = permissionName;
    Description = description;
    ModifiedBy = modifiedBy;
    ModifiedOn = DateTime.UtcNow;
    Active = active;
  }

  public void Deactivate(int? modifiedBy)
  {
    Active = false;
    ModifiedOn = DateTime.UtcNow;
    ModifiedBy = modifiedBy;
  }
}
