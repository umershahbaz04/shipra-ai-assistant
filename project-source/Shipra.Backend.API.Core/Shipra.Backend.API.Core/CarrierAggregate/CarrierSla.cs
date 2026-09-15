using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public partial class CarrierSla
{
  public int CarrierSlaId { get; set; } 
  public int? CarrierId { get; set; } 
  public string? DeliveryTime { get; set; } 
  public string? DropOffMethod { get; set; } 
  public string? DeliveryMethod { get; set; } 
  public string? Config { get; set; }
}
