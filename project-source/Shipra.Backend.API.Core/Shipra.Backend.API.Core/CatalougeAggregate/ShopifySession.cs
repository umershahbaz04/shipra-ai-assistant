namespace Shipra.Backend.API.Core.CatalougeAggregate;

public class ShopifySession
{
  public string? Id { get; set; }
  public string? Shop { get; set; }
  public string? Token { get; set; }
  public string? Scope { get; set; }
  public bool? IsConnected { get; set; } 
  public int? SaleChannelConfigId { get; set; }
}
