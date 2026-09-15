using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.DeleteDriver;
public class DeleteDriverCommandHandler : RequestHandlerBase<DeleteDriverCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IDriverRepository _driverRepository;

  public DeleteDriverCommandHandler(IEmployeeRepository employeeRepository,IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<DeleteDriverCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteDriverCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      Guid guidID;
      var hasGUID = Guid.TryParse(request!.DriverId!, out guidID);
      if (!hasGUID)
      {
        throw new InvalidIdTypeException(request!.DriverId!);
      }
      var driverId = new DriverId(new Guid(request!.DriverId!));
      var driver = await _driverRepository.GetDriverById(driverId);
      if (driver is not null)
      {
        driver.EnableDisableDriver(request.IsActive,_currentUser.EmployeeId!); 
        var isDriverDeleted = await _driverRepository.DeleteDriver(driver);

        var oEmployee = await _employeeRepository.GetEmployeeById(driver.EmployeeId!, _currentUser.ClientId!);  
        //delete employee against driver
        oEmployee.EnableDisableEmployee(request.IsActive,_currentUser.EmployeeId!);
        var delEmploye = await _employeeRepository.UpdateEmployee(oEmployee);
      }
      else
      {
        serviceResult.Errors?.Add("NotFound", new[] {"Driver not found!!!"});
        serviceResult.StatusCode = 404;
        serviceResult.IsSuccess = false;
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
