using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class PriceCalculatorFilter
{
  public string? ClientId { get; set; }
  public string? Search { get; set; } = "";
  public long? From { get; set; } = 0;
  public long? To { get; set; } = 0;
  public decimal? Amount { get; set; } = 0;
  public int? OrigionTypeId { get; set; } = 0;
  public string? DeliveryTime { get; set; }
  public string? DropOfMethod { get; set; }
  public string? DeliveryMethod { get; set; }
  public decimal? MinPrice { get; set; }
  public decimal? MaxPrice { get; set; }
  public decimal? Weight { get; set; }
  public int? CarrierId { get; set; }

  public string? SaleChannelConfigIds { get; set; }
  public Dictionary<string, object> AddressFrom { get; set; } = new();
  public Dictionary<string, object> AddressTo { get; set; } = new();
}
public class PriceCalculatorFilterForShipperRate
{
  public string? ClientId { get; set; }
  public string? Search { get; set; } = "";
  public OrderRateSeed? OrderRateSeed { get; set; } 
  public int? OrigionTypeId { get; set; } = 0;

  public decimal? Weight { get; set; }
  public decimal? MinPrice { get; set; }
  public decimal? Amount { get; set; } = 0;
  public decimal? MaxPrice { get; set; } 
  public int? CarrierId { get; set; }

  public string? SaleChannelConfigIds { get; set; }
  public Dictionary<string, object> AddressFrom { get; set; } = new();
  public Dictionary<string, object> AddressTo { get; set; } = new();
}
