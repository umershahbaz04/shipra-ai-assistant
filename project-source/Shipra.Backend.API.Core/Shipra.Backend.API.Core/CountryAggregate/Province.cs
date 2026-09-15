using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.CountryAggregate;

public class Province
{
  public int ProvinceId { get; private set; } 
  public int? CountryId { get; private set; } 
  public string? Name { get; private set; }


  public static Province AddDefault()
  {
    return new Province()
    {
      ProvinceId = 0,
      Name = ShipraConstants.DropDownPlaceHolderName
    };
  }

  public static Province Create(int countryId, string name)
  {
    return new Province()
    {
      CountryId = countryId,
      Name = name,
    };
  }
}
