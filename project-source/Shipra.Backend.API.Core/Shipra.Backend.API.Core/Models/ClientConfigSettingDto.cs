using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ClientConfigSettingDto
{
  public Guid? ClientId { get; set; }
  public bool? AutoOrderStatusUpdate { get; set; }
  public bool? AllowShipperInvocie { get; set; }
  public int? RefreshOrderMinut { get; set; }
  public bool? AutoCreateDeliveryTask { get; set; }
  public string? SettingConfig { get; set; }
}
