using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Security.Service.IManagers;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateClient;
public class UpdateClientCommandHandler : RequestHandlerBase<UpdateClientCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;
  private readonly IClientRepository _clientRepository;
  private readonly IKeyGeneratorManager _keyGeneratorManager;
  public UpdateClientCommandHandler(IEmployeeRepository employeeRepository,ICountryRepository countryRepository,IConfigRepository configRepository, ISharedUserManagement userManagement, IClientRepository clientRepository, IServiceProvider serviceProvider, IKeyGeneratorManager keyGeneratorManager, ILogger<UpdateClientCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
    _countryRepository = countryRepository;
    _configRepository = configRepository;
    _userManagement = userManagement;
    _clientRepository = clientRepository;
    _keyGeneratorManager = keyGeneratorManager;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(UpdateClientCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }

      //var createdUser = await _userManagement.UpdateUser(request.Email!, request.Mobile, request.StreetAddress, request.Password, _currentUser.AccessToken, mcconfig?.Value!);


      //if (!string.IsNullOrEmpty(createdUser))
      //{
      //  //create client user
      //  var cognitoUser = JsonConvert.DeserializeObject<AuthResponseModel<ConfirmUserModel>>(createdUser);
      //  if (cognitoUser!.isSuccess && cognitoUser!.result != null)
      //  {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);

      if (client == null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientId!.Value!.ToString());
      }

      if (client is not null)
      {
        if (string.IsNullOrEmpty(client.PublicKey))
        {
          client.PublicKey = Utility.GetPublicKey();
        }
        if (string.IsNullOrEmpty(client.SecretKey))
        {
          client.SecretKey = Utility.GetSecretKey();
        }
        if (string.IsNullOrEmpty(client!.EncryptedKey))
        {
          var keyModel = new KeyModel()
          {
            ClientId = _currentUser.ClientId,
            PublicKey = client.PublicKey,
          };
          var content = JsonConvert.SerializeObject(keyModel);
          client.EncryptedKey = _keyGeneratorManager.EncryptString(content);
        }
        client?.UpdateClient(request.ClientName, request.ClientImage, request.ClientCompanyName, request.Mobile, request.Phone, request.LicenseNo, request.RegionTimeZoneId, _currentUser.EmployeeId!);
        
        var clnt = await _clientRepository.UpdateClient(client!);

        var clientAddress = await _clientRepository.GetClientAddresByClientId(client!.ClientId);

        var objAddress = request.ClientAddress!; 
        if (clientAddress is not null)
        {
          string fullAddress = await _countryRepository.GetFullAddress(objAddress.StreetAddress, objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId);

          clientAddress?.UpdateClientAddress(objAddress!.CountryId, objAddress!.CityId, objAddress!.AreaId, objAddress!.StreetAddress, objAddress.StreetAddress2, objAddress.HouseNo, objAddress.BuildingName, objAddress.Landmark, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId, fullAddress, objAddress!.Zip, (int)EnumAddressType.Shipping, objAddress.Latitude, objAddress.Longitude);
          response.IsSuccess = await _clientRepository.UpdateClientAddress(clientAddress!);
          response.CreateSuccessResponse();
        }

        var employeeId = new EmployeeId(new Guid(_currentUser.ClientIdStr!));

        var employee = await _employeeRepository.GetEmployeeById(employeeId!, _currentUser.ClientId!);

        employee.UpdateEmployee(request.ClientName, request.Phone, request.Mobile, employee.WorkEmail, request.ClientImage, employee.GenderId.GetValueOrDefault(3), employee.ClientUserRoleId,employee.DateOfBirth, _currentUser.EmployeeId, (int)EnumEmployeeType.Employee);
        var addedEmployee = await _employeeRepository.UpdateEmployee(employee);
      }
      //}
      //else
      //{
      //  response.Errors = cognitoUser.errors;
      //  response.IsSuccess = cognitoUser.isSuccess;

      //}
      return response;
      //}
      //else
      //{
      //  response.Errors?.Add("CognitoError", new string[] { "Something went wrong on updating user on cognito" });
      //  response.IsSuccess = false;
      //  return response;
      //}
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;

    }
  }
}
