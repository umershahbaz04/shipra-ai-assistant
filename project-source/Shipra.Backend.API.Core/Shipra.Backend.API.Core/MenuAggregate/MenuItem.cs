using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.MenuAggregate;
public class MenuItem
{
  public int MenuItemId { get; set; } 
  public string? MenuItemName { get; set; } 
  public string? MenuItemIcon { get; set; }  
  public string? Description { get; set; } 
  public int? MenuId { get; set; }
  public string? RoutePath { get; set; } 
  public string? TabBarTitle { get; set; }
  public DateTime? CreatedOn { get; set; } 
  public EmployeeId? CreatedBy { get; set; } 
  public DateTime? UpdatedOn { get; set; } 
  public EmployeeId? UpdatedBy { get; set; } 
  public bool? Active { get; set; } 
  public int? DisplayOrder { get; set; } 
  public bool? IsDeleted { get; set; }
}
