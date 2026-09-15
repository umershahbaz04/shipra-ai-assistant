using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteMyCarrierRetunReport;
public class DeleteMyCarrierReturnReportCommandHandler : RequestHandlerBase<DeleteMyCarrierReturnReportCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly ICarrierReturnReport _carrierReturnReport;
  private readonly IOrderRepository _orderRepository;
  private readonly IProductRepository _productRepository;

  public DeleteMyCarrierReturnReportCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,IOrderTrackingHistoryRepository historyRepository, ICarrierReturnReport carrierReturnReport, IOrderRepository orderRepository, IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<DeleteMyCarrierReturnReportCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _historyRepository = historyRepository;
    _carrierReturnReport = carrierReturnReport;
    _orderRepository = orderRepository;
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(DeleteMyCarrierReturnReportCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oCarrierReturnReport = await _carrierReturnReport.GetCarrierReturnReportById(new CarrierRRId(new Guid(request.CarrierRRId!)));
      if (oCarrierReturnReport is null)
      {
        throw new EntityNotFoundException("Carrier return report ", new CarrierRRId(new Guid(request.CarrierRRId!)));
      }
      string returnReportNo = oCarrierReturnReport!.ReturnReportNo!;
      var isDeletedCRR = await _carrierReturnReport.DeleteCarrierReturnRerport(oCarrierReturnReport);
      if (isDeletedCRR)
      {
        var orders = await _orderRepository.GetOrdersByReturnReportId(new CarrierRRId(new Guid(request.CarrierRRId!)), _currentUser.ClientId!);

        foreach (var order in orders!)
        {
          order!.UpdateStatusForReturnReport(null, _currentUser.EmployeeId, (int)EnumCarrierTrackingStatus.PendingForReturn,"Pending for return", false);
          var update = await _orderRepository.UpdateOrder(order);
          if (order!.OrderTypeId == (int)EnumOrderType.FullFilable)
          {
            //3. Get orderitems if order is fullfillable
            var oOrderItemsList = await _orderRepository.GetOrderItemsByOrderId(order.OrderId);
            if (oOrderItemsList is not null && oOrderItemsList.Count > 0)
            {
              foreach (var item in oOrderItemsList)
              {
                //4.  Get product by productId
                var oProduct = await _productRepository.GetProductByIdAsync(item.ProductId!, _currentUser.ClientId!);
                if (oProduct is not null && oProduct.TrackInventory.GetValueOrDefault() && order.StationId.HasValue && order.StationId.Value > 0)
                {
                  var inventoryBalance = await _productRepository.GetInventoryBalanceByVariantAndStationAsync(item.ProductVariantId.GetValueOrDefault(), order.StationId.Value);
                  if (inventoryBalance != null)
                  {
                    int previousQuantity = inventoryBalance.QuantityAvailable;
                    int quantityAdjusted = item.Quantity.GetValueOrDefault();
                    int quantity = previousQuantity - quantityAdjusted;

                    var onHand = inventoryBalance.QuantityOnHand - quantityAdjusted;
                    var available = inventoryBalance.QuantityAvailable - quantityAdjusted;
                    inventoryBalance.UpdateFulfillmentAndHandStock(
                        onHand < 0 ? 0 : onHand,
                        available < 0 ? 0 : available,
                        inventoryBalance.QuantityCommitted,
                        inventoryBalance.QuantityOnOrder
                    );
                    await _productRepository.UpdateInventoryBalanceAsync(inventoryBalance);

                    var transaction = InventoryTransaction.Create(
                        inventoryBalance.ProductVariantId,
                        inventoryBalance.ProductStationId,
                        (int)InventoryTransactionType.Deallocation,
                        quantityAdjusted,
                        previousQuantity,
                        quantity < 0 ? 0 : quantity,
                        "Pending For Return Order History ",
                        _currentUser.EmployeeId!
                    );
                    await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> { transaction });
                  }
                }
              }
            }
          }

          #region order history
          //create order note
          var oOrderTrackingHistory = await _historyRepository.CreateOrderNote(OrderNote.CreateOrderNote(order!.OrderId!, "Carrier return report deleted against " + returnReportNo, _currentUser.EmployeeId!));

          //create history
          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

          OrderTrackingHistory orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, (int)EnumCarrierTrackingStatus.PendingForReturn, "Pending For Return", _currentUser.EmployeeId!, createdByName);
          OrderTrackingHistory createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);
          #endregion
        }
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = request.CarrierRRId,
          Message = "Carrier return report with Id " + request.CarrierRRId + " deleted successfully "
        });
      }
      #region publish notification 
      await _mediator.Publish(new
              RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(request),
        Response = JsonConvert.SerializeObject(serviceResult),
        EventName = "oncreateOrder",
        ClientId = _currentUser.ClientIdStr,
        EmployeeId = _currentUser.EmployeeIdStr,
        CreateOn = DateTime.UtcNow
      });
      #endregion
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
