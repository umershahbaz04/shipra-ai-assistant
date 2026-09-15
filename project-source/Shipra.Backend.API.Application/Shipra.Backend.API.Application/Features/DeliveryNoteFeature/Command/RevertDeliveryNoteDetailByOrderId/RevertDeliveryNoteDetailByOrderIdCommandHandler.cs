using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.RevertDeliveryNote;
public class RevertDeliveryNoteDetailByOrderIdCommandHandler : RequestHandlerBase<RevertDeliveryNoteDetailByOrderIdCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public RevertDeliveryNoteDetailByOrderIdCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,IOrderRepository orderRepository, IDeliveryTaskRepository deliveryTaskRepository, IOrderTrackingHistoryRepository historyRepository, IDeliveryNoteRepository deliveryNoteRepository, IServiceProvider serviceProvider, ILogger<RevertDeliveryNoteDetailByOrderIdCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _historyRepository = historyRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(RevertDeliveryNoteDetailByOrderIdCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    var result = new BaseResponseDto();
    try
    {
      var orderId = new OrderId(new Guid(request.OrderId!));
      var oOrder = await _orderRepository.GetOrderById(orderId, _currentUser!.ClientId!);
      var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(orderId);
      var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailByOrderId(orderId);
      if (oOrder is not null)
      {
        if (oDeliveryTask is not null)
        {
          //Update delivery task status from Pending => Started
          oDeliveryTask.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Allocated, _currentUser.EmployeeId!);
          await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
        }

        if (oDeliveryNoteDetail is not null)
        {
          //Update delivery note detail status as Pending
          oDeliveryNoteDetail.UpdateDeliveryNoteDetailStatusForRevert(_currentUser.EmployeeId!);
          await _deliveryNoteRepository.UpdateDeliveryNoteDetail(oDeliveryNoteDetail);
        }        
        //Update order carrier tracking status as InOperation
        oOrder!.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.InOperation, _currentUser.EmployeeId!, "In Operation");
        var oUpdatedOrder = await _orderRepository.UpdateOrder(oOrder);

        //Update order tracking history as InOperation
        string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

        var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(orderId, (int)EnumCarrierTrackingStatus.InOperation, "Order is reverted to InOperation ", _currentUser.EmployeeId!, createdByName, (int)EnumOrderHistoryType.ShipraApp));
        result = new BaseResponseDto()
        {
          Data = oDeliveryNoteDetail!.DeliveryNoteDetailId!.value,
          Message = "Delivery note detail status reverted successfully "
        };
        serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      }
      else
      {
        result = new BaseResponseDto()
        {
          Data = oDeliveryNoteDetail!.DeliveryNoteDetailId!.value,
          Message = "Delivery note detail status cannot be revert "
        };
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
      }
      serviceResult = new ServiceResultDTO(result);
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
