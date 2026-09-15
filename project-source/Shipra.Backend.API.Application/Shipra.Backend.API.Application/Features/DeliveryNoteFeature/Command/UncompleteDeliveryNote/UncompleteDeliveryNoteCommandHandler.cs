using System.Net;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.UncompleteDeliveryNote;
public class UncompleteDeliveryNoteCommandHandler : RequestHandlerBase<UncompleteDeliveryNoteCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IDriverAccountRepository _driverAccountRepository;

  public UncompleteDeliveryNoteCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IDeliveryTaskRepository deliveryTaskRepository, IOrderTrackingHistoryRepository historyRepository, IDeliveryNoteRepository deliveryNoteRepository, IDriverAccountRepository driverAccountRepository, IServiceProvider serviceProvider, ILogger<UncompleteDeliveryNoteCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _historyRepository = historyRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
    _driverAccountRepository = driverAccountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UncompleteDeliveryNoteCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    var result = new BaseResponseDto();
    try
    {
      var deliveryNoteId = new DeliveryNoteId(new Guid(request.DeliveryNoteId!));
      var oDeliveryNote = await _deliveryNoteRepository.GetDeliveryNoteById(deliveryNoteId);

      if (oDeliveryNote != null)
      {
        if (oDeliveryNote.DeliveryNoteStatusId != (int)EnumDeliveryNoteStatusLookup.Completed)
        {
          serviceResult.IsSuccess = false;
          serviceResult.StatusCode = (int)HttpStatusCode.BadRequest;
          if (serviceResult.Errors == null) serviceResult.Errors = new Dictionary<string, string[]>();
          serviceResult.Errors.Add("InvalidState", new string[] { "Delivery note is not in a Completed state." });
          return serviceResult;
        }

        string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

        // 1. Restore Delivery Note Status to InProgress
        oDeliveryNote.DeliveryNoteStatusId = (int)EnumDeliveryNoteStatusLookup.InProgress;
        oDeliveryNote.IsCompleted = false;
        oDeliveryNote.CompletedOn = null;
        oDeliveryNote.UpdatedBy = _currentUser.EmployeeId;
        oDeliveryNote.UpdatedOn = DateTime.UtcNow;
        await _deliveryNoteRepository.UpdateDeliveryNote(oDeliveryNote);

        // 2. Fetch and delete DriverReceivables and Expenses associated with this Delivery Note
        if (oDeliveryNote.DriverId != null)
        {
          var receivablesResult = await _driverAccountRepository.GetAllIDriverReceivables(null, null, 0, 100, "", 0, "ASC", _currentUser.ClientIdStr ?? "", new List<Guid> { oDeliveryNote.DriverId.Value });
          if (receivablesResult != null)
          {
            var receivablesList = receivablesResult.list as System.Collections.IEnumerable;
            if (receivablesList != null)
            {
              foreach (var rec in receivablesList)
              {
                if (rec != null)
                {
                  var dict = (System.Collections.Generic.IDictionary<string, object>)rec;
                  if (dict.ContainsKey("DeliveryNoteId") && dict["DeliveryNoteId"] != null && oDeliveryNote.DeliveryNoteId != null)
                  {
                    var noteGuid = (Guid)dict["DeliveryNoteId"];
                    if (noteGuid == oDeliveryNote.DeliveryNoteId.Value && dict.ContainsKey("DriverReceivableId") && dict["DriverReceivableId"] != null)
                    {
                      var dbRecId = new DriverReceivableId((Guid)dict["DriverReceivableId"]);
                      var fullRecObj = await _driverAccountRepository.GetDriverReceivableById(dbRecId);
                      if (fullRecObj != null)
                      {
                        await _driverAccountRepository.DeleteDriverReceivable(fullRecObj);
                        await _driverAccountRepository.DeleteExpensesByDeliveryNoteId(oDeliveryNote.DeliveryNoteId);
                      }
                    }
                  }
                }
              }
            }
          }
        }

        // 3. Revert all details associated with this Delivery Note
        var details = await _deliveryNoteRepository.GetAllDeliveryNoteDetailByDeliveryNoteId(oDeliveryNote.DeliveryNoteId);
        if (details != null && details.Any())
        {
          foreach (var oDeliveryNoteDetail in details)
          {
            var orderId = oDeliveryNoteDetail.OrderId!;
            var oOrder = await _orderRepository.GetOrderById(orderId, _currentUser.ClientId!);
            var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(orderId);

            if (oOrder != null)
            {
              // Reset order paid status to unpaid using core domain model method
              oOrder.MarkStatusUnPaidAndUnLockOrder(oOrder.PaymentMethodId, _currentUser.EmployeeId);

              // Update order carrier tracking status back to OutForDelivery
              oOrder.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.OutForDelivery, _currentUser.EmployeeId!, "Out For Delivery");
              await _orderRepository.UpdateOrder(oOrder);

              // Update order tracking history
              await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(orderId, (int)EnumCarrierTrackingStatus.OutForDelivery, "Order reverted to Out For Delivery due to Debrief Uncompletion", _currentUser.EmployeeId!, createdByName, (int)EnumOrderHistoryType.ShipraApp));
            }

            if (oDeliveryTask != null)
            {
              // Update delivery task status back to Allocated
              oDeliveryTask.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Allocated, _currentUser.EmployeeId!);
              await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
            }

            // Update delivery note detail status back to Pending
            oDeliveryNoteDetail.UpdateDeliveryNoteDetailStatusForRevert(_currentUser.EmployeeId!);
            await _deliveryNoteRepository.UpdateDeliveryNoteDetail(oDeliveryNoteDetail);
          }
        }

        result = new BaseResponseDto()
        {
          Data = oDeliveryNote.DeliveryNoteId?.Value,
          Message = "Delivery note successfully reverted to In Operation state."
        };
        serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      }
      else
      {
        result = new BaseResponseDto()
        {
          Message = "Delivery note not found"
        };
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
      }

      serviceResult = new ServiceResultDTO(result);

      #region publish notification 
      await _mediator.Publish(new RequestActivityLog
      {
        Request = JsonConvert.SerializeObject(request),
        Response = JsonConvert.SerializeObject(serviceResult),
        EventName = "onuncompletedebrief",
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
