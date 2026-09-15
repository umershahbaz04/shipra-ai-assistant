using System.Text;
using Shipra.Backend.API.Application.DTOs.ProductStationTransferUseCase.Request;
using Shipra.Backend.API.Application.DTOs.ProductUseCase.Request;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands;
public sealed class ProductCommon
{
  public List<ProductOption> GetProductOptions(List<ProductOptionReuqestModel>? productOptions, ProductId? productId)
  {
    var options = new List<ProductOption>();
    if (productId != null)
    {
      foreach (var item in productOptions!)
      {
        var option = ProductOption.CreateProductOption(productId, item.OptionId, item.OptionValue, item.DisplayOrder);
        options.Add(option);
      }
    }
    return options;
  }
  public List<ProductStock> GetProductStocks(List<ProductStockReuqestModel>? productStocks, ProductId? productId)
  {
    var productStockList = new List<ProductStock>();

    foreach (var item in productStocks!)
    {
      var productStock = ProductStock.CreateProductStock(productId, item.Sku, item.Price, item.QuantityAvailable, item.LowQuantityLimit, item.ProductStationId, item.VarientOption, EmployeeId.New,item.ImageGalleryId);
      //productStock.UpdateLoWQuantityLimit(item.QuantityAvailable, item.LowQuantityLimit);
      productStockList.Add(productStock);
    }

    return productStockList;
  }

  public List<ProductVariant> GetProductVariants(List<ProductStockReuqestModel>? productStocks, ProductId? productId, ClientId? clientId, decimal? purchasePrice, decimal? weight, EmployeeId? createdBy)
  {
      var productVariantList = new List<ProductVariant>();
      
      // Deduplicate by SKU since frontend sends Cartesian product of Variants x Stations
      var uniqueVariants = productStocks!
          .GroupBy(x => x.Sku)
          .Select(g => g.First())
          .ToList();

      foreach (var item in uniqueVariants)
      {
          var variantWeight = item.VariantAttributes?.Weight ?? weight;
          var variant = ProductVariant.Create(
              productId, 
              clientId, 
              item.Sku, 
              item.VariantAttributes?.Barcode, 
              item.Price, 
              purchasePrice, 
              variantWeight, 
              item.VariantAttributes?.Length, 
              item.VariantAttributes?.Width, 
              item.VariantAttributes?.Height, 
              item.LowQuantityLimit, 
              item.VarientOption, 
              item.ImageGalleryId, 
              item.ProductStockStatusId, 
              createdBy);
          productVariantList.Add(variant);
      }
      return productVariantList;
  }

  public List<InventoryBalance> GetInventoryBalances(List<ProductVariant> variants, List<ProductStockReuqestModel>? productStocks)
  {
      var balances = new List<InventoryBalance>();
      
      foreach (var stockModel in productStocks!)
      {
          // Only create InventoryBalance for stations where they entered quantity
          if (stockModel.ProductStationId.HasValue && stockModel.QuantityAvailable.GetValueOrDefault() > 0)
          {
              var variant = variants.FirstOrDefault(v => v.SKU == stockModel.Sku);
              if (variant != null)
              {
                  var balance = InventoryBalance.Create(variant.ProductVariantId, stockModel.ProductStationId.Value, stockModel.QuantityAvailable ?? 0);
                  balances.Add(balance);
              }
          }
      }
      return balances;
  }

  public List<InventoryTransaction> GetInventoryTransactions(List<ProductVariant> variants, List<ProductStockReuqestModel>? productStocks, EmployeeId? createdBy)
  {
      var transactions = new List<InventoryTransaction>();
      
      foreach (var stockModel in productStocks!)
      {
          if (stockModel.ProductStationId.HasValue && stockModel.QuantityAvailable.GetValueOrDefault() > 0)
          {
              var variant = variants.FirstOrDefault(v => v.SKU == stockModel.Sku);
              if (variant != null)
              {
                  var transaction = InventoryTransaction.Create(
                      variant.ProductVariantId,
                      stockModel.ProductStationId.Value,
                      1, // Receipt/AddStock TransactionType
                      stockModel.QuantityAvailable.GetValueOrDefault(),
                      0,
                      stockModel.QuantityAvailable.GetValueOrDefault(),
                      "New Product Created with stock",
                      createdBy);
                  transactions.Add(transaction);
              }
          }
      }
      return transactions;
  }
  public List<TransferProduct> GetTransferProducts(List<TransferProductRequestModel>? transferProducts, ProductStaionTransferId? productStaionTransferId)
  {
    var transferProductList = new List<TransferProduct>();
    foreach (var item in transferProducts!)
    {
      var transferProduct = TransferProduct.CreateTransferProduct(productStaionTransferId, item.ProductSku, new ProductId(new Guid(item.ProductId!)), item.Quantity, item.Accepted, item.Rejected);
      transferProductList.Add(transferProduct);
    }
    return transferProductList;
  }
  public static string GetDescriptionForShopifyProductItem(ShopifySharp.Product item)
  {
    StringBuilder productDes = new StringBuilder();
    productDes.AppendLine($"Name :{item.Title} - Description: {item.BodyHtml}");

    return productDes.ToString();
  }

  public static bool GetTrackInventoryForShopifyProduct(List<ShopifySharp.ProductVariant> item)
  {
    bool isTrackInventory = false;
    if (item is not null && item.Count > 0)
    {
      isTrackInventory = item.Select(x => x.InventoryManagement).Distinct().Count() == 1;
    }

    return isTrackInventory;
  }

  public static string GetVarientDescriptionForShopifyProductItem(ShopifySharp.ProductVariant item)
  {
    StringBuilder productDes = new StringBuilder();
    if (!string.IsNullOrEmpty(item.Option1) && !string.IsNullOrEmpty(item.Option2) && !string.IsNullOrEmpty(item.Option3))
    {
      productDes.AppendLine($"{item.Option1} / {item.Option2} / {item.Option3}");
    }
    else if (!string.IsNullOrEmpty(item.Option1) && !string.IsNullOrEmpty(item.Option2))
    {
      productDes.AppendLine($"{item.Option1} / {item.Option2}");
    }
    else if (!string.IsNullOrEmpty(item.Option1))
    {
      productDes.AppendLine($"{item.Option1}");
    }
    return productDes.ToString();
  }
}
