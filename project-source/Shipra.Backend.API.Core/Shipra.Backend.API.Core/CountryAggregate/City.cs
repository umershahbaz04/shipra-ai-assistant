using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.CountryAggregate;
public class City
{
  public City()
  { }
  public int CityId { get; set; } 
  public string? Code { get; set; } 
  public string? Name { get; set; } 
  public string? NameArabic { get; set; } 
  public int? CountryId { get; set; }
  public int? ProvinceId { get; set; }
  public int? StateId { get; set; }

  public static City AddDefault()
  {
    return new City()
    {
      CityId = 0,
      Name = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
