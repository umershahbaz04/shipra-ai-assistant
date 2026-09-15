using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;
public class CreateUpdateShipraContractClientCarrierRequestModel
{
  public int ShipraContractClientCarrierId { get; set; } 
  public int ShipraContractCarrierId { get; set; }
}
