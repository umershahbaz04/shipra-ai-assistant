using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.CheckMobileNoDuplicate;
public class CheckMobileNoDuplicateQueryHandler : RequestHandlerBase<CheckMobileNoDuplicateQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public CheckMobileNoDuplicateQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CheckMobileNoDuplicateQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CheckMobileNoDuplicateQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      string clientId = _currentUser?.ClientIdStr ?? "";
      var result = await _orderRepository.CheckMobileNoDuplicate(request.MobileNo, clientId);
      serviceResult = new ServiceResultDTO(result);
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
