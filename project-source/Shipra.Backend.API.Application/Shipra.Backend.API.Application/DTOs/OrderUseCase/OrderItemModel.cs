namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;

public class OrderItemModel
{
  public string? OrderItemId { get; set; }
  public string? ProductId { get; set; } = string.Empty;
  public string? StockSku { get; set; }
  public string? ProductName { get; set; }
  public long? ProductVariantId { get; set; } = 0;
  public long? SaleChannelVariantId { get; set; } = 0;
  public decimal? Price { get; set; }
  public string? Description { get; set; } = string.Empty;
  public string? Remarks { get; set; } = string.Empty;
  public int? Quantity { get; set; }
  public decimal? Discount { get; set; }
  public string? HsCode { get; set; }
  public string? OriginCountryCode { get; set; }
  public decimal? UnitRate { get; set; }
  public decimal? Weight { get; set; }
}
