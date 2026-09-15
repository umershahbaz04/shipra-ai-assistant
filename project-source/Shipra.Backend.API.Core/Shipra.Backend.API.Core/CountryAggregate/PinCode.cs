using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.CountryAggregate;
public class PinCode
{
  public int PinCodeId { get; set; } 
  public int? CityId { get; set; } 
  public string? PinCodeValue { get; set; }

  public static PinCode AddDefault()
  {
    return new PinCode()
    {
      PinCodeId = 0,
      PinCodeValue = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
