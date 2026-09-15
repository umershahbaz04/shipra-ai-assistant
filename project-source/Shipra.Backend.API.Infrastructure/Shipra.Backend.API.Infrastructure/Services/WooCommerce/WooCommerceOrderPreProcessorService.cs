using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommereceOrder;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelOrderPreProcessor;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Services.WooCommerce;

public class WooCommerceOrderPreProcessorService : IWooCommerceOrderPreProcessorService
{
  private readonly ISaleChannelOrderRepository _saleChannelOrderRepository;
  private readonly IProductRepository _productRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly ILogger<WooCommerceOrderPreProcessorService> _logger;

  public WooCommerceOrderPreProcessorService(
    ISaleChannelOrderRepository saleChannelOrderRepository,
    IProductRepository productRepository,
    ICountryRepository countryRepository,
    ILogger<WooCommerceOrderPreProcessorService> logger)
  {
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

      //Deserialize SaleChannel Config values
      var saleChannelConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(oSaleChannelConfig.Config!);

      //Convert Keys To CamelCase with Deserialize Stripe object
      var saleChannelSetting = Utils.ConvertKeysToCamelCase(saleChannelConfig!);

      //Sale Channel keys
      var consumerKey = Utils.GetValueFromDictionryByKey("consumerKey", saleChannelSetting);
      var consumerSecret = Utils.GetValueFromDictionryByKey("consumerSecret", saleChannelSetting);
      var shopURL = Utils.GetValueFromDictionryByKey("shopURL", saleChannelSetting);

      if (!string.IsNullOrEmpty(consumerKey) && !string.IsNullOrEmpty(consumerSecret) && !string.IsNullOrEmpty(shopURL))
      {
        //2. Fetch Orders data from WooCommerce
        var data = await GetAllOrderListOnShopForWooCommerce(shopURL, consumerKey, consumerSecret);
        var oWooCommerceOrders = JsonConvert.DeserializeObject<List<OrderWooCommerceModal>>(data);
        var oWooCommerceOrderList = oWooCommerceOrders as List<OrderWooCommerceModal>;

        if (oWooCommerceOrderList != null && oWooCommerceOrderList!.Count > 0)
        {
          //3. Convert new order data OrderId to String (,) Ids
          var orderIds = string.Join(',', oWooCommerceOrderList.Select(x => x.OrderId));
          //4. Get record from existing Shipra sale order data to compare with new shopify order list
          var oSaleChannelOrderList = await _saleChannelOrderRepository.GetSaleChannelOrderListByOrderIds(orderIds, (int)EnumSaleChannelLookup.Shopify, clientId);

          if (oSaleChannelOrderList is not null && oSaleChannelOrderList.Count > 0)
          {
            //5 If order found in sale channel Order Data then first get orderIds
            List<string> saleChannelOrderIds = oSaleChannelOrderList.Select(x => x.OrderId!).ToList();

            //6 Convert the list of strings to a list of long values using LINQ
            List<long> longList = saleChannelOrderIds.Select(s => long.Parse(s!)).ToList();

            //7. Discard those record which already exist
            oWooCommerceOrderList = oWooCommerceOrderList.Where(x => !longList!.Contains((int)x.OrderId!)).ToList();
          }

          //Filter those oders from list have no customer address and zero Item count
          oWooCommerceOrderList = oWooCommerceOrderList.Where(x => !string.IsNullOrEmpty(x.Shipping!.first_name) && !string.IsNullOrEmpty(x.Shipping!.address_1) && x.Status! == "processing").ToList();

          if (request.OrderTypeId == (int)EnumOrderType.Regular)
          {
            //Filter those oders from list have no stock item
            oWooCommerceOrderList = oWooCommerceOrderList.Where(x => x.LineItems!.Any(v => v.SKU == null && v.VariationId == null)).ToList();
            WoocommerceOrderDetailResponse response = WoocommerceOrderDetailSimplified.ConverWooCommercetoShipraOrderDetail(oWooCommerceOrderList, countries, cities, provinces, states, areas, pinCodes, oStore, (int)EnumStationLookup.Dubai, oSaleChannelConfig, (int)EnumSaleChannelLookup.WooCommerce, request.OrderTypeId);
            serviceResult = new ServiceResultDTO(response.WoocommerceOrderDetail!);

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
            oWooCommerceOrderList = oWooCommerceOrderList.Where(x => x.LineItems!.Any(v => v.SKU != null && v.SKU != "")).ToList();
            //Filter and select shopify orders line items SKUs to match with Shipra
            var wooCommerceProductSKUsList = oWooCommerceOrderList!.SelectMany(order => order.LineItems!).Select(lineItem => lineItem.SKU);
            //Join list to comman seprated string
            var wooCommerceProductSKUs = string.Join(",", wooCommerceProductSKUsList);
            //Get all match SKUs product from shipra product stock
            oShipraProductList = await _productRepository.GetAllProductStocksForSaleChannelInventorySync(clientId.Value.ToString(), wooCommerceProductSKUs);
            var castedList = (IEnumerable<dynamic>)oShipraProductList!;
            //Filter Shipra Product SKUs List
            var productSKUs = castedList!.Select(p => p.SKU).ToList();
            //Filter record from shipra order list get only dose oders match with Shipra stock
            var matchedOrders = oWooCommerceOrderList.Where(order => order.LineItems!.Any(item => productSKUs.Contains(item.SKU!))).ToList();
            WoocommerceOrderDetailResponse response = WoocommerceOrderDetailSimplified.ConverWooCommercetoShipraOrderDetail(matchedOrders, countries, cities, provinces, states, areas, pinCodes, oStore, (int)EnumStationLookup.Dubai, oSaleChannelConfig, (int)EnumSaleChannelLookup.WooCommerce, request.OrderTypeId, castedList);
            serviceResult = new ServiceResultDTO(response.WoocommerceOrderDetail!);

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
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "WooCommerce Order not found");
        }
      }
      else
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "SaleChannelConfig not found");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error preprocessing woocommerce orders");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  public async Task<dynamic?> GetAllOrderListOnShopForWooCommerce(string baseURL, string consumerKey, string consumerSecret)
  {
    using (HttpClient client = new HttpClient())
    {
      client.BaseAddress = new Uri(baseURL + "//wp-json/wc/v3/");
      var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

      try
      {
        HttpResponseMessage response = await client.GetAsync("orders");
        if (response.IsSuccessStatusCode)
        {
          string responseBody = await response.Content.ReadAsStringAsync();
          return responseBody;
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Error: {response.StatusCode} - {response.ReasonPhrase}");
        }
      }
      catch (Exception ex)
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Exception: {ex.Message}");
      }
    }
  }
}
