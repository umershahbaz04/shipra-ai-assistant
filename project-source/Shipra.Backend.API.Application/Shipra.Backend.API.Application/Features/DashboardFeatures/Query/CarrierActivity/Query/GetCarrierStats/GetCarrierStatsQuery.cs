using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.CarrierActivity.Query.GetCarrierStats;
public class GetCarrierStatsQuery : IRequest<ServiceResultDTO>
{
  public FilterDateClientModel? FilterModel { get; set; }
}
public class GetCarrierStatsQueryHandler : RequestHandlerBase<GetCarrierStatsQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetCarrierStatsQueryHandler(IDashboardRepository dashboardRepository ,IServiceProvider serviceProvider, ILogger<GetCarrierStatsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCarrierStatsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var result = await _dashboardRepository.GetCarrierStatesStackedChart(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(result);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
