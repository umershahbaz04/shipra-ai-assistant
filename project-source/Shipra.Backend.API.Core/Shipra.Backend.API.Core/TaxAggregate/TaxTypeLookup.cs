using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.TaxAggregate;
public class TaxTypeLookup
{
  public int TaxId { get; set; } 
  public string? Name { get; set; } 
  public decimal? Percentage { get; set; }
  public bool? Active { get; set; }

  public static TaxTypeLookup AddDefault()
  {
    return new TaxTypeLookup()
    {
      TaxId = 0,
      Name = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
