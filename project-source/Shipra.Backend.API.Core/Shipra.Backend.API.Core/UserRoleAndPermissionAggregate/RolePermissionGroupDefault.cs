namespace Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;
public class RolePermissionGroupDefault
{
  public int RolePermissionGroupId { get;private set; } 
  public int? RoleId { get; private set; } 
  public int? PermissionGroupId { get; private set; } 
  public bool? Active { get; private set; }
}
