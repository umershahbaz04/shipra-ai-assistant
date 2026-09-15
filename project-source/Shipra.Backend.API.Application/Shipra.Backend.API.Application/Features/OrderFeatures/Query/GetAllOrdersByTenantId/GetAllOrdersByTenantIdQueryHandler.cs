using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersByTenantId;

public class GetAllOrdersByTenantIdQueryHandler : RequestHandlerBase<GetAllOrdersByTenantIdQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllOrdersByTenantIdQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllOrdersByTenantIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrdersByTenantIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      dynamic response = await _orderRepository.GetAllOrdersByTenantId(request.TenantId, request.DateFrom, request.DateTo);
      serviceResult = new ServiceResultDTO(response);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
