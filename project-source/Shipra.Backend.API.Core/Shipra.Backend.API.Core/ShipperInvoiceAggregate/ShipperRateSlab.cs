namespace Shipra.Backend.API.Core.ShipperInvoiceAggregate;
public class ShipperRateSlab
{
  public int ShipperRateSlabId { get; private set; }
  public long? ShipperRateId { get; private set; }
  public int? ServiceRateGroupSlabId { get; private set; }
  public decimal? Rate { get; private set; }

  public static ShipperRateSlab Create(long? shipperRateId,int? serviceRateGroupSlabId,decimal? rate)
  {
    if (rate < 0)
      throw new ArgumentException("Rate cannot be negative.");

    return new ShipperRateSlab
    {
      ShipperRateId = shipperRateId,
      ServiceRateGroupSlabId = serviceRateGroupSlabId,
      Rate = rate, 
    };
  }

  public void Update(decimal? rate)
  {
    if (rate < 0)
      throw new ArgumentException("Rate cannot be negative.");

    Rate = rate;
  }
}
