using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class CarrierLocationWithCountryResponseModel
{
  public int CarrierLocationId { get; set; }
  public int? CountryId { get; set; }
  public int? CarrierId { get; set; }
  public bool? Active { get; set; }
  public string? AddressingScheme { get; set; }
  public string? CountryName { get; set; }
}
