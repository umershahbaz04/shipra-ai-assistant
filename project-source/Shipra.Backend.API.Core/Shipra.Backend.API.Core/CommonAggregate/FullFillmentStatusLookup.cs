using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.CommonAggregate;
public class FullFillmentStatusLookup : EntityBase, IAggregateRoot
{
  public FullFillmentStatusLookup() { }
  public int FullFillmentStatusId { get; private set; }
  public string? FullFillmentStatus { get; private set; }

  public static FullFillmentStatusLookup AddDefault()
  {
    return new FullFillmentStatusLookup()
    {
      FullFillmentStatusId = 0,
      FullFillmentStatus = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
