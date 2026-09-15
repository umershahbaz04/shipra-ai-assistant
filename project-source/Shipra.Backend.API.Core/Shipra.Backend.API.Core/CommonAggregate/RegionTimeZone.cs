using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.CommonAggregate;
public class RegionTimeZone
{
  public int RegionTimeZoneId { get; set; }
  public string? TimeZoneName { get; set; }
  public int? Minutes { get; set; }
  public string? HoursDifference { get; set; }
  public string? TimeZone { get; set; } 
}
