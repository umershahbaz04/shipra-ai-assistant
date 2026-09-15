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
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.CarrierActivity.Query;
public class GetCarrierActivityWithDetailQuery : IRequest<ServiceResultDTO>
{
  public FilterDateClientModel? FilterModel { get; set; }
}
public class GetCarrierActivityWithDetailQueryHandler : RequestHandlerBase<GetCarrierActivityWithDetailQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetCarrierActivityWithDetailQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetCarrierActivityWithDetailQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCarrierActivityWithDetailQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var result = await _dashboardRepository.GetCarrierActivityWithDetail(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
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
