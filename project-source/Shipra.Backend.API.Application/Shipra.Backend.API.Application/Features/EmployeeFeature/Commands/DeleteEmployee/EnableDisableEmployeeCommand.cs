using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.DeleteEmployee;
public class EnableDisableEmployeeCommand : IRequest<ServiceResultDTO>
{
  public string? EmployeeId { get; set; } 
}
public class DeleteEmployeeCommandHandler : RequestHandlerBase<EnableDisableEmployeeCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;
  private readonly IDriverRepository _driverRepository;
  private readonly IEmployeeRepository _employeeRepository;

  public DeleteEmployeeCommandHandler(IPermissionRepository permissionRepository, IDriverRepository driverRepository, IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<DeleteEmployeeCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
    _driverRepository = driverRepository;
    _employeeRepository = employeeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(EnableDisableEmployeeCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      EmployeeId employeeId = new EmployeeId(new Guid(request.EmployeeId!));
      var target = await _employeeRepository.GetEmployeeByIdForEdit(employeeId!, _currentUser.ClientId!);

      if (target is null)
      {
        throw new EntityNotFoundException("Employee", request.EmployeeId!);
      }

      bool? IsActive = target.Active.GetValueOrDefault() ? false : true;
      //delete employee
      target.EnableDisableEmployee(IsActive,_currentUser.EmployeeId!);

      var ddt = await _employeeRepository.DeleteEmployee(target);

      var oClientUserRole = await _permissionRepository.GetClientUserRoleById(target!.ClientUserRoleId.GetValueOrDefault(), _currentUser.ClientId!);
      if (oClientUserRole is not null)
      {
        if (oClientUserRole.RoleName == ApplicationConstants.Driver)
        {
          #region delete driver
          Driver? driver = await _driverRepository.GetDriverByEmployeeId(employeeId);
          if (driver is not null)
          {
            driver.EnableDisableDriver(IsActive, _currentUser.EmployeeId!);
            var isDriverDeleted = await _driverRepository.DeleteDriver(driver);
          }
          #endregion
        }

      }
      if (ddt)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
           Message = "Deleted successfully"
        });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {

      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class EnableDisableEmployeeCommandValidator : AbstractValidator<EnableDisableEmployeeCommand>
{
  public EnableDisableEmployeeCommandValidator()
  {
    RuleFor(x => x.EmployeeId).NotNull().NotEmpty(); 
  } 
}

