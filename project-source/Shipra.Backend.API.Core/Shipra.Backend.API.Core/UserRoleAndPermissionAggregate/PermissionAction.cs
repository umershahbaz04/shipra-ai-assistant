namespace Shipra.Backend.API.Core.UserRoleAndPermissionAggregate;
public partial class PermissionAction
{
  public int PermissionActionId { get; set; }
  public int? PermissionGroupId { get; set; }
  public string? ControllerName { get; set; }
  public string? ActionName { get; set; }
  public bool? Active { get; set; }

  public static PermissionAction Create(string controller, string action, int rolePermissionGroupId)
  {
    return new PermissionAction
    {
      ControllerName = controller,
      ActionName = action,
      PermissionGroupId = rolePermissionGroupId,
      Active = true
    };
  }
  public void Update(int permissionGroupId)
  {
    PermissionGroupId = permissionGroupId;
  }
}
