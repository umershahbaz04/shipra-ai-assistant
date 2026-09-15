using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetAllItemsGroupCount;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetAllProductVariantsCount;
public class GetAllProductVariantsCountQueryHandler : RequestHandlerBase<GetAllProductVariantsCountQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetAllProductVariantsCountQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetAllProductVariantsCountQuery> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllProductVariantsCountQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var allItemGroupCount = await _dashboardRepository.GetAllProductVariantsCount(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(allItemGroupCount);
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

