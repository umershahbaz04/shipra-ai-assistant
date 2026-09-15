using System;
using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;

namespace Shipra.Backend.API.Application.Features.PerformanceReportFeatures.Query.GetOrderByRole;

public class GetOrderByRoleQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public string? EmployeeId { get; set; }
  public int RoleId { get; set; }
  public string? CountryIds { get; set; }
}
