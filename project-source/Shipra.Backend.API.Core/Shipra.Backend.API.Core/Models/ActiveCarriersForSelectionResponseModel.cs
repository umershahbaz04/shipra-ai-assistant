using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ActiveCarriersForSelectionResponseModel
{
  public int ActiveCarrierId { get; set; }
  public int CarrierId { get; set; }
  public int? CarrierContractTypeId { get; set; }
  public int? ShipraContractCarrierId { get; set; }
  public string? Name { get; set; }
  public string? CarrierAlias { get; set; }
  public bool? IsRateCheck { get; set; }
  public bool? ValidateAddress { get; set; }
  public bool? IsDispatchExCompany { get; set; }
  public string? Config { get; set; }
}
