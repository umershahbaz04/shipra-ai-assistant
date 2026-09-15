using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Query.GetAllEmployees;
public class GetAllEmployeesQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int? UserRoleId { get; set; }
}
public class GetAllEmployeesQueryHandler : RequestHandlerBase<GetAllEmployeesQuery, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;

  public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<GetAllEmployeesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllEmployeesQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var data = await _employeeRepository.GetAllEmployees(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,request.UserRoleId, _currentUser.ClientIdStr!);

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
