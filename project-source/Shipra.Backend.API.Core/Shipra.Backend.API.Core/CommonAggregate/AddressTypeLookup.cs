using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.Core.Constants;

namespace Shipra.Backend.API.Core.CommonAggregate;
public class AddressTypeLookup : EntityBase, IAggregateRoot
{
  public AddressTypeLookup() { }
  public int AddressTypeId { get; private set; }
  public string? AddressTypeName { get; private set; }
  public static AddressTypeLookup AddDefault()
  {
    return new AddressTypeLookup()
    {
      AddressTypeId = 0,
      AddressTypeName = ShipraConstants.DropDownPlaceHolderName
    };
  }
}
