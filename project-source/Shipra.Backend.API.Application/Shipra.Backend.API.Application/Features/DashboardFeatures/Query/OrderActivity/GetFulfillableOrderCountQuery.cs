using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.OrderActivity;
public class GetFulfillableOrderCountQuery : IRequest<ServiceResultDTO>
{
  public FilterDateClientModel? FilterModel { get; set; }
}
public class GetFulfillableShipmentCountQueryHandler : RequestHandlerBase<GetFulfillableOrderCountQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetFulfillableShipmentCountQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetFulfillableShipmentCountQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetFulfillableOrderCountQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var result = await _dashboardRepository.GetFulfillableOrderCount(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
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
