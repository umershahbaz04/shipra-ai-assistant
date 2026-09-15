using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.PerformanceReportFeatures.Query.GetRoleForPerFormanceReport;

public class GetRoleForPerFormanceReportQueryHandler : RequestHandlerBase<GetRoleForPerFormanceReportQuery, ServiceResultDTO>
{
  private readonly IPerformanceReportRepository _performanceReportRepository;

  public GetRoleForPerFormanceReportQueryHandler(
    IPerformanceReportRepository performanceReportRepository,
    IServiceProvider serviceProvider,
    ILogger<GetRoleForPerFormanceReportQueryHandler> logger) : base(serviceProvider, logger)
  {
    _performanceReportRepository = performanceReportRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetRoleForPerFormanceReportQuery request, CancellationToken cancellationToken)
  {
    var filter = request?.FilterModel!;
    var serviceResult = new ServiceResultDTO();
    try
    {
      var result = await _performanceReportRepository.GetRoleForPerformanceReport(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, request!.RoleId, _currentUser.ClientIdStr!, request?.CountryIds);
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
