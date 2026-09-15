using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public class DeliveryService
{
  public int DeliveryServiceId { get; set; } 
  public string? ServiceName { get; set; } 
  public string? ServiceLogo { get; set; }

  public static DeliveryService Create(string? serviceLogo, string? serviceName)
  {
    return new DeliveryService()
    {
      ServiceLogo = serviceLogo,
      ServiceName = serviceName
    };
  }
}
