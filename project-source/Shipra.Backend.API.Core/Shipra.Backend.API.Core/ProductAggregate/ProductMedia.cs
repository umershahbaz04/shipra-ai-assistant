using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ProductAggregate;
public class ProductMedia
{
  public long ProductMediaId { get; set; }
  public long ImageGalleryId { get; set; }
  public ProductId? ProductId { get; set; } 
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public bool? IsFeatured { get; set; }
  public bool? Active { get; set; }

  public static ProductMedia CreateProductMedia(long imageGalleryId, ProductId? productId, bool? isFeatured,EmployeeId employeeId)
  {
    return new ProductMedia()
    {
      ImageGalleryId = imageGalleryId,
      ProductId = productId,
      CreatedOn = DateTime.UtcNow,
      IsFeatured = isFeatured,
      Active = true,
      CreatedBy = employeeId
    };
  }

  public void UpdateGalleryImage(long imageGalleryId,ProductId? productId, EmployeeId employeeId)
  {
    ImageGalleryId = imageGalleryId;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
    ProductId = productId;
  }
}
