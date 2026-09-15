using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrderWithSaleChannel;
public class UpdateOrderWithSaleChannelCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
  public int SaleChannelConfigId { get; set; }
  public bool? IsRemove { get; set; }
}
public class UpdateOrderWithSaleChannelCommandHandler : RequestHandlerBase<UpdateOrderWithSaleChannelCommand, ServiceResultDTO>
{
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly IOrderRepository _orderRepository;

  public UpdateOrderWithSaleChannelCommandHandler(ISaleChannelConfigRepository saleChannelConfigRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<UpdateOrderWithSaleChannelCommandHandler> logger) : base(serviceProvider, logger)
  {
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateOrderWithSaleChannelCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      List<Order> validatedOrder = new();
      var orders = await _orderRepository.GetOrdersWithOrderNos(request.OrderNos, _currentUser.ClientId!);

      if (!request.IsRemove.GetValueOrDefault())
      {
        var oSc = await _saleChannelConfigRepository.GetSaleChannelConfigById(request.SaleChannelConfigId, _currentUser.ClientId!);
        if (oSc is null)
        {
          throw new EntityNotFoundException("SaleChannelConfig", request.SaleChannelConfigId!);
        }
        List<Order> notSameStoreSalePersonOrders = orders.Where(x => x.StoreId != oSc.StoreId).ToList();
        List<Order> lockOrders = orders.Where(x => x.TrackingLock.GetValueOrDefault() == true).ToList();
        if (lockOrders.Count > 0)
        {
          serviceResult.CreateError("OrderLocked", new string[] { $"The following orders are currently locked: {string.Join(",", lockOrders.Select(x => x.OrderNo).ToList())}" });
          return serviceResult;
        }
        if (lockOrders.Count > 0)
        {
          serviceResult.CreateError("OrderLocked", new string[] { $"The following orders lack a salesperson in the same store: {string.Join(",", notSameStoreSalePersonOrders.Select(x => x.OrderNo).ToList())}" });
          return serviceResult;
        }
        validatedOrder = orders.Where(x => x.StoreId == oSc.StoreId).ToList();
      }
      else
      {
        validatedOrder = orders.Where(x => x.SaleChannelConfigId != null && x.SaleChannelConfigId > 0).ToList();
        if (validatedOrder.Count == 0)
        {
          serviceResult.CreateError("NotFound", new string[] { $"No order found with sale channel" });
          return serviceResult;
        }
      }
      foreach (Order order in validatedOrder)
      {
        if (!order.TrackingLock.GetValueOrDefault(false)) //If Order tracking history is not locked
        {
          if (request.IsRemove.GetValueOrDefault())
          {
            order.RemoveSaleChannelConfigId(_currentUser.EmployeeId!);
            var oData = await _orderRepository.UpdateOrder(order);
          }
          else
          {
            if (order.SaleChannelConfigId != request.SaleChannelConfigId) //conditions for validation
            {
              order.UpdateSaleChannelConfigId(request.SaleChannelConfigId, _currentUser.EmployeeId!);
              var oData = await _orderRepository.UpdateOrder(order);
            }
          }
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
public class UpdateOrderWithSaleChannelCommandValidator : AbstractValidator<UpdateOrderWithSaleChannelCommand>
{
  public UpdateOrderWithSaleChannelCommandValidator()
  {
    RuleFor(x => x.OrderNos).NotEmpty().NotNull();
    When(v => !v.IsRemove.GetValueOrDefault(), () =>
    {
      RuleFor(x => x.SaleChannelConfigId).NotEmpty().NotNull().GreaterThan(0); 
    });
  }
}

