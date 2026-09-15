using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.ShipperInvoiceUserCase;
public class ServiceRateGroupEditDto
{
  public int? ServiceRateGroupId { get; set; }
  public int? From { get; set; }
  public int? To { get; set; }
  public int? OriginTypeId { get; set; }
  public int? CalculationMethodId { get; set; }
  public decimal? Unit { get; set; }
  public decimal AdditionalRate { get; set; }
  public string? Code { get; set; }
  public int? ServiceTypeId { get; set; }
  public string? EntityFromAddress { get; set; }
  public string? EntityToAddress { get; set; }

  public List<ServiceRateGroupSlabEditDto> Slabs { get; set; } = new();
}

public class ServiceRateGroupSlabEditDto
{
  public int ServiceRateGroupSlabId { get; set; }
  public decimal? WeightFrom { get; set; }
  public decimal? WeightTo { get; set; }
  public decimal? Rate { get; set; }
}

