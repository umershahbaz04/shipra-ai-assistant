using System.Net;
using iText.Layout.Properties;
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

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CreateDeliveryNoteDetailPendingForReturnStatus;
public class CreateDeliveryNoteDetailPendingForReturnStatusCommandHandler : RequestHandlerBase<CreateDeliveryNoteDetailPendingForReturnStatusCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;

  public CreateDeliveryNoteDetailPendingForReturnStatusCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,IOrderRepository orderRepository, IDeliveryNoteRepository deliveryNoteRepository, IOrderTrackingHistoryRepository historyRepository, IDeliveryTaskRepository deliveryTaskRepository, IServiceProvider serviceProvider, ILogger<CreateDeliveryNoteDetailPendingForReturnStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _historyRepository = historyRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateDeliveryNoteDetailPendingForReturnStatusCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var orderId = new OrderId(new Guid(request.OrderId!));
      var oOrder = await _orderRepository.GetOrderById(orderId, _currentUser.ClientId!);
      if (oOrder is not null)
      {
        if (!oOrder!.TrackingLock.GetValueOrDefault())
        {
          var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailByOrderId(orderId);
          if (oDeliveryNoteDetail is not null)
          {
            //Update delivery note detail status as Completed
            oDeliveryNoteDetail.UpdateDeliveryNoteDetailStatus((int)EnumDeliveryNoteDetailStatusLookup.Completed, _currentUser.EmployeeId!);
            await _deliveryNoteRepository.UpdateDeliveryNoteDetail(oDeliveryNoteDetail);
          }
          var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(orderId);
          if (oDeliveryTask is not null)
          {
            //Update delivery task status from Pending => Completed
            oDeliveryTask.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Completed, _currentUser.EmployeeId!);
            await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
          }

          //update order carrier tracking status as Pending For Return
          oOrder.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.PendingForReturn, _currentUser.EmployeeId!, "Pending For Return"); 
          await _orderRepository.UpdateOrder(oOrder);

          //create order tracking history of this order for Pending For Return 

          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

          var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder!.OrderId!, (int)EnumCarrierTrackingStatus.PendingForReturn, "Order is set as Pending For Return ", _currentUser.EmployeeId!, createdByName));
        }
        var result = new BaseResponseDto
        {
          Data = oOrder.OrderNo,
          Message = "Order status successfully set as Pending For Return "
        };
        serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Order not found");
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
