using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelOrderPostProcessor;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;
using Formatting = Newtonsoft.Json.Formatting;

namespace Shipra.Backend.API.Infrastructure.Services.Noon;

public class NoonOrderPostProcessorService : ISaleChannelOrderPostProcessorService
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly ISaleChannelOrderRepository _saleChannelOrderRepository;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IProductRepository _productRepository;
  private readonly ILogger<NoonOrderPostProcessorService> _logger;

  public NoonOrderPostProcessorService(IEmployeeRepository employeeRepository, IOrderRepository orderRepository, ISaleChannelOrderRepository saleChannelOrderRepository, ISaleChannelConfigRepository saleChannelConfigRepository, IOrderTrackingHistoryRepository historyRepository, IProductRepository productRepository, ILogger<NoonOrderPostProcessorService> logger)
  {
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _saleChannelOrderRepository = saleChannelOrderRepository;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _historyRepository = historyRepository;
    _productRepository = productRepository;
    _logger = logger;
  }

  public async Task<ServiceResultDTO> PostProcessOrdersAsync(SaleChannelOrderPostProcessorCommand request,ClientId clientId,EmployeeId? employeeId,CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    var successOrderList = new List<string>();
    BaseResponseDto baseResponse = new BaseResponseDto();
    
    try
    {
      List<string>? saleChannelOrderIds = null;
      List<string> orderList = request.OrderIds!.Split(',').ToList();
      var oSaleChannelOrderList = await _saleChannelOrderRepository.GetSaleChannelOrderListByOrderIds(request.OrderIds, request.SaleChannelLookupId ?? (int)EnumSaleChannelLookup.Noon, clientId);

      if (oSaleChannelOrderList is not null && oSaleChannelOrderList.Count > 0)
      {
        saleChannelOrderIds = oSaleChannelOrderList.Select(x => x.OrderId!).ToList();
        orderList = orderList.Except(saleChannelOrderIds).ToList();
      }

      if (orderList is not null && orderList.Count > 0)
      {
        var oSaleChannelConfig = await _saleChannelConfigRepository.GetSaleChannelConfigById(request.SaleChannelConfigId ?? (int)EnumSaleChannelLookup.Noon, clientId);
        if (oSaleChannelConfig is not null && oSaleChannelConfig.StoreId == request.StoreId && oSaleChannelConfig.SaleChannelConfigId == request.SaleChannelConfigId)
        {
          var saleChannelConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(oSaleChannelConfig.Config!);
          var saleChannelSetting = Utils.ConvertKeysToCamelCase(saleChannelConfig!);

          var privateKey = Utils.GetValueFromDictionryByKey("privateKey", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("private_key", saleChannelSetting);
          var keyId = Utils.GetValueFromDictionryByKey("keyId", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("key_id", saleChannelSetting);
          var channelIdentifier = Utils.GetValueFromDictionryByKey("channelIdentifier", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("channel_identifier", saleChannelSetting);
          var projectCode = Utils.GetValueFromDictionryByKey("projectCode", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("project_code", saleChannelSetting);
          var baseUrl = Utils.GetValueFromDictionryByKey("baseUrl", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("base_url", saleChannelSetting) ?? "https://noon-api-gateway.noon.partners";

          if (string.IsNullOrEmpty(privateKey))
          {
              privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCQgWlha5HZePxU\nyMnDLvFr+OByZmeGIOuwp6V2kKdIJcMnoyvrBB/dL2Sl5rpRlXDl02/gFj6lTO1V\nhnSkSZlNfuaMCIR+7WDkEgkOXtNTe2Dh1rsYK8oRKyFkt5d9mTbAy02ijveiS6Mq\nRdYbDnxz9rzLCAYBfRpumbiOpAyn+mmm2RydFRmAcM1t2jvPhK8ng3hM4UeG9goI\n39UTYkPPCbYY06lNlul5ujqFQFzy14Fi7L0Kukk7bGkKod8vHZiSrhJ6zWHrf4mz\nYIsZUCTm7YLnKeKXeg2WvmUH11GOcX0lfsytT1I3j1YCePP2/MUoLWi80GJ/fHCf\nnBiiW4JvAgMBAAECggEAFzoV3CjUKqZ9uIsFky/qcjZwrTK0lSSZfa2UtPgPS1N2\niNp7Zq0lCgJiJSBu9koU+XwA0X4B18QDqemQug9yarhpCj0cPuKc3kvf1MV9JkAA\nlIxVSk9PjW7nUS8JVJDZ8ic7dVORji6mLVdIUNUFQAZ61g+WF4sqQnjG53aK6jzi\nBUjmQ8bLFZLIYqn+jiiR+K4hHhSZnWhaTJkujv+01NLut32pY8XpX8AdWKB4oiYH\nUBaWtI0qW7x9h1XmhptzOByyJ/Y8x1xBZuuoi6r5KW9q8qyLYaI5cAj1Q0uc8IW1\n9yYAVoANk1MSR3JLbwIef8aCNeYyf34r0FUTZ2KHEQKBgQDB6VkG1piVijOIBgmb\ntwUBJFwydFF1rc7FwnR5ANQqTeZYJxOmLNfDAiM1AdfaeO4rkMTwInQrLwBSh/4V\nihR1Cl5ZPaSjI2ATK3Wq277g5N5pXJ6D6nzgji9n9O2vsZY9tyh4c8uP6LTRz/BC\nwAHB18PWs6N7tBf/ovAiPqBq+wKBgQC+xlLxQcU0+Hp3F8EK09eBRAm56I8+bjln\nP0mAt0T0Nj6MTYGVJk34l7rdcfayJGZoYo/X6a/C8RX6hLHvWXTNUZW1I+AQQK+C\nHmt/GLA04dHyK1cqz3WLE63rCRK++TnFjwARhu6jdZNed7ubv55ie/x0Ile2ZFWi\nQn6rYbDsHQKBgDKj2O8TPdfXtqtwQDQdML5im31FqTxdPqGgrcAn+kBuBZjB47zC\n+znfJgiiyZcxe6l+7h90L/hTFvd2smE3pS4HnioaEhPUmjOHZvxO1ONwgbDsUi1L\nIH+YQkMY0LXQX9cQLQ5/1wpnEEm2zxzvfcX8rhU05p3Yo2fMSn/28PffAoGAIfZG\nf8KYq/RsQNVOvXG3FMEbBiibj56pw3Kl0C9QLDWX7vxBTF8UVGQWlSObql0GiiC5\nwNNOQeMPaZjD4HtJat/SSfwIAHyzgfOOaYLoo5FsAbOrgeiK4WZweL4Vwz+1BDGP\n7o7Z3umogZHJKVH0jU3LRJV0jfjQseEqkbIDgBUCgYEAvhfgyWxbuxUJcqm8LYlr\n+NXUhNHrfR3DpNk2i+EBhfjD+nM5jYt6Xdo/LLPuqSwbe7e7VWl41zKFCzdFdLdB\n+lns7Bfylz0ORV8UO16Juw9cORfzdlKPOwnct2/V8y2BMlJDbsBFiDcl6TvcrIcK\nEU/qwlaSVcsAL6+mibc0Xvk=\n-----END PRIVATE KEY-----";
          keyId = "noon-partners-key-id-ced2e157b4f244578939d31ccd35044b";
          channelIdentifier = "integrationnoon@p581228.idp.noon.partners";
          projectCode = "PRJ581228";
          baseUrl = "https://noon-api-gateway.noon.partners";
          }

          var jwtToken = NoonAuthHelper.GenerateJwtToken(privateKey, keyId, channelIdentifier);
          var accessToken = await ExchangeJwtForAccessToken(baseUrl, jwtToken, projectCode);

          if (string.IsNullOrEmpty(accessToken))
             throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Failed to retrieve Noon Access Token.");

          var noonOrderList = new List<NoonFbpiOrderResponse>();
          foreach(var orderNr in orderList)
          {
              var orderDetail = await GetNoonFbpiOrderDetailsAsync(baseUrl, accessToken, orderNr);
              if (orderDetail != null)
              {
                  noonOrderList.Add(orderDetail);
              }
          }

          if (noonOrderList.Count > 0)
          {
            foreach (var noonOrder in noonOrderList)
            {
              // Create Address
              var completeAddress = "Noon Address, " + noonOrder.CountryCode;
              var oOrderAddress = OrderAddress.CreateOrderAddress("Noon Customer", completeAddress, string.Empty, string.Empty, string.Empty, null, null, null, "Noon Address", null, null);
              
              var nextOrderNo = await _orderRepository.GetClientNextOrderNo(clientId, employeeId!);
              await _orderRepository.CreateOrderAddress(oOrderAddress);

              var orderDate = noonOrder.CreatedAt ?? DateTime.UtcNow;
              var totalAmount = noonOrder.Items?.Sum(i => i.Price) ?? 0;
              
              // Note: using enum logic similar to shopify
              var oOrder = Core.OrderAggregate.Order.CreateOrderForShopify(clientId, request?.StoreId!, nextOrderNo, request?.SaleChannelConfigId, (int)EnumStationLookup.Dubai, orderDate, $"Noon Order {noonOrder.FbpiOrderNr}", "", totalAmount, 0, 0, 0, "paid", oOrderAddress.OrderAddressId, totalAmount, employeeId!, request?.OrderTypeId, "unfulfilled");
              await _orderRepository.CreateOrder(oOrder);

              // Process Items
              if (noonOrder.Items != null)
              {
                  foreach(var item in noonOrder.Items)
                  {
                      var orderItemModel = new OrderItemModel
                      {
                          ProductId = "", // Need to find by sku if fullfillable
                          Price = item.Price,
                          Description = $"SKU: {item.PartnerSku} - ItemNr: {item.MpItemNr}",
                          Quantity = 1,
                          Discount = 0
                      };
                      
                      if (request?.OrderTypeId == (int)EnumOrderType.FullFilable)
                      {
                           var pList = await _productRepository.GetAllProductStocksForSaleChannelInventorySync(clientId.Value.ToString(), item.PartnerSku!);
                           var castedProductList = (IEnumerable<dynamic>)pList!;
                           var p = castedProductList.FirstOrDefault(p => (string)p.SKU == item.PartnerSku);
                            if (p != null)
                            {
                                orderItemModel.ProductId = p.ProductId;
                                orderItemModel.ProductVariantId = p.ProductVariantId;
                            }
                      }

                      if (request?.OrderTypeId == (int)EnumOrderType.Regular || (request?.OrderTypeId == (int)EnumOrderType.FullFilable && !string.IsNullOrEmpty(orderItemModel.ProductId)))
                      {
                           await CreateOrderItemCommon(orderItemModel, oOrder.OrderId, request.OrderTypeId, clientId, employeeId);
                      }
                  }
              }

              var oSaleChannelOrder = await _saleChannelOrderRepository.CreateSaleChannelOrder(SaleChannelOrder.CreateSaleChannelOrder(request?.SaleChannelLookupId!, request?.SaleChannelConfigId, noonOrder.FbpiOrderNr!, noonOrder.FbpiOrderNr!, JsonConvert.SerializeObject(noonOrder, Formatting.Indented), orderDate, oOrder.OrderId!, clientId, employeeId!));
              successOrderList.Add(noonOrder.FbpiOrderNr!);

              oOrder.UpdateOrderForSaleChannel(oSaleChannelOrder.SaleChannelOrderId, oSaleChannelOrder.SaleChannelLookupId!, employeeId!);
              await _orderRepository.UpdateOrder(oOrder);

              await _historyRepository.CreateOrderNote(OrderNote.CreateOrderNote(oOrder.OrderId!, "", employeeId!));
              string? createdByName = await _employeeRepository.GetEmployeeNameById(employeeId);

              var oOrderTrackingHistory = OrderTrackingHistory.CreateOrderTrackingHistory(oOrder.OrderId!, (int)EnumCarrierTrackingStatus.OrderPlaced, "Noon Order Placed ", employeeId!, createdByName);
              await _historyRepository.CreateOrderTrackingHistory(oOrderTrackingHistory!);
            }

            baseResponse = new BaseResponseDto
            {
              Data = successOrderList,
              Message = "Noon order created successfully for the selected orderIds."
            };
          }
          else
          {
            throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Noon Order not found");
          }
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Sale Channel Config not found");
        }
      }
      else
      {
        baseResponse = new BaseResponseDto
        {
          Data = saleChannelOrderIds,
          Message = "Selected order(s) are already created."
        };
      }

      serviceResult = new ServiceResultDTO(baseResponse);
      return serviceResult;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error post-processing Noon orders");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task CreateOrderItemCommon(OrderItemModel item, OrderId? orderId, int? orderTypeId, ClientId clientId, EmployeeId? employeeId)
  {
    OrderItem? orderItem;
    if (orderTypeId == (int)EnumOrderType.FullFilable)
    {
      var product = await _productRepository.GetProductByIdAsync(new ProductId(new Guid(item.ProductId!)), clientId);
      if (product is null) throw new EntityNotFoundException("Product ", item.ProductId!);
      if (product!.TrackInventory.GetValueOrDefault())
      {
        InventoryBalance? inventoryBalance = null;
        ProductVariant? productVariant = null;
        long? inventoryBalanceId = null;
        long? productVariantId = item.ProductVariantId;

        var order = await _orderRepository.GetOrderById(orderId!, clientId);
        if (productVariantId.GetValueOrDefault() > 0 && order != null && order.StationId.HasValue && order.StationId.Value > 0)
        {
          var resolvedBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(productVariantId.GetValueOrDefault(), order.StationId.Value);
          if (resolvedBalance != null)
          {
            inventoryBalanceId = resolvedBalance.InventoryBalanceId;
          }
        }

        if (inventoryBalanceId.GetValueOrDefault() > 0)
        {
          inventoryBalance = await _productRepository.GetInventoryBalanceByIdAsync(inventoryBalanceId.GetValueOrDefault());
          if (inventoryBalance is null) throw new EntityNotFoundException("InventoryBalance ", inventoryBalanceId.GetValueOrDefault());
          var quantityCommited = inventoryBalance.QuantityCommitted + item.Quantity.GetValueOrDefault();
          var quantityAvailable = inventoryBalance.QuantityAvailable - item.Quantity.GetValueOrDefault();
          inventoryBalance.UpdateQuantities(inventoryBalance.QuantityOnHand, quantityAvailable, quantityCommited);
          await _productRepository.UpdateInventoryBalanceAsync(inventoryBalance);
          productVariantId = inventoryBalance.ProductVariantId;
        }

        if (productVariantId.GetValueOrDefault() > 0)
        {
          productVariant = await _productRepository.GetProductVariantByIdAsync(productVariantId.GetValueOrDefault());
        }

        if (productVariant != null)
        {
          item.Description = OrderCommon.GetDescriptionForShipmentItem(product, productVariant);
        }
        else
        {
          item.Description = "";
        }

        orderItem = OrderItem.CreateOrderItemFullFilable(orderId!, new ProductId(new Guid(item.ProductId!)), item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, null, null, null, null, productVariantId);
        _ = await _orderRepository.CreateOrderItem(orderItem!);
      }
      else
      {
        orderItem = OrderItem.CreateOrderItemFullFilable(orderId!, new ProductId(new Guid(item.ProductId!)), item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, null, null, null, null, null);
        _ = await _orderRepository.CreateOrderItem(orderItem!);
      }
    }
    else if (orderTypeId == (int)EnumOrderType.Regular)
    {
      orderItem = OrderItem.CreateOrderItemRegular(orderId!, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount);
      _ = await _orderRepository.CreateOrderItem(orderItem!);
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

  private async Task<NoonFbpiOrderResponse?> GetNoonFbpiOrderDetailsAsync(string baseUrl, string accessToken, string orderNr)
  {
      using (var client = new HttpClient())
      {
          var normalizedBaseUrl = baseUrl.TrimEnd('/');
          var requestUri = $"{normalizedBaseUrl}/fbpi/v1/fbpi-order/{orderNr}/get";
          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
          var response = await client.GetAsync(requestUri);
          if (response.IsSuccessStatusCode)
          {
              var responseContent = await response.Content.ReadAsStringAsync();
              return JsonConvert.DeserializeObject<NoonFbpiOrderResponse>(responseContent);
          }
          return null;
      }
  }
}
