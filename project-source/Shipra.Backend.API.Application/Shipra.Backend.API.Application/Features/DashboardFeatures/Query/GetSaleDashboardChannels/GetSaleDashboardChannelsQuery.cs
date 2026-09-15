using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetSaleDashboardChannels;

public class GetSaleDashboardChannelsQuery : IRequest<ServiceResultDTO>
{
  public int StoreId { get; set; }
  public DateTime? CreatedFrom { get; set; }
  public DateTime? CreatedTo { get; set; }
}

public class GetSaleDashboardChannelsQueryHandler : RequestHandlerBase<GetSaleDashboardChannelsQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetSaleDashboardChannelsQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetSaleDashboardChannelsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSaleDashboardChannelsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _dashboardRepository.GetSaleDashboardChannels(request.StoreId, _currentUser.ClientIdStr!, request.CreatedFrom, request.CreatedTo);
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
