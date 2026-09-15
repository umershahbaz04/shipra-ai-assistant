using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderAmount;
public class UpdateOrderAmountCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
  public decimal? Amount { get; set; }
}
public class UpdateOrderAmountCommandHandler : RequestHandlerBase<UpdateOrderAmountCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;

  public UpdateOrderAmountCommandHandler(IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<UpdateOrderAmountCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _historyRepository = historyRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateOrderAmountCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var oOrder = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (oOrder is null)
      {
        throw new EntityNotFoundException("Order", request.OrderNo!);
      }
      if (!oOrder.TrackingLock.GetValueOrDefault())
      {
        oOrder.UpdateOrderAmount(request.Amount, _currentUser.EmployeeId);

        var IsUpdate = await _orderRepository.UpdateOrder(oOrder);

        //Update order tracking history as delivered

        string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

        var oOrderTrackingHistory = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(oOrder!.OrderId!, (int)EnumCarrierTrackingStatus.Delivered, "Order Amount is updated", _currentUser.EmployeeId!, createdByName));

        if (IsUpdate)
        {
          return new ServiceResultDTO(new BaseResponseDto() { Data = oOrder.OrderId!.Value.ToString(), Message = NotificationConstants.Success });
        }
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
