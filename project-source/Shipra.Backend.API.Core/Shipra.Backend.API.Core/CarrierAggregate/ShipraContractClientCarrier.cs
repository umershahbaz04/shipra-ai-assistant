using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public partial class ShipraContractClientCarrier
{
  public int ShipraContractClientCarrierId { get; set; }
  public int? ShipraContractCarrierId { get; set; }
  public bool? AllowCodOrder { get; set; }
  public ClientId? ClientId { get; set; }

  public static ShipraContractClientCarrier Create(int shipraContractCarrierId, ClientId? clientId)
  {
    return new ShipraContractClientCarrier
    {
      ShipraContractCarrierId = shipraContractCarrierId,
      ClientId = clientId,
      AllowCodOrder = true
    };
  }
}
