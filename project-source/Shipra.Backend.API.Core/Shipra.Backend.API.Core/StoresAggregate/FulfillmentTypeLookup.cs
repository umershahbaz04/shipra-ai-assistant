namespace Shipra.Backend.API.Core.StoresAggregate;

public class FulfillmentTypeLookup
{
  public int FulfillmentTypeId { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public bool Active { get; private set; }
}
