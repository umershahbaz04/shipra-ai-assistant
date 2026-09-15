using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductInventory : EntityBase, IAggregateRoot
{
  public ProductInventory()
  {
    
  }

  public ProductInventoryId? ProductInventoryId { get; set; } 
  public ProductId? ProductId { get; set; } 
  public int? AvailableCount { get; set; } 
  public DateTime? LastPurchase { get; set; } 
  public string? CreatedBy { get; set; } 
  public DateTime? CreatedOn { get; set; } 
  public string? UpdatedBy { get; set; } 
  public DateTime? UpdatedOn { get; set; }

  public static ProductInventory CreateProductInventory(ProductInventoryId? productInventoryId, ProductId? productId, int? availableCount, DateTime? lastPurchase, string? createdBy, DateTime? createdOn, string? updatedBy, DateTime? updatedOn)
  {
    return new ProductInventory()
    {
      ProductInventoryId = productInventoryId,
      ProductId = productId,
      AvailableCount = availableCount,
      LastPurchase = lastPurchase,
      CreatedBy = createdBy,
      CreatedOn = createdOn,
      UpdatedBy = updatedBy,
      UpdatedOn = updatedOn,
    };

  }
}
public record ProductInventoryId(Guid Value);
