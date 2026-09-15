using System.Dynamic;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderDraft;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrderItems;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrder;
public class UpdateOrderCommandHandler : RequestHandlerBase<UpdateOrderCommand, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;
  private readonly IMediator _mediator;
  private readonly IOrderBoxRepository _orderBoxRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IMetaFieldRepository _metaFieldRepository;

  public UpdateOrderCommandHandler(IShipperInvoiceRepository shipperInvoiceRepository, IMediator mediator, IOrderBoxRepository orderBoxRepository, IEmployeeRepository employeeRepository, ICountryRepository countryRepository, IClientRepository clientRepository, IProductRepository productRepository, IOrderRepository orderRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, IMetaFieldRepository metaFieldRepository, ILogger<UpdateOrderCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
    _mediator = mediator;
    _orderBoxRepository = orderBoxRepository;
    _employeeRepository = employeeRepository;
    _countryRepository = countryRepository;
    _clientRepository = clientRepository;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
    _historyRepository = historyRepository;
    _metaFieldRepository = metaFieldRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateOrderCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    {
      /// <summary>
      /// 1. update order
      /// 2. update order note
      /// 3. update order address
      /// 4. update order item
      /// 5. update order history
      /// 6. update product stock quantity 
      /// </summary>
       #region create draft order
      if (request.OrderDraftId.GetValueOrDefault() > 0)
      {
        var fisrtOrder = request;
        response = await _mediator.Send(
          new CreateOrderDraftCommand
          {
            OrderDraftId = request.OrderDraftId,
            OrderInfo = JsonConvert.SerializeObject(fisrtOrder),
            OrderTypeId = request.OrderTypeId
          }
        );

        return response;
      }
      #endregion
      var orderId = new OrderId(new Guid(request!.OrderId!));
      var order = await _orderRepository.GetOrderById(orderId, _currentUser.ClientId!);
      if (order == null)
      {
        throw new EntityNotFoundException("Order ", orderId.Value);
      }
      if (!order!.TrackingLock.GetValueOrDefault(false))
      {
        if (order.OrderTypeId.GetValueOrDefault() == (int)EnumOrderType.FullFilable)
        {
          if (order.FullFillmentStatusId == (int)EnumFullfillmentStatus.Fulfilled)
          {
            response.CreateError("FulfilledStats", new string[] { "you cannot update an order with Fulfilled status. Please change the status to Unfulfilled from the order dashboard before updating the order." });
            return response;
          }
        }
        var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
        if (client is null)
        {
          throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
        }
        var orderAddress = await _orderRepository.GetOrderAddressById(request.OrderAddress!.OrderAddressId);
        if (orderAddress == null)
        {
          throw new EntityNotFoundException("OrderAddress ", request.OrderAddress!.OrderAddressId);
        }

        #region remove order items and then add if order type regular (must be delete before update other things)
        List<OrderItem> orderItems = await _orderRepository.GetOrderItemsByOrderId(orderId);
        foreach (var oItem in orderItems)
        {
          if (oItem.OrderId is not null && oItem.OrderItemId is not null)
          {
            var req = new DeleteOrderItemByIdCommand()
            {
              OrderId = oItem.OrderId!.Value.ToString(),
              OrderItemId = oItem.OrderItemId!.Value.ToString()
            };
            var serviceResultDTO = await _mediator.Send(req);
          }
        }
        #endregion


        if (request.StoreId == 0)
        {
          request.StoreId = client!.DefaultStoreId;
        }
        if (request.StationId == 0)
        {
          request.StationId = client!.DefaultProductStationId.GetValueOrDefault();
        }

        #region amount
        //if (request.PaymentMethodId == (int)EnumPaymentMethod.PP)
        //{
        //  request.PaymentStatusId = (int)EnumPaymentStatus.Paid;
        //}
        //else
        //{
        //  request.PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
        //}
        request.ItemValue = request.Amount;
        #endregion

        #region order
        decimal? totalTax = request.OrderTaxes!.Sum(x => x.TaxValue);


        order?.UpdateOrder(request.StoreId, request.OrderTypeId, request.OrderDate, request.OrderAddress?.OrderAddressId!, request.Amount, request.Description, request.Remarks, request!.OrderItems?.Count(), request.CShippingCharges, request.PaymentStatusId, request.Weight, request.ItemValue, request.OrderRequestVia, request.Discount, totalTax.GetValueOrDefault(), request.PaymentMethodId, request.StationId, request.RefNo, _currentUser.EmployeeId,request.SaleChannelConfigId,request.SaleChannelLookupId);
        response.IsSuccess = await _orderRepository.UpdateOrder(order!);

        #region order tax
        foreach (var item in request.OrderTaxes!)
        {
          if (!string.IsNullOrEmpty(item.OrderTaxId))
          {
            OrderTaxId orderTaxId = new OrderTaxId(new Guid(item.OrderTaxId!));
            var target = await _orderRepository.GetOrderTaxById(orderTaxId!);
            if (target is not null)
            {
              target!.Update(item.TaxValue);
              var updated = await _orderRepository.UpdateOrderTax(target!);
            }
          }
          else
          {
            OrderTax orderTax = OrderTax.Create(item.ClientTaxId, item.TaxValue, order!.OrderId!);
            bool added = await _orderRepository.CreateOrderTax(orderTax);
          }
        }
        #endregion
        #endregion

        #region order note
        var orderNote = await _historyRepository.GetOrderNoteByOrderId(order!.OrderId!);
        if (orderNote != null)
        {
          orderNote!.UpdateOrderNote(request?.OrderNote?.Note, _currentUser.EmployeeId!);
          await _historyRepository.UpdateOrderNote(orderNote);
        }
        #endregion

        #region order address 
        var objOrderAddress = request!.OrderAddress!;
        #region update street address 2
        string? modifyStreet = objOrderAddress.StreetAddress;

        if (!string.IsNullOrEmpty(objOrderAddress.StreetAddress2))
        {
          modifyStreet = objOrderAddress.StreetAddress + "," + objOrderAddress.StreetAddress2;
        }
        #endregion
        string fullAddress = await _countryRepository.GetFullAddress(modifyStreet, objOrderAddress.CountryId, objOrderAddress.CityId, objOrderAddress.AreaId, objOrderAddress.ProvinceId, objOrderAddress.PinCodeId, objOrderAddress.StateId,objOrderAddress.EntityAddressDataJson);

        orderAddress.UpdateOrderAddress(request.OrderAddress!.CustomerName, fullAddress, request.OrderAddress!.Email, request.OrderAddress!.Mobile1, request.OrderAddress!.Mobile2, request.OrderAddress!.CountryId, request.OrderAddress!.CityId, request.OrderAddress!.AreaId, request.OrderAddress!.StreetAddress, request.OrderAddress!.Latitude, request.OrderAddress!.Longitude, objOrderAddress!.StreetAddress2, objOrderAddress!.HouseNo, objOrderAddress!.BuildingName, objOrderAddress!.Landmark, objOrderAddress!.ProvinceId, objOrderAddress!.PinCodeId, objOrderAddress.StateId, objOrderAddress.SelectedCarrierId, objOrderAddress.EntityAddressDataJson);
        orderAddress = await _orderRepository.UpdateOrderAddress(orderAddress!);
        #endregion

        #region order Item

        foreach (var item in request!.OrderItems!)
        {
          // becuase we have deleted all the order items so we have to create new one
          await CreateOrderItemCommon(item, order!, request.OrderTypeId);


          //Guid guidID;
          //var hasGUID = Guid.TryParse(item.OrderItemId!, out guidID);
          //if (!string.IsNullOrEmpty(item.OrderItemId) && hasGUID) //if we have valida orderitem id then just update 
          //{

          //  if (request.OrderTypeId == (int)EnumOrderType.FullFilable)
          //  {
          //    var product = await _productRepository.GetProductByIdAsync(new ProductId(new Guid(item.ProductId!)), _currentUser.ClientId!);
          //    if (product!.TrackInventory.GetValueOrDefault())
          //    {
          //      #region update productstock
          //      var productStock = await _productRepository.GetProductStockByIdAsync(item.ProductStockId);
          //      var quantityCommited = productStock.QuantityCommited + item.Quantity;
          //      productStock.UpdateProductStockQuantityCommited(quantityCommited, _currentUser.EmployeeId!);
          //      ProductStock updatedProductStock = await _productRepository.UpdateProductStockAsync(productStock);
          //      #endregion

          //      item.Description = GetDescriptionForShipmentItem(product, productStock);

          //      var orderItem = orderItems?.FirstOrDefault(x => x.OrderItemId == new OrderItemId(new Guid(item.OrderItemId!)));
          //      orderItem?.UpdateOrderItem(new ProductId(new Guid(item.ProductId!)), item.ProductStockId, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount);
          //      var updatedItem = await _orderRepository.UpdateOrderItem(orderItem);
          //    }
          //    else
          //    {
          //      ///without  inventory stock id will be 0
          //      item.ProductStockId = null;

          //      var orderItem = orderItems?.FirstOrDefault(x => x.OrderItemId == new OrderItemId(new Guid(item.OrderItemId!)));
          //      orderItem?.UpdateOrderItem(new ProductId(new Guid(item.ProductId!)), item.ProductStockId, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount);
          //      var updatedItem = await _orderRepository.UpdateOrderItem(orderItem);
          //    }

          //  }
          //  else if (request.OrderTypeId == (int)EnumOrderType.Regular)
          //  {
          //    var orderItem = orderItems?.FirstOrDefault(x => x.OrderItemId == new OrderItemId(new Guid(item.OrderItemId!)));

          //    if (orderItem != null)
          //    {
          //      orderItem.UpdateOrderItemRegular(item.Price, item.Description, item.Remarks, item.Quantity, item.Discount);
          //      OrderItem upitem = await _orderRepository.UpdateOrderItem(orderItem!);
          //    }
          //  }

          //}
          //else
          //{
          //  await CreateOrderItemCommon(item, order!, request.OrderTypeId);
          //}

        }
        #endregion

        #region order box
        List<OrderBox> orderBoxes = await _orderBoxRepository.GetOrderBoxsByOrderId(orderId);
        #region find order box not in list and remove
        var orderBoxesIds = request!.OrderBoxs!
                            .Where(x => !string.IsNullOrEmpty(x.OrderBoxId)) // Exclude null or empty IDs
                            .Select(x => new OrderBoxId(new Guid(x.OrderBoxId!)))
                            .ToList();
        // Find boxes for delete where OrderBoxId does not match the given orderBoxesIds
        var boxesForDelete = orderBoxes
            .Where(x => !orderBoxesIds.Contains(x.OrderBoxId!))
            .ToList();

        foreach (var box in boxesForDelete)
        {
          await _orderBoxRepository.DeleteOrderBox(box);
        }
        #endregion
        foreach (var item in request!.OrderBoxs!)
        {
          Guid guidID;
          var hasGUID = Guid.TryParse(item.OrderBoxId!, out guidID);
          if (!string.IsNullOrEmpty(item.OrderBoxId) && hasGUID) //if we have valida orderitem id then just update 
          {
            var oBox = orderBoxes.FirstOrDefault(x => x.OrderBoxId == new OrderBoxId(new Guid(item.OrderBoxId)));
            if (oBox is not null)
            {
              //oBox.Update(item.ClientOrderBoxId);
              oBox.Update(item.Length, item.Width, item.Height);
              await _orderBoxRepository.UpdateOrderBox(oBox);
            }
            else
            {
              //OrderBox orderBox = OrderBox.Create(item.ClientOrderBoxId, order!.OrderId!);
              OrderBox orderBox = OrderBox.Create(item.Length, item.Width, item.Height, order!.OrderId!);
              await _orderBoxRepository.CreateOrderBox(orderBox);
            }
          }
          else
          {
            OrderBox orderBox = OrderBox.Create(item.Length, item.Width, item.Height, order!.OrderId!);
            await _orderBoxRepository.CreateOrderBox(orderBox);
          }
        }

        #endregion
        #region order history
        if (!order!.TrackingLock.GetValueOrDefault(false))
        {
          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

          OrderTrackingHistory orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(order!.OrderId!, order.CarrierTrackingStatusId, request.OrderNote?.Note, _currentUser.EmployeeId!, createdByName);
          OrderTrackingHistory createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);
        }
        #endregion

        #region UpdateClientMeta
        //var settingConfigJson = JsonConvert.SerializeObject(request.settingConfig);
        var existingmetas = await _metaFieldRepository.GetMetaFieldDataByOrderId(order.OrderId!.Value.ToString());
        if (request.settingConfig != null && request.settingConfig.Any())
        {
          var filteredConfig = request.settingConfig.Where(c => c.value != null && !string.IsNullOrWhiteSpace(c.value.ToString())).ToList();
          if (filteredConfig.Any() && existingmetas != null)
          {
            var filteredConfigJson = JsonConvert.SerializeObject(filteredConfig);
            existingmetas.UpdateMetaField(filteredConfigJson);
            var updatedMetas = await _metaFieldRepository.UpdateMetaFieldData(existingmetas);
          }
        }
        #endregion
        #region add order 
        bool isAllowedShipperInvocie = (await _orderRepository.GetClientConfigSetting(_currentUser.ClientId!))
                  ?.AllowShipperInvocie ?? false;
        if (isAllowedShipperInvocie)
        {
          try
          { 
            if (order is not null && order.SaleChannelConfigId.GetValueOrDefault() > 0)
            {
              var oShipperOrder = await _shipperInvoiceRepository.GetShipperOrderByOrderNo(order.OrderNo, _currentUser.ClientId!.Value);

              var oAddress = await _orderRepository.GetOrderAddressById(order.OrderAddressId.GetValueOrDefault());
              var store = await _orderRepository.GetStoreWithAddress(order.StoreId.GetValueOrDefault(), _currentUser!.ClientIdStr);

              if (oShipperOrder is null)
              { 
                var aa = ShipperOrder.Create(order, oAddress, store);
                var a = await _shipperInvoiceRepository.CreateShipperOrder(aa);
              }
              else
              {
                if (request.SaleChannelConfigId != oShipperOrder.SaleChannelConfigId)
                {
                  oShipperOrder!.UpdateSaleChannel(order, oAddress, store);
                  await _shipperInvoiceRepository.UpdateShipperOrder(oShipperOrder);
                }
              }
            }
          }
          catch (Exception ex)
          {
            _ = ex;
          }
        } 
        // SAME request model (no change)
        #endregion

        dynamic resData = new ExpandoObject();
        resData.OrderId = order!.OrderId!.Value.ToString();
        resData.OrderNo = order.OrderNo;
        response = new ServiceResultDTO(new BaseResponseDto { Data = resData, Message = NotificationConstants.Success });
      }
      else
      {
        response.CreateError("OrderLocked", new string[] { "Order already locked" });
      }

      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
  private string GetDescriptionForShipmentItem(Product? p, ProductStock ps)
  {
    StringBuilder sbf = new StringBuilder($"Name :{p!.ProductName} Sku: {ps.Sku} - ");

    if (p!.HaveOptions.HasValue)
    {
      sbf.Append($"{ps.VarientOption} - ");
    }
    sbf.Append("Q2");
    return sbf.ToString();
  }
  private async Task CreateOrderItemCommon(OrderItemModel item, Order createdOrder, int? orderTypeId)
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
        long? inventoryBalanceId = 0;
        long? productVariantId = item.ProductVariantId;

        if (inventoryBalanceId.GetValueOrDefault() == 0 && productVariantId.GetValueOrDefault() > 0 && createdOrder.StationId.HasValue && createdOrder.StationId.Value > 0)
        {
          var resolvedBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(productVariantId.GetValueOrDefault(), createdOrder.StationId.Value);
          if (resolvedBalance != null)
          {
            inventoryBalanceId = resolvedBalance.InventoryBalanceId;
          }
        }

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

        orderItem = OrderItem.CreateOrderItemFullFilable(createdOrder.OrderId!, new ProductId(new Guid(item.ProductId!)), item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, item.HsCode, item.OriginCountryCode, item.UnitRate, item.Weight, productVariantId);
        OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
      }
      else
      {
        orderItem = OrderItem.CreateOrderItemFullFilable(createdOrder.OrderId!, new ProductId(new Guid(item.ProductId!)), item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, item.HsCode, item.OriginCountryCode, item.UnitRate, item.Weight, item.ProductVariantId);
        OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
      }
    }
    else if (orderTypeId == (int)EnumOrderType.Regular)
    {
      orderItem = OrderItem.CreateOrderItemRegular(createdOrder.OrderId!, item.Price, item.Description, item.Remarks, item.Quantity, item.Discount, item.HsCode, item.OriginCountryCode, item.UnitRate, item.Weight);
      OrderItem createdItem = await _orderRepository.CreateOrderItem(orderItem!);
    }
  }

}
