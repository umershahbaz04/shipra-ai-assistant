using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class AssignCarrierListRequestModel
{
  public int? ActiveCarrierPickupLocationId { get; set; }
  public string? OrderNo { get; set; }
  public bool? CheckPikupLocation { get; set; } = false; 
  public string? ServiceType { get; set; }
  public Dictionary<string, string>? Others { get; set; }
}
