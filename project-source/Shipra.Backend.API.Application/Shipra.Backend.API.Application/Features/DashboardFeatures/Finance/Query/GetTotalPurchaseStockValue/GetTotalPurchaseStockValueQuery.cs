using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MediatR.Wrappers;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DashboardFeatures.Finance.Query.GetTotalPurchaseStockValue;
public class GetTotalPurchaseStockValueQuery : IRequest<ServiceResultDTO>
{
  public FilterDateClientModel? FilterModel { get; set; } 
}
public class GetTotalPurchaseStockValueQueryHandler : RequestHandlerBase<GetTotalPurchaseStockValueQuery,ServiceResultDTO>
{
  private readonly IDashboardRepository _dashboardRepository;

  public GetTotalPurchaseStockValueQueryHandler(IDashboardRepository dashboardRepository,IServiceProvider serviceProvider, ILogger<GetTotalPurchaseStockValueQueryHandler> logger) : base(serviceProvider, logger)
  {
    _dashboardRepository = dashboardRepository;
  }
    
  protected override async Task<ServiceResultDTO> HandleRequest(GetTotalPurchaseStockValueQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var result = await _dashboardRepository.GetTotalPurchaseStockValue(filter.CreatedFrom, filter.CreatedTo, _currentUser.ClientIdStr!);
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
