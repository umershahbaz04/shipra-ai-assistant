using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ShipmentDashboardResponseModel
{
  public int ShipmentGridColumnId { get; set; }
  public string? DashboardStatusName { get; set; }
  public string? DashboardStatusNameForKey { get; set; }
  public string? DashboardStatusValue { get; set; }
  public bool IsDisplay { get; set; }
  public bool Active { get; set; }
  public bool IsDefaultStatusTab { get; set; }
  public int DisplayOrder { get; set; }  
  public string? ClientId { get; set; }  
}
