using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.OrderActivity;
public class GetToBePackedCountQuery : IRequest<ServiceResultDTO>
{
  public FilterDateClientModel? FilterModel { get; set; }
}
public class GetToBePackedCountQueryHandler : RequestHandlerBase<GetToBePackedCountQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetToBePackedCountQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetToBeShippedCountQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetToBePackedCountQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;

      var tobePackedCount = await _dashboardRepository.GetToBePackedCount(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(tobePackedCount);
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

