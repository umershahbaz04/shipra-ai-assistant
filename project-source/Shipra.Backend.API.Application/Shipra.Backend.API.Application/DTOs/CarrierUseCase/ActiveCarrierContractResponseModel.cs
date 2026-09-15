using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;
public class ActiveCarrierContractResponseModel
{
  public int ActiveCarrierId { get;  set; }
  public int CarrierId { get;  set; } 
  public string? Config { get;  set; }
  public string? SettingConfig { get;  set; }
  public string? UserName { get;  set; }
  public string? CarrierAlias { get;  set; }
  public int? RegionTimeZoneId { get; set; }
  public bool? IsWebhookSupported { get; set; } 
  public DateTime? CreatedOn { get;  set; } 
  public DateTime? UpdatedOn { get;  set; }
  public bool? IsActiveCarrier { get;  set; }
  public bool? Active { get;  set; }
  public bool? IsDefault { get;  set; }
  public decimal? FlatRate { get; private set; }
}
