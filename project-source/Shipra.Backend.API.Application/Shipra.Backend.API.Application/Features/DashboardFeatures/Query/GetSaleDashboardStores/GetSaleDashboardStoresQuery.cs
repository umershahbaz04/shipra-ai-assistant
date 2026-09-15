using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetSaleDashboardStores;

public class GetSaleDashboardStoresQuery : IRequest<ServiceResultDTO>
{
}

public class GetSaleDashboardStoresQueryHandler : RequestHandlerBase<GetSaleDashboardStoresQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetSaleDashboardStoresQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetSaleDashboardStoresQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSaleDashboardStoresQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _dashboardRepository.GetSaleDashboardStores(_currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(result);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
