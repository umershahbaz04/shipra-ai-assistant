using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.UpdateOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using FluentValidation;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.AssignCarrierManualAdmin;

// ✅ Command
public class AssignCarrierManualAdminCommand : IRequest<ServiceResultDTO>
{
  public int? ActiveCarrierId { get; set; }
  public int? CarrierId { get; set; }
  public List<TrackingWithOrderNo>? OrderNoList { get; set; }

}
public class TrackingWithOrderNo
{
  public string? OrderNo { get; set; }
  public string? CarrierTrackingNo { get; set; }

}

public class AssignCarrierManualAdminCommandHandler : RequestHandlerBase<AssignCarrierManualAdminCommand, ServiceResultDTO>
{ 
  private readonly IOrderRepository _orderRepository;
  private readonly IEmployeeRepository _employeeRepository;

  public AssignCarrierManualAdminCommandHandler(IEmployeeRepository employeeRepository, ICountryRepository countryRepository, IProductRepository productRepository, IOrderRepository orderRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, IMetaFieldRepository metaFieldRepository, ILogger<UpdateOrderCommandHandler> logger) : base(serviceProvider, logger)
  { 
    _orderRepository = orderRepository;
    _employeeRepository= employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AssignCarrierManualAdminCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    try
    {
      foreach (var order in request.OrderNoList!)
      {
        var objOrder = await _orderRepository.GetOrderByOrderNo(order.OrderNo!, _currentUser.ClientId!);
        if (objOrder is not null && objOrder.CarrierId.GetValueOrDefault() == 0)
        {
          objOrder.UpdateCarrierManual(request.CarrierId, request.ActiveCarrierId, order.CarrierTrackingNo);
          var updateResult = await _orderRepository.UpdateOrder(objOrder);

          var objHistories = await _orderRepository.GetOrderTrackingHistoryByOrderId(objOrder.OrderId!);
          if (objHistories is not null && objHistories.Any())
          {
            var filteredHistory = objHistories.Where(x => x.CarrierTrackingStatusId != (int)EnumCarrierTrackingStatus.OrderPlaced).ToList();

            if (filteredHistory.Any())
            {
              await _orderRepository.DeleteOrderTrackingHistory(filteredHistory);
            }
          }
          string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);


          var orderHistory = OrderTrackingHistory.CreateOrderTrackingHistory(
              objOrder.OrderId!,
              (int)EnumCarrierTrackingStatus.AssignedTocarrier,
              createdByName,
              _currentUser.EmployeeId!,
              createdByName
          );

          await _orderRepository.CreateOrderTrackingHistory(orderHistory);

        }
      }

      response = new ServiceResultDTO(new BaseResponseDto { Message = "Update successfully" });
      return response;
    }
    catch (Exception ex)
    {
      //_logger.LogError(ex, "Error updating order carrier for OrderNo: {OrderNo}, ClientId: {ClientId}", request.OrderNo, _currentUser.ClientId!);
      response.CreateErrorResponse(ex);
      return response;
    }
  }


  public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
  {
    public UpdateOrderCommandValidator()
    {

    }
  }

}
