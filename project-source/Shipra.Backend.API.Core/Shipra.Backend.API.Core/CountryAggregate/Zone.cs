using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.CountryAggregate;
public class Zone : EntityBase, IAggregateRoot
{
  public Zone()
  { }
  public int ZoneId { get; private set; }

  public string? Code { get; private set; }

  public string? Name { get; private set; }

  public string? NameArabic { get; private set; }

  public int? CityID { get; private set; }

  public string? Coords { get; private set; }

  public bool Active { get; private set; } = true;

  public Zone(string name, int? cityID, string? coords, string? code = null, string? nameArabic = null, bool active = true)
  {
    Name = name;
    CityID = cityID;
    Coords = coords;
    Code = code;
    NameArabic = nameArabic;
    Active = active;
  }

  public void UpdateZone(string name, int? cityID, string? coords = null, string? code = null, string? nameArabic = null, bool? active = null)
  {
    Name = name;
    if (cityID.HasValue && cityID.Value > 0)
    {
      CityID = cityID;
    }
    if (!string.IsNullOrEmpty(coords))
    {
      Coords = coords;
    }
    if (!string.IsNullOrEmpty(code))
    {
      Code = code;
    }
    if (!string.IsNullOrEmpty(nameArabic))
    {
      NameArabic = nameArabic;
    }
    if (active.HasValue)
    {
      Active = active.Value;
    }
  }

  public void Deactivate()
  {
    Active = false;
  }
}


