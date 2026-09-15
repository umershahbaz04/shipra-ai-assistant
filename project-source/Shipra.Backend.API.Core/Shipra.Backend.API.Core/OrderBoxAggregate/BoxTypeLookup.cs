using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.OrderBoxAggregate;
public partial class BoxTypeLookup
{
  public int BoxTypeLookupId { get; set; } 
  public string? BoxName { get; set; } 
  public decimal? Length { get; set; }  
  public decimal? Width { get; set; } 
  public decimal? Height { get; set; } 
  public decimal? Volume { get; set; }
  public bool? IsDefault { get; set; }
}
