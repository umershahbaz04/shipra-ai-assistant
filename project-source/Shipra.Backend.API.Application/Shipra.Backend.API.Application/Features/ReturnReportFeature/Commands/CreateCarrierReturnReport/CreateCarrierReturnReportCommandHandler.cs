using DocumentFormat.OpenXml.Drawing;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Commands.CreateCarrierReturnReport;
public class CreateCarrierReturnReportCommandHandler : RequestHandlerBase<CreateCarrierReturnReportCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly ICarrierReturnReport _carrierReturnReport;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;

  public CreateCarrierReturnReportCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,ICarrierReturnReport carrierReturnReport, IOrderTrackingHistoryRepository historyRepository, IClientRepository clientRepository, IProductRepository productRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CreateCarrierReturnReportCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _carrierReturnReport = carrierReturnReport;
    _historyRepository = historyRepository;
    _clientRepository = clientRepository;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateCarrierReturnReportCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
      }
      var orderList = await _orderRepository.GetOrdersByOrderNos(request.TrackingNos!, _currentUser.ClientId!);
      var alreadyDelivered = orderList.Where(x => x.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.Delivered).ToList();
      if (alreadyDelivered.Count == 0)
      {
        var returnReportExisted = orderList.Where(x => x.CarrierRRId != null).ToList();
        if (returnReportExisted.Count == 0)
        {
          var nullCarrierList = orderList.Where(x => x.CarrierId == null).ToList();
          if (nullCarrierList.Count == 0)
          {
            var notSameCarrierList = orderList.Where(x => x.CarrierId != request.CarrierId).ToList();
            if (notSameCarrierList.Count == 0)
            {
              var ordersNotLocked = orderList.Where(x => x.TrackingLock.GetValueOrDefault() != true);

              string returnReportNo = CarrierReturnReport.GetReturnReportNo(client.ClientIdentifier);
              int totalOrders = ordersNotLocked.Count();
              var lockedOrders = orderList.Where(x => x.TrackingLock.GetValueOrDefault() == true).ToList();

              //save return report
              CarrierReturnReport carrierReturnReport = CarrierReturnReport.CreateCarrierReturnReport(returnReportNo, totalOrders, request.CarrierId, _currentUser.EmployeeId);
              var addedReeport = await _carrierReturnReport.CreateCarrierReturnReport(carrierReturnReport);

              foreach (var order in ordersNotLocked!)
              {
                if (!order.TrackingLock.GetValueOrDefault(false))
                {
                  order.UpdateStatusForReturnReport(carrierReturnReport.CarrierRrid, _currentUser.EmployeeId, (int)EnumCarrierTrackingStatus.Returned,"Returned", true);
                  var update = await _orderRepository.UpdateOrder(order);

                  //if order type fullfileable then update product stock 
                  if (order.OrderTypeId == (int)EnumOrderType.FullFilable)
                  {
                    if (order.FullFillmentStatusId == (int)EnumFullfillmentStatus.Fulfilled)
                    {
                      var oItems = await _orderRepository.GetOrderItemsByOrderId(order.OrderId);
                      foreach (var item in oItems)
                      {
                        var product = await _productRepository.GetProductByIdAsync(item.ProductId!, _currentUser.ClientId!);
                        if (product is null)
                        {
                          throw new EntityNotFoundException("Product ", _currentUser.ClientIdStr!.ToString());
                        }
                        if (product!.TrackInventory.GetValueOrDefault())
                        {
                          //with product inventory
                          #region update productstock
                          if (order.StationId.HasValue && order.StationId.Value > 0)
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

                              var comment = "Quantity Add against Return Report " + addedReeport.ReturnReportNo;
                              var transaction = InventoryTransaction.Create(
                                  inventoryBalance.ProductVariantId,
                                  inventoryBalance.ProductStationId,
                                  (int)InventoryTransactionType.Return,
                                  quantityAdjusted,
                                  previousQuantity,
                                  quantity,
                                  comment,
                                  _currentUser.EmployeeId!
                              );
                              await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> { transaction });
                            }
                          }
                          #endregion
                        }
                      }
                    }
                  }


                  #region order history
                  string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

                  OrderTrackingHistory orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, (int)EnumCarrierTrackingStatus.Returned, $"Carrier Returned report created against Report No: {returnReportNo}.", _currentUser.EmployeeId!, createdByName);
                  OrderTrackingHistory createdOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(orderHistory);
                  #endregion
                }
              } 
              if (lockedOrders.Count > 0)
              {
                serviceResult.CreateError("LockedOrders", new string[] { "Following order's already locked. " + string.Join(", ", lockedOrders.Select(x => x.OrderNo)) }); 
              }
              return serviceResult;
            }
            else
            {

              serviceResult.Errors!.Add("NotSameCarrierOrders", new string[] { "The Order does not belong to the same carrier: " + string.Join(',', notSameCarrierList.Select(x => x.OrderNo).ToList()) });
              serviceResult.IsSuccess = false;
              return serviceResult;
            }
          }
          else
          {
            serviceResult.Errors!.Add("NullCarrierOrders", new string[] { "The Order does not have carrier: " + string.Join(',', nullCarrierList.Select(x => x.OrderNo).ToList()) });
            serviceResult.IsSuccess = false;
            return serviceResult;
          }
        }
        else
        {
          serviceResult.Errors!.Add("AlreadyCreatedReturnReport", new string[] { "The Selected order's Return report was already created against the following detail: " + string.Join(',', returnReportExisted.Select(x => x.OrderNo).ToList()) });
          serviceResult.IsSuccess = false;
          return serviceResult;
        }
      }
      else
      {
        serviceResult.Errors!.Add("AlreadyDelivered", new string[] { "The Selected order's status is already delivered against the following detail: " + string.Join(',', alreadyDelivered.Select(x => x.OrderNo).ToList()) });
        serviceResult.IsSuccess = false;
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
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}

