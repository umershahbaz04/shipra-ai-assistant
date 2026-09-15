using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetMapApiKey;
public class GetMapApiKeyQuery : IRequest<ServiceResultDTO>
{
}
public class GetMapApiKeyQueryHandler : RequestHandlerBase<GetMapApiKeyQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;
  public GetMapApiKeyQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetMapApiKeyQuery> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetMapApiKeyQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      string? mapApiKey = await _orderRepository.GetMapApiKey();

      if (mapApiKey is not null)
      {
        serviceResult = new ServiceResultDTO(mapApiKey);
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
