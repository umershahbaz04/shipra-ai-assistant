using System;
using System.Collections.Generic;

namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;

public partial class ServiceRateGroupSlab
{
  public int ServiceRateGroupSlabId { get; private set; }
  public int? ServiceRateGroupId { get; private set; }
  public decimal? WeightFrom { get; private set; }
  public decimal? WeightTo { get; private set; }
  public decimal? Rate { get; private set; }

  public static ServiceRateGroupSlab Create(int serviceRateGroupId, decimal? weightFrom, decimal? weightTo, decimal? rate)
  {
    return new ServiceRateGroupSlab
    {
      ServiceRateGroupId = serviceRateGroupId,
      WeightFrom = weightFrom.GetValueOrDefault(),
      WeightTo = weightTo.GetValueOrDefault(),
      Rate = rate
    };
  }

  public void Update(decimal? weightFrom, decimal? weightTo, decimal? rate)
  { 
    WeightFrom = weightFrom.GetValueOrDefault();
    WeightTo = weightTo.GetValueOrDefault();
    Rate = rate;
  }
}
