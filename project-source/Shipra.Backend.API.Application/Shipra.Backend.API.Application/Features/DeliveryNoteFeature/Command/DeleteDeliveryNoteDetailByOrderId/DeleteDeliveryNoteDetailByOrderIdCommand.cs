using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteDeliveryNoteDetailByOrderId;

public class DeleteDeliveryNoteDetailByOrderIdCommand : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
}

public class DeleteDeliveryNoteDetailByOrderIdCommandHandler : RequestHandlerBase<DeleteDeliveryNoteDetailByOrderIdCommand, ServiceResultDTO>
{
  private readonly IDeliveryNoteRepository _deliveryNoteRepository;
  private readonly IDeliveryTaskRepository _deliveryTaskRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;

  public DeleteDeliveryNoteDetailByOrderIdCommandHandler(
    IDeliveryNoteRepository deliveryNoteRepository,
    IDeliveryTaskRepository deliveryTaskRepository,
    IOrderRepository orderRepository,
    IEmployeeRepository employeeRepository,
    IOrderTrackingHistoryRepository historyRepository,
    IServiceProvider serviceProvider,
    ILogger<DeleteDeliveryNoteDetailByOrderIdCommandHandler> logger
  ) : base(serviceProvider, logger)
  {
    _deliveryNoteRepository = deliveryNoteRepository;
    _deliveryTaskRepository = deliveryTaskRepository;
    _orderRepository = orderRepository;
    _employeeRepository = employeeRepository;
    _historyRepository = historyRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteDeliveryNoteDetailByOrderIdCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      if (string.IsNullOrEmpty(request.OrderId))
      {
        serviceResult.CreateError("OrderId", new string[] { "OrderId is required" });
        return serviceResult;
      }

      var orderId = new OrderId(new Guid(request.OrderId));
      var oOrder = await _orderRepository.GetOrderById(orderId, _currentUser!.ClientId!);
      var oDeliveryTask = await _deliveryTaskRepository.GetDeliveryTaskByOrderId(orderId);
      var oDeliveryNoteDetail = await _deliveryNoteRepository.GetDeliveryNoteDetailByOrderId(orderId);

      if (oDeliveryNoteDetail != null)
      {
        var deliveryNoteId = oDeliveryNoteDetail.DeliveryNoteId;

        // 1. Delete DeliveryNoteDetail
        await _deliveryNoteRepository.DeleteDeliveryNoteDetail(oDeliveryNoteDetail);

        // 2. Check remaining details count on the note
        if (deliveryNoteId != null)
        {
          var oDeliveryNote = await _deliveryNoteRepository.GetDeliveryNoteById(deliveryNoteId);
          if (oDeliveryNote != null)
          {
            var remainingDetails = await _deliveryNoteRepository.GetAllDeliveryNoteDetailByDeliveryNoteId(deliveryNoteId);
            if (remainingDetails == null || remainingDetails.Count == 0)
            {
              // Delete entire delivery note if zero details remain
              await _deliveryNoteRepository.DeleteDeliveryNoteById(oDeliveryNote);
            }
            else
            {
              int newCount = remainingDetails.Count;
              oDeliveryNote.AddToExisting(newCount, _currentUser.EmployeeId);
              await _deliveryNoteRepository.UpdateDeliveryNote(oDeliveryNote);
            }
          }
        }

        // 3. Unassign Driver and reset DeliveryTask
        if (oDeliveryTask != null)
        {
          oDeliveryTask.UnAssignDriver(_currentUser.EmployeeId!);
          await _deliveryTaskRepository.UpdateDeliveryTask(oDeliveryTask);
        }

        // 4. Update Order carrier tracking status back to In Operation
        if (oOrder != null)
        {
          oOrder.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.AssignInHouse, _currentUser.EmployeeId!, "Assign InHouse");
          await _orderRepository.UpdateOrder(oOrder);

          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);
          await _historyRepository.CreateOrderTrackingHistory(
            OrderTrackingHistory.CreateOrderTrackingHistory(
              orderId,
              (int)EnumCarrierTrackingStatus.AssignInHouse,
              "Order removed from Delivery Note",
              _currentUser.EmployeeId!,
              createdByName
            )
          );

          await _historyRepository.CreateOrderNote(
            OrderNote.CreateOrderNote(
              orderId,
              "Order removed from Delivery Note",
              _currentUser!.EmployeeId!
            )
          );
        }

        var result = new BaseResponseDto()
        {
          Data = request.OrderId,
          Message = "Order removed from Delivery Note successfully"
        };
        serviceResult = new ServiceResultDTO(result);
        serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      }
      else
      {
        var result = new BaseResponseDto()
        {
          Data = request.OrderId,
          Message = "Delivery Note detail not found for this order"
        };
        serviceResult = new ServiceResultDTO(result);
        serviceResult.CreateErrorResponse(HttpStatusCode.NotFound);
      }

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
