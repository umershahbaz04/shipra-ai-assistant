using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.UpdateDeliveryNoteInOperation;
public class UpdateDeliveryNoteInOperationCommandHandler : RequestHandlerBase<UpdateDeliveryNoteInOperationCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IDeliveryNoteRepository _deliveryNote;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IClientRepository _clientRepository;

  public UpdateDeliveryNoteInOperationCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IDeliveryTaskRepository deliveryTaskRepository, IOrderTrackingHistoryRepository historyRepository, IDeliveryNoteRepository deliveryNoteRepository,IDeliveryNoteRepository deliveryNote, IServiceProvider serviceProvider, ILogger<UpdateDeliveryNoteInOperationCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
    _deliveryNote = deliveryNote;
    _deliveryTaskRepository = deliveryTaskRepository;
    _historyRepository = historyRepository;
    _clientRepository = serviceProvider.GetRequiredService<IClientRepository>();


  }
  protected override async Task<ServiceResultDTO> HandleRequest(UpdateDeliveryNoteInOperationCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    BaseResponseDto result = new BaseResponseDto();
    try
    {
      //Get Delivery Note Detail
      var deliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailById(new DeliveryNoteDetailId(new Guid(request.DeliveryNoteDetailId!))!);

      if (deliveryNoteDetail is not null)
      {
        //Get Order Detail
        var oOrder = await _orderRepository.GetOrderById(deliveryNoteDetail!.OrderId!, _currentUser.ClientId!);
        if (oOrder is not null)
        {
          var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(oOrder!.OrderId!);
          
          //Update when Order is not delivered and DND Status is pending
          deliveryNoteDetail.UpdateDeliveryNoteDetailStatus((int)EnumDeliveryNoteDetailStatusLookup.Attempted, _currentUser.EmployeeId!);
          var isUpdatedDeliveryNote = await _deliveryNoteRepository.UpdateDeliveryNoteDetail(deliveryNoteDetail);

          //update delivery task to change the status from Start => Pending and unassign driver as null
          var clientSetting = await _clientRepository.GetGenericSettingByClientIdAsync(_currentUser.ClientId!);
          bool removeDriver = true;
          if (clientSetting != null && !string.IsNullOrEmpty(clientSetting.SettingConfig))
          {
            var val = UtilityHelper.GetClientSettingValueWithByKey(clientSetting.SettingConfig, "others", "removeDriverOnInOperationDebrief");
            if (!string.IsNullOrEmpty(val))
            {
              removeDriver = UtilityHelper.GetBoolFromString(val);
            }
          }

          if (removeDriver)
          {
            oDeliveryTask!.UnAssignDriver(_currentUser.EmployeeId!);
            await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
          }
          else
          {
            oDeliveryTask!.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Unallocated, _currentUser.EmployeeId!);
            await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
          }

          //update order to change the status as InOperation coz order not delivered
          oOrder!.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.InOperation, _currentUser.EmployeeId!, "In Operation");
          await _orderRepository.UpdateOrder(oOrder);

          if (!oOrder.TrackingLock.GetValueOrDefault())
          {
            //Update order tracking history as InOperation
            string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

            var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder!.OrderId!, (int)EnumCarrierTrackingStatus.InOperation, "Order is pending not delivered by driver", _currentUser.EmployeeId!, createdByName));
          }
          result = new BaseResponseDto()
          {
            Message = NotificationConstants.UpdateSuccess
          };
        }
        else
        {
          result = new BaseResponseDto()
          {
            Message = NotificationConstants.UpdateError
          };
        }
        serviceResult = new ServiceResultDTO(result);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
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
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Order not found");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

