using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class AddressCommonLookupModel
{ 
  public object? Id { get; set; }
  public string? Name { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public long? ParentId { get; set; }
  public bool? IsLatLng { get; set; }
}
