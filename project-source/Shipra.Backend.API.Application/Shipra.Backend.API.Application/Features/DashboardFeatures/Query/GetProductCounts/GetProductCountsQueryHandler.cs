using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetProductCounts;
public class GetProductCountsQueryHandler : RequestHandlerBase<DashboardGetProductCountsQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetProductCountsQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetProductCountsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DashboardGetProductCountsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;

      var data = await _dashboardRepository.GetAllItemCount(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

