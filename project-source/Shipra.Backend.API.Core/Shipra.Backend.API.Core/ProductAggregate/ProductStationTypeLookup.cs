namespace Shipra.Backend.API.Core.ProductAggregate;

public class ProductStationTypeLookup
{
  public int ProductStationTypeId { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public bool Active { get; private set; }
}
