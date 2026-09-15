using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Finance.Query.GetTotalUncollected;
public class GetTotalUncollectedQueryHandler : RequestHandlerBase<GetTotalUncollectedQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetTotalUncollectedQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetTotalUncollectedQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetTotalUncollectedQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var result = await _dashboardRepository.GetTotalUncollected(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
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
