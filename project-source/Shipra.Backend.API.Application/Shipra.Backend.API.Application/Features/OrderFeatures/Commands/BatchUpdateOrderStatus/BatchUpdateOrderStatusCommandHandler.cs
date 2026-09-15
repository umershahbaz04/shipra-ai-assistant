using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.BatchUpdateOrderStatus;
public class BatchUpdateOrderStatusCommandHandler : RequestHandlerBase<BatchUpdateOrderStatusCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IMediator _mediator;
  private readonly IOrderRepository _orderRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;

  public BatchUpdateOrderStatusCommandHandler(
      IEmployeeRepository employeeRepository, 
      IMediator mediator, 
      IOrderRepository orderRepository, 
      IClientRepository clientRepository, 
      IDeliveryTaskRepository deliveryTaskRepository,
      IDeliveryNoteRepository deliveryNoteRepository,
      IServiceProvider serviceProvider, 
      ILogger<BatchUpdateOrderStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
    _mediator = mediator;
    _orderRepository = orderRepository;
    _clientRepository = clientRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(BatchUpdateOrderStatusCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
      }

      List<Order> orderList = await _orderRepository.GetOrdersWithOrderNos(request.OrderNos, _currentUser.ClientId!);
      List<string> OrderIds = new List<string>();
      var listOfLockOrder = new List<Order>();
      #region get status name
      string? carrierTrackingStatusName = string.Empty;
      var carrierTrackingStatuses = await _orderRepository.GetAllCarrierTrackingStatusesByClientId(_currentUser.ClientId!);
      var objClientTrackingStatus = carrierTrackingStatuses.FirstOrDefault(x => x.CarrierTrackingStatusId == request.CarrierStatusId);
      if (objClientTrackingStatus != null)
      {
        carrierTrackingStatusName = objClientTrackingStatus.TrackingStatus;
      }
      #endregion

      #region Check Revert Logic Flag
      bool isRevertTargetStatus = await IsRevertTargetStatusAsync(request, carrierTrackingStatusName);
      #endregion
      foreach (Order order in orderList)
      {
        if (!order.TrackingLock.GetValueOrDefault(false)) //If Order tracking history is not locked
        {
          if (order.CarrierTrackingStatusId != request.CarrierStatusId) //conditions for validation
          {
            if (isRevertTargetStatus)
            {
               var revertError = await RevertOrderToFreshStateAsync(order);
               // If revertError is not null, it means we cannot revert the order from its Delivery Note (e.g. because it's completed),
               // but we still want to proceed with updating its carrier tracking status below.
            }

            order.UpdateOrderCarrierStatus(request.CarrierStatusId, _currentUser.EmployeeId!,carrierTrackingStatusName);
            var oData = await _orderRepository.UpdateOrder(order);
            if (oData)
            {
              OrderIds.Add(order.OrderId!.Value.ToString());
            }

            #region order history
            if (!order.TrackingLock.GetValueOrDefault(false)) //If Order tracking history is not locked
            {

              string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);
              OrderTrackingHistory orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, request.CarrierStatusId, request.Comments, _currentUser.EmployeeId!, createdByName);
              OrderTrackingHistory createdOrderTrackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderHistory);
            }
            #endregion
          }
        }
        else
        {
          listOfLockOrder.Add(order);
        }
      }
      serviceResult = new ServiceResultDTO(new BaseResponseDto() { Data = OrderIds, Message = NotificationConstants.Success });
      #region publish notification 
      await _mediator.Publish(new
              RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(request),
        Response = JsonConvert.SerializeObject(serviceResult),
        EventName = "onordertrackingstatus",
        IsWebhookEvent = true,
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

  private async Task<bool> IsRevertTargetStatusAsync(BatchUpdateOrderStatusCommand request, string? carrierTrackingStatusName)
  {
    bool isRevertLogicEnabled = false;
    if (request.FromDeliveryTaskScreen == true)
    {
      var completionClientSetting = await _clientRepository.GetGenericSettingByClientIdAsync(_currentUser.ClientId!);
      if (completionClientSetting != null && !string.IsNullOrEmpty(completionClientSetting.SettingConfig))
      {
        var val = UtilityHelper.GetClientSettingValueWithByKey(completionClientSetting.SettingConfig, "others", "revertOrder");
        if (!string.IsNullOrEmpty(val))
        {
          isRevertLogicEnabled = UtilityHelper.GetBoolFromString(val);
        }
      }
    }

    if (isRevertLogicEnabled && !string.IsNullOrEmpty(carrierTrackingStatusName))
    {
       var targetStatuses = new string[] { "duplicate order", "already delivered", "with carrier" };
       if (targetStatuses.Contains(carrierTrackingStatusName.Trim().ToLower()))
       {
          return true;
       }
    }
    return false;
  }

  private async Task<string?> RevertOrderToFreshStateAsync(Order order)
  {
     if (order.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.Delivered)
     {
       return $"Order {order.OrderNo} is already Delivered/Completed, cannot revert.";
     }

     // Check if it belongs to a completed delivery note first
     var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailByOrderId(order.OrderId!);
     if (oDeliveryNoteDetail != null && oDeliveryNoteDetail.DeliveryNoteId != null)
     {
       var oDeliveryNote = await _deliveryNoteRepository.GetDeliveryNoteById(oDeliveryNoteDetail.DeliveryNoteId);
       if (oDeliveryNote != null && (oDeliveryNote.IsCompleted == true || oDeliveryNote.DeliveryNoteStatusId == (int)EnumDeliveryNoteStatusLookup.Completed))
       {
         return $"Order {order.OrderNo} belongs to a completed Delivery Note, cannot revert.";
       }
     }

     // Remove from Delivery Task
     var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(order.OrderId!);
     if (oDeliveryTask != null)
     {
       await _deliveryTaskRepository.DeleteDeliveryTaskById(oDeliveryTask);
     }

     // Remove from Delivery Note Detail
     if (oDeliveryNoteDetail != null)
     {
       var deliveryNoteId = oDeliveryNoteDetail.DeliveryNoteId;
       await _deliveryNoteRepository.DeleteDeliveryNoteDetail(oDeliveryNoteDetail);

       if (deliveryNoteId != null)
       {
         var oDeliveryNote = await _deliveryNoteRepository.GetDeliveryNoteById(deliveryNoteId);
         if (oDeliveryNote != null)
         {
           var remainingDetails = await _deliveryNoteRepository.GetAllDeliveryNoteDetailByDeliveryNoteId(deliveryNoteId);
           if (remainingDetails == null || remainingDetails.Count == 0)
           {
             await _deliveryNoteRepository.DeleteDeliveryNoteById(oDeliveryNote);
           }
           else
           {
             oDeliveryNote.AddToExisting(remainingDetails.Count, _currentUser.EmployeeId);
             await _deliveryNoteRepository.UpdateDeliveryNote(oDeliveryNote);
           }
         }
       }
     }

     // Reset Order to Fresh State
     order.UpdateOrderForUnAssignDeliveryTask(_currentUser.EmployeeId!);
     string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId); 
     var revertMessage = "Order Reverted to Fresh State due to Status Update";
     await _orderRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, (int)EnumCarrierTrackingStatus.UnAssignfromcarrier, revertMessage, _currentUser.EmployeeId!, createdByName));

     // Create Order Note for Revert
     var orderNote = OrderNote.CreateOrderNote(order.OrderId!, revertMessage, _currentUser.EmployeeId!);
     await _orderRepository.CreateOrderNote(orderNote);

     return null;
  }
}
