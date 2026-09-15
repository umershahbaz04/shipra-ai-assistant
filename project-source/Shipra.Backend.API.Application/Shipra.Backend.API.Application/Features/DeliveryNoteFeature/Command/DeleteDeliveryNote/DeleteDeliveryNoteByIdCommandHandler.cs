using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteDeliveryNote;

public class DeleteDeliveryNoteByIdCommandHandler : RequestHandlerBase<DeleteDeliveryNoteByIdCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;

  public DeleteDeliveryNoteByIdCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IDeliveryTaskRepository deliveryTaskRepository, IDeliveryNoteRepository deliveryNoteRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<DeleteDeliveryNoteByIdCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
    _historyRepository = historyRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteDeliveryNoteByIdCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    BaseResponseDto oBaseResponse = new BaseResponseDto();
    try
    {
      var deliveryNoteId = new DeliveryNoteId(new Guid(request!.DeliveryNoteId!));
      var deliveryNote = await _deliveryNoteRepository.GetDeliveryNoteById(deliveryNoteId);
      
      if (deliveryNote is null)
      {
        throw new EntityNotFoundException("DeliveryNote not found ", deliveryNoteId.Value);
      }
      
      var deliveryNoteDetails = await _deliveryNoteRepository.GetAllDeliveryNoteDetailByDeliveryNoteId(deliveryNoteId);
      
      if (deliveryNoteDetails != null && deliveryNoteDetails.Count > 0)
      {
        string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

        foreach (var detail in deliveryNoteDetails)
        {
          var order = await _orderRepository.GetOrderById(detail.OrderId!, _currentUser.ClientId!);
          if (order != null)
          {
            // Revert Order Status to AssignInHouse
            order.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.AssignInHouse, _currentUser.EmployeeId!, "Order Reverted to Assign In House");
            await _orderRepository.UpdateOrder(order);

            // Revert Delivery Task
            var deliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(order.OrderId!);
            if (deliveryTask != null)
            {
              deliveryTask.UnAssignDriver(_currentUser.EmployeeId!);
              await _deliveryTaskRepository.UpdateDeliveryTask(deliveryTask);
            }

            // Create Tracking History
            await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, (int)EnumCarrierTrackingStatus.AssignInHouse, "Delivery Note Deleted", _currentUser.EmployeeId!, createdByName));
          }

          await _deliveryNoteRepository.DeleteDeliveryNoteDetail(detail);
        }
      }

      bool isDeleted = await _deliveryNoteRepository.DeleteDeliveryNoteById(deliveryNote);
      if (isDeleted)
      {
        oBaseResponse = new BaseResponseDto()
        {
          Data = deliveryNoteId.Value,
          Message = "Delivery Note Deleted Successfully"
        };
        serviceResultDTO = new ServiceResultDTO(oBaseResponse);
      }
      else
      {
        oBaseResponse = new BaseResponseDto()
        {
          Data = deliveryNoteId.Value,
          Message = "Delivery Note Cannot Be Deleted"
        };
        serviceResultDTO = new ServiceResultDTO(oBaseResponse, false);
      }

      #region publish notification 
      await _mediator.Publish(new
              RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(request),
        Response = JsonConvert.SerializeObject(serviceResultDTO),
        EventName = "ondeleteDeliveryNote",
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
