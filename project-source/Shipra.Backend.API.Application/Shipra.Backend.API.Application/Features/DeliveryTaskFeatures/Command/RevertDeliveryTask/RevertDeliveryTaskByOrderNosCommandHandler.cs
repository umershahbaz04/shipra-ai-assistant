using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.RevertDeliveryTask;
public class RevertDeliveryTaskByOrderNosCommandHandler : RequestHandlerBase<RevertDeliveryTaskByOrderNosCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public RevertDeliveryTaskByOrderNosCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,IOrderRepository orderRepository, IOrderTrackingHistoryRepository historyRepository, IDeliveryTaskRepository deliveryTaskRepository, IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<RevertDeliveryTaskByOrderNosCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _historyRepository = historyRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(RevertDeliveryTaskByOrderNosCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    List<string> successList = new List<string>();
    List<string> unsuccessList = new List<string>();
    List<string> trackingLockList = new List<string>();
    try
    {
      var oOrderList = await _orderRepository.GetOrdersByOrderNos(request.OrderNos!, _currentUser.ClientId!);
      if (oOrderList is not null && oOrderList.Count > 0)
      {
        List<OrderId> orderIds = oOrderList!.Select(x => x.OrderId!).ToList();
        var oDeliveryTaskList = await _deliveryTaskRepository.GetDeliveryTaskListByOrderId(orderIds);

        if (oDeliveryTaskList is not null && oDeliveryTaskList.Count > 0)
        {
          foreach (var deliveryTask in oDeliveryTaskList)
          {
            var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailByOrderId(deliveryTask!.OrderId!);
            var order = await _orderRepository.GetOrderById(deliveryTask.OrderId!, _currentUser.ClientId!);
            if (!order!.TrackingLock.GetValueOrDefault(false))
            {
              if (oDeliveryNoteDetail is null)
              {
                //Update Order UnAssign Carrier Id as null and CarrierTrackingStatus as OrderPlaced
                order!.UpdateOrderForRevertDeliveryTask(_currentUser.EmployeeId);
                await _orderRepository.UpdateOrder(order);

                //Create Order Tracking History from Assign Inhouse => OrderPlaced
                string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

                var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(order!.OrderId!, (int)EnumCarrierTrackingStatus.OrderPlaced, "Order's " + order.OrderNo + " DeliveryTask Reverted", _currentUser.EmployeeId!,createdByName));

                var isDeliveryTaskDeleted = await _deliveryTaskRepository.DeleteDeliveryTaskById(deliveryTask);
                successList.Add(order.OrderNo!);
              }
              else
              {
                unsuccessList.Add(order?.OrderNo!);
              }
            }
            else
            {
              trackingLockList.Add(order.OrderNo!);
            }
          }
          var result = new BaseResponseDto();
          if (successList is not null && successList.Count > 0)
          {
            result = new BaseResponseDto()
            {
              Data = string.Join(", ", successList),
              Message = "Delivery Task reverted successfully "
            };
            serviceResult = new ServiceResultDTO(result);
            serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
          }
          else if (trackingLockList.Count > 0)
          {
            serviceResult.Errors!.Add("OrderLocked", new string[] { "These orders are already locked " + string.Join(',', trackingLockList) });
            serviceResult.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest);
          }
          else
          {
            serviceResult.Errors!.Add("Delivery Task could'nt be reverted", new string[] { "These orders has delivery note " + string.Join(',', unsuccessList) });
            serviceResult.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest);
          }

        }
        else
        {
          throw new EntityNotFoundException("Delivery Task with order no ", request.OrderNos!);
        }
      }
      else
      {
        throw new EntityNotFoundException("Orders not found", request.OrderNos!);
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
