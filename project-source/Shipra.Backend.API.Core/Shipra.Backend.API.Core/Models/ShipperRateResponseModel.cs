using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ShipperRateResponseModel
{
  public int? RowNum { get; set; }
  public int? TotalCount { get; set; }
  public int ServiceRateGroupId { get; set; }
  public int? ContractCarrierId { get; set; }
  public int? ContractCarrierRateId { get; set; }
  public int? SaleChannelConfigId { get; set; }
  public int? ServiceTypeId { get; set; }
  public int? From { get; set; }
  public int? To { get; set; }
  public Guid? ClientId { get; set; }
  public string? FromName { get; set; }
  public string? OrderNo { get; set; }
  public string? ToName { get; set; }
  public string? EmployeeName { get; set; }
  public decimal? Rate { get; set; }
  public decimal? AdditionalRate { get; set; }
  public string? Code { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? CreatedBy { get; set; }

  public int? OriginTypeId { get; set; }
  public int? CalculationMethodId { get; set; }
  public string? OriginTypeName { get; set; }
  public string? ServiceName { get; set; }  
  public bool? IsAssignedRate { get; set; } = false;
  public long? ShipperRateId { get; set; } = 0;
  public decimal? ClientRate { get; set; } = 0;
  public decimal? ClientAddRate { get; set; } = 0;
  public decimal? WeightFrom { get; set; }
  public decimal? WeightTo { get; set; }
  public decimal? CalculatedRate { get; set; }
  public decimal? ShipperAdditionalRate { get; set; }
  public int? Unit { get; set; }
  public int? SlabCount { get; set; }
  public dynamic? CarrierRateSlabs { get; set; }
}

