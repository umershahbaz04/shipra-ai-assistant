using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public class CarrierLocation
{
  public int CarrierLocationId { get; set; }
  public int? CountryId { get; set; }
  public int? CarrierId { get; set; }
  public string? AddressingScheme { get; set; }
  public bool? Active { get; set; }
  public int? RegionTimeZoneId { get; set; }

  public static CarrierLocation Create(int? carrierId, int? countryId, bool? active,string? addressingScheme)
  {
    return new CarrierLocation()
    {
      CarrierId = carrierId,
      CountryId = countryId,
      AddressingScheme = addressingScheme,
      Active = active
    };
  }
}
