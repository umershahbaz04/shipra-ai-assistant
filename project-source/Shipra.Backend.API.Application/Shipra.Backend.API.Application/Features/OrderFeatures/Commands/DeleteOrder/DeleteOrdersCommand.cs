using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrder;
public class DeleteOrdersCommand : IRequest<ServiceResultDTO>
{
  public string? OrderIds { get; set; }
}
public class DeleteOrderCommandHandler : RequestHandlerBase<DeleteOrdersCommand, ServiceResultDTO>
{
  private readonly IShipperInvoiceRepository _shipperInvoiceRepository;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;

  public DeleteOrderCommandHandler(IShipperInvoiceRepository shipperInvoiceRepository, IProductRepository productRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<DeleteOrderCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shipperInvoiceRepository = shipperInvoiceRepository;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteOrdersCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      var orderList = await _orderRepository.GetOrdersByOrderIds(request.OrderIds!, _currentUser.ClientId!);

      //var lockedOrders = orderList.Where(x => x.TrackingLock.GetValueOrDefault(false)).ToList();
      //if (lockedOrders.Count > 0)
      //{
      //  response.CreateError("OrderLocked", new string[] { $"You cannot delete locked orders: {string.Join(',', lockedOrders.Select(x => x.OrderNo))}" });
      //  return response;
      //}

      var processOrderList = orderList.Where(x => x.FullFillmentStatusId.GetValueOrDefault((int)EnumFullfillmentStatus.Unfulfilled) == (int)EnumFullfillmentStatus.Unfulfilled && x.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.OrderPlaced);

      var discardOrderList = orderList.Where(x => x.CarrierTrackingStatusId != (int)EnumCarrierTrackingStatus.OrderPlaced || x.FullFillmentStatusId.GetValueOrDefault((int)EnumFullfillmentStatus.Unfulfilled) == (int)EnumFullfillmentStatus.Fulfilled);

      if (discardOrderList.Count() > 0)
      {

        response.CreateError("DiscardList", new string[] { $"You cannot delete these order no's: {string.Join(',', discardOrderList.Select(x => x.OrderNo))} please check order status is order placed and order is unfulfilled. " });


        return response;
      }


      #region delete order and its items
      foreach (var order in processOrderList.ToList())
      {
        if (order is not null)
        {
          #region release committed inventory stock before deleting
          if (order.FullFillmentStatusId.GetValueOrDefault((int)EnumFullfillmentStatus.Unfulfilled) == (int)EnumFullfillmentStatus.Unfulfilled)
          {
              var orderItems = await _orderRepository.GetOrderItemsByOrderId(order.OrderId);
              foreach (var item in orderItems)
              {
                  if (item.ProductId == null) continue;
                  var product = await _productRepository.GetProductByIdAsync(item.ProductId, _currentUser.ClientId!);
                  if (product != null && product.TrackInventory.GetValueOrDefault() && order.StationId.HasValue && order.StationId.Value > 0)
                  {
                      var inventoryBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(item.ProductVariantId.GetValueOrDefault(), order.StationId.Value);
                      if (inventoryBalance != null)
                      {
                          var committed = inventoryBalance.QuantityCommitted - item.Quantity.GetValueOrDefault();
                          var quantityAvailable = inventoryBalance.QuantityAvailable + item.Quantity.GetValueOrDefault();
                          inventoryBalance.UpdateQuantities(inventoryBalance.QuantityOnHand, quantityAvailable, committed < 0 ? 0 : committed);
                          await _productRepository.UpdateInventoryBalanceAsync(inventoryBalance);

                          // Create transaction log
                          var transaction = InventoryTransaction.Create(
                              inventoryBalance.ProductVariantId,
                              inventoryBalance.ProductStationId,
                              (int)InventoryTransactionType.Adjustment,
                              item.Quantity.GetValueOrDefault(),
                              inventoryBalance.QuantityAvailable - item.Quantity.GetValueOrDefault(),
                              inventoryBalance.QuantityAvailable,
                              $"Release committed quantity {item.Quantity} on Deletion of Order: {order.OrderNo}",
                              _currentUser.EmployeeId!
                          );
                          await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> { transaction });
                      }
                  }
              }
          }
          #endregion

          await CreateOrderDeletedAndDeleteOrder(order);
          #region unused code we dont need here
          //if (order.OrderTypeId == (int)EnumOrderType.Regular)
          //{
          //  await RemoveOrderAndItsItemsRegular(order);
          //}
          //else
          //{
          //  await RemoveOrderAndItsItemsFulFillable(order);
          //} 
          #endregion
          #region remove shipper invoice order 
          bool isAllowedShipperInvocie = (await _orderRepository.GetClientConfigSetting(_currentUser.ClientId!))
                    ?.AllowShipperInvocie ?? false;

          try
          {
            if (isAllowedShipperInvocie)
            {
              var oShiperOrder = await _shipperInvoiceRepository.GetShiperOrderByOrderNo(order.OrderNo!, _currentUser.ClientId!.Value);
              if (oShiperOrder is not null)
              {
                await _shipperInvoiceRepository.DeleteShiperOrder(oShiperOrder);
              }
            }
          }
          catch (Exception ex)
          {
            _ = ex;

          } 
          #endregion
        }
      }
      #endregion

      response = new ServiceResultDTO(new BaseResponseDto
      {
        Message = "Order Deleted successfully"
      });


      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }

  #region we will delete order related things after few days we dont need this code here
  //#region delete fulfilable order 
  //private async Task RemoveOrderAndItsItemsFulFillable(Order oOrder)
  //{
  //  if (oOrder.OrderTypeId == (int)EnumOrderType.FullFilable)
  //  {
  //    List<OrderItem> orderItems = await _orderRepository.GetOrderItemsByOrderId(oOrder.OrderId!);
  //    foreach (OrderItem item in orderItems)
  //    {
  //      #region stock related
  //      var product = await _productRepository.GetProductByIdAsync(item.ProductId!, _currentUser.ClientId!);
  //      if (product!.TrackInventory.GetValueOrDefault())
  //      {
  //        dynamic psQuantity = new System.Dynamic.ExpandoObject();// new { QuantityAvailable = 0, QuantityCommited = 0, QuantityOnOrder = 0 };

  //        var productStock = await _productRepository.GetProductStockByIdAsync(item.ProductStockId);

  //        if (oOrder!.FullFillmentStatusId == (int)EnumFullfillmentStatus.Unfulfilled)
  //        {
  //          psQuantity.QuantityCommited = productStock.QuantityCommited - item.Quantity;
  //          //no need to change
  //          psQuantity.QuantityOnOrder = productStock.QuantityOnOrder;
  //          psQuantity.QuantityAvailable = productStock.QuantityAvailable;


  //          ///update and create product stock history
  //          await UpdateProductStockAsync(psQuantity, productStock);
  //        }

  //      }

  //      #endregion


  //      await DeleteOrderItemAndCreateHistory(item);
  //    }

  //    //delete order
  //    await DeleteOrderAndCreateHistory(oOrder!);
  //  }
  //}
  //private async Task UpdateProductStockAsync(dynamic psQuantity, ProductStock? productStock)
  //{
  //  #region update product stock 
  //  productStock!.UpdateFullfilmentStock(psQuantity.QuantityAvailable, psQuantity.QuantityCommited, psQuantity.QuantityOnOrder, _currentUser.EmployeeId!);
  //  var updatedProdctStock = await _productRepository.UpdateProductStockAsync(productStock);
  //  #endregion
  //}

  //#endregion
  //#region remove regular items 
  //private async Task RemoveOrderAndItsItemsRegular(Order oOrder)
  //{
  //  if (oOrder.OrderTypeId == (int)EnumOrderType.Regular)
  //  {
  //    List<OrderItem> orderItems = await _orderRepository.GetOrderItemsByOrderId(oOrder.OrderId!);
  //    foreach (OrderItem item in orderItems)
  //    {
  //      //create history
  //      //DeletedItem
  //      //remove entry
  //      await DeleteOrderItemAndCreateHistory(item);
  //    }
  //    await DeleteOrderAndCreateHistory(oOrder);
  //  }
  //}
  //#endregion

  #endregion
  #region order common
  //private async Task DeleteOrderAndCreateHistory(Order order)
  //{
  //  //create history
  //  bool isCreated = await CreateOrderDeletedAndDeleteOrder(order);
  //  //delete order
  //}

  private async Task<bool> CreateOrderDeletedAndDeleteOrder(Order order)
  {
    OrderDeleted orderDeleted = OrderDeleted.CreateOrderDeleted(orderId: order.OrderId,
        clientId: order.ClientId,
        storeId: order.StoreId,
        saleChannelConfigId: order.SaleChannelConfigId,
        orderTypeId: order.OrderTypeId,
        orderNo: order.OrderNo,
        orderDate: order.OrderDate,
        orderAddressId: order.OrderAddressId,
        amount: order.Amount,
        carrierId: order.CarrierId,
        activeCarrierId: order.ActiveCarrierId,
        carrierTrackingNo: order.CarrierTrackingNo,
        carrierTrackingStatus: order.CarrierTrackingStatus,
        carrierTrackingStatusId: order.CarrierTrackingStatusId,
        carrierAssignDate: order.CarrierAssignDate,
        carrierLastUpdateDateTime: order.CarrierLastUpdateDateTime,
        carrierPaymentSettlementId: order.CarrierPaymentSettlementId,
        carrierRRId: order.CarrierRRId,
        description: order.Description,
        remarks: order.Remarks,
        fullFillmentStatusId: order.FullFillmentStatusId,
        fulFiledById: order.FulFiledById,
        fulFilledDate: order.FulFilledDate,
        itemsCount: order.ItemsCount,
        deliveryCharges: order.DeliveryCharges,
        paymentStatusId: order.PaymentStatusId,
        paymentRef: order.PaymentRef,
        returnRef: order.ReturnRef,
        weight: order.Weight,
        itemValue: order.ItemValue,
        orderRequestVia: order.OrderRequestVia,
        paymentMethodId: order.PaymentMethodId,
        stationId: order.StationId,
        discount: order.Discount,
        vat: order.Vat,
        totalTax: order.TotalTax,
        cShippingCharges: order.CShippingCharges,
        trackingLock: order.TrackingLock,
        refNo: order.RefNo,
        createdOn: order.CreatedOn,
        createdBy: order.CreatedBy,
        updatedOn: order.UpdatedOn,
        updatedBy: order.UpdatedBy,
        stripeCustomerId: order.StripeCustomerId,
        stripeInvoiceHostURL: order.StripeInvoiceHostURL,
        stripeInvoicePDFURL: order.StripeInvoicePDFURL,
        stripeInvoiceId: order.StripeInvoiceId,
        saleChannelOrderId: order.SaleChannelOrderId,
        saleChannelLookupId: order.SaleChannelLookupId);

    bool isCreated = await _orderRepository.CreateOrderDeleted(orderDeleted);

    if (isCreated)
    {
      await _orderRepository.DeleteOrder(order);
    }
    return isCreated;
  }

  private async Task DeleteOrderItemAndCreateHistory(OrderItem item)
  {
    //create history

    //delete order item
    await _orderRepository.DeleteOrderItem(item);
  }

  #endregion
}
public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrdersCommand>
{
  public DeleteOrderCommandValidator()
  {
    RuleFor(x => x.OrderIds).NotEmpty().NotNull();
  }
}
