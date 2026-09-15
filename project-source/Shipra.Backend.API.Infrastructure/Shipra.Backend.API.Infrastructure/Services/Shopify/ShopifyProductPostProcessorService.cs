using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelProductPostProcessor;
using Shipra.Backend.API.Application.Features.ProductFeatures.Commands;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;
using ShopifySharp;
using ShopifySharp.Filters;
using Formatting = Newtonsoft.Json.Formatting;
using Product = ShopifySharp.Product;

namespace Shipra.Backend.API.Infrastructure.Services.Shopify;

public class ShopifyProductPostProcessorService : ISaleChannelProductPostProcessorService
{
  private readonly IShopifyRepository _shopifyRepository;
  private readonly IProductRepository _productRepository;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly ISaleChannelProductRepository _saleChannelProductRepository;
  private readonly IClientRepository _clientRepository;
  private readonly ILogger<ShopifyProductPostProcessorService> _logger;

  public ShopifyProductPostProcessorService(
    IShopifyRepository shopifyRepository,
    IProductRepository productRepository,
    ISaleChannelConfigRepository saleChannelConfigRepository,
    ISaleChannelProductRepository saleChannelProductRepository,
    IClientRepository clientRepository,
    ILogger<ShopifyProductPostProcessorService> logger)
  {
    _shopifyRepository = shopifyRepository;
    _productRepository = productRepository;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _saleChannelProductRepository = saleChannelProductRepository;
    _clientRepository = clientRepository;
    _logger = logger;
  }

  public async Task<ServiceResultDTO> PostProcessProductsAsync(
    SaleChannelProductPostProcessorCommand request,
    ClientId clientId,
    EmployeeId? employeeId,
    CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    var successProductList = new List<long?>();
    BaseResponseDto baseResponse = new BaseResponseDto();
    try
    {
      var oClient = await _clientRepository.GetClientById(clientId);
      var productOptionLookups = await _productRepository.GetProductOptionLookups();

      List<string> skusInput = (request.Skus ?? request.ProductIds ?? "")
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim())
        .ToList();

      List<Product>? shopifyProductList = null;
      decimal? productPrice = 0;
      decimal? weight = 0;
      long qtyAvailable = 0;
      bool isTrackInventory = false;

      if (skusInput.Count > 0)
      {
        var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(request.SaleChannelConfigId, clientId);
        if (oShopifyConfig != null)
        {
          var oSaleChannelConfig = await _saleChannelConfigRepository.GetSaleChannelConfigById(oShopifyConfig.SaleChannelConfigId, clientId);
          if (oSaleChannelConfig is not null && oSaleChannelConfig.StoreId == request.StoreId && oSaleChannelConfig.SaleChannelConfigId == request.SaleChannelConfigId)
          {
            shopifyProductList = await ListAllProductsOnShop(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken!);

            var shopifyProductListWithSKU = shopifyProductList
              .Where(x => x.Variants.Any(v => v.SKU != null && skusInput.Contains(v.SKU.Trim())))
              .ToList();

            if (shopifyProductListWithSKU is not null && shopifyProductListWithSKU.Count > 0)
            {
              var oSaleChannelProductList = await _saleChannelProductRepository.GetAllSaleChannelProduct(clientId, request.SaleChannelLookupId);
              var existingProductIds = oSaleChannelProductList?.Select(x => x.ProductId).Where(id => id != null).ToHashSet() ?? new HashSet<string?>();
              var productsToCreate = shopifyProductListWithSKU.Where(item => !existingProductIds.Contains(item.Id.ToString())).ToList();

              if (productsToCreate.Count > 0)
              {
                #region salechannelproduct
                foreach (var item in productsToCreate)
                {
                  DateTimeOffset dateTimeOffset = (DateTimeOffset)item.CreatedAt!;
                  DateTime productDate = dateTimeOffset.DateTime;
                  if (item.Variants.Count() > 0)
                  {
                    qtyAvailable = item.Variants.Sum(x => x.InventoryQuantity).GetValueOrDefault();
                    bool allSamePrice = item.Variants.Select(x => x.Price).Distinct().Count() == 1;
                    if (allSamePrice) productPrice = item.Variants.Select(x => x.Price).FirstOrDefault();
                    bool allSameWeight = item.Variants.Select(x => x.Weight).Distinct().Count() == 1;
                    if (allSameWeight) weight = item.Variants.Select(x => x.Weight).FirstOrDefault();
                    isTrackInventory = item.Variants.Select(x => x.InventoryManagement).Distinct().Count() == 1;
                  }

                  var oShipraProduct = Core.ProductAggregate.Product.CreateProductForShopify(
                    item.Variants.Select(v => v.SKU).FirstOrDefault(), item.Title,
                    ProductCommon.GetDescriptionForShopifyProductItem(item),
                    item.Images.Select(s => s.Src).FirstOrDefault(),
                    item.Variants.Count(), qtyAvailable, request?.StoreId,
                    oSaleChannelConfig.SaleChannelConfigId, productPrice, isTrackInventory,
                    oClient!.DefaultCurrencyId, weight, oClient.DefaultProductCategoryId,
                    clientId, employeeId);
                  await _productRepository.CreateProductAsync(oShipraProduct);

                  foreach (var productStock in item.Variants)
                  {
                    long? longQuantityAvailable = productStock.InventoryQuantity;
                    int intQuantityAvailable = Convert.ToInt32(longQuantityAvailable);
                    var oProductStock = Core.ProductAggregate.ProductStock.CreateProductStock(
                      oShipraProduct.ProductId, productStock.SKU, productStock!.Price!,
                      intQuantityAvailable, 0, oClient!.DefaultProductStationId,
                      ProductCommon.GetVarientDescriptionForShopifyProductItem(productStock),
                      employeeId!, productStock.Id!);
                    await _productRepository.CreateProductStockAsync(oProductStock);
                    if (intQuantityAvailable > 0 && productStock.RequiresShipping.GetValueOrDefault())
                    {
                      var productStockHistory = GetProductStockHistory(intQuantityAvailable, oProductStock.ProductStockId, employeeId);
                      var addedHistory = await _productRepository.CreateProductStockHistory(productStockHistory);
                    }
                  }
                  foreach (var productOption in item.Options)
                  {
                    foreach (var optionName in productOption.Values)
                    {
                      var objOption = productOptionLookups!.FirstOrDefault(x => x.Name == productOption.Name);
                      int optionId = objOption != null ? objOption.ProductOptionId : (int)EnumProductOptionLookup.Other;
                      var oProductOption = Core.ProductAggregate.ProductOption.CreateProductOption(oShipraProduct.ProductId, optionId.ToString(), optionName, 1);
                      await _productRepository.CreateProductOptionAsync(oProductOption);
                    }
                  }
                  await _saleChannelProductRepository.CreateSaleChannelProduct(
                    SaleChannelProduct.CreateSaleChannelProduct(
                      request?.SaleChannelLookupId!, item.Id.ToString(), "",
                      JsonConvert.SerializeObject(item, Formatting.Indented),
                      productDate, oShipraProduct.ProductId!, clientId, employeeId!));
                  successProductList.Add(item.Id);
                }
                #endregion
              }
            }
            else
            {
              throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Shopify Product not found.");
            }
          }
          else
          {
            throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Sale Channel Config not found.");
          }
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Shopify Config not found.");
        }
      }

      baseResponse = new BaseResponseDto { Data = successProductList, Message = "Products created successfully." };
      serviceResult = new ServiceResultDTO(baseResponse);
      return serviceResult;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error post-processing Shopify products");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private ProductStockHistory GetProductStockHistory(int? quantityAvailable, long productStockId, EmployeeId? employeeId)
  {
    return ProductStockHistory.CreateProductStockHistory((int)InventoryTransactionType.ExternalSync, productStockId, quantityAvailable, quantityAvailable, "Shopify Product ", employeeId!);
  }

  private async Task<List<Product>> ListAllProductsOnShop(string shopDomain, string accesstoken)
  {
    var executionPolicy = new LeakyBucketExecutionPolicy();
    var service = new ProductService(shopDomain, accesstoken);
    var allProducts = new List<Product>();
    var page = await service.ListAsync(new ProductListFilter { Limit = 250 });
    while (true)
    {
      allProducts.AddRange(page.Items);
      if (!page.HasNextPage) break;
      page = await service.ListAsync(page.GetNextPageFilter());
    }
    return allProducts;
  }
}
