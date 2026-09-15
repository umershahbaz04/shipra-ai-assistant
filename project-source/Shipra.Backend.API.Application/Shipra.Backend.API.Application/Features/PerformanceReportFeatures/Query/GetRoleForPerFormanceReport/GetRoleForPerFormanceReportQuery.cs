using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.PerformanceReportFeatures.Query.GetRoleForPerFormanceReport;

public class GetRoleForPerFormanceReportQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int RoleId { get; set; }
  public string? CountryIds { get; set; }
}
