using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.PerformanceReportFeatures.Query.GetOrderByRole;

public class GetOrderByRoleQueryHandler : RequestHandlerBase<GetOrderByRoleQuery, ServiceResultDTO>
{
  private readonly IPerformanceReportRepository _performanceReportRepository;

  public GetOrderByRoleQueryHandler(
    IPerformanceReportRepository performanceReportRepository,
    IServiceProvider serviceProvider,
    ILogger<GetOrderByRoleQueryHandler> logger) : base(serviceProvider, logger)
  {
    _performanceReportRepository = performanceReportRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderByRoleQuery request, CancellationToken cancellationToken)
  {
    var filter = request?.FilterModel ?? new FilterModelDTO();
    var serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _performanceReportRepository.GetOrderByRole(
        filter.CreatedFrom, 
        filter.CreatedTo, 
        filter.Start, 
        filter.Length, 
        filter.Search, 
        filter.SortCol, 
        filter.SortDir, 
        request?.EmployeeId ?? string.Empty, 
        request?.RoleId ?? 0, 
        _currentUser.ClientIdStr!,
        request?.CountryIds);
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
