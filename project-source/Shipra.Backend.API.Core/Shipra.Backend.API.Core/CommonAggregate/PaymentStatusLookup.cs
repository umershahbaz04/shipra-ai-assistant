using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.CommonAggregate;
public class PaymentStatusLookup : EntityBase, IAggregateRoot
{
  public PaymentStatusLookup() { }
  public int PaymentStatusId { get; private set; }
  public string? StatusName { get; private set; }

  public static PaymentStatusLookup AddDefault()
  {
    return new PaymentStatusLookup()
    {
      PaymentStatusId = 0,
      StatusName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}

