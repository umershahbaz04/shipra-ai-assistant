using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class CarrierStatsResponseModel
{
  public int CarrierId { get; set; }
  public string? CarrierName { get; set; }
  public int TotalOrder { get; set; }
  public int InProgress { get; set; }
  public int Delivered { get; set; }
  public int CODPending { get; set; }
  public int CODSettled { get; set; }
  public decimal DeliveryRatio { get; set; }
}
