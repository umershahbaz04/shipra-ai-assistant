using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.EmployeeUseCase;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.UpdateEmployee;
public class UpdateEmployeeCommand : IRequest<ServiceResultDTO>
{
  public string? EmployeeId { get; set; }
  public string? EmployeeName { get; set; }
  public string? EmployeeImage { get; set; }
  public string? WorkEmail { get; set; }
  public string? Mobile { get; set; }
  public string? Password { get; set; }
  public string? Phone { get; set; }
  public int? GenderId { get; set; }
  public int? StoreId { get; set; }
  public int? UserRoleId { get; set; }
  public int? EmployeeTypeId { get; set; }
  public DateTime? DateOfBirth { get; set; }
  public UpdateEmployeeAddressRequestModel? Address { get; set; }

}
public class UpdateEmployeeCommandHandler : RequestHandlerBase<UpdateEmployeeCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;

  public UpdateEmployeeCommandHandler(IPermissionRepository permissionRepository, IEmployeeRepository employeeRepository, ICountryRepository countryRepository, IConfigRepository configRepository, ISharedUserManagement userManagement, IServiceProvider serviceProvider, ILogger<UpdateEmployeeCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
    _employeeRepository = employeeRepository;
    _countryRepository = countryRepository;
    _configRepository = configRepository;
    _userManagement = userManagement;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateEmployeeCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var employeeId = new EmployeeId(new Guid(request.EmployeeId!));

      var employee = await _employeeRepository.GetEmployeeById(employeeId!, _currentUser.ClientId!);

      if (employee is null)
      {
        throw new EntityNotFoundException("Employee", request.EmployeeId!);
      }
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
      #region MyRegion  
      if (oClientUserRole is not null)
      {
        employee.UpdateEmployee(request.EmployeeName, request.Phone, request.Mobile, request.WorkEmail, request.EmployeeImage, request.GenderId.GetValueOrDefault(3), oClientUserRole!.ClientUserRoleId, request.DateOfBirth, _currentUser.EmployeeId,request.EmployeeTypeId.GetValueOrDefault());

        var addedEmployee = await _employeeRepository.UpdateEmployee(employee);
        #region address
        var employeeAddress = await _employeeRepository.GetEmployeeAddressById(employeeId!); 
        if (employeeAddress is null)
        {
          throw new EntityNotFoundException("Employee Address", request.EmployeeId!);
        }

        var objAddress = request.Address!;

        string storeFullAddress = await _countryRepository.GetFullAddress(objAddress.StreetAddress, objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId);
        employeeAddress!.UpdateEmployeeAddress(objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.StreetAddress, objAddress.StreetAddress2, objAddress.HouseNo, objAddress.BuildingName, objAddress.Landmark, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId, storeFullAddress, objAddress.Zip, (int)EnumAddressType.Shipping, objAddress.Latitude, objAddress.Longitude);

        bool addeda = await _employeeRepository.UpdateEmployeeAddress(employeeAddress); 
        #endregion

        #region sync to cognito
        var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
        if (mcconfig?.Value != null && !string.IsNullOrEmpty(employee.EmployeeCode))
        {
          await _userManagement.UpdateEmployeeCognitoAsync(
            employee.EmployeeCode,
            request.EmployeeName,
            oClientUserRole!.ClientUserRoleId,
            _currentUser.ClientId?.Value.ToString(),
            mcconfig.Value
          );
        }
        #endregion

        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = employee.EmployeeId?.Value.ToString(), Message = NotificationConstants.Success });
      }
   

      return serviceResult;

      #endregion
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
  public UpdateEmployeeCommandValidator()
  {
    RuleFor(v => v.EmployeeName).NotNull().NotEmpty();
    RuleFor(v => v.Mobile).NotNull().NotEmpty();
    RuleFor(v => v.UserRoleId).NotNull().NotEmpty().GreaterThan(0);
  }
}
