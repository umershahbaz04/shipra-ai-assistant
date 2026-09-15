using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Constants;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ReturnAggregate;
public class ReturnStatusLookup
{
  public int ReturnStatusLookupId { get; private set; }
  public string? ReturnStatus { get; private set; }
  public static ReturnStatusLookup AddDefault()
  {
    return new ReturnStatusLookup()
    {
      ReturnStatusLookupId = 0,
      ReturnStatus = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
