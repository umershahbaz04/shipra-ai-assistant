using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.StoresAggregate;
public class StoreProduct
{
  public StoreProductId? StoreProductId { get; set; } 
  public int? StoreId { get; set; } 
  public ProductId? ProductId { get; set; } 
  public bool? Active { get; set; } 
  public DateTime? CreatedOn { get; set; } 
  public EmployeeId? CreatedBy { get; set; } 
  public DateTime? UpdatedOn { get; set; } 
  public EmployeeId? UpdatedBy { get; set; }

  public static StoreProduct CreateStoreProduct(int? storeId, ProductId? productId, EmployeeId? createdBy)
  {
    return new StoreProduct()
    {
      StoreProductId = StoreProductId.New,
      StoreId = storeId,
      ProductId = productId,
      Active = true,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy
    };
  }

  public void Activate(EmployeeId? updatedBy)
  {
    Active = true;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = updatedBy;
  }

  public void DeleteStoreProduct(EmployeeId? updatedBy)
  {
    Active = false;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = updatedBy;
  }
}
public sealed record StoreProductId(Guid Value)
{
  public static StoreProductId New => new(Guid.NewGuid());
}
