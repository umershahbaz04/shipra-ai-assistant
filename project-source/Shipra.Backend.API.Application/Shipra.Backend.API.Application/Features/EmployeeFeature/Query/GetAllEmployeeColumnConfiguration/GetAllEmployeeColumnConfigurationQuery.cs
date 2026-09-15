using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllEmployeeColumnConfiguration;
public class GetAllEmployeeColumnConfigurationQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllEmployeeColumnConfigurationQueryHandler : RequestHandlerBase<GetAllEmployeeColumnConfigurationQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;

  public GetAllEmployeeColumnConfigurationQueryHandler(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<GetAllEmployeeColumnConfigurationQueryHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllEmployeeColumnConfigurationQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var oList = await _employeeRepository.GetAllEmployeeColumnConfiguration(_currentUser.ClientId); 
      var employeeList = oList.Where(x => x.EmployeeId == _currentUser.EmployeeId).ToList();

      serviceResult = new ServiceResultDTO(employeeList); 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
