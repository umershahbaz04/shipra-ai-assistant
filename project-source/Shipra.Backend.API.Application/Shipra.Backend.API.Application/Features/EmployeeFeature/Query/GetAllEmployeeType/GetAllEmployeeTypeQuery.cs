using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllEmployeeType;
public class GetAllEmployeeTypeQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllEmployeeTypeQueryHandler : RequestHandlerBase<GetAllEmployeeTypeQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  public GetAllEmployeeTypeQueryHandler(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<GetAllEmployeeTypeQueryHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllEmployeeTypeQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var employeeTypes = await _employeeRepository.GetAllEmployeeTypesAsync();
      serviceResult = new ServiceResultDTO(employeeTypes);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
