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

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetEmployeeProfileById;
public class GetEmployeeProfileByIdQuery : IRequest<ServiceResultDTO>
{
  public string? EmployeeId { get; set; }
}
public class GetEmployeeProfileByIdQueryHandler : RequestHandlerBase<GetEmployeeProfileByIdQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;

  public GetEmployeeProfileByIdQueryHandler(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<GetEmployeeProfileByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetEmployeeProfileByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var data = await _employeeRepository.GetEmployeeProfileById(request.EmployeeId!, _currentUser.ClientIdStr!); 
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
