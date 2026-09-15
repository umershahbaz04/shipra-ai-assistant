using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.CommonAggregate;
public class LookupAdjustReason 
{ 
  public int LookupAdjustReasonId { get; private set; }
  public string? Reason { get; private set; }

  public static LookupAdjustReason AddDefault()
  {
    return new LookupAdjustReason()
    {
      LookupAdjustReasonId = 0,
      Reason = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
