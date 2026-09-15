using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.GetNextEmployeeUserName;
public class GetNextEmployeeUserNameQuery : IRequest<ServiceResultDTO>
{
  public string? Name { get; set; }
}
public class GetNextEmployeeUserNameQueryHandler : RequestHandlerBase<GetNextEmployeeUserNameQuery, ServiceResultDTO>
{ 
  private readonly IEmployeeRepository _employeeRepository;

  public GetNextEmployeeUserNameQueryHandler(IEmployeeRepository employeeRepository,IServiceProvider serviceProvider, ILogger<GetNextEmployeeUserNameQueryHandler> logger) : base(serviceProvider, logger)
  { 
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetNextEmployeeUserNameQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
     
    try
    { 
      var employeeCode = await _employeeRepository.GetEmployeeNextCodeByUserName(request.Name,_currentUser.ClientId!,_currentUser.EmployeeId!);

      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Data = employeeCode,
        Message = ""
      }); 
      return serviceResult;
    }
    catch (Exception ex)
    { 
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  } 
}
public class GetNextEmployeeUserNameQueryValidator : AbstractValidator<GetNextEmployeeUserNameQuery>
{
  public GetNextEmployeeUserNameQueryValidator()
  {
    RuleFor(x => x.Name).NotNull().NotEmpty();
  }
}
