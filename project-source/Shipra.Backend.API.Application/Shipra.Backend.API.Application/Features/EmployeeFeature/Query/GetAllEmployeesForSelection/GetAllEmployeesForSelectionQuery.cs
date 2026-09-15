using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllEmployeesForSelection;
public class GetAllEmployeesForSelectionQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllEmployeesForSelectionQueryHandler : RequestHandlerBase<GetAllEmployeesForSelectionQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;

  public GetAllEmployeesForSelectionQueryHandler(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<GetAllEmployeesForSelectionQuery> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllEmployeesForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    { 
      var data = await _employeeRepository.GetAllEmployeesForSelection(_currentUser.ClientId);
      var selectedList = data?.Select(x => new { id = x.EmployeeId?.Value.ToString(), text = x.EmployeeName }).ToList(); 
      serviceResult = new ServiceResultDTO(selectedList!);

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
