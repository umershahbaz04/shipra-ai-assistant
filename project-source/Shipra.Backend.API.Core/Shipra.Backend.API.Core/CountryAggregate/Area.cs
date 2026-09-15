using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.CountryAggregate;
public class Area 
{
  public Area()
  { }
  public int AreaId { get; private set; }
  public string? Code { get; private set; }
  public string? Name { get; private set; }
  public string? NameArabic { get; private set; }
  public int CityId { get; private set; }
  public decimal? Latitude { get; private set; }
  public decimal? Longitude { get; private set; }
  public int? ProviceId { get; private set; }
  public int? StateId { get; private set; }
  public long? ExtendId { get; private set; }

  public static Area AddDefault()
  {
    return new Area()
    {
      AreaId = 0,
      Name = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
