using Shipra.Backend.API.SharedKernel;

namespace Shipra.Backend.API.Core.ProductAggregate;

public class ProductVariantOption : EntityBase
{
  public ProductVariantOption() { }

  public long ProductVariantOptionId { get; private set; }
  public long ProductVariantId { get; private set; }
  public Guid ProductOptionsId { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public static ProductVariantOption Create(long productVariantId, Guid productOptionsId)
  {
      return new ProductVariantOption
      {
          ProductVariantId = productVariantId,
          ProductOptionsId = productOptionsId,
          CreatedOn = DateTime.UtcNow
      };
  }
}
