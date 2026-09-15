using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.CreateDriver;
public class CreateDriverCommandHandler : RequestHandlerBase<CreateDriverCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;
  private readonly IClientRepository _clientRepository;
  private readonly ISharedUserManagement _userManagement;
  private readonly IDriverRepository _driverRepository;
  private readonly IConfigRepository _configRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly ISharedUserManagement _sharedUserManagement;

  public CreateDriverCommandHandler(IPermissionRepository permissionRepository,IClientRepository clientRepository, ISharedUserManagement userManagement, IDriverRepository driverRepository, IConfigRepository configRepository, IEmployeeRepository employeeRepository, ISharedUserManagement sharedUserManagement, IServiceProvider serviceProvider, ILogger<CreateDriverCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
    _clientRepository = clientRepository; 
    _userManagement = userManagement;
    _driverRepository = driverRepository;
    _configRepository = configRepository;
    _employeeRepository = employeeRepository;
    _sharedUserManagement = sharedUserManagement;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateDriverCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
    if (mcconfig is null)
    {
      throw new EntityNotFoundException("Mcconfig", "Cognito Value");
    }
    try
    {
      Client? oClient = await _clientRepository.GetClientById(_currentUser.ClientId!);
      string clientIdentifier = request.UserName!.Substring(0, 3);
      if (oClient!.ClientIdentifier != Int32.Parse(clientIdentifier))
      {
        serviceResult.CreateError("MisMatchIdentifier", new[] {"Identifier not match re login to create user."} );
        return serviceResult;
      }
      var streetAddress = request.AddressLine1! + " " + (!string.IsNullOrEmpty(request.AddressLine2) ? request.AddressLine2 : "");
      var createdUser = await _sharedUserManagement.SignupEmployeeAsync(request.WorkEmail!, request.UserName, request.Mobile, streetAddress, _currentUser.ClientId?.Value.ToString(), request.Password, request.Password, (int)EnumUserRole.Driver, request.IsPreverifyEmail, mcconfig?.Value!);

      if (!string.IsNullOrEmpty(createdUser))
      {
        //create client user
        var cognitoUser = JsonConvert.DeserializeObject<AuthResponseModel<SignupResult>>(createdUser);
        if (cognitoUser!.isSuccess && cognitoUser!.result != null)
        {
          var requestResult = cognitoUser!.result;
          var employeeId = new Guid(cognitoUser.result!.userId!);
          var employeeCode = await _employeeRepository.GetEmployeeNextCode(_currentUser.ClientId!);
          #region get and create user role 
          var oClientUserRole = await _permissionRepository.GetClientUserRoleById(request.UserRoleId.GetValueOrDefault(), _currentUser.ClientId!);
          //get user role by name if not exist then create for existing users
          if (oClientUserRole is null)
          {
            #region Permission lookups
            var allUserRoleList = await _permissionRepository.GetAllUserRole();
            foreach (var oUserRole in allUserRoleList)
            {
              ClientUserRole clientUserRole = ClientUserRole.Create(oUserRole.RoleName, oUserRole.RoleDescription, _currentUser.ClientId!, true);
              await _permissionRepository.CreateClientUserRole(clientUserRole);

              var allGroupPermission = await _permissionRepository.GetAllRolePermissionGroupDefaultsByRoleId(oUserRole.RoleId);
              foreach (var oRolePermissionGroup in allGroupPermission)
              {
                var oClientRolePermissionGroup = ClientRolePermissionGroup.Create(clientUserRole.ClientUserRoleId, oRolePermissionGroup.RolePermissionGroupId, _currentUser.ClientId!);
                await _permissionRepository.CreateClientRolePermissionGroup(oClientRolePermissionGroup);
              }
            }
            #endregion
            oClientUserRole = await _permissionRepository.GetClientUserRoleByName(ApplicationConstants.Driver, _currentUser.ClientId!);
          }
          #endregion

          var oEmployee = Employee.CreateEmployee(new EmployeeId(employeeId), _currentUser.ClientId!, employeeCode, request.EmployeeName, request.GenderId, request.DateOfBirth, request.Phone, request.Mobile, request.WorkEmail, request.EmployeeImage, oClientUserRole!.ClientUserRoleId, (int)EnumUserType.Driver, _currentUser.EmployeeId!);

          var addedEmployee = await _employeeRepository.CreateEmployee(oEmployee);
          if (addedEmployee is not null)
          {
            //username is a employe code
            var driverCode = request.UserName;
            if (driverCode is not null)
            {
              var driver = Driver.CreateDriver(_currentUser.ClientId, driverCode, request.UserName, request.Password, oEmployee.EmployeeId, _currentUser.EmployeeId);
              var createdDriver = await _driverRepository.CreateDriver(driver);
              if (createdDriver is not null)
              {
                BaseResponseDto responseDto = new BaseResponseDto()
                {
                  Data = driver.DriverId!.Value!,
                  Message = NotificationConstants.SavedSuccess
                };
                serviceResult = new ServiceResultDTO(responseDto);
                serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
              }
              else
              {
                serviceResult.CreateErrorResponse(new Exception(NotificationConstants.SavedError));
              }
            }
          }
        }
        else
        {
          serviceResult.Errors = cognitoUser.errors;
          serviceResult.IsSuccess = cognitoUser.isSuccess;
        }
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      var existingkey = _configuration.GetValue<string>("AESKeyForDeleteUser");
      if (!string.IsNullOrEmpty(existingkey))
      {
        var userdeleted = await _userManagement.DeleteUserAsync(request.UserName!, existingkey!, mcconfig?.Value!);
      }
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

