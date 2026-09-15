using System.Dynamic;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderForShopify;
public class CreateOrderForShopifyCommandHandler : RequestHandlerBase<CreateOrderForShopifyCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IProductRepository _productRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IOrderRepository _orderRepository;
 

  public CreateOrderForShopifyCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository, IOrderTrackingHistoryRepository orderTrackingHistoryRepository, ICountryRepository countryRepository, IProductRepository productRepository, IClientRepository clientRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CreateOrderForShopifyCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _countryRepository = countryRepository;
    _productRepository = productRepository;
    _clientRepository = clientRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateOrderForShopifyCommand orderRequest, CancellationToken cancellationToken)
  {
    ServiceResultDTO response = new ServiceResultDTO();
    try
    {
      /// <summary>
      /// 1. create order address
      /// 2. create order
      /// 3. create order history
      /// 4. create order item
      /// 5. create ordernote
      /// 6. update product stock quantity 
      /// </summary>
      List<dynamic> orderData = new List<dynamic>();
      foreach (var request in orderRequest.OrderList!)
      {
        #region order address
        int countryId = 0;
        int cityId = 0;
        int areaId = 0;

        var countryCityData = await _countryRepository.GetCounrtyCityRegionIdByName(request.OrderAddress!.Country!, request.OrderAddress!.Region!, request.OrderAddress!.City!);
        if (countryCityData != null)
        {
          countryId = countryCityData?.CountryId;
          cityId = countryCityData?.CityId;
          areaId = countryCityData?.AreaId;
        }
        string fullAddress = await _countryRepository.GetFullAddress(request.OrderAddress!.StreetAddress, countryId, cityId, areaId,0,0);
        var orderAddress = OrderAddress.CreateOrderAddress(request.OrderAddress!.CustomerName, fullAddress, request.OrderAddress!.Email, request.OrderAddress!.Mobile1, request.OrderAddress!.Mobile2, countryId, areaId, cityId, request.OrderAddress!.StreetAddress, request.OrderAddress!.Latitude, request.OrderAddress!.Longitude);

        OrderAddress createdOrderAddress = await _orderRepository.CreateOrderAddress(orderAddress);
        #endregion

        #region create order
        var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
        if (client is null)
        {
          throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
        }

        var orderNo = await _orderRepository.GetClientNextOrderNo(client.ClientId!,_currentUser.EmployeeId!);

        //int randomNom = GenerateRandom(1, 999);
        //var orderNo = $"{randomNom}{orderCount}";
        if (request.StoreId == 0)
        {
          request.StoreId = client!.DefaultStoreId;
        }
        if (request.StationId == 0)
        {
          request.StationId = client!.DefaultProductStationId.GetValueOrDefault();
        }
        #region get status name
        string? carrierTrackingStatusName = string.Empty;
        var carrierTrackingStatuses = await _orderRepository.GetAllCarrierTrackingStatusesByClientId(_currentUser.ClientId!);
        var objClientTrackingStatus = carrierTrackingStatuses.FirstOrDefault(x => x.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.OrderPlaced);
        if (objClientTrackingStatus != null)
        {
          carrierTrackingStatusName = objClientTrackingStatus.TrackingStatus;
        }
        #endregion
        request.PaymentMethodId = (int)EnumPaymentMethod.COD;
        request.ItemValue = request.Amount;
        request.PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
        var order = Order.CreateOrder(_currentUser.ClientId!, request.StoreId, request.ChannelId, request.OrderTypeId, orderNo, request.OrderDate, orderAddress.OrderAddressId, request.Amount, request.Description, request.Remarks, request!.OrderItems?.Count(),request.DeliveryCharges, request.PaymentStatusId, request.Weight, request.ItemValue, request.OrderRequestVia, request.Discount, request.VAT, request.PaymentMethodId, request.StationId, (int)EnumCarrierTrackingStatus.OrderPlaced,null, _currentUser.EmployeeId!,carrierTrackingStatusName);

        var createdOrder = await _orderRepository.CreateOrder(order);
        dynamic resData = new ExpandoObject();
        resData.OrderId = createdOrder.OrderId!.Value.ToString();
        resData.OrderNo = createdOrder.OrderNo;
        orderData.Add(resData);

        #endregion

        #region order history
        string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

        OrderTrackingHistory orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(createdOrder.OrderId!, (int)EnumOrderOrderTrackingHistory.ORDERPLACED, request.OrderNote?.Note, _currentUser.EmployeeId!, createdByName);
        OrderTrackingHistory createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);

        #endregion

        #region orderitems
        foreach (var item in request!.OrderItems!)
        {
          await CreateOrderItemCommon(item, createdOrder, request.OrderTypeId);
        }

        #endregion

        #region order note
        var createdOrderNote = await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, request?.OrderNote?.Note, _currentUser.EmployeeId!));
        #endregion
      }

      response = new ServiceResultDTO(new BaseResponseDto { Data = orderData, Message = NotificationConstants.Success });
      #region publish notification 
      await _mediator.Publish(new
              RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(orderRequest),
        Response = JsonConvert.SerializeObject(response),
        EventName = "oncreateOrder",
        ClientId = _currentUser.ClientIdStr,
        EmployeeId = _currentUser.EmployeeIdStr,
        CreateOn = DateTime.UtcNow
      });
      #endregion
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
  private async Task CreateOrderItemCommon(OrderItemShopifyModel item, Order createdOrder, int? orderTypeId)
  {
    OrderItem? orderItem = null;
    if (orderTypeId == (int)EnumOrderType.FullFilable)
    {
      var product = await _productRepository.GetProductByIdAsync(new ProductId(new Guid(item.ProductId!)), _currentUser.ClientId!);
      if (product is null)
      {
        throw new EntityNotFoundException("Product ", _currentUser.ClientIdStr!.ToString());
      }
      if (product!.TrackInventory.GetValueOrDefault())
      {
        //with product inventory
        #region update inventorybalance
        InventoryBalance? inventoryBalance = null;
        ProductVariant? productVariant = null;
        long? inventoryBalanceId = item.InventoryBalanceId;
        long? productVariantId = item.ProductVariantId;

        if (inventoryBalanceId.GetValueOrDefault() > 0)
        {
          inventoryBalance = await _productRepository.GetInventoryBalanceByIdAsync(inventoryBalanceId.GetValueOrDefault());
          if (inventoryBalance is null)
          {
            throw new EntityNotFoundException("InventoryBalance ", inventoryBalanceId.GetValueOrDefault());
          }
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
        #endregion

        if (productVariant != null)
        {
          item.Description = OrderCommon.GetDescriptionForShipmentItem(product, productVariant);
        }
        else
        {
          item.Description = "";
        }

        orderItem = OrderItem.CreateOrderItemFullFilable(createdOrder.OrderId!, new ProductId(new Guid(item.ProductId!)), item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, null, null, null, null, productVariantId);
        OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
      }
      else
      {
        orderItem = OrderItem.CreateOrderItemFullFilable(createdOrder.OrderId!, new ProductId(new Guid(item.ProductId!)), item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, null, null, null, null, null);
        OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
      }
    }
    else if (orderTypeId == (int)EnumOrderType.Regular)
    {
      orderItem = OrderItem.CreateOrderItemRegular(createdOrder.OrderId!, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount);
      OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
    }
  }
}
