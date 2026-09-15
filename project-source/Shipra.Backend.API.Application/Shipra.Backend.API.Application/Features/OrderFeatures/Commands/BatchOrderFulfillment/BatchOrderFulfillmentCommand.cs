using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.BatchOrderFulfillment;
public class BatchOrderFulfillmentCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
  public int FullfilmentStatusId { get; set; }
}
public class BatchOrderFulfillmentCommandHandler : RequestHandlerBase<BatchOrderFulfillmentCommand, ServiceResultDTO>
{
  private readonly IOrderTrackingHistoryRepository _orderTrackingHistoryRepository;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;

  public BatchOrderFulfillmentCommandHandler(IOrderTrackingHistoryRepository orderTrackingHistoryRepository, IProductRepository productRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<BatchOrderFulfillmentCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderTrackingHistoryRepository = orderTrackingHistoryRepository;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(BatchOrderFulfillmentCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<dynamic> notAvailableQty = new List<dynamic>();

      List<Order> regularOrderList = new List<Order>(); //rejectedList 
      List<Order> fullfilableOrderList = new List<Order>();

      var orders = await _orderRepository.GetOrdersWithOrderNos(request.OrderNos, _currentUser.ClientId!);

      foreach (var order in orders)
      {
        int fulfilmentStatus = order.FullFillmentStatusId.HasValue ? order.FullFillmentStatusId.Value : 1;
        if (fulfilmentStatus == request.FullfilmentStatusId) //check if already status supplied from api and in order
        {
          notAvailableQty.Add(new { OrderNo = order.OrderNo, OrderType = order.OrderTypeId, Message = "This order already on same status" });
          regularOrderList.Add(order);
        }
        else
        {
          if (order.OrderTypeId == (int)EnumOrderType.Regular) //conditions for validation
          {
            notAvailableQty.Add(new { OrderNo = order.OrderNo, OrderType = (int)EnumOrderType.Regular, Message = "This order type not fullfilable" });
            regularOrderList.Add(order);
          }
          else if (order.OrderTypeId == (int)EnumOrderType.FullFilable)
          {
            fullfilableOrderList.Add(order);
          }
        }
        //if (true) //conditions for validation
        //{
        //  order.UpdateOrderFulfillmentStatusId(request.FullfilmentStatusId, new EmployeeId(new Guid(_currentUser.Id!)));
        //}
      }

      foreach (var order in fullfilableOrderList)
      {
        if (!order.StationId.HasValue || order.StationId.Value == 0)
        {
          notAvailableQty.Add(new { OrderNo = order.OrderNo, OrderType = order.OrderTypeId, Message = "Order is not assigned to a station." });
          continue;
        }

        var orderItems = await _orderRepository.GetOrderItemsByOrderId(order.OrderId);
        foreach (var item in orderItems)
        {
          var product = await _productRepository.GetProductByIdAsync(item.ProductId!, _currentUser.ClientId!);

          if (product is null)
          {
            throw new EntityNotFoundException("Product", item.ProductId!);
          }

          var productVariant = await _productRepository.GetProductVariantByIdAsync(item.ProductVariantId.GetValueOrDefault());
          string sku = productVariant?.SKU ?? "Unknown SKU";

          ///if product has flag track inventory then we have to deduct inventory
          if (product.TrackInventory.GetValueOrDefault())
          {
            var inventoryBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(item.ProductVariantId.GetValueOrDefault(), order.StationId.Value);
            if (inventoryBalance == null)
            {
              notAvailableQty.Add(new { OrderNo = order.OrderNo, OrderType = (int)EnumOrderType.FullFilable, Message = $"Inventory balance record not found for variant ID: {item.ProductVariantId} in station ID: {order.StationId.Value}." });
              continue;
            }

            if (item.Quantity <= inventoryBalance.QuantityOnHand || request.FullfilmentStatusId == (int)EnumFullfillmentStatus.Unfulfilled)
            {
              int previousQuantity = inventoryBalance.QuantityOnHand;
              int quantityAdjusted = item.Quantity.GetValueOrDefault();

              if (request.FullfilmentStatusId == (int)EnumFullfillmentStatus.Unfulfilled)
              {
                int newQuantity = previousQuantity + quantityAdjusted;
                int reasonId = (int)InventoryTransactionType.Deallocation;
                string comment = $"Quantity UnFulfilled against {order.OrderNo}";

                await CreateInventoryTransactionAndUpdateBalance(reasonId, previousQuantity, newQuantity, quantityAdjusted, comment, inventoryBalance);

                order.UpdateOrderFulfillmentStatus(request.FullfilmentStatusId, null, null); 
                var response = await _orderRepository.UpdateOrder(order);
                #region order note 
                await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, "Order UnFulfilled", _currentUser.EmployeeId!));
                #endregion
              }
              else if (request.FullfilmentStatusId == (int)EnumFullfillmentStatus.Fulfilled)
              {
                int newQuantity = previousQuantity - quantityAdjusted;
                int reasonId = (int)InventoryTransactionType.Dispatch;
                string comment = $"Quantity Fulfilled against {order.OrderNo}";

                await CreateInventoryTransactionAndUpdateBalance(reasonId, previousQuantity, newQuantity, quantityAdjusted, comment, inventoryBalance);

                order.UpdateOrderFulfillmentStatus(request.FullfilmentStatusId, _currentUser.EmployeeId!, DateTime.UtcNow);
                var response = await _orderRepository.UpdateOrder(order);
                #region order note 
                await _orderTrackingHistoryRepository.CreateOrderNote(OrderNote.CreateOrderNote(order.OrderId!, "Order Fulfilled", _currentUser.EmployeeId!));
                #endregion
              }

            }
            else
            {
              notAvailableQty.Add(new { OrderNo = order.OrderNo, OrderType = (int)EnumOrderType.FullFilable, Message = $"This order did not meet the quantity against: {sku} " });
            }
          }
          else
          {
            ///without track inventory
          }
        }
      }
      if (notAvailableQty.Count > 0)
      {
        var json = JsonConvert.SerializeObject(notAvailableQty);
        serviceResult.Errors?.Add("InvalidOrders", new[] { json });
        serviceResult.IsSuccess = false;
      }
      else
      {
        serviceResult.IsSuccess = true;
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Message = "Order Successfully Fulfilled" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task CreateInventoryTransactionAndUpdateBalance(
      int reasonId, 
      int previousQuantity, 
      int newQuantity, 
      int quantityAdjusted,
      string comment, 
      InventoryBalance inventoryBalance)
  {
      #region update inventory balance 
      if (reasonId == (int)InventoryTransactionType.Deallocation)
      {
          inventoryBalance.UpdateFulfillmentAndHandStock(
              inventoryBalance.QuantityOnHand + quantityAdjusted,
              inventoryBalance.QuantityAvailable,
              inventoryBalance.QuantityCommitted + quantityAdjusted,
              inventoryBalance.QuantityOnOrder - quantityAdjusted
          );
      }
      else if (reasonId == (int)InventoryTransactionType.Dispatch)
      {
          var committed = inventoryBalance.QuantityCommitted - quantityAdjusted;
          inventoryBalance.UpdateFulfillmentAndHandStock(
              inventoryBalance.QuantityOnHand - quantityAdjusted,
              inventoryBalance.QuantityAvailable,
              committed < 0 ? 0 : committed,
              inventoryBalance.QuantityOnOrder + quantityAdjusted
          );
      }
      await _productRepository.UpdateInventoryBalanceAsync(inventoryBalance);
      #endregion

      #region inventory transaction history 
      var transaction = InventoryTransaction.Create(
          inventoryBalance.ProductVariantId,
          inventoryBalance.ProductStationId,
          reasonId,
          quantityAdjusted,
          previousQuantity,
          newQuantity,
          comment,
          _currentUser.EmployeeId!
      );
      await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> { transaction });
      #endregion
  }
}
public class BatchOrderFulfillmentCommandValidator : AbstractValidator<BatchOrderFulfillmentCommand>
{
  public BatchOrderFulfillmentCommandValidator()
  {
    RuleFor(v => v.OrderNos).NotNull().NotEmpty();
    RuleFor(v => v.FullfilmentStatusId).NotNull().NotEmpty().GreaterThan(0);
  }
}
