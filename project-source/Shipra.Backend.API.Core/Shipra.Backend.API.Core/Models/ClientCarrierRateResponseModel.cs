using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class ClientCarrierRateResponseModel
{ 
  public int ClientRateId { get; set; }
  public int? CarrierRateId { get; set; } 
  public string? CarrierName { get; set; } 
  public int? CarrierId { get; set; } 
  public string? CarrierImage { get; set; } 
  public string? SettingConfig { get; set; } 
  public bool IsRateCheck { get; set; } 
  public int? CalculationTypeId { get; set; }
  public int? From { get; set; }
  public int? To { get; set; }
  public int? UomId { get; set; }
  public string? FromName { get; set; }
  public string? ToName { get; set; }  
  public decimal? ClientAdditionalRate { get; set; } 
  public decimal? CalculatedRate { get; set; }
  public decimal? ClientRate { get; set; }
  public decimal? Weight { get; set; }
  public string? Code { get; set; }  
  public int? OriginTypeId { get; set; }
  public int? CalculationMethodId { get; set; }
  public string? OriginTypeName { get; set; } 
  public string? ServiceName { get; set; }
  public string? RateTypeName { get; set; }
  public int? RateTypeId { get; set; }
  public string? UOMName { get; set; }  
  public decimal? Unit { get; set; }  

  public int? ClientRateSlabId { get; set; }
  public decimal? UomFrom { get; set; }
  public decimal? UomTo { get; set; }
  public decimal? Rate { get; set; }
  public string? OrderNo { get; set; }
  //public List<ClientRateSlabResponseModel>? ClientRateSlabs { get; set; } = new();
}
public class ClientRateSlabResponseModel
{
  public int? ClientRateSlabId { get; set; }
  public decimal? UomFrom { get; set; }
  public decimal? UomTo { get; set; } 
  public decimal? Rate { get; set; } 
}

