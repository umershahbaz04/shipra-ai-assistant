using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.PerformanceReportFeatures.Query.GetRoleForPerFormanceReport;
using Shipra.Backend.API.Application.Features.PerformanceReportFeatures.Query.GetOrderByRole;

namespace Shipra.Backend.API.Web.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PerformanceReportController : BaseApiController
{
  public PerformanceReportController(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  [HttpPost("GetRoleForPerFormanceReport")]
  public async Task<ActionResult> GetRoleForPerFormanceReport([FromBody] GetRoleForPerFormanceReportQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }

  [HttpPost("GetOrderByRole")]
  public async Task<ActionResult> GetOrderByRole([FromBody] GetOrderByRoleQuery request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    return Ok(response);
  }
}
