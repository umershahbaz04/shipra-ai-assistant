using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.CountryAggregate;
public class Region
{
  public int RegionId { get; set; } 
  public string? Name { get; set; } 
  public int? CountryId { get; set; }
  public string? Code { get; set; }
}
