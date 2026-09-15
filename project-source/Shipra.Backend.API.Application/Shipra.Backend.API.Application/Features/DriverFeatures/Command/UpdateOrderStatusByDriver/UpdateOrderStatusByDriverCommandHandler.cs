using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.MediatorNotification;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.UpdateOrderStatusByDriver;
public class UpdateOrderStatusByDriverCommandHandler : RequestHandlerBase<UpdateOrderStatusByDriverCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IDriverRepository _driverRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;

  public UpdateOrderStatusByDriverCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IDriverRepository driverRepository, IDeliveryTaskRepository deliveryTaskRepository, IDeliveryNoteRepository deliveryNoteRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<UpdateOrderStatusByDriverCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _driverRepository = driverRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _deliveryNoteRepository = deliveryNoteRepository;
    _historyRepository = historyRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(UpdateOrderStatusByDriverCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailById(new DeliveryNoteDetailId(new Guid(request.DeliveryNoteDetailId!)));
      if (oDeliveryNoteDetail is not null)
      {
        var oOrder = await _orderRepository.GetOrderById(oDeliveryNoteDetail.OrderId!, _currentUser.ClientId!);
        if (oOrder is not null)
        {
          //if (oOrder.TrackingLock.GetValueOrDefault(false))
          //{
          //  throw new ShipraApplicationException(System.Net.HttpStatusCode.Conflict, "Order is locked, cannot update status");
          //}
          var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(oDeliveryNoteDetail.OrderId!);
          if (oDeliveryTask is not null)
          {
            if (oDeliveryNoteDetail is not null && oDeliveryNoteDetail.DeliveryNoteDetailStatusId == (int)EnumDeliveryNoteDetailStatusLookup.Pending
               && oDeliveryTask is not null && oDeliveryTask!.DeliveryTaskStatusId == (int)EnumDeliveryTaskStatusLookup.Allocated)
            {
              var oDriver = await _driverRepository.GetDriverById(oDeliveryTask!.DriverId!);
              if (oDriver is not null)
              {
                #region get status name
                string? carrierTrackingStatusName = string.Empty;
                var carrierTrackingStatuses = await _orderRepository.GetAllCarrierTrackingStatusesByClientId(_currentUser.ClientId!);
                var objClientTrackingStatus = carrierTrackingStatuses.FirstOrDefault(x => x.CarrierTrackingStatusId == request.CarrierTrackingStatusId);
                if (objClientTrackingStatus != null)
                {
                  carrierTrackingStatusName = objClientTrackingStatus.TrackingStatus;
                }
                #endregion

                oOrder.UpdateOrderCarrierStatus(request.CarrierTrackingStatusId, oDriver!.EmployeeId!,carrierTrackingStatusName);
                await _orderRepository.UpdateOrder(oOrder);

                //Update order tracking history as delivered
                string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

                var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder!.OrderId!, request.CarrierTrackingStatusId, "Order Status updated by driver", _currentUser.EmployeeId!,createdByName, (int)EnumOrderHistoryType.DriverApp));

                if (request.CarrierTrackingStatusId == (int)EnumCarrierTrackingStatus.Delivered)
                {
                  oDeliveryTask.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Completed, oDriver!.EmployeeId!);
                  await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);

                  oDeliveryNoteDetail!.UpdateDeliveryNoteDetailStatusByDriverForCompletion(oDriver!.EmployeeId!);
                  await _deliveryNoteRepository.UpdateDeliveryNoteDetail(oDeliveryNoteDetail);
                }
                else
                {
                  oDeliveryTask.UpdateDeliveryTaskStatus((int)EnumDeliveryTaskStatusLookup.Attempted, oDriver!.EmployeeId!);
                  await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);

                  oDeliveryNoteDetail!.UpdateDeliveryNoteDetailStatus((int)EnumDeliveryNoteDetailStatusLookup.Attempted, oDriver!.EmployeeId!);
                  await _deliveryNoteRepository.UpdateDeliveryNoteDetail(oDeliveryNoteDetail);
                }
                var result = new BaseResponseDto()
                {
                  Data = oOrder.OrderNo,
                  Message = "Order completed successfully"
                };
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
                return serviceResult = new ServiceResultDTO(result);
              }
              else
              {
                throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Driver not found");
              }
            }
            else
            {
              throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Order already completed");
            }
          }
          else
          {
            throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Delivery Task already completed");
          }
        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Order not found");
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Delivery Note Detail not found");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
