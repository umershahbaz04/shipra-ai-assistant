using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelOrderPostProcessor;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;
using ShopifySharp;
using ShopifySharp.Filters;
using Order = ShopifySharp.Order;

namespace Shipra.Backend.API.Infrastructure.Services.Shopify;

public class ShopifyOrderPostProcessorService : ISaleChannelOrderPostProcessorService
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IShopifyRepository _shopifyRepository;
  private readonly ISaleChannelOrderRepository _saleChannelOrderRepository;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IProductRepository _productRepository;
  private readonly ILogger<ShopifyOrderPostProcessorService> _logger;

  public ShopifyOrderPostProcessorService(IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IShopifyRepository shopifyRepository, ISaleChannelOrderRepository saleChannelOrderRepository, ISaleChannelConfigRepository saleChannelConfigRepository, ICountryRepository countryRepository, IOrderTrackingHistoryRepository orderTrackingHistory, IProductRepository productRepository, ILogger<ShopifyOrderPostProcessorService> logger)
  {
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _shopifyRepository = shopifyRepository;
    _saleChannelOrderRepository = saleChannelOrderRepository;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _countryRepository = countryRepository;
    _historyRepository = orderTrackingHistory;
    _productRepository = productRepository;
    _logger = logger;
  }

  public async Task<ServiceResultDTO> PostProcessOrdersAsync(
    SaleChannelOrderPostProcessorCommand request,
    ClientId clientId,
    EmployeeId? employeeId,
    CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    var successOrderList = new List<long>();
    BaseResponseDto baseResponse = new BaseResponseDto();
    try
    {
      List<Order>? shopifyOrderList = null;
      List<string>? saleChannelOrderIds = null;

      List<string> orderList = request.OrderIds!.Split(',').ToList();
      var oSaleChannelOrderList = await _saleChannelOrderRepository.GetSaleChannelOrderListByOrderIds(request.OrderIds, (int)EnumSaleChannelLookup.Shopify, clientId);

      if (oSaleChannelOrderList is not null && oSaleChannelOrderList.Count > 0)
      {
        saleChannelOrderIds = oSaleChannelOrderList.Select(x => x.OrderId!).ToList();
        orderList = orderList.Except(saleChannelOrderIds).ToList();
      }

      if (orderList is not null && orderList.Count > 0)
      {
        var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(request.SaleChannelConfigId!, clientId);
        if (oShopifyConfig is not null)
        {
          var oSaleChannelConfig = await _saleChannelConfigRepository.GetSaleChannelConfigById(oShopifyConfig.SaleChannelConfigId, clientId);
          if (oSaleChannelConfig is not null && oSaleChannelConfig.StoreId == request.StoreId && oSaleChannelConfig.SaleChannelConfigId == request.SaleChannelConfigId)
          {
            shopifyOrderList = await ListAllOrdersOnShop(oShopifyConfig.ShopDomain!, orderList!, oShopifyConfig.AccessToken!);

            if (shopifyOrderList is not null && shopifyOrderList.Count > 0)
            {
              #region salechannelorder
              foreach (var shopifyOrder in shopifyOrderList!)
              {
                OrderAddress oOrderAddress = new OrderAddress();
                var shopifyOrderAddress = shopifyOrder.ShippingAddress is not null ? shopifyOrder.ShippingAddress : shopifyOrder.BillingAddress;

                if (shopifyOrderAddress is not null)
                {
                  var oCounrtyCityRegion = await _countryRepository.GetCounrtyCityRegionIdByName(shopifyOrderAddress.Country, shopifyOrderAddress.Province, shopifyOrderAddress.City);
                  var completeAddress = shopifyOrderAddress.Address1 + "," + shopifyOrderAddress.Province + "," + shopifyOrderAddress.Country;
                }
                else
                {
                  var completeAddress = shopifyOrder.Customer.DefaultAddress.Address1 + "," + shopifyOrder.Customer.DefaultAddress.Province + "," + shopifyOrder.Customer.DefaultAddress.Country;

                  oOrderAddress = OrderAddress.CreateOrderAddress(shopifyOrder.Customer.DefaultAddress.Name, completeAddress, shopifyOrder.Email, shopifyOrder.Customer.DefaultAddress.Phone, shopifyOrder.Phone, null, null, null, shopifyOrder.Customer.DefaultAddress.Address1, shopifyOrder.Customer.DefaultAddress.Latitude, shopifyOrder.Customer.DefaultAddress.Longitude);
                }
                if (oOrderAddress is not null)
                {
                  var nextOrderNo = await _orderRepository.GetClientNextOrderNo(clientId, employeeId!);
                  await _orderRepository.CreateOrderAddress(oOrderAddress);

                  DateTimeOffset dateTimeOffset = (DateTimeOffset)shopifyOrder.CreatedAt!;
                  DateTime orderDate = dateTimeOffset.DateTime;
                  var oOrder = Core.OrderAggregate.Order.CreateOrderForShopify(clientId, request?.StoreId!, nextOrderNo, request?.SaleChannelConfigId, (int)EnumStationLookup.Dubai, orderDate, OrderCommon.GetDescriptionForShopifyOrderItems(shopifyOrder.LineItems.ToList()), shopifyOrder.Note, shopifyOrder.TotalPrice, shopifyOrder.TotalDiscounts, shopifyOrder.TotalTax, shopifyOrder.TotalWeight, shopifyOrder.FinancialStatus, oOrderAddress.OrderAddressId, shopifyOrder.TotalLineItemsPrice, employeeId!, request?.OrderTypeId, shopifyOrder.FulfillmentStatus);
                  await _orderRepository.CreateOrder(oOrder);

                  var oSaleChannelOrder = await _saleChannelOrderRepository.CreateSaleChannelOrder(SaleChannelOrder.CreateSaleChannelOrder(request?.SaleChannelLookupId!, request?.SaleChannelConfigId, shopifyOrder.Id.ToString(), shopifyOrder.OrderNumber!.ToString(), JsonConvert.SerializeObject(shopifyOrder, Formatting.Indented), orderDate, oOrder.OrderId!, clientId, employeeId!));
                  successOrderList.Add((long)shopifyOrder.Id!);

                  oOrder.UpdateOrderForSaleChannel(oSaleChannelOrder.SaleChannelOrderId, oSaleChannelOrder.SaleChannelLookupId!, employeeId!);
                  await _orderRepository.UpdateOrder(oOrder);

                  await _historyRepository.CreateOrderNote(OrderNote.CreateOrderNote(oOrder.OrderId!, shopifyOrder.Note, employeeId!));
                  string? createdByName = await _employeeRepository.GetEmployeeNameById(employeeId);

                  var oOrderTrackingHistory = OrderTrackingHistory.CreateOrderTrackingHistory(oOrder.OrderId!, (int)EnumCarrierTrackingStatus.OrderPlaced, "Shopify Order Placed ", employeeId!, createdByName);
                  await _historyRepository.CreateOrderTrackingHistory(oOrderTrackingHistory!);
                }
              }

              baseResponse = new BaseResponseDto
              {
                Data = successOrderList,
                Message = "Shopify order created successfully for the selected orderIds."
              };
              #endregion
            }
            else
            {
              throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Shopify Order not found");
            }
          }
          else
          {
            throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Sale Channel Config not found");
          }
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Shopify Config not found");
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
      _logger.LogError(ex, "Error post-processing Shopify orders");
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
        Shipra.Backend.API.Core.ProductAggregate.ProductVariant? productVariant = null;
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

  private async Task<List<Order>> ListAllOrdersOnShop(string shopDomain, List<string> orderIds, string accesstoken)
  {
    var filterOrderIds = orderIds.Select(s => long.Parse(s!)).ToList();
    var executionPolicy = new LeakyBucketExecutionPolicy();
    var service = new OrderService(shopDomain, accesstoken);
    var allOrders = new List<Order>();
    var page = await service.ListAsync(new OrderListFilter
    {
      Limit = 250,
      Ids = filterOrderIds
    });

    while (true)
    {
      allOrders.AddRange(page.Items);
      if (!page.HasNextPage) break;
      page = await service.ListAsync(page.GetNextPageFilter());
    }
    return allOrders;
  }
}
