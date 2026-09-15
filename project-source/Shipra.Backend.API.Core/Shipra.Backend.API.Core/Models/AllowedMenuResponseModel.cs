using System.Collections.Generic;

namespace Shipra.Backend.API.Core.Models;

public class AllowedMenuResponseModel
{
  public int MenuId { get; set; }
  public string? MenuName { get; set; }
  public bool? IsCollapse { get; set; }
  public string? MenuTabBarTitle { get; set; }
  public string? RoutePath { get; set; }
  public bool HasPermission { get; set; }
  public List<AllowedMenuItemResponseModel> MenuItems { get; set; } = new();
  public List<MenuOtherRouteDto> MenuOtherRoutes { get; set; } = new();
}

public class AllowedMenuItemResponseModel
{
  public string? MenuItemName { get; set; }
  public string? MenuItemTabBarTitle { get; set; }
  public string? RoutePath { get; set; }
  public bool HasPermission { get; set; }
}

public class MenuOtherRouteDto
{
  public string? TabBarTitle { get; set; }
  public string? RoutePath { get; set; }
}
