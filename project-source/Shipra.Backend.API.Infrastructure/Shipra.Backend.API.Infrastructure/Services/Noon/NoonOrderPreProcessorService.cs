using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelOrderPreProcessor;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Services.Noon;

public class NoonOrderPreProcessorService : ISaleChannelOrderPreProcessorService
{
  private readonly ISaleChannelOrderRepository _saleChannelOrderRepository;
  private readonly IProductRepository _productRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly ILogger<NoonOrderPreProcessorService> _logger;

  public NoonOrderPreProcessorService(ISaleChannelOrderRepository saleChannelOrderRepository, IProductRepository productRepository, ICountryRepository countryRepository, ISaleChannelConfigRepository saleChannelConfigRepository, ILogger<NoonOrderPreProcessorService> logger)
  {
    _saleChannelOrderRepository = saleChannelOrderRepository;
    _productRepository = productRepository;
    _countryRepository = countryRepository;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _logger = logger;
  }

  public async Task<ServiceResultDTO> PreProcessOrdersAsync(SaleChannelOrderPreProcessorCommand request,SaleChannelConfig oSaleChannelConfig,Store? oStore,ClientId clientId,CancellationToken cancellationToken)
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

      var saleChannelConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(oSaleChannelConfig.Config!);
      var saleChannelSetting = Utils.ConvertKeysToCamelCase(saleChannelConfig!);

      var privateKey = Utils.GetValueFromDictionryByKey("privateKey", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("private_key", saleChannelSetting);
      var keyId = Utils.GetValueFromDictionryByKey("keyId", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("key_id", saleChannelSetting);
      var channelIdentifier = Utils.GetValueFromDictionryByKey("channelIdentifier", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("channel_identifier", saleChannelSetting);
      var projectCode = Utils.GetValueFromDictionryByKey("projectCode", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("project_code", saleChannelSetting);
      var warehouseCode = Utils.GetValueFromDictionryByKey("warehouseCode", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("warehouse_code", saleChannelSetting) ?? "W00221196AE";
      
      var saleChannelLookup = await _saleChannelConfigRepository.GetSaleChannelLookupById(oSaleChannelConfig.SaleChannelLookupId ?? (int)EnumSaleChannelLookup.Noon);
      var baseUrl = saleChannelLookup?.AppUrl ?? "";

      // Fallback for testing based on provided credentials file if missing in DB
      if (string.IsNullOrEmpty(privateKey))
      {
          privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCQgWlha5HZePxU\nyMnDLvFr+OByZmeGIOuwp6V2kKdIJcMnoyvrBB/dL2Sl5rpRlXDl02/gFj6lTO1V\nhnSkSZlNfuaMCIR+7WDkEgkOXtNTe2Dh1rsYK8oRKyFkt5d9mTbAy02ijveiS6Mq\nRdYbDnxz9rzLCAYBfRpumbiOpAyn+mmm2RydFRmAcM1t2jvPhK8ng3hM4UeG9goI\n39UTYkPPCbYY06lNlul5ujqFQFzy14Fi7L0Kukk7bGkKod8vHZiSrhJ6zWHrf4mz\nYIsZUCTm7YLnKeKXeg2WvmUH11GOcX0lfsytT1I3j1YCePP2/MUoLWi80GJ/fHCf\nnBiiW4JvAgMBAAECggEAFzoV3CjUKqZ9uIsFky/qcjZwrTK0lSSZfa2UtPgPS1N2\niNp7Zq0lCgJiJSBu9koU+XwA0X4B18QDqemQug9yarhpCj0cPuKc3kvf1MV9JkAA\nlIxVSk9PjW7nUS8JVJDZ8ic7dVORji6mLVdIUNUFQAZ61g+WF4sqQnjG53aK6jzi\nBUjmQ8bLFZLIYqn+jiiR+K4hHhSZnWhaTJkujv+01NLut32pY8XpX8AdWKB4oiYH\nUBaWtI0qW7x9h1XmhptzOByyJ/Y8x1xBZuuoi6r5KW9q8qyLYaI5cAj1Q0uc8IW1\n9yYAVoANk1MSR3JLbwIef8aCNeYyf34r0FUTZ2KHEQKBgQDB6VkG1piVijOIBgmb\ntwUBJFwydFF1rc7FwnR5ANQqTeZYJxOmLNfDAiM1AdfaeO4rkMTwInQrLwBSh/4V\nihR1Cl5ZPaSjI2ATK3Wq277g5N5pXJ6D6nzgji9n9O2vsZY9tyh4c8uP6LTRz/BC\nwAHB18PWs6N7tBf/ovAiPqBq+wKBgQC+xlLxQcU0+Hp3F8EK09eBRAm56I8+bjln\nP0mAt0T0Nj6MTYGVJk34l7rdcfayJGZoYo/X6a/C8RX6hLHvWXTNUZW1I+AQQK+C\nHmt/GLA04dHyK1cqz3WLE63rCRK++TnFjwARhu6jdZNed7ubv55ie/x0Ile2ZFWi\nQn6rYbDsHQKBgDKj2O8TPdfXtqtwQDQdML5im31FqTxdPqGgrcAn+kBuBZjB47zC\n+znfJgiiyZcxe6l+7h90L/hTFvd2smE3pS4HnioaEhPUmjOHZvxO1ONwgbDsUi1L\nIH+YQkMY0LXQX9cQLQ5/1wpnEEm2zxzvfcX8rhU05p3Yo2fMSn/28PffAoGAIfZG\nf8KYq/RsQNVOvXG3FMEbBiibj56pw3Kl0C9QLDWX7vxBTF8UVGQWlSObql0GiiC5\nwNNOQeMPaZjD4HtJat/SSfwIAHyzgfOOaYLoo5FsAbOrgeiK4WZweL4Vwz+1BDGP\n7o7Z3umogZHJKVH0jU3LRJV0jfjQseEqkbIDgBUCgYEAvhfgyWxbuxUJcqm8LYlr\n+NXUhNHrfR3DpNk2i+EBhfjD+nM5jYt6Xdo/LLPuqSwbe7e7VWl41zKFCzdFdLdB\n+lns7Bfylz0ORV8UO16Juw9cORfzdlKPOwnct2/V8y2BMlJDbsBFiDcl6TvcrIcK\nEU/qwlaSVcsAL6+mibc0Xvk=\n-----END PRIVATE KEY-----";
          keyId = "noon-partners-key-id-ced2e157b4f244578939d31ccd35044b";
          channelIdentifier = "integrationnoon@p581228.idp.noon.partners";
          projectCode = "PRJ581228";
      }

      if (string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(channelIdentifier))
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Noon integration parameters are missing in SaleChannelConfig.");
      }

      var jwtToken = NoonAuthHelper.GenerateSessionJwtToken(privateKey, keyId);
      var sessionCookie = await ExchangeJwtForSessionCookie(baseUrl, jwtToken, projectCode);

      if (string.IsNullOrEmpty(sessionCookie))
      {
         throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Failed to retrieve Noon Session Cookie.");
      }

       
      // Step 2: Fetch Orders from Noon FBPI API
      var orderNrs = await GetNoonFbpiOrdersListAsync(baseUrl, sessionCookie, warehouseCode, request.CreatedFrom, true);
      
      if (orderNrs == null || orderNrs.Count == 0)
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Noon Orders not found");
      }

      // Step 3: Fetch order details
      var rawOrders = new List<NoonFbpiOrderResponse>();

      //foreach(var orderNr in orderNrs)
      //{
      //    var orderDetail = await GetNoonFbpiOrderDetailsAsync(baseUrl, sessionCookie, orderNr, true);
      //    if (orderDetail != null)
      //    {
      //        rawOrders.Add(orderDetail);
      //    }
      //}


      // Filter existing database orders
      var noonOrderIdsStr = string.Join(',', rawOrders.Select(x => x.FbpiOrderNr).Where(id => !string.IsNullOrEmpty(id)));
      var oSaleChannelOrderList = await _saleChannelOrderRepository.GetSaleChannelOrderListByOrderIds(noonOrderIdsStr, oSaleChannelConfig.SaleChannelLookupId ?? (int)EnumSaleChannelLookup.Noon, clientId);

      if (oSaleChannelOrderList is not null && oSaleChannelOrderList.Count > 0)
      {
        List<string> saleChannelOrderIds = oSaleChannelOrderList.Select(x => x.OrderId!).ToList();
        rawOrders = rawOrders.Where(x => !saleChannelOrderIds.Contains(x.FbpiOrderNr!)).ToList();
      }

      if (rawOrders.Count == 0)
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "All fetched Noon orders are duplicates or missing details.");
      }

      // Step 4: Regular vs Fulfillable order filtering and mapping
      var mappedOrders = new List<CreateUploadOrderResponseModel>();
      var errors = new List<NoonOrderError>();

      if (request.OrderTypeId == (int)EnumOrderType.Regular)
      {
        var (shipments, mappingErrors) = MapNoonToShipraOrders(rawOrders, countries, cities, provinces, states, areas, pinCodes, oStore, oSaleChannelConfig, request.OrderTypeId);
        mappedOrders.AddRange(shipments);
        errors.AddRange(mappingErrors);
      }
      else if (request.OrderTypeId == (int)EnumOrderType.FullFilable)
      {
        // Keep orders containing at least one valid item
        rawOrders = rawOrders.Where(x => x.Items != null && x.Items.Any(item => !string.IsNullOrEmpty(item.PartnerSku))).ToList();

        var noonProductSKUsList = rawOrders.SelectMany(x => x.Items!).Select(item => item.PartnerSku).Where(sku => !string.IsNullOrEmpty(sku));
        var noonProductSKUs = string.Join(",", noonProductSKUsList); // For Amazon they were hardcoding for test, here using actual SKUs

        // Fetch products matching noon SKUs
        oShipraProductList = await _productRepository.GetAllProductStocksForSaleChannelInventorySync(clientId.Value.ToString(), noonProductSKUs);
        var castedProductList = (IEnumerable<dynamic>)oShipraProductList!;
        var productSKUs = castedProductList.Select(p => (string)p.SKU).ToList();

        // Filter orders by SKUs matched in stock
        rawOrders = rawOrders.Where(x => x.Items!.Any(item => productSKUs.Contains(item.PartnerSku!))).ToList();

        var (shipments, mappingErrors) = MapNoonToShipraOrders(rawOrders, countries, cities, provinces, states, areas, pinCodes, oStore, oSaleChannelConfig, request.OrderTypeId, castedProductList);
        mappedOrders.AddRange(shipments);
        errors.AddRange(mappingErrors);
      }

      serviceResult = new ServiceResultDTO(mappedOrders);
      var isSuccessed = errors.All(x => x.IsSuccessed);
      serviceResult.IsSuccess = isSuccessed;

      if (!isSuccessed)
      {
        serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
        var erMsg = errors.Select(x => new { x.IsSuccessed, x.Row, Msg = $"Please correct the following: " + string.Join(',', x.Msg) });
        var json = JsonConvert.SerializeObject(erMsg);
        serviceResult.Errors?.Add("InvalidParameter", new[] { json });
      }
      else
      {
        serviceResult.CreateSuccessResponse();
      }

      return serviceResult;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error preprocessing noon orders");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<string?> ExchangeJwtForAccessToken(string baseUrl, string jwtToken, string projectCode)
  {
      using (var client = new HttpClient())
      {
          var normalizedBaseUrl = baseUrl.TrimEnd('/');
          var requestUri = $"{normalizedBaseUrl}/identity/public/v1/api/login";
          var requestBody = new { token = jwtToken, default_project_code = projectCode };
          var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
          var response = await client.PostAsync(requestUri, content);
          
          if (response.IsSuccessStatusCode)
          {
              var responseString = await response.Content.ReadAsStringAsync();
              var authResponse = JsonConvert.DeserializeObject<NoonAuthResponse>(responseString);
              return authResponse?.Token; 
          }
          return null;
      }
  }

  private async Task<string?> ExchangeJwtForSessionCookie(string baseUrl, string jwtToken, string projectCode)
  {
      using (var client = new HttpClient())
      {
          var normalizedBaseUrl = baseUrl.TrimEnd('/');
          var requestUri = $"{normalizedBaseUrl}/identity/public/v1/api/login";
          var requestBody = new { token = jwtToken, default_project_code = projectCode };
          var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
          var response = await client.PostAsync(requestUri, content);
          
          if (response.IsSuccessStatusCode)
          {
              if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
              {
                  var cookieList = new List<string>();
                  foreach (var cookie in cookies)
                  {
                      var parts = cookie.Split(';');
                      if (parts.Length > 0)
                      {
                          cookieList.Add(parts[0]);
                      }
                  }
                  return string.Join("; ", cookieList);
              }
          }
          return null;
      }
  }

  private async Task<List<string>?> GetNoonFbpiOrdersListAsync(string baseUrl, string authIdentifier, string warehouseCode, DateTime createdAfter, bool isCookieAuth = false)
  {
      using (var client = new HttpClient())
      {
          var normalizedBaseUrl = baseUrl.TrimEnd('/');
          var requestUri = $"{normalizedBaseUrl}/fbpi/v1/fbpi-orders/list";

          if (isCookieAuth)
          {
              client.DefaultRequestHeaders.Add("Cookie", authIdentifier);
          }
          else
          {
              client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authIdentifier);
          }

          var formattedCreatedFrom = createdAfter.ToString("yyyy-MM-ddTHH:mm:ssZ");
          var requestBody = new NoonOrderListRequest 
          { 
              WarehouseCode = "W00221196AE", //warehouseCode,
              CreatedAfter = formattedCreatedFrom,
              CreatedBefore = "" 
          };
          var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

          var response = await client.PostAsync(requestUri, content);

          if (response.IsSuccessStatusCode)
          {
              var responseContent = await response.Content.ReadAsStringAsync();
              var ordersResponse = JsonConvert.DeserializeObject<NoonOrderListResponse>(responseContent);
              return ordersResponse?.Items?.Select(x => x.FbpiOrderNr!).ToList();
          }
          return null;
      }
  }

  private async Task<NoonFbpiOrderResponse?> GetNoonFbpiOrderDetailsAsync(string baseUrl, string authIdentifier, string orderNr, bool isCookieAuth = false)
  {
      using (var client = new HttpClient())
      {
          var normalizedBaseUrl = baseUrl.TrimEnd('/');
          var requestUri = $"{normalizedBaseUrl}/fbpi/v1/fbpi-order/{orderNr}/get";

          if (isCookieAuth)
          {
              client.DefaultRequestHeaders.Add("Cookie", authIdentifier);
          }
          else
          {
              client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authIdentifier);
          }

          var response = await client.GetAsync(requestUri);

          if (response.IsSuccessStatusCode)
          {
              var responseContent = await response.Content.ReadAsStringAsync();
              return JsonConvert.DeserializeObject<NoonFbpiOrderResponse>(responseContent);
          }
          return null;
      }
  }

  private (List<CreateUploadOrderResponseModel> Shipments, List<NoonOrderError> Errors) MapNoonToShipraOrders(
    List<NoonFbpiOrderResponse> noonOrders,
    List<Country>? countries,
    List<Core.CountryAggregate.City> cities,
    List<Province> provinces,
    List<State> states,
    List<Area> areas,
    List<PinCode> pinCodes,
    Store? oStore,
    SaleChannelConfig oSaleChannelConfig,
    int? orderTypeId,
    IEnumerable<dynamic>? castedProductList = null)
  {
    var shipments = new List<CreateUploadOrderResponseModel>();
    var errors = new List<NoonOrderError>();

    int row = 0;
    foreach (var x in noonOrders)
    {
      row++;
      var amount = x.Items?.Sum(i => i.Price) ?? 0;
      var obj = new CreateUploadOrderResponseModel
      {
        StoreId = oStore?.StoreId ?? 0,
        OrderTypeId = orderTypeId,
        OrderDate = x.CreatedAt ?? DateTime.UtcNow,
        Description = $"Noon Order {x.FbpiOrderNr}",
        Remarks = string.Empty,
        Amount = amount,
        CShippingCharges = 0,
        PaymentStatusId = (int)EnumPaymentStatus.Paid, // Typically pre-paid
        Weight = 0,
        ItemValue = amount,
        OrderRequestVia = (int)EnumOrderRequestVia.Api,
        PaymentMethodId = (int)EnumPaymentMethod.PP, // Pre-paid
        StationId = (int)EnumStationLookup.Dubai,
        Discount = 0,
        VAT = 0,
        SCOrderNo = x.FbpiOrderNr,
        SCOrderId = x.FbpiOrderNr,
        SaleChannelConfigId = oSaleChannelConfig.SaleChannelConfigId,
        SaleChannelLookupId = oSaleChannelConfig.SaleChannelLookupId,
        OrderNote = new OrderNoteModel { Note = "" },
        OrderAddress = new OrderAddressResponseModel
        {
          CustomerName = "Noon Customer", // Replace with customer details fetch if applicable
          Email = string.Empty,
          Mobile1 = string.Empty,
          StreetAddress = "Noon Address"
        },
        OrderItems = new List<OrderItemModel>()
      };

      var error = new NoonOrderError
      {
        Row = row,
        IsSuccessed = true
      };

      // Country Lookup
      Country? matchedCountry = null;
      if (!string.IsNullOrEmpty(x.CountryCode))
      {
        matchedCountry = countries?.FirstOrDefault(y => y.MapCountryCode?.Trim().ToLower() == x.CountryCode.Trim().ToLower());
        if (matchedCountry != null)
        {
          obj.OrderAddress.Country = matchedCountry.CountryId;
        }
      }

      // Map Order Items
      if (orderTypeId == (int)EnumOrderType.Regular)
      {
        foreach (var item in x.Items ?? new List<NoonFbpiOrderItem>())
        {
          obj.OrderItems.Add(new OrderItemModel
          {
            ProductId = "",
            ProductVariantId = 0,
            Price = item.Price,
            Description = $"SKU: {item.PartnerSku} - ItemNr: {item.MpItemNr}",
            Remarks = string.Empty,
            Quantity = 1, // Usually 1 item per MP Item Nr in Noon
            Discount = 0
          });
        }
      }
      else if (orderTypeId == (int)EnumOrderType.FullFilable && castedProductList != null)
      {
        foreach (var item in x.Items ?? new List<NoonFbpiOrderItem>())
        {
          var product = castedProductList.FirstOrDefault(p => (string)p.SKU == item.PartnerSku);
          if (product != null)
          {
            obj.OrderItems.Add(new OrderItemModel
            {
              ProductId = product.ProductId,
              ProductVariantId = product.ProductVariantId,
              Price = item.Price,
              Description = $"SKU: {item.PartnerSku} - ItemNr: {item.MpItemNr}",
              Remarks = string.Empty,
              Quantity = 1,
              Discount = 0
            });
          }
          else
          {
            error.IsSuccessed = false;
            error.Msg.Add($"Product with SKU {item.PartnerSku} not found in stock.");
          }
        }
      }

      shipments.Add(obj);
      errors.Add(error);
    }

    return (shipments, errors);
  }
}

public class NoonOrderError
{
  public int Row { get; set; }
  public bool IsSuccessed { get; set; }
  public List<string> Msg { get; set; } = new List<string>();
}
