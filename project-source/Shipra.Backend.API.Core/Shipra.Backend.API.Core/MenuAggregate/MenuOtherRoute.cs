namespace Shipra.Backend.API.Core.MenuAggregate;
public partial class MenuOtherRoute
{
  public int MenuOtherRouteId { get; set; } 
  public int? MenuId { get; set; } 
  public string? RoutePath { get; set; } 
  public string? TabBarTitle { get; set; } 
  public bool? Active { get; set; }
}
