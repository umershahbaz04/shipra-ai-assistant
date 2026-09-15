using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.BatchOutScanDeliveryTask;
public class CreateDeliveryTaskAndAddToExistingNoteCommandHandler : RequestHandlerBase<CreateDeliveryTaskAndAddToExistingNoteCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;

  public CreateDeliveryTaskAndAddToExistingNoteCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,IOrderRepository orderRepository, IDeliveryNoteRepository deliveryNoteRepository, IDeliveryTaskRepository deliveryTaskRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<CreateDeliveryTaskAndAddToExistingNoteCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _historyRepository = historyRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(CreateDeliveryTaskAndAddToExistingNoteCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var discardTaskList = new List<string>();
      var successTaskList = new List<string>();
      var oDeliveryNote = new DeliveryNote();
      int index = 0;
      var driverId = new DriverId(new Guid(request!.DriverId!));
      var orderList = await _orderRepository.GetOrdersWithOrderNos(request.OrderNos!, _currentUser.ClientId!);
      List<OrderId> orderIds = orderList!.Select(x => x.OrderId!).ToList();
      var unAssignedDriverList = await _deliveryTaskRepository.GetUnAssignedDeliveryTaskListByOrderId(orderIds);
      if (orderList is not null && orderList.Count() > 0)
      {
        #region add to existing 
        //get delivery note for add to existing note
        if (request.AddToExisting.GetValueOrDefault() && !string.IsNullOrEmpty(request.NoteNo))
        {
          //get existing note
          oDeliveryNote = await _deliveryNoteRepository.GetDeliveryNoteByNoteNo(request.NoteNo, _currentUser.ClientIdStr);
          if (oDeliveryNote is null)
          {
            throw new EntityNotFoundException("DeliveryNote ", request!.NoteNo!);
          }

          if (oDeliveryNote.DeliveryNoteStatusId == (int)EnumDeliveryNoteStatusLookup.Completed)
          {
            serviceResult.CreateError("AlreadyCompleted", new string[] { $"You cannot add delivery task to exsting Delivery Note {request.NoteNo} because it's already Completed." });
            return serviceResult;
          }

          driverId = oDeliveryNote?.DriverId;
        }
        #endregion

        foreach (var order in orderList)
        {
          var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(order.OrderId!);
          if (oDeliveryTask is not null && (oDeliveryTask!.DriverId is null || oDeliveryTask.DeliveryTaskStatusId == (int)EnumDeliveryTaskStatusLookup.Unallocated || request.IsTransfer == true))
          {
            // if add to existing 
            oDeliveryTask!.AssignDriver(request.AssigningDate, driverId!, _currentUser.EmployeeId!);
            await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);

            // if we have not delivery note then create new one
            if (!request.AddToExisting.GetValueOrDefault() && string.IsNullOrEmpty(request.NoteNo))
            {
              if (index == 0)
              {
                var activeNoteToday = await _deliveryNoteRepository.GetDriverActiveDeliveryNoteToday(driverId!, _currentUser.ClientId!);
                if (activeNoteToday != null)
                {
                  oDeliveryNote = activeNoteToday;
                  request.AddToExisting = true;
                  request.NoteNo = oDeliveryNote.NoteNo;
                  
                  int totalShipments = oDeliveryNote.ShipmentCount.GetValueOrDefault() + 1;
                  oDeliveryNote.AddToExisting(totalShipments, _currentUser.EmployeeId);
                  await _deliveryNoteRepository.UpdateDeliveryNote(oDeliveryNote);
                }
                else
                {
                  var nextNoteNumber = await _deliveryNoteRepository.GetClientNextNoteNumber(_currentUser.ClientId);
                  // For new notes, we can use the count of successfully processed orders later, or just the orderList.Count
                  oDeliveryNote = DeliveryNote.CreateDeliveryNote(nextNoteNumber, driverId!, orderList.Count, _currentUser!.ClientId!, _currentUser!.EmployeeId!);
                  await _deliveryNoteRepository.CreateDeliveryNote(oDeliveryNote);
                }
              }
            }
            else
            {
              int totalShipments = oDeliveryNote!.ShipmentCount.GetValueOrDefault() + 1;
              oDeliveryNote.AddToExisting(totalShipments, _currentUser.EmployeeId);
              await _deliveryNoteRepository.UpdateDeliveryNote(oDeliveryNote);
            }
            var oDeliveryNoteDetail = await _deliveryNoteRepository.AssignOrderToDeliveryNoteAndCleanup(order.OrderId!, oDeliveryNote!.DeliveryNoteId!, _currentUser.EmployeeId!);

            order.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.OutForDelivery, _currentUser.EmployeeId!, "Out For Delivery");
            await _orderRepository.UpdateOrder(order);

            string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

            await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(order!.OrderId!, (int)EnumCarrierTrackingStatus.OutForDelivery, "Order Assigned to Delivery Note", _currentUser.EmployeeId!, createdByName));
            
            await _historyRepository.CreateOrderNote(OrderNote.CreateOrderNote(order!.OrderId!, $"Delivery Note added: {oDeliveryNote?.NoteNo}", _currentUser!.EmployeeId!));
            successTaskList.Add(order.OrderNo!);

          }
          else
          {
            discardTaskList.Add(order.OrderNo!);
          }
          index++;
        }
        var result = new BaseResponseDto();
        if (discardTaskList.Count() > 0)
        {
          serviceResult!.Errors!["Data"] = new string[] { string.Join(", ", discardTaskList) + "  Driver cannot be assigned to these Order's because Its already assigned" };
          serviceResult.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest);
        }
        else
        {
          result = new BaseResponseDto()
          {
            Data = string.Join(", ", successTaskList),
            Message = "Driver successfully assigned for these Order's."
          };
          serviceResult = new ServiceResultDTO(result);
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Orders not found");
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
