using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.CountryAggregate;

public class State
{
  public int StateId { get; private set; } 
  public int? CountryId { get; private set; } 
  public string? Name { get; private set; }

  public static State AddDefault()
  {
    return new State()
    {
      StateId = 0,
      Name = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
