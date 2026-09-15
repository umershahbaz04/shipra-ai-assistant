using FluentValidation;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateCustomerEmailAddress;
public class UpdateCustomerEmailAddressCommandHandler : RequestHandlerBase<UpdateCustomerEmailAddressCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public UpdateCustomerEmailAddressCommandHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<UpdateCustomerEmailAddressCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateCustomerEmailAddressCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      OrderAddress? orderAddress = null;
      if (request.OrderAddressId.HasValue && request.OrderAddressId.Value > 0)
      {
        orderAddress = await _orderRepository.GetOrderAddressById(request.OrderAddressId.Value);
      }
      else if (!string.IsNullOrEmpty(request.OrderNo))
      {
        var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo, _currentUser.ClientId!);
        if (order != null && order.OrderAddressId.HasValue && order.OrderAddressId.Value > 0)
        {
          orderAddress = await _orderRepository.GetOrderAddressById(order.OrderAddressId.Value);
        }
      }

      if (orderAddress is null)
      {
        throw new EntityNotFoundException("orderAddress / OrderNo", request.OrderAddressId.HasValue ? request.OrderAddressId.Value.ToString() : request.OrderNo ?? "");
      }
      orderAddress.UpdateCustomerEmail(request.Email, _currentUser.EmployeeId, request.CustomerName, request.Mobile1, request.Mobile2);

      var IsUpdate = await _orderRepository.UpdateCustomerEmail(orderAddress);
      if (IsUpdate)
      {
        return new ServiceResultDTO(new BaseResponseDto() { Data = orderAddress.OrderAddressId.ToString(), Message = NotificationConstants.Success });
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
public class UpdateCustomerEmailAddressCommandValidator : AbstractValidator<UpdateCustomerEmailAddressCommand>
{
  public UpdateCustomerEmailAddressCommandValidator()
  {
  }
}
