using FluentValidation;
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

namespace Shipra.Backend.API.Application.Features.CarrierPendingForReturnReportFeatures.Command.CreateCarrierPendingForReturn;
public class CreateCarrierPendingForReturnCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}
public class CreateCarrierPendingForReturnCommandHandler : RequestHandlerBase<CreateCarrierPendingForReturnCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;

  public CreateCarrierPendingForReturnCommandHandler(IMediator mediator, IEmployeeRepository employeeRepository,IOrderRepository orderRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<CreateCarrierPendingForReturnCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _historyRepository = historyRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateCarrierPendingForReturnCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var orders = await _orderRepository.GetOrdersWithOrderNos(request.OrderNos, _currentUser.ClientId!);
      var lockedOrders = orders.Where(x => x.TrackingLock.GetValueOrDefault() == true).ToList();

      foreach (var oOrder in orders)
      {
        if (!oOrder!.TrackingLock.GetValueOrDefault())
        {
          //update order carrier tracking status as Pending For Return
          oOrder.UpdateOrderCarrierStatus((int)EnumCarrierTrackingStatus.PendingForReturn, _currentUser.EmployeeId!, "Pending For Return");
          await _orderRepository.UpdateOrder(oOrder);

          //create order tracking history of this order for Pending For Return  
          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId); 
          var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder!.OrderId!, (int)EnumCarrierTrackingStatus.PendingForReturn, "Carrier Order is set as Pending For Return ", _currentUser.EmployeeId!, createdByName));
        }
      }
      if (lockedOrders.Count > 0)
      {
        serviceResult.CreateError("LockedOrders", new string[] { "Following order's already locked. " + string.Join(", ", lockedOrders.Select(x => x.OrderNo)) });
      }
      else
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = "",Message = "Action perform successfully!"});
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
public class CreateCarrierPendingForReturnCommandValidator : AbstractValidator<CreateCarrierPendingForReturnCommand>
{
  public CreateCarrierPendingForReturnCommandValidator()
  {
    RuleFor(x => x.OrderNos).NotEmpty().NotNull();
  }
}
