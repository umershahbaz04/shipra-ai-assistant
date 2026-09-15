using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelProductPreProcessor;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using ShopifySharp;
using ShopifySharp.Filters;

namespace Shipra.Backend.API.Infrastructure.Services.Shopify;

public class ShopifyProductPreProcessorService : ISaleChannelProductPreProcessorService
{
  private readonly IShopifyRepository _shopifyRepository;
  private readonly ISaleChannelProductRepository _saleChannelProductRepository;
  private readonly ILogger<ShopifyProductPreProcessorService> _logger;

  public ShopifyProductPreProcessorService(
    IShopifyRepository shopifyRepository,
    ISaleChannelProductRepository saleChannelProductRepository,
    ILogger<ShopifyProductPreProcessorService> logger)
  {
    _shopifyRepository = shopifyRepository;
    _saleChannelProductRepository = saleChannelProductRepository;
    _logger = logger;
  }

  public async Task<ServiceResultDTO> PreProcessProductsAsync(
    SaleChannelProductPreProcessorCommand request,
    SaleChannelConfig oSaleChannelConfig,
    ClientId clientId,
    CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(oSaleChannelConfig.SaleChannelConfigId, clientId);
      if (oShopifyConfig != null)
      {
        var saleChannelExistingProductList = _saleChannelProductRepository.GetAllSaleChannelProduct(clientId, oSaleChannelConfig.SaleChannelLookupId);

        var shopifyProductList = await ListAllProductsOnShop(
          oShopifyConfig.ShopDomain!,
          oShopifyConfig.AccessToken!,
          Utility.ConvertUTCDateToDateTimeOffSet(request.CreatedFrom),
          Utility.ConvertUTCDateToDateTimeOffSet(request.CreatedTo));

        var shopifyProductListWithSKU = shopifyProductList
          .Where(x => x.Variants.Any(v => v.SKU != null && v.SKU != ""))
          .ToList();

        if (shopifyProductListWithSKU is not null && shopifyProductListWithSKU.Count > 0)
        {
          dynamic data = shopifyProductListWithSKU.Select(x => new
          {
            ImageSrc = x.Images.Select(s => s.Src).FirstOrDefault(),
            ProductName = x.Title,
            VariantCount = x.Variants.Count(),
            x.Variants,
            x.Vendor,
            x.CreatedAt,
            ProductId = x.Id,
            x.ProductType,
            ProductPrice = x.Variants.Sum(x => x.Price).GetValueOrDefault(),
            Description = x.BodyHtml,
            InventoryQuantity = x.Variants.Sum(x => x.InventoryQuantity).GetValueOrDefault(),
            SaleChannelLookupId = (int)EnumSaleChannelLookup.Shopify,
            request.StoreId,
            request.SaleChannelConfigId
          });
          serviceResult = new ServiceResultDTO(data);
          serviceResult.CreateSuccessResponse();
          return serviceResult;
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Shopify Product not found");
        }
      }
      else
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Shopify Config not found");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error pre-processing Shopify products");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<List<Product>> ListAllProductsOnShop(
    string shopDomain,
    string accesstoken,
    DateTimeOffset createdFrom,
    DateTimeOffset createdTo)
  {
    var executionPolicy = new LeakyBucketExecutionPolicy();
    var service = new ProductService(shopDomain, accesstoken);
    var allProducts = new List<Product>();
    var page = await service.ListAsync(new ProductListFilter
    {
      Limit = 250,
      CreatedAtMin = createdFrom,
      CreatedAtMax = createdTo,
    });

    while (true)
    {
      allProducts.AddRange(page.Items);
      if (!page.HasNextPage) break;
      page = await service.ListAsync(page.GetNextPageFilter());
    }
    return allProducts;
  }
}
