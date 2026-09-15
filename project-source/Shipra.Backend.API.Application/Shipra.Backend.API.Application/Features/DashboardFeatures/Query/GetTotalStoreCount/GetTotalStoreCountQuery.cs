using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.DashboardUserCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetTotalStoreCount;
public class GetTotalStoreCountQuery : IRequest<ServiceResultDTO>
{
  public FilterDateClientModel? FilterModel { get; set; } 
}
public class GetTotalStoreCountQueryHandler : RequestHandlerBase<GetTotalStoreCountQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetTotalStoreCountQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetTotalStoreCountQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetTotalStoreCountQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var result = await _dashboardRepository.GetTotalStoreCount(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
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
