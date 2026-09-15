using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderPaymentStatus;
public class UpdateOrderPaymentStatusCommand : IRequest<ServiceResultDTO>
{
  public string? orderNos { get; set; }
  public int? PaymentStatusId { get; set; }
  public int CarrierStatusId { get; set; }
  public string? Comments { get; set; }
}
public class UpdateOrderPaymentStatusCommandHandler : RequestHandlerBase<UpdateOrderPaymentStatusCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  public UpdateOrderPaymentStatusCommandHandler(IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<UpdateOrderPaymentStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateOrderPaymentStatusCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<Order> orders = await _orderRepository.GetOrdersWithOrderNos(request.orderNos, _currentUser.ClientId!);

      foreach (Order order in orders)
      {
        if (!order.TrackingLock.GetValueOrDefault(false))
        {
          if (order.PaymentMethodId == (int)EnumPaymentMethod.COD) //if ordertype is COD
          {
            order.UpdateOrderPaymentStatus(request.PaymentStatusId!, _currentUser.EmployeeId!);
            serviceResult.IsSuccess = await _orderRepository.UpdateOrderPaymentStatus(orders);
          }
        }
      }
      if (serviceResult.IsSuccess)
      {
        #region order history

        foreach (Order order in orders)
        {
          if (!order.TrackingLock.GetValueOrDefault(false))
          {
            string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

            OrderTrackingHistory orderTrackingHistory = OrderTrackingHistory.CreateOrderTrackingHistory(order.OrderId!, request.CarrierStatusId, request.Comments, _currentUser.EmployeeId!, createdByName);
            OrderTrackingHistory createdOrderTackingHistory = await _orderRepository.CreateOrderTrackingHistory(orderTrackingHistory);
          }
        }
        #endregion
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
