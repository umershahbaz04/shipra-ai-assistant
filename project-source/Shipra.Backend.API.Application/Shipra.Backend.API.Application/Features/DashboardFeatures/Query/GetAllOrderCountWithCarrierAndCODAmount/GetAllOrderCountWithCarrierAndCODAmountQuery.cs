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

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Query.GetAllOrderCountWithCarrierAndCODAmount;
public class GetAllOrderCountWithCarrierAndCODAmountQuery : IRequest<ServiceResultDTO>
{
  public FilterDateClientModel? FilterModel { get; set; }
}
public class GetAllOrderCountWithCarrierAndCODAmountQueryHandler : RequestHandlerBase<GetAllOrderCountWithCarrierAndCODAmountQuery, ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetAllOrderCountWithCarrierAndCODAmountQueryHandler(IDashboardRepository dashboardRepository, IServiceProvider serviceProvider, ILogger<GetAllOrderCountWithCarrierAndCODAmountQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrderCountWithCarrierAndCODAmountQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      var data = await _dashboardRepository.GetAllOrderCountWithCarrierAndCODAmount(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
      serviceResult = new ServiceResultDTO(data);
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
