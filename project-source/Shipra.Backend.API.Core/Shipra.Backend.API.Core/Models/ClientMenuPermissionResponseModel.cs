using System.Collections.Generic;

namespace Shipra.Backend.API.Core.Models;

public class ClientMenuPermissionResponseModel
{
  public int MenuId { get; set; }
  public string? MenuName { get; set; }
  public string? MenuIcon { get; set; }
  public string? Description { get; set; }
  public int? DisplayOrder { get; set; }
  public bool? IsCollapse { get; set; }
  public string? TabBarTitle { get; set; }
  public string? RoutePath { get; set; }
  public bool HasPermission { get; set; }
  public List<ClientMenuItemPermissionModel> MenuItems { get; set; } = new();
}

public class ClientMenuItemPermissionModel
{
  public int MenuItemId { get; set; }
  public string? MenuItemName { get; set; }
  public string? MenuItemIcon { get; set; }
  public string? Description { get; set; }
  public int? MenuId { get; set; }
  public string? RoutePath { get; set; }
  public string? TabBarTitle { get; set; }
  public int? DisplayOrder { get; set; }
  public bool HasPermission { get; set; }
}
