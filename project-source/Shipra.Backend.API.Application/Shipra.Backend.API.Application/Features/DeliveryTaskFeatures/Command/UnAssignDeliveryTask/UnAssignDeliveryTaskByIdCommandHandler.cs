using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.DeleteDeliveryTask;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.UnAssignDeliveryTask;
public class UnAssignDeliveryTaskByIdCommandHandler : RequestHandlerBase<UnAssignDeliveryTaskByIdCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;

  public UnAssignDeliveryTaskByIdCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,IOrderRepository orderRepository, IDeliveryTaskRepository deliveryTaskRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<UnAssignDeliveryTaskByIdCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _historyRepository = historyRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(UnAssignDeliveryTaskByIdCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    BaseResponseDto oBaseResponse = new BaseResponseDto();
    try
    {
      var deliveryTaskId = new DeliveryTaskId(new Guid(request!.DeliveryTaskId!));
      var deliveryTask = await _deliveryTaskRepository.GetDeliveryTaskById(deliveryTaskId);
      if (deliveryTask is null)
      {
        throw new EntityNotFoundException("DeliveryTask not found ", deliveryTaskId.Value);
      }
      else
      { 
        var order = await _orderRepository.GetOrderById(deliveryTask!.OrderId!, _currentUser.ClientId!);
        if (order!.PaymentMethodId == (int)EnumPaymentMethod.COD)
        {
          oBaseResponse = new BaseResponseDto()
          {
            Data = deliveryTaskId.Value,
            Message = "Delivery Task Cannot Be Deleted Because It Has COD "
          };
          serviceResultDTO = new ServiceResultDTO(oBaseResponse, false);
        }
        else
        {
          bool isDeliveryTaskDeleted = await _deliveryTaskRepository.DeleteDeliveryTaskById(deliveryTask);
          if (isDeliveryTaskDeleted)
          {
            order!.UpdateOrderForUnAssignDeliveryTask(_currentUser!.EmployeeId!);
            await _orderRepository.UpdateOrder(order!);

            string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId); 
            var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(order!.OrderId!, (int)EnumCarrierTrackingStatus.UnAssignfromcarrier, "Order's " + order.OrderNo + " DeliveryTask Deleted", _currentUser.EmployeeId!, createdByName));

            oBaseResponse = new BaseResponseDto()
            {
              Data = deliveryTaskId.Value,
              Message = "Delivery Task Deleted Successfully"
            };
            serviceResultDTO = new ServiceResultDTO(oBaseResponse);
          }
          else
          {
            oBaseResponse = new BaseResponseDto()
            {
              Data = deliveryTaskId.Value,
              Message = "Delivery Task Cannot Be Deleted"
            };
            serviceResultDTO = new ServiceResultDTO(oBaseResponse, false);
          }
        }
      }
      #region publish notification 
      await _mediator.Publish(new
              RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(request),
        Response = JsonConvert.SerializeObject(serviceResultDTO),
        EventName = "oncreateOrder",
        ClientId = _currentUser.ClientIdStr,
        EmployeeId = _currentUser.EmployeeIdStr,
        CreateOn = DateTime.UtcNow
      });
      #endregion

      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
