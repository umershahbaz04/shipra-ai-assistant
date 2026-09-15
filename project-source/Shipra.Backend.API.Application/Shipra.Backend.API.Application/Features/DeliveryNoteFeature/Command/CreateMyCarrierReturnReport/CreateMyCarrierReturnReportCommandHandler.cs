using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CreateMyCarrierReturnReport;
public class CreateMyCarrierReturnReportCommandHandler : RequestHandlerBase<CreateMyCarrierReturnReportCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly ICarrierReturnReport _carrierReturnReport;
  private readonly IOrderRepository _orderRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IProductRepository _productRepository;

  public CreateMyCarrierReturnReportCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository, ICarrierReturnReport carrierReturnReport, IOrderRepository orderRepository, IClientRepository clientRepository, IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<CreateMyCarrierReturnReportCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator=mediator;
    _employeeRepository = employeeRepository;
    _carrierReturnReport = carrierReturnReport;
    _orderRepository = orderRepository;
    _clientRepository = clientRepository;
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(CreateMyCarrierReturnReportCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    List<string> successList = new List<string>();
    try
    {
      //1.  Fetch orders by order nos
      var oOrderList = await _orderRepository.GetOrdersWithOrderNos(request.OrderNos, _currentUser.ClientId!);
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      string returnReportNo = CarrierReturnReport.GetReturnReportNo(client!.ClientIdentifier);
      int totalOrders = oOrderList.Count();
      //2.  Create return report
      CarrierReturnReport carrierReturnReport = CarrierReturnReport.CreateCarrierReturnReport(returnReportNo, totalOrders, client.DefaultCarrierId, _currentUser.EmployeeId);
      await _carrierReturnReport.CreateCarrierReturnReport(carrierReturnReport);
      if (oOrderList is not null && oOrderList.Count > 0)
      {
        foreach (var order in oOrderList!)
        {
          if (order.CarrierTrackingStatusId != (int)EnumCarrierTrackingStatus.Returned)
          {

            order.UpdateStatusForReturnReport(carrierReturnReport.CarrierRrid, _currentUser.EmployeeId, (int)EnumCarrierTrackingStatus.Returned,"Returned", true);
            var updatedOrder = await _orderRepository.UpdateOrder(order);
            if (order.OrderTypeId == (int)EnumOrderType.FullFilable)
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
                      int quantity = previousQuantity + quantityAdjusted;

                      inventoryBalance.UpdateFulfillmentAndHandStock(
                          inventoryBalance.QuantityOnHand + quantityAdjusted,
                          inventoryBalance.QuantityAvailable + quantityAdjusted,
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
                          quantity,
                          "Return Order History ",
                          _currentUser.EmployeeId!
                      );
                      await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> { transaction });
                    }
                  }
                }
              }
            }

            #region order history
            string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

            OrderTrackingHistory orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, (int)EnumCarrierTrackingStatus.Returned, "My Carrier Returne Report Created", _currentUser.EmployeeId!, createdByName);
            OrderTrackingHistory createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);
            #endregion

            successList.Add(order.OrderNo!);
          } 
        }
      }
      if (successList is not null && successList.Count > 0)
      {
        BaseResponseDto baseResponse = new BaseResponseDto()
        {
          Data = successList,
          Message = "Return report created succcessfully for the selected orders."
        };
        serviceResult = new ServiceResultDTO(baseResponse);
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
