using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderItemByOrderNo;
public class GetOrderItemByOrderNoQueryHandler : RequestHandlerBase<GetOrderItemByOrderNoQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetOrderItemByOrderNoQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetOrderItemByOrderNoQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderItemByOrderNoQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var orderList = await _orderRepository.GetOrderInfoByOrderNo(request.OrderNo!, _currentUser!.ClientIdStr!);
      if (Enumerable.Count(orderList) <= 0)
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Order not found");
      }
      var orderId = orderList[0].OrderId.ToString();
      var orderItemsList = await _orderRepository.GetOrderItemsInfoByOrderId(orderId, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(orderItemsList);
      serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

