namespace Shipra.Backend.API.Core.Models;

public class FlatMenuPermissionResult
{
  public int MenuId { get; set; }
  public string? MenuName { get; set; }
  public string? MenuIcon { get; set; }
  public string? MenuDescription { get; set; }
  public int? MenuDisplayOrder { get; set; }
  public bool? IsCollapse { get; set; }
  public string? MenuTabBarTitle { get; set; }
  public string? MenuRoutePath { get; set; }
  public int? MenuItemId { get; set; }
  public string? MenuItemName { get; set; }
  public string? MenuItemIcon { get; set; }
  public string? MenuItemDescription { get; set; }
  public string? MenuItemRoutePath { get; set; }
  public string? MenuItemTabBarTitle { get; set; }
  public int? MenuItemDisplayOrder { get; set; }
  public bool HasPermission { get; set; }
  public bool MenuHasPermission { get; set; }
}

public class AllowedMenuQueryResult
{
  public int MenuId { get; set; }
  public string? MenuName { get; set; }
  public bool? IsCollapse { get; set; }
  public string? MenuTabBarTitle { get; set; }
  public string? MenuRoutePath { get; set; }
  public string? MenuItemName { get; set; }
  public string? MenuItemTabBarTitle { get; set; }
  public string? MenuItemRoutePath { get; set; }
}
