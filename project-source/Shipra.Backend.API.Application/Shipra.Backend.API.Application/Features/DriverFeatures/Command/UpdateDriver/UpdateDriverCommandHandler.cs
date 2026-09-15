using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.UpdateDriver;
public class UpdateDriverCommandHandler : RequestHandlerBase<UpdateDriverCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;
  private readonly IDriverRepository _driverRepository;
  private readonly IEmployeeRepository _employeeRepository;

  public UpdateDriverCommandHandler(IConfigRepository configRepository, ISharedUserManagement userManagement, IDriverRepository driverRepository, IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<UpdateDriverCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _userManagement = userManagement;
    _driverRepository = driverRepository;
    _employeeRepository = employeeRepository;
  }

  protected override Task<ServiceResultDTO> HandleRequest(UpdateDriverCommand request, CancellationToken cancellationToken)
  {
    //ServiceResultDTO serviceResult = new ServiceResultDTO();
    //try
    //{
    //  var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey);
    //  if (mcconfig is null)
    //  {
    //    throw new EntityNotFoundException("Mcconfig", "Cognito Value");
    //  }

    //  var createdUser = await _userManagement.UpdateDriverAsync(request.WorkEmail!, request.PhoneNo, _currentUser!.Id!, request.AppPassword, _currentUser.AccessToken, mcconfig?.Value!);
    //  if (!string.IsNullOrEmpty(createdUser))
    //  {
    //    //create client user
    //    var cognitoUser = JsonConvert.DeserializeObject<AuthResponseModel<ConfirmUserModel>>(createdUser);
    //    if (cognitoUser!.isSuccess && cognitoUser!.result != null)
    //    {
    //      Guid guidID;
    //      var hasGUID = Guid.TryParse(request!.DriverId!, out guidID);
    //      if (!hasGUID)
    //      {
    //        throw new InvalidIdTypeException(request!.DriverId!);
    //      }
    //      var driverId = new DriverId(new Guid(request!.DriverId!));
    //      var hasGUID2 = Guid.TryParse(request!.DriverId!, out guidID);
    //      if (!hasGUID2)
    //      {
    //        throw new InvalidIdTypeException(request!.EmployeeId!);
    //      }
    //      var employeeId = new EmployeeId(new Guid(request!.EmployeeId!));
    //      var employee = await _employeeRepository.GetEmployeeById(employeeId, _currentUser!.ClientId!);
    //      var driver = await _driverRepository.GetDriverById(driverId);
    //      if (driver == null)
    //      {
    //        throw new EntityNotFoundException("Driver", driverId);
    //      }
    //      else if (employee is null)
    //      {
    //        throw new EntityNotFoundException("Employee", employeeId);
    //      }
    //      else
    //      {
    //        driver.UpdateDriver(request.AppUsername, request.AppPassword, request.EmployeeId, _currentUser.UserId);
    //        var isDriverUpdated = await _driverRepository.UpdateDriver(driver!);
    //        employee.UpdateEmployee()
    //      }
    //    }
    throw new NotImplementedException();
  }
}
