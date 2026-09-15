using Amazon.Runtime.Internal.Transform;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.Util;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.EmployeeUseCase;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAirWayBillWithDynamicTemplate;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.CreateSaleChannelConfig;
using Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Query.GetSalePersonConfig;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.CreateEmployee;
public class CreateEmployeeCommand : IRequest<ServiceResultDTO>
{
  public string? EmployeeName { get; set; }
  public int? GenderId { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public string? EmployeeImage { get; set; }
  public string? UserName { get; set; }
  public int? StoreId { get; set; }
  public string? Mobile { get; set; }
  public string? WorkEmail { get; set; }
  public string? Password { get; set; }
  public string? Phone { get; set; }
  public bool? IsPreverifyEmail { get; set; }
  public int? UserRoleId { get; set; }
  public int? EmployeeTypeId { get; set; }
  public AddressRequestDTO? EmployeeAddress { get; set; }
}
public class CreateEmployeeCommandHandler : RequestHandlerBase<CreateEmployeeCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly IDriverRepository _driverRepository;
  private readonly IPermissionRepository _permissionRepository;
  private readonly IClientRepository _clientRepository; 
  private readonly IEmployeeRepository _employeeRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;

  public CreateEmployeeCommandHandler(IMediator mediator, IDriverRepository driverRepository, IPermissionRepository permissionRepository, IClientRepository clientRepository, IEmployeeRepository employeeRepository, ICountryRepository countryRepository, IConfigRepository configRepository, ISharedUserManagement userManagement, IServiceProvider serviceProvider, ILogger<CreateEmployeeCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _driverRepository = driverRepository;
    _permissionRepository = permissionRepository;
    _clientRepository = clientRepository; 
    _employeeRepository = employeeRepository;
    _countryRepository = countryRepository;
    _configRepository = configRepository;
    _userManagement = userManagement;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateEmployeeCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
    if (mcconfig is null)
    {
      throw new EntityNotFoundException("Mcconfig", "Cognito Value");
    }
    try
    {
      Client? oClient = await _clientRepository.GetClientById(_currentUser.ClientId!);
      //string clientIdentifier = request.UserName!.Substring(0, 3);
      //if (oClient!.ClientIdentifier != Int32.Parse(clientIdentifier))
      //{
      //  serviceResult.CreateError("MisMatchIdentifier", new[] { "Identifier not match re login to create user" });
      //  return serviceResult;
      //}
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
        oClientUserRole = await _permissionRepository.GetClientUserRoleById(oClientUserRole!.ClientUserRoleId, _currentUser.ClientId!);
      }
      #endregion

      var objAddress = request.EmployeeAddress!;
      string fullAddress = await _countryRepository.GetFullAddress(objAddress.StreetAddress, objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId);


      #region if sale person then (we already did work inside another command)
      if (oClientUserRole!.RoleName == ApplicationConstants.SalePerson)
      {
        if (request.StoreId is null || request.StoreId == 0)
        {
          request.StoreId = oClient!.DefaultStoreId;
        }
        CreateSaleChannelConfigCommand scRequest = new CreateSaleChannelConfigCommand();
        scRequest.IsActive = true;
        scRequest.IsAllowToDisplayInSaleChannel = true;
        scRequest.StoreId = request.StoreId.GetValueOrDefault();
        scRequest.SaleChannelLookupId = (int)EnumSaleChannelLookup.SalePerson;
        scRequest.SaleChannelName = request.UserName;

        scRequest.InputParameters = new Dictionary<string, string>
                            {
                                { "salePersonName", request.EmployeeName! },
                                { "phoneNo", request.Mobile! },
                                { "email", request.WorkEmail! },
                                { "password", request.Password! },
                                { "username", request.UserName! }, 

                                { "countryId", objAddress.CountryId!.GetValueOrDefault().ToString() },
                                { "cityId",  objAddress.CityId!.GetValueOrDefault().ToString() },
                                { "areaId",  objAddress.AreaId!.GetValueOrDefault().ToString() },
                                { "streetAddress",  objAddress.StreetAddress! },
                                { "streetAddress2",  objAddress.StreetAddress2! },
                                { "houseNo",  objAddress.HouseNo! },
                                { "buildingName",  objAddress.BuildingName! },
                                { "landmark",  objAddress.Landmark! },
                                { "provinceId",  objAddress.ProvinceId!.GetValueOrDefault().ToString() },
                                { "pinCodeId",  objAddress.PinCodeId!.GetValueOrDefault().ToString() },
                                { "stateId",  objAddress.StateId!.GetValueOrDefault().ToString() },
                                { "fullAddress", fullAddress },
                                { "zip",  objAddress.Zip! },
                                { "dateOfBirth", request.DateOfBirth?.ToString() ?? string.Empty }, // Add dateOfBirth or null
                                { "genderId", request.GenderId?.ToString() ?? "3".ToString() }, // Add dateOfBirth or null
                                { "employeeTypeId", request.EmployeeTypeId?.ToString() ?? ((int)EnumEmployeeType.Shipper!).ToString() }, // Add dateOfBirth or null

                                { "isPreverifyEmail",  request.IsPreverifyEmail!.GetValueOrDefault().ToString() },
                                { "latitude",  objAddress.Latitude!.GetValueOrDefault().ToString() },
                                { "longitude" ,  objAddress!.Longitude!.GetValueOrDefault().ToString() }
                            };


        serviceResult = await _mediator.Send(scRequest);

      }
      #endregion
      #region for all other user 
      else
      {
        #region MyRegion 
        var streetAddress = objAddress!.StreetAddress! + " " + (!string.IsNullOrEmpty(objAddress.StreetAddress2) ? objAddress.StreetAddress2 : "");
        var createdUser = await _userManagement.SignupEmployeeAsync(request!.WorkEmail!, request.UserName, request.Mobile, streetAddress, _currentUser.ClientId?.Value.ToString(), request.Password, request.Password, oClientUserRole!.ClientUserRoleId, request.IsPreverifyEmail, mcconfig?.Value!);

        if (!string.IsNullOrEmpty(createdUser))
        {
          
          //create client user
          var cognitoUser = JsonConvert.DeserializeObject<AuthResponseModel<SignupResult>>(createdUser);
          if (cognitoUser!.isSuccess && cognitoUser!.result != null)
          {
            var requestResult = cognitoUser!.result;
            var employeeId = new Guid(cognitoUser.result!.userId!);
            var employeeCode = request.UserName!;
            var employee = Employee.CreateEmployee(new EmployeeId(employeeId), _currentUser.ClientId!, employeeCode, request.EmployeeName, request.GenderId, request.DateOfBirth, request.Phone, request.Mobile, request.WorkEmail, request.EmployeeImage, oClientUserRole!.ClientUserRoleId, request.EmployeeTypeId, new EmployeeId(_currentUser.ClientId!.Value));

            var addedEmployee = await _employeeRepository.CreateEmployee(employee);
            if (addedEmployee is not null)
            {
              #region create employee address
              EmployeeAddress oOrderAddress = EmployeeAddress.CreateEmployeeAddress(addedEmployee.EmployeeId, objAddress!.CountryId, objAddress!.CityId, objAddress!.AreaId, objAddress!.StreetAddress, objAddress!.StreetAddress2, objAddress!.HouseNo, objAddress!.BuildingName, objAddress!.Landmark, objAddress!.ProvinceId, objAddress!.PinCodeId, objAddress.StateId, fullAddress, objAddress!.Zip, (int)EnumAddressType.Shipping, objAddress!.Latitude, objAddress!.Longitude);

              bool isAdded = await _employeeRepository.CreateEmployeeAddress(oOrderAddress);

              #endregion
              if (oClientUserRole.RoleName == ApplicationConstants.Driver)
              {
                //username is a employe code
                var driverCode = request.UserName;
                if (driverCode is not null)
                {
                  var driver = Driver.CreateDriver(_currentUser.ClientId, driverCode, request.UserName, request.Password, employee.EmployeeId, _currentUser.EmployeeId);
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
            serviceResult = new ServiceResultDTO(new BaseResponseDto
            {
              Data = employee.EmployeeId?.Value.ToString(),
              Message = NotificationConstants.SavedSuccess
            });

            return serviceResult;
          }
          else
          {
            serviceResult.IsSuccess = cognitoUser!.isSuccess;
            serviceResult.Errors = cognitoUser!.errors;
          }
        }
        #endregion
      }
      #endregion
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
public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
  public CreateEmployeeCommandValidator()
  {
    RuleFor(v => v.EmployeeName).NotNull().NotEmpty();
    RuleFor(v => v.Mobile).NotNull().NotEmpty();
    RuleFor(v => v.UserRoleId).NotNull().NotEmpty().GreaterThan(0);

    //client user information

    RuleFor(v => v.WorkEmail).NotEmpty().WithMessage("Email address is required").EmailAddress().WithMessage("A valid email is required");
    RuleFor(v => v.UserName).NotNull().NotEmpty();
    RuleFor(v => v.Password).NotNull().NotEmpty()
      .MinimumLength(8).WithMessage("Your password length must be at least 8.")
      .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
      .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
      .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
      .Matches(@"[^A-Za-z0-9]+").WithMessage("Your password must contain special chracter.");
  }
}
