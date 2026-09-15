using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public  class CarrierDeliveryService
{
  public int CarrierDeliveryServiceId { get;private set; } 
  public int? DeliveryServiceId { get; private set; } 
  public int? CarrierId { get; private set; } 
  public bool? Active { get; private set; }

  public static CarrierDeliveryService Create(int? deliveryServiceId, int? carrierId, bool? active)
  {
    return new CarrierDeliveryService()
    {
      DeliveryServiceId = deliveryServiceId,
      CarrierId = carrierId,
      Active = active
    };
  }
}
