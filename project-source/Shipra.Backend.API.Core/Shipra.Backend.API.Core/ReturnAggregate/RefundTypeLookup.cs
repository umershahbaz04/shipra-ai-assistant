using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.ReturnAggregate;
public class RefundTypeLookup
{
  public int RefundTypeId { get; set; }
  public string? RefundTypeName { get; set; }
  public static RefundTypeLookup AddDefault()
  {
    return new RefundTypeLookup()
    {
      RefundTypeId = 0,
      RefundTypeName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
