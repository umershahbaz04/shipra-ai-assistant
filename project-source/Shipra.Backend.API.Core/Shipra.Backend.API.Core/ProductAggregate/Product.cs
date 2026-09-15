using System.Xml.Linq;
using Ardalis.GuardClauses;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.SharedKernel;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Core.ProductAggregate;


public class Product : EntityBase, IAggregateRoot
{
  public Product()
  {

  }
  public ProductId? ProductId { get; private set; } // = new ProductId(Guid.NewGuid()); 
  public string? Sku { get; private set; } = null!;
  public string? ProductName { get; private set; }
  public decimal? Price { get; private set; }
  public decimal? PurchasePrice { get; private set; }
  public string? Description { get; private set; }
  public int? ProductCategoryId { get; private set; }
  public int? CurrencyId { get; private set; }
  public string? FeatureImage { get; private set; }
  public decimal? Weight { get; private set; }
  public bool? HaveOptions { get; private set; }
  public int? VarientCount { get; private set; }
  public int? QuantityAvailable { get; private set; }
  public ClientId? ClientId { get; private set; } 
  public int? ProductStatusId { get; private set; }
  public int? SaleChannelConfigId { get; private set; }
  public bool? TrackInventory { get; private set; }
  public bool? Active { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }

  public static Product CreateProduct(string? sku, string? productName, decimal? price, decimal? purchasePrice, string? description, int? productCategoryId, string? featureImage, decimal? weight, bool? haveOptions, int? varientCount, int? quantityAvailable, ClientId? clientId, int? productStatusId, bool? trackInventory, int currencyId, EmployeeId? createdBy)
  {
    return new Product()
    {
      ProductId = ProductId.New,
      Sku = sku?.Trim(),
      ProductName = productName,
      Price = price,
      PurchasePrice = purchasePrice,
      Description = description,
      ProductCategoryId = productCategoryId,
      FeatureImage = featureImage,
      Weight = weight,
      HaveOptions = haveOptions,
      VarientCount = varientCount,
      QuantityAvailable = quantityAvailable,
      ClientId = clientId, 
      ProductStatusId = productStatusId,
      TrackInventory = trackInventory,
      CurrencyId = currencyId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true,
    };
  }

  public static Product CreateProductForShopify(string? sku, string title, string? description, string? featureImage, int varientCount, long quantityAvailable, int? storeId, int saleChannelConfigId, decimal? price, bool? trackInventory, int? currencyId, decimal? weight, int? productCategoryId, ClientId? clientId, EmployeeId? createdBy)
  {
    return new Product
    {
      ProductId = ProductId.New,
      Sku = sku?.Trim(),
      ProductName = title,
      Description = description,
      FeatureImage = featureImage,
      Price = price,
      Weight = weight,
      HaveOptions = varientCount > 0 ? true : false,
      VarientCount = varientCount,
      SaleChannelConfigId = saleChannelConfigId,
      QuantityAvailable = (int)quantityAvailable,
      ProductStatusId = (int)EnumProductStockStatus.Active,
      ProductCategoryId = productCategoryId,
      TrackInventory = trackInventory,
      CurrencyId = currencyId,
      ClientId = clientId, 
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public static Product CreateProductForWooCommerce(string name, string? sku, string description, string featureImage, int varientCount, ClientId? clientId, int? storeId, EmployeeId? createdBy)
  {
    return new Product
    {
      ProductId = ProductId.New,
      ProductName = name,
      Sku = sku?.Trim()!,
      Description = description,
      FeatureImage = featureImage,
      VarientCount = varientCount,
      ClientId = clientId, 
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
    };
  }

  public void DisableProduct(EmployeeId employeeId)
  {
    Active = false;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }
  public void EnableProduct(EmployeeId employeeId)
  {
    Active = true;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }

  public void UpdateProduct(string? sku, string? productName, decimal? price, decimal? purchasePrice, string? description, int? productCategoryId, string? featureImage, decimal? weight, bool? haveOptions, int? varientCount, int? quantityAvailable, ClientId? clientId, int? storeId, EmployeeId? updatedBy)
  {
    Sku = sku?.Trim();
    ProductName = productName;
    Price = price;
    PurchasePrice = purchasePrice;
    Description = description;
    ProductCategoryId = productCategoryId;
    FeatureImage = featureImage;
    Weight = weight;
    HaveOptions = haveOptions;
    VarientCount = varientCount;
    QuantityAvailable = quantityAvailable;
    ClientId = clientId; 
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateVarientCount(int varientCount, EmployeeId? employeeId)
  {
    VarientCount = varientCount;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
  }
}
public sealed record ProductId(Guid Value)
{
  public static ProductId New => new(Guid.NewGuid());
}
