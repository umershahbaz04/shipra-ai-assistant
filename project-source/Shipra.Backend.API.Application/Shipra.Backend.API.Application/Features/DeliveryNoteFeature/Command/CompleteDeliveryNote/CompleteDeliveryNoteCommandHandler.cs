using Ardalis.Result;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

using Microsoft.Extensions.DependencyInjection;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CompleteDeliveryNote;
public class CompleteDeliveryNoteCommandHandler : RequestHandlerBase<CompleteDeliveryNoteCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IDriverAccountRepository _driverAccount;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IClientRepository _clientRepository;


  public CompleteDeliveryNoteCommandHandler(IOrderRepository orderRepository, IDeliveryNoteRepository deliveryNoteRepository, IOrderTrackingHistoryRepository historyRepository, IDriverAccountRepository driverAccount, IDeliveryTaskRepository deliveryTaskRepository, IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<CompleteDeliveryNoteCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
    _driverAccount = driverAccount;
    _historyRepository = historyRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _employeeRepository = employeeRepository;
    _clientRepository = serviceProvider.GetRequiredService<IClientRepository>();
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CompleteDeliveryNoteCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    BaseResponseDto result = new BaseResponseDto();
    var successTaskList = new List<string>();
    try
    {
      decimal? totalAmount = 0;
      decimal? totalCash = 0;
      decimal? totalExpense = 0;
      var deliveredOrdersList = new List<Order>();

      var oDeliveryNote = await _deliveryNoteRepository.GetDeliveryNoteById(new DeliveryNoteId(new Guid(request.DeliveryNoteId!)));
      if (oDeliveryNote is not null)
      {
        if (request.DebriefItems != null && request.DebriefItems.Any())
        {
          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

          foreach (var item in request.DebriefItems)
          {
            var detailId = new DeliveryNoteDetailId(new Guid(item.DeliveryNoteDetailId!));
            var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailById(detailId);
            if (oDeliveryNoteDetail != null)
            {
              var oOrder = await _orderRepository.GetOrderById(oDeliveryNoteDetail.OrderId!, _currentUser.ClientId!);
              if (oOrder != null)
              {
                // Update Order Amount
                if (item.Amount.HasValue && item.Amount.Value != oOrder.Amount)
                {
                  oOrder.UpdateOrderAmount(item.Amount.Value, _currentUser.EmployeeId);
                  await _orderRepository.UpdateOrder(oOrder);
                }

                var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(oOrder.OrderId!);

                // Apply status updates
                if (item.StatusId == 1) // InOperation
                {
                  oDeliveryNoteDetail.UpdateDeliveryNoteDetailStatus((int)EnumDeliveryNoteDetailStatusLookup.Attempted, _currentUser.EmployeeId!);
                  await _deliveryNoteRepository.UpdateDeliveryNoteDetail(oDeliveryNoteDetail);

                  if (oDeliveryTask != null)
                  {
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
                      oDeliveryTask.UnAssignDriver(_currentUser.EmployeeId!);
                      await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
                    }
                    else
                    {
                      oDeliveryTask.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Unallocated, _currentUser.EmployeeId!);
                      await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
                    }
                  }

                  oOrder.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.InOperation, _currentUser.EmployeeId!, "In Operation");
                  await _orderRepository.UpdateOrder(oOrder);

                  if (!oOrder.TrackingLock.GetValueOrDefault())
                  {
                    await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder.OrderId!, (int)EnumCarrierTrackingStatus.InOperation, "Order is pending not delivered by driver", _currentUser.EmployeeId!, createdByName));
                  }
                }
                else if (item.StatusId == 2) // Cancelled
                {
                  if (!oOrder.TrackingLock.GetValueOrDefault())
                  {
                    oDeliveryNoteDetail.UpdateDeliveryNoteDetailStatus((int)EnumDeliveryNoteDetailStatusLookup.Completed, _currentUser.EmployeeId!);
                    await _deliveryNoteRepository.UpdateDeliveryNoteDetail(oDeliveryNoteDetail);

                    if (oDeliveryTask != null)
                    {
                      oDeliveryTask.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Completed, _currentUser.EmployeeId!);
                      await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
                    }

                    oOrder.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.Cancelled, _currentUser.EmployeeId!, "Cancelled");
                    await _orderRepository.UpdateOrder(oOrder);

                    await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder.OrderId!, (int)EnumCarrierTrackingStatus.Cancelled, "Order is Cancelled", _currentUser.EmployeeId!, createdByName));
                  }
                }
                else if (item.StatusId == 3) // Delivered
                {
                  oOrder.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.Delivered, _currentUser.EmployeeId!, "Delivered");
                  await _orderRepository.UpdateOrder(oOrder);

                  await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder.OrderId!, (int)EnumCarrierTrackingStatus.Delivered, "Order Status updated by Shipra App For Complete On Debrief", _currentUser.EmployeeId!, createdByName, (int)EnumOrderHistoryType.ShipraApp));

                  if (oDeliveryTask != null)
                  {
                    oDeliveryTask.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Completed, _currentUser.EmployeeId!);
                    await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
                  }

                  oDeliveryNoteDetail.UpdateDeliveryNoteDetailStatusByDriverForCompletion(_currentUser.EmployeeId!);
                  await _deliveryNoteRepository.UpdateDeliveryNoteDetail(oDeliveryNoteDetail);

                  deliveredOrdersList.Add(oOrder);
                  successTaskList.Add(oOrder.OrderNo!);
                }
              }
            }
          }

          // Calculate total COD amount from Delivered orders in DebriefItems
          totalAmount = request.TotalAmount ?? request.DebriefItems
            .Where(x => x.StatusId == 3)
            .Sum(x => x.Amount) ;

          totalExpense = request.ExpenseList!.Sum(x => x.Amount);
          totalCash = totalAmount - totalExpense;

          var driverReceiveable = DriverReceivable.CreateDriverRecivable(oDeliveryNote.DriverId!, totalExpense, totalCash, totalAmount, _currentUser.EmployeeId!, new DeliveryNoteId(new Guid(request.DeliveryNoteId!)));
          await _driverAccount.CreateDriverReceivable(driverReceiveable);

          var completionClientSetting = await _clientRepository.GetGenericSettingByClientIdAsync(_currentUser.ClientId!);
          bool trackingLock = true;
          if (completionClientSetting != null && !string.IsNullOrEmpty(completionClientSetting.SettingConfig))
          {
            var val = UtilityHelper.GetClientSettingValueWithByKey(completionClientSetting.SettingConfig, "others", "trackingLock");
            if (!string.IsNullOrEmpty(val))
            {
              trackingLock = UtilityHelper.GetBoolFromString(val);
            }
          }

          foreach (var oOrder in deliveredOrdersList)
          {
              if (oOrder.PaymentMethodId == (int)EnumPaymentMethod.COD)
              {
                  oOrder.MarkStatusPaidAndLockOrder(_currentUser.EmployeeId, trackingLock);
                  await _orderRepository.UpdateOrder(oOrder);
                  await _historyRepository.CreateOrderNote(OrderNote.CreateOrderNote(oOrder.OrderId!, "Driver receivable mark paid against " + driverReceiveable.DriverReceivableNo, _currentUser.EmployeeId!));
              }
          }

          if (request.ExpenseList != null && request.ExpenseList.Count > 0)
          {
            foreach (var item in request.ExpenseList)
            {
              await _driverAccount.CreateExpense(Expense.CreateExpense(_currentUser.ClientId!, oDeliveryNote.DriverId, driverReceiveable.DriverReceivableId!, oDeliveryNote.DeliveryNoteId!, item.Amount, item.ExpenseDate, item.ExpenseCategoryId, item.Detail, _currentUser.EmployeeId!));
            }
          }

          if (oDeliveryNote.DeliveryNoteStatusId == (int)EnumDeliveryNoteStatusLookup.InProgress)
          {
            oDeliveryNote.UpdateDeliveryNoteStatusForCompletion(_currentUser.EmployeeId!);
            await _deliveryNoteRepository.UpdateDeliveryNote(oDeliveryNote);
          }

          result = new BaseResponseDto()
          {
            Data = string.Join(", ", successTaskList),
            Message = "Delivery note successfully completed."
          };
          return serviceResult = new ServiceResultDTO(result, true);
        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Debrief items list is empty");
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Delivery Note not found");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
