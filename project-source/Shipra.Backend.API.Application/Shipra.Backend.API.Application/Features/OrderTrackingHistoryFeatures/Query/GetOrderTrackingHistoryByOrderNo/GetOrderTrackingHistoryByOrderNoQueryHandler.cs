using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.RefreshCarrierStatus;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Query.GetOrderTrackingHistoryByOrderId;
public class GetOrderTrackingHistoryByOrderNoQueryHandler : RequestHandlerBase<GetOrderTrackingHistoryByOrderNoQuery, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IOrderRepository _orderRepository;
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  public GetOrderTrackingHistoryByOrderNoQueryHandler(IMediator mediator, IOrderRepository orderRepository, IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<GetOrderTrackingHistoryByOrderNoQueryHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _orderRepository = orderRepository;
    _historyRepository = historyRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderTrackingHistoryByOrderNoQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      if (request.IsCarrierRefreshTracking.GetValueOrDefault())
      {
        try
        {
          var order = await _orderRepository.GetOrderByOrderNo(request!.OrderNo!.ToString(), _currentUser.ClientId!);
          if (order is not null)
          {
            RefreshCarrierStatusCommand refreshCarrierStatusCommand = new RefreshCarrierStatusCommand()
            {
              OrderId = order.OrderId!.Value.ToString(),
              OrderNo = order.OrderNo,
            };
            var da = await _mediator.Send(refreshCarrierStatusCommand);
          }
        }
        catch (Exception)
        {
        }
      }
      var response = await _historyRepository.GetOrderTrackingHistoryByOrderNo(request!.OrderNo!.ToString(), _currentUser.ClientId!.Value.ToString());

      serviceResult = new ServiceResultDTO(response!);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
