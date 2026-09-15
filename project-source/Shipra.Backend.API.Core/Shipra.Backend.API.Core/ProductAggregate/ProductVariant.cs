using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.SharedKernel;

namespace Shipra.Backend.API.Core.ProductAggregate;

public class ProductVariant : EntityBase
{
  public ProductVariant() { }

  public long ProductVariantId { get; private set; }
  public ProductId? ProductId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public string? SKU { get; private set; }
  public string? Barcode { get; private set; }
  public decimal? Price { get; private set; }
  public decimal? PurchasePrice { get; private set; }
  public decimal? Weight { get; private set; }
  public decimal? Length { get; private set; }
  public decimal? Width { get; private set; }
  public decimal? Height { get; private set; }
  public int? LowQuantityLimit { get; private set; }
  public string? VariantOptionText { get; private set; }
  public long? ImageGalleryId { get; private set; }
  public int? ProductVariantStatusId { get; private set; }
  public bool Active { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }

  public static ProductVariant Create(
      ProductId? productId,
      ClientId? clientId,
      string? sku,
      string? barcode,
      decimal? price,
      decimal? purchasePrice,
      decimal? weight,
      decimal? length,
      decimal? width,
      decimal? height,
      int? lowQuantityLimit,
      string? variantOptionText,
      long? imageGalleryId,
      int? productVariantStatusId,
      EmployeeId? createdBy)
  {
      return new ProductVariant
      {
          ProductId = productId,
          ClientId = clientId,
          SKU = sku?.Trim(),
          Barcode = barcode?.Trim(),
          Price = price,
          PurchasePrice = purchasePrice,
          Weight = weight,
          Length = length,
          Width = width,
          Height = height,
          LowQuantityLimit = lowQuantityLimit,
          VariantOptionText = variantOptionText,
          ImageGalleryId = imageGalleryId,
          ProductVariantStatusId = productVariantStatusId,
          Active = true,
          CreatedOn = DateTime.UtcNow,
          CreatedBy = createdBy
      };
  }

  public void Update(
      string? sku,
      string? barcode,
      decimal? price,
      decimal? purchasePrice,
      decimal? weight,
      decimal? length,
      decimal? width,
      decimal? height,
      int? lowQuantityLimit,
      string? variantOptionText,
      long? imageGalleryId,
      int? productVariantStatusId,
      EmployeeId? updatedBy)
  {
      SKU = sku?.Trim();
      Barcode = barcode?.Trim();
      Price = price;
      PurchasePrice = purchasePrice;
      Weight = weight;
      Length = length;
      Width = width;
      Height = height;
      LowQuantityLimit = lowQuantityLimit;
      VariantOptionText = variantOptionText;
      ImageGalleryId = imageGalleryId;
      ProductVariantStatusId = productVariantStatusId;
      UpdatedOn = DateTime.UtcNow;
      UpdatedBy = updatedBy;
  }
}
