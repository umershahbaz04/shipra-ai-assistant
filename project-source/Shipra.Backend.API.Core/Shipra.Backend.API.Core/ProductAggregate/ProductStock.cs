using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductStock
{
  public ProductStock()
  {

  }
  public long ProductStockId { get; private set; }
  public ProductId? ProductId { get; private set; }
  public string? Sku { get; private set; }
  public decimal? Price { get; private set; }
  public int? QuantityCommited { get; private set; }
  public int? QuantityInComing { get; private set; }
  public int? QuantityAvailable { get; private set; }
  public int? LowQuantityLimit { get; private set; }
  public bool? IsLowQuantity { get; private set; }
  public int? QuantityOnOrder { get; private set; }
  public int? ProductStationId { get; private set; }
  public string? VarientOption { get; private set; }
  public bool? Active { get; private set; }
  public int? QuantityDamage { get; private set; }
  public int? ProductStockStatusId { get; private set; }
  public long? SaleChannelVariantId { get; private set; }
  public long? ImageGalleryId { get; set; } 
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public static ProductStock CreateProductStock(ProductId? productId, string? sku, decimal? price, int? quantityAvailable, int lowQuantityLimit, int? productStationId, string? varientOption, EmployeeId createdBy, long? imageGalleryId = null, long? saleChannelVariantId = null)
  {

    return new ProductStock()
    {
      ProductId = productId,
      Sku = sku?.Trim(),
      Price = price,
      QuantityCommited = 0,
      QuantityInComing = 0,
      QuantityAvailable = quantityAvailable,
      LowQuantityLimit = lowQuantityLimit,
      QuantityOnOrder = 0,
      ProductStationId = productStationId,
      VarientOption = varientOption,
      Active = true,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy,
      QuantityDamage = 0,
      ImageGalleryId = imageGalleryId,
      IsLowQuantity = (lowQuantityLimit >= quantityAvailable),
      ProductStockStatusId = (int)EnumProductStockStatus.Active,
      SaleChannelVariantId = saleChannelVariantId
    };
  }

  public void UpdateFullfilmentStock(dynamic quantityAvailable, dynamic quantityCommited, dynamic quantityOnOrder, EmployeeId? employeId)
  {
    //update low quantity flag
    UpdateLoWQuantityLimit(quantityAvailable, LowQuantityLimit);

    QuantityCommited = quantityCommited;
    QuantityAvailable = quantityAvailable;
    QuantityOnOrder = quantityOnOrder;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeId;
  }

  public void UpdateProdctStockQuantityByReason(dynamic quantityDamage, dynamic quantityAvailable, EmployeeId? userId)
  {
    //update low quantity flag
    UpdateLoWQuantityLimit(quantityAvailable, LowQuantityLimit);

    QuantityDamage = quantityDamage;
    QuantityAvailable = quantityAvailable;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = userId;
  }

  public void UpdateProductStock(string sku, decimal? price, int? quantityAvailable, int lowQuantityLimit, int? productStationId, string? varientOption, int? productStockStatusId, bool? active,long? imageGalleryId)
  {
    //update low quantity flag
    UpdateLoWQuantityLimit(quantityAvailable, LowQuantityLimit);

    Sku = sku?.Trim();
    Price = price;
    QuantityAvailable = quantityAvailable;
    LowQuantityLimit = lowQuantityLimit;
    ProductStationId = productStationId;
    VarientOption = varientOption;
    ProductStockStatusId = productStockStatusId;
    ImageGalleryId = imageGalleryId;
  }

  public void UpdateProductStockQuantityCommited(int? quantityCommited, EmployeeId updatedBy)
  {
    QuantityCommited = quantityCommited;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateProductStockQuantityAvailable(int? quantityAvailable, EmployeeId updatedBy)
  {
    //update low quantity flag
    UpdateLoWQuantityLimit(quantityAvailable, LowQuantityLimit);

    QuantityAvailable = quantityAvailable;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateProductStockQuantityOnReturnOrder(int? quantity, EmployeeId employeeId)
  {
    //update low quantity flag
    UpdateLoWQuantityLimit(quantity, LowQuantityLimit);

    QuantityAvailable = quantity;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateProductStockStaus(int productStockStatusId, EmployeeId employeeId)
  {
    ProductStockStatusId = productStockStatusId;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void EnableProductStockStaus(EmployeeId employeeId)
  {
    Active = true;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void DisableProductStockStaus(EmployeeId employeeId)
  {
    Active = false;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateLoWQuantityLimit(int? quantityAvailable, int? lowQuantityLimit)
  {
    if (lowQuantityLimit >= quantityAvailable)
    {
      IsLowQuantity = true;
    }
    else
    {
      IsLowQuantity = false;
    }
  }
}
//public sealed record ProductStockId(Guid Value)
//{
//  public static ProductStockId New => new(Guid.NewGuid());
//}
