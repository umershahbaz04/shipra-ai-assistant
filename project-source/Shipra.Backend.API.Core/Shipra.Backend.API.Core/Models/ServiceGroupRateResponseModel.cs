using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ServiceGroupRateResponseModel
{ 
  public int? RowNum { get; set; }
  public int? TotalCount { get; set; }
  public int ServiceRateGroupId { get; set; } 
  public int? ContractShipperRatesId { get; set; }
  public int? SaleChannelConfigId { get; set; }
  public int? From { get; set; }
  public int? To { get; set; }
  public string? FromName { get; set; }
  public string? ToName { get; set; }
  public decimal? Rate { get; set; }
  public decimal? AdditionalRate { get; set; }
  public string? Code { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? CreatedBy { get; set; }
  public int? CalculationMethodId { get; set; }
  public int? OriginTypeId { get; set; } 
  public string? OriginTypeName { get; set; }
  public string? ServiceName { get; set; }    
  public decimal? Weight { get; set; }
  public bool? IsAssignedRate { get; set; } = false;
  public long? ShipperRateId { get; set; } = 0;
  public decimal? ShipperRate { get; set; } = 0;
  public decimal? ShipperAddRate { get; set; } = 0;

  public dynamic? CarrierRateSlabs { get; set; }
}
