using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;

public class ProductStockReuqestModel
{
  public long? InventoryBalanceId { get; set; }
  public long? ProductVariantId { get; set; }
  public long ProductStockId { get; set; }
  public string? ProductId { get; set; }
  public string? Sku { get; set; }
  public decimal? Price { get; set; }
  public int? QuantityAvailable { get; set; }
  public int LowQuantityLimit { get; set; }
  public int? ProductStationId { get; set; }
  public string? VarientOption { get; set; }
  public int? ProductVariantStatusId { get; set; }
  public int? ProductStockStatusId { get; set; }
  public long? ImageGalleryId { get; set; }
  public string? ImageUrl { get; set; }
  public bool? Active { get; set; }
  public ProductVariantRequestModel? VariantAttributes { get; set; }
}

public class ProductVariantRequestModel
{
    public string? Barcode { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
}

public class StationQuantityRequest
{
    public int StationId { get; set; }
    public int Quantity { get; set; }
}

public class QuickAddProductStockReuqestModel : ProductStockReuqestModel
{ 
  public List<ProductOptionReuqestModel>? ProductOptions { get; set; } = new();
  public List<int>? ProductStationIds { get; set; } = new();
  public List<StationQuantityRequest>? StationQuantities { get; set; } = new();
}
public class ProductMediaResponseModal
{
  public long ProductMediaId { get; set; }
  public long ImageGalleryId { get; set; }
  public bool? IsFeatured { get; set; }
  public bool? Active { get; set; }
}
