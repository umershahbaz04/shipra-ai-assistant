using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.AccountUserCase;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.Login;
public class LoginCommand : IRequest<ServiceResultDTO>
{
  public string? UserName { get; set; }
  public string? Password { get; set; }
}
public class LoginCommandHandler : RequestHandlerBase<LoginCommand, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;
  private readonly ICurrentTenantService _currentTenantService;
  private readonly IClientLoginRepository _clientRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;
  private readonly ISharedStripeRepository _sharedStripeRepository;

  public LoginCommandHandler(ICurrentTenantService currentTenantService, IClientLoginRepository clientRepository, ICountryRepository countryRepository, IConfigRepository configRepository, ISharedUserManagement userManagement, IServiceProvider serviceProvider, ISharedStripeRepository sharedStripeRepository, ILogger<LoginCommandHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
    _currentTenantService = currentTenantService;
    _clientRepository = clientRepository;
    _configRepository = configRepository;
    _userManagement = userManagement;
    _sharedStripeRepository = sharedStripeRepository;
  }


  protected override async Task<ServiceResultDTO> HandleRequest(LoginCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      #region 
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }

      var result = await _userManagement.LoginAsync(request.UserName!, request.Password!, mcconfig.Value!);
      var mcconfigTenantAdmin = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, _currentUser.EnvironmentTypeId);

      if (!string.IsNullOrEmpty(result))
      {
        var deserialised = JsonConvert.DeserializeObject<AuthResponseModel<LoginResult>>(result);

        if (!deserialised!.isSuccess)
        {
          string keyToUpdate = "InvalidUserNamePassword";
          if (deserialised.errors!.ContainsKey(keyToUpdate)! && Helpers.Utility.ContainsWord(deserialised.errors![keyToUpdate].ToList(), "pool"))
          {
            // Update the error message
            deserialised.errors![keyToUpdate] = new string[] { "User doesn't exist!!" };
          }
          keyToUpdate = "UsernameNotConfirm";
          if (deserialised.errors!.ContainsKey(keyToUpdate)!)
          {

            if (mcconfigTenantAdmin is null)
            {
              throw new EntityNotFoundException("Mcconfig", "Admin ControlPan Value");
            }
            if (string.IsNullOrEmpty(deserialised.result!.client_id))
            {
              throw new EntityNotFoundException("Mcconfig Client", "client_id Value not found");
            }

            //var oTenant = await _sharedStripeRepository.GetTenantById(deserialised.result!.client_id!, mcconfigTenantAdmin.Value!);
            //var deseralisedTenantResponse = JsonConvert.DeserializeObject<AuthResponseModel<TenantResponseModel>>(oTenant);
            //if (deseralisedTenantResponse!.isSuccess)
            //{
            //  if (deseralisedTenantResponse!.result!.TenantId == deserialised.result.client_id)
            //  {
            //    await _sharedStripeRepository.CreateTenant(deserialised.result!.client_id,)
            //  }
            //}
            var data = await _sharedStripeRepository.GetCustomerSessionClientSecret(deserialised.result!.client_id!, mcconfigTenantAdmin.Value!);

            var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<StripeSubscriptionModel>>(data);
            if (deseralisedResponse!.isSuccess)
            {
              serviceResult = new ServiceResultDTO(new
              {
                user_name = request.UserName!,
                deserialised.result!.client_id,
                deseralisedResponse.result!.CustomerSessionClientSecret,
                deseralisedResponse.result.IsPlannedSubscribed,
                deseralisedResponse.result.IsSubscriptionCancel,
                deseralisedResponse.result.IsOnTrail,
                deseralisedResponse.result.PublishableKey,
                deseralisedResponse.result.PricingTableId,
                serviceResult.Errors
              });
            }
          }
          serviceResult.Errors = deserialised.errors;
          serviceResult.IsSuccess = false;
        }
        else
        {
          var handler = new JwtSecurityTokenHandler();
          var jwtSecurityToken = handler.ReadJwtToken(deserialised!.result?.access_token);
          var jwtSecurityIdToken = handler.ReadJwtToken(deserialised!.result?.id_token);
          var clientId = jwtSecurityIdToken.Claims.FirstOrDefault(claim => claim.Type == "custom:ClientId")?.Value;
          var employeeId = jwtSecurityIdToken.Claims.FirstOrDefault(claim => claim.Type == "custom:EmployeeId")?.Value;
          var userRole = jwtSecurityIdToken.Claims.FirstOrDefault(claim => claim.Type == "custom:roleId")?.Value;
          var connectionString = _currentTenantService.GetConnectionStringByTenant(clientId!);

          if (!string.IsNullOrEmpty(connectionString))
          {
            #region run code if we have client catalog
            int userRoleId = 1;
            if (userRole != null)
            {
              userRoleId = Int32.Parse(userRole);
            }
            var client = await _clientRepository.GetClientByClientId(clientId!, connectionString);
            var oClientConfigSetting = await _clientRepository.GetClientConfigSetting(clientId!, connectionString);
            var oEmployee = await _clientRepository.GetEmployeebyId(clientId!, employeeId!, connectionString);
            RegionTimeZone? oRegionTimeZone = new RegionTimeZone();
            Country? oCountry = new Country();
            if (client is not null)
            {
              oRegionTimeZone = await _countryRepository.GetRegionTimeZoneById(client.RegionTimeZoneId);
              oCountry = await _countryRepository.GetCountryById(client.CountryId!);
            }

            var countries = await _countryRepository.GetAllCountries();
            var restrictedCountry = countries.Select(x => x.MapCountryCode).ToList();

            if (mcconfigTenantAdmin is null)
            {
              throw new EntityNotFoundException("Mcconfig", "Admin ControlPan Value");
            }
            var data = await _sharedStripeRepository.GetCustomerSessionClientSecret(clientId!, mcconfigTenantAdmin.Value!);

            serviceResult.CreateSuccessResponse();
            deserialised!.result!.email = (oEmployee != null ? oEmployee.Email : "");
            deserialised!.result!.client_code = client!.ClientCode;
            deserialised!.result!.companyImage = (oEmployee != null ? oEmployee.EmployeeImage : "");

            deserialised!.result!.settingConfig = (oClientConfigSetting != null
                                                    ? new LoginSettingConfigDto
                                                    {
                                                      ShowOrderLabel = oClientConfigSetting.AllowShipperInvocie.GetValueOrDefault()
                                                    } : new LoginSettingConfigDto());
            deserialised!.result!.userRoleId = userRoleId;
            deserialised!.result!.allowPersonalCarrierContract = (client != null ? client.AllowPersonalClientCarrierContract : false);
            deserialised!.result!.allowShipperInvocie = (client != null ? client.AllowShipperInvocie : false);
            deserialised!.result!.clientPrefix = (client != null ? client.ClientIdentifier.ToString() : ""); ;
            deserialised!.result!.region = oRegionTimeZone;
            deserialised!.result!.country = oCountry;
            deserialised!.result!.restrictedCountry = restrictedCountry!;
            deserialised!.result!.isShowMetafield = client?.IsShowMetafield;

            if (Shipra.Backend.API.Application.Helpers.Utility.IsValidJson(data))
            {
              var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<StripeSubscriptionModel>>(data);
              if (deseralisedResponse is not null && deseralisedResponse.isSuccess)
              {
                deserialised!.result!.user_name = request.UserName!;
                deserialised!.result!.customerSessionClientSecret = deseralisedResponse.result!.CustomerSessionClientSecret!;
                deserialised!.result!.isPlannedSubscribed = deseralisedResponse.result!.IsPlannedSubscribed!;
                deserialised.result!.isSubscriptionCancel = deseralisedResponse.result.IsSubscriptionCancel!;
                deserialised.result!.isOnTrail = deseralisedResponse.result.IsOnTrail!;
                deserialised.result!.pricingTableId = deseralisedResponse.result.PricingTableId!;
                deserialised.result!.publishableKey = deseralisedResponse.result.PublishableKey!;
                deserialised.errors = deseralisedResponse.errors;
                deserialised.isSuccess = deseralisedResponse.isSuccess;
              }
            }
            if (deserialised.isSuccess)
            {
              serviceResult = new ServiceResultDTO(deserialised.result!);
            }
            else
            {
              serviceResult.CreateErrorResponse(new Exception(string.Join(", ", deserialised.errors!.Values.SelectMany(e => e))));
            }
            #endregion
          }
          else
          {
            serviceResult.CreateError("InvalidClientConnection", new string[] { "Invalid connection" });
          }
        }
      }
      #endregion
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
  public LoginCommandValidator()
  {
    RuleFor(x => x.UserName).NotNull().NotEmpty();
    RuleFor(x => x.Password).NotNull().NotEmpty();
  }
}
