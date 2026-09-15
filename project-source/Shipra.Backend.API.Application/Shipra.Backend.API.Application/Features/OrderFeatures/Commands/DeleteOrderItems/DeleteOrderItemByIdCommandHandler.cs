using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrderItems;
public class DeleteOrderItemByIdCommandHandler : RequestHandlerBase<DeleteOrderItemByIdCommand, ServiceResultDTO>
{
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;
  public DeleteOrderItemByIdCommandHandler(IProductRepository productRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<DeleteOrderItemByIdCommandHandler> logger) : base(serviceProvider, logger)
  {
    _productRepository = productRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteOrderItemByIdCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();

    try
    {
      var oOrder = await _orderRepository.GetOrderById(new OrderId(new Guid(request.OrderId!)), _currentUser.ClientId!);
      if (oOrder is null)
      {
        throw new EntityNotFoundException("Order ", request.OrderId!);
      }
      else
      {
        //delete item if order in unfulfilled and status order placed only
       if (!oOrder.TrackingLock.GetValueOrDefault()) 
        {
          var oOrderItem = await _orderRepository.GetOrderItemByIdAndOrderItemId(new OrderId(new Guid(request.OrderId!)), new OrderItemId(new Guid(request.OrderItemId!)));
          if (oOrderItem is not null)
          {
            if (oOrder.OrderTypeId == (int)EnumOrderType.FullFilable)
            {
              #region stock related
              var product = await _productRepository.GetProductByIdAsync(oOrderItem.ProductId!, _currentUser.ClientId!);
              if (product!.TrackInventory.GetValueOrDefault() && oOrder.StationId.HasValue && oOrder.StationId.Value > 0)
              {
                var inventoryBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(oOrderItem.ProductVariantId.GetValueOrDefault(), oOrder.StationId.Value);
                if (inventoryBalance != null)
                {
                  int previousQuantity = inventoryBalance.QuantityAvailable;
                  int quantityAdjusted = oOrderItem.Quantity.GetValueOrDefault();

                  if (oOrder!.FullFillmentStatusId == (int)EnumFullfillmentStatus.Unfulfilled)
                  {
                    var committed = inventoryBalance.QuantityCommitted - quantityAdjusted;
                    inventoryBalance.UpdateFulfillmentStock(
                        inventoryBalance.QuantityAvailable + quantityAdjusted,
                        committed < 0 ? 0 : committed,
                        inventoryBalance.QuantityOnOrder
                    );
                    await _productRepository.UpdateInventoryBalanceAsync(inventoryBalance);

                    var transaction = InventoryTransaction.Create(
                        inventoryBalance.ProductVariantId,
                        inventoryBalance.ProductStationId,
                        (int)InventoryTransactionType.Adjustment,
                        quantityAdjusted,
                        previousQuantity,
                        previousQuantity + quantityAdjusted,
                        $"Fixed quantity {quantityAdjusted} on Deletion of Order item: {oOrder.OrderNo}",
                        _currentUser.EmployeeId!
                    );
                    await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> { transaction });
                  }
                }
              }
              #endregion

              await _orderRepository.DeleteOrderItem(oOrderItem);
            }
            else
            {
              await _orderRepository.DeleteOrderItem(oOrderItem);
            }

            var baseResponse = new BaseResponseDto()
            {
              Data = oOrderItem!.OrderItemId!,
              Message = "OrderItem deleted successfully "
            };
            response = new ServiceResultDTO(baseResponse);
            response.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
          }
          else
          {
            throw new EntityNotFoundException("OrderItem ", request.OrderItemId!);
          }
        }
        else
        { 
          response.CreateError("DiscardList", new string[] { $"You cannot delete this order no: {oOrder.OrderNo} please check order status is order placed and order is unfulfilled. " }); 
          return response;
        }
      }
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }

  #region order common 
  private async Task DeleteOrderItemAndCreateHistory(OrderItem item)
  {
    //create history

    //delete order item
    await _orderRepository.DeleteOrderItem(item);
  }

  #endregion
}
