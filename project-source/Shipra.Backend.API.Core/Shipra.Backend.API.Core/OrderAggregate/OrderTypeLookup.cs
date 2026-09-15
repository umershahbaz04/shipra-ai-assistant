using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderTypeLookup
{
  public int OrderTypeId { get; set; }
  public string? OrderTypeName { get; set; }

  public static OrderTypeLookup AddDefault()
  {
    return new OrderTypeLookup()
    {
      OrderTypeId = 0,
      OrderTypeName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
