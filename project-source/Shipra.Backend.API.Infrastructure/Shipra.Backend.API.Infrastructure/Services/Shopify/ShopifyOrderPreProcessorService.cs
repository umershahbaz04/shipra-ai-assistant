using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.ShopifyOrder;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelOrderPreProcessor;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using ShopifySharp;
using ShopifySharp.Filters;

namespace Shipra.Backend.API.Infrastructure.Services.Shopify;

public class ShopifyOrderPreProcessorService : IShopifyOrderPreProcessorService
{
  private readonly IShopifyRepository _shopifyRepository;
  private readonly ISaleChannelOrderRepository _saleChannelOrderRepository;
  private readonly IProductRepository _productRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly ILogger<ShopifyOrderPreProcessorService> _logger;

  public ShopifyOrderPreProcessorService(
    IShopifyRepository shopifyRepository,
    ISaleChannelOrderRepository saleChannelOrderRepository,
    IProductRepository productRepository,
    ICountryRepository countryRepository,
    ILogger<ShopifyOrderPreProcessorService> logger)
  {
    _shopifyRepository = shopifyRepository;
    _saleChannelOrderRepository = saleChannelOrderRepository;
    _productRepository = productRepository;
    _countryRepository = countryRepository;
    _logger = logger;
  }

  public async Task<ServiceResultDTO> PreProcessOrdersAsync(
    SaleChannelOrderPreProcessorCommand request,
    SaleChannelConfig oSaleChannelConfig,
    Store? oStore,
    ClientId clientId,
    CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    List<dynamic>? oShipraProductList = null;

    try
    {
      var countries = await _countryRepository.GetAllCountries();
      var cities = await _countryRepository.GetAllCities();
      var provinces = await _countryRepository.GetAllProvinces();
      var states = await _countryRepository.GetAllStates();
      var areas = await _countryRepository.GetAllAreas();
      var pinCodes = await _countryRepository.GetAllPinCodes();

      //1. Validate to get shopify config
      var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(oSaleChannelConfig.SaleChannelConfigId, clientId);
      if (oShopifyConfig is not null)
      {
        //2. Fetch Orders data from shopify
        var shopifyOrderList = await GetAllOrdersListOnShopForShopify(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken!, Utility.ConvertUTCDateToDateTimeOffSet(request.CreatedFrom), Utility.ConvertUTCDateToDateTimeOffSet(request.CreatedTo));
        if (shopifyOrderList is not null && shopifyOrderList.Count > 0)
        {
          //3. Convert new order data OrderId to String (,) Ids
          var orderIds = string.Join(',', shopifyOrderList.Select(x => x.Id));

          //4. Get record from existing Shipra sale order data to compare with new shopify order list
          var oSaleChannelOrderList = await _saleChannelOrderRepository.GetSaleChannelOrderListByOrderIds(orderIds, (int)EnumSaleChannelLookup.Shopify, clientId);

          if (oSaleChannelOrderList is not null && oSaleChannelOrderList.Count > 0)
          {
            //5 If order found in sale channel Order Data then first get orderIds
            List<string> saleChannelOrderIds = oSaleChannelOrderList.Select(x => x.OrderId!).ToList();

            //6 Convert the list of strings to a list of long values using LINQ
            List<long> longList = saleChannelOrderIds.Select(s => long.Parse(s!)).ToList();

            //7. Discard those record which already exist
            shopifyOrderList = shopifyOrderList.Where(x => !longList!.Contains((long)x.Id!)).ToList();
          }
          //Filter those oders from list have no customer address and zero Item count
          shopifyOrderList = shopifyOrderList.Where(x => x.ShippingAddress.Name != null && x.ShippingAddress.Address1 != null && x.LineItems.Count() > 0).ToList();
          if (request.OrderTypeId == (int)EnumOrderType.Regular)
          {
            //Filter those oders from list have no stock item
            shopifyOrderList = shopifyOrderList.Where(x => x.LineItems.Any(v => v.SKU == null && v.VariantId == null)).ToList();
            ShopifyOrderDetailResponse response = ShopifyOrderDetailSimplified.ConverShopifytoShipraOrderDetail(shopifyOrderList, countries, cities, provinces, states, areas, pinCodes, oStore, (int)EnumStationLookup.Dubai, oSaleChannelConfig, (int)EnumSaleChannelLookup.Shopify, request.OrderTypeId);
            serviceResult = new ServiceResultDTO(response.ShopifyOrderDetail!);

            if (!response.IsSuccessed)
            {
              serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
              serviceResult.IsSuccess = false;
              var erMSg = response.Errors?.Select(x => new { x.IsSuccessed, x.Row, Msg = $"Please correct the following: " + string.Join(',', x.Msg) });
              var json = JsonConvert.SerializeObject(erMSg);
              serviceResult.IsSuccess = response.IsSuccessed;
              serviceResult.Errors?.Add("InvalidParameter", new[] { json });
            }
          }
          else if (request.OrderTypeId == (int)EnumOrderType.FullFilable)
          {
            //Filter those oders from list have  SKUs
            shopifyOrderList = shopifyOrderList.Where(x => x.LineItems.Any(v => v.SKU != null && v.SKU != "")).ToList();
            //Filter and select shopify orders line items SKUs to match with Shipra
            var shopifyProductSKUsList = shopifyOrderList!.SelectMany(order => order.LineItems).Select(lineItem => lineItem.SKU);
            //Join list to comman seprated string
            var shopifyProductSKUs = string.Join(",", shopifyProductSKUsList);
            //Get all match SKUs product from shipra product stock
            oShipraProductList = await _productRepository.GetAllProductStocksForSaleChannelInventorySync(clientId.Value.ToString(), shopifyProductSKUs);
            var castedList = (IEnumerable<dynamic>)oShipraProductList!;
            //Filter Shipra Product SKUs List
            var productSKUs = castedList!.Select(p => p.SKU).ToList();
            //Filter record from shipra order list get only dose oders match with Shipra stock
            var matchedOrders = shopifyOrderList.Where(order => order.LineItems.Any(item => productSKUs.Contains(item.SKU))).ToList();

            ShopifyOrderDetailResponse response = ShopifyOrderDetailSimplified.ConverShopifytoShipraOrderDetail(matchedOrders, countries, cities, provinces, states, areas, pinCodes, oStore, (int)EnumStationLookup.Dubai, oSaleChannelConfig, (int)EnumSaleChannelLookup.Shopify, request.OrderTypeId, castedList);
            serviceResult = new ServiceResultDTO(response.ShopifyOrderDetail!);

            if (!response.IsSuccessed)
            {
              serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
              serviceResult.IsSuccess = false;
              var erMSg = response.Errors?.Select(x => new { x.IsSuccessed, x.Row, Msg = $"Please correct the following: " + string.Join(',', x.Msg) });
              var json = JsonConvert.SerializeObject(erMSg);
              serviceResult.IsSuccess = response.IsSuccessed;
              serviceResult.Errors?.Add("InvalidParameter", new[] { json });
            }
          }
          serviceResult.CreateSuccessResponse();
          return serviceResult;
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Shopify Order not found");
        }
      }
      else
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Shopify Config not found");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error preprocessing shopify orders");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  public async Task<List<ShopifySharp.Order>> GetAllOrdersListOnShopForShopify(string shopDomain, string accesstoken, DateTimeOffset createdFrom, DateTimeOffset createdTo)
  {
    var executionPolicy = new LeakyBucketExecutionPolicy();
    var service = new OrderService(shopDomain, accesstoken);
    var allOrders = new List<ShopifySharp.Order>();

    var page = await service.ListAsync(new OrderListFilter
    {
      Limit = 250,
      CreatedAtMin = createdFrom,
      CreatedAtMax = createdTo,
    });

    while (true)
    {
      allOrders.AddRange(page.Items);
      if (!page.HasNextPage)
      {
        break;
      }
      page = await service.ListAsync(page.GetNextPageFilter());
    }
    return allOrders;
  }
}
