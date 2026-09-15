using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Finance.Query.GetTotalStockValue;
public class GetTotalStockValueQueryHandler : RequestHandlerBase<GetTotalStockValueQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetTotalStockValueQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetTotalStockValueQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetTotalStockValueQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var result = await _dashboardRepository.GetTotalStockValue(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
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
