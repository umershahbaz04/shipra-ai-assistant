using System;
using System.Collections.Generic;

namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;

public partial class ContractShipperRate
{
  public long ContractShipperRatesId { get; private set; }
  public int? ServiceRateGroupId { get; private set; }
  public DateTime? CreatedDate { get; private set; }
  public bool? Active { get; private set; }

  public static ContractShipperRate Create(int? serviceRateGroupId)
  {
    return new ContractShipperRate
    {
      ServiceRateGroupId = serviceRateGroupId,
      CreatedDate = DateTime.UtcNow,
      Active = true
    };
  }
  public void Update(int? serviceRateGroupId, bool? active)
  {
    ServiceRateGroupId = serviceRateGroupId;
    Active = active;
  }
   
  public void Deactivate()
  {
    Active = false;
  }
}
