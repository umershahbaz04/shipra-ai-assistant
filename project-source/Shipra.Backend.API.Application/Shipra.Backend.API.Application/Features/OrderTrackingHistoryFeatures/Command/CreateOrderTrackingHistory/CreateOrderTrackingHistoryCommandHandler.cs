using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Command.CreateOrderTrackingHistory;
public class CreateOrderTrackingHistoryCommandHandler : RequestHandlerBase<CreateOrderTrackingHistoryCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IOrderRepository _orderRepository;
  public CreateOrderTrackingHistoryCommandHandler(IEmployeeRepository employeeRepository,IOrderTrackingHistoryRepository historyRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CreateOrderTrackingHistoryCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
    _historyRepository = historyRepository;
    _orderRepository = orderRepository;
  }


  protected override async Task<ServiceResultDTO> HandleRequest(CreateOrderTrackingHistoryCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      Order? result = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (!result!.TrackingLock.GetValueOrDefault(false))
      {
        string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

        var response = await _historyRepository.CreateOrderTrackingHistory(OrderTrackingHistory.CreateOrderTrackingHistory(result!.OrderId!, null, request?.TrackingStatusComments, _currentUser.EmployeeId!, createdByName));
        if (response.OrderTrackingHistoryId != null)
        {
          serviceResult = new ServiceResultDTO(new BaseResponseDto()
          {
            Data = response!.OrderTrackingHistoryId,
            Message = NotificationConstants.Success,
          });
          return serviceResult;
        }
        else
        {
          serviceResult.CreateErrorResponse(new Exception(NotificationConstants.InvalidResponse));
          return serviceResult;
        }
      }
      else
      {
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
