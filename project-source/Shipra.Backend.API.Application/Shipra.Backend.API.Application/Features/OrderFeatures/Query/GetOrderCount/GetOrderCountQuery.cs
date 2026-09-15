using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.Response;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderCount;
public class GetOrderCountQuery : IRequest<ServiceResultDTO>
{
}
public class GetOrderCountQueryHandler : RequestHandlerBase<GetOrderCountQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetOrderCountQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetOrderCountQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderCountQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var orderCount = await _orderRepository.GetOrderCount(_currentUser.ClientId!);
      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = orderCount, Message = "Order Count: " + orderCount });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
