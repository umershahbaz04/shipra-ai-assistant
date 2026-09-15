using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductCategory
{
  public int ProductCategoryId { get; set; }
  public string? CategoryName { get; set; }
  public ClientId? ClientId { get; set; }
  public bool? Active { get; set; }
  public bool? IsDefault { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }

  public static ProductCategory CreateProductCategory(string? categoryName, ClientId clientId, EmployeeId createdBy, bool? isDefault = false)
  {
    var productcategory = new ProductCategory()
    {
      CategoryName = categoryName,
      ClientId = clientId,
      Active = true,
      IsDefault = isDefault,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy
    };
    return productcategory;
  }

  public void UpdateProductCategory(int productCategoryId, string? categoryName, EmployeeId updateBy)
  {
    ProductCategoryId = productCategoryId;
    CategoryName = categoryName;
    UpdatedBy = updateBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void DeleteProductCategory(int productCategoryId, EmployeeId updateBy)
  {
    ProductCategoryId = productCategoryId;
    Active = false;
    UpdatedBy = updateBy;
    UpdatedOn = DateTime.UtcNow;
  }
}
