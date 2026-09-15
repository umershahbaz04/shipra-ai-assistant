using Microsoft.Extensions.Logging;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NPOI.SS.Util;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.CreateSaleChannelConfig;

public class CreateSaleChannelConfigCommandHandler : RequestHandlerBase<CreateSaleChannelConfigCommand, ServiceResultDTO>
{
  private readonly IPermissionRepository _permissionRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;
  private readonly IShopifyRepository _shopifyRepository;
  private readonly IStoreRepository _storeRepository;

  public CreateSaleChannelConfigCommandHandler(IPermissionRepository permissionRepository, IClientRepository clientRepository, IEmployeeRepository employeeRepository, IConfigRepository configRepository, ISharedUserManagement userManagement, ISaleChannelConfigRepository SaleChannelConfigRepository, IShopifyRepository shopifyRepository, IServiceProvider serviceProvider, IStoreRepository storeRepository, ILogger<CreateSaleChannelConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _permissionRepository = permissionRepository;
    _clientRepository = clientRepository;
    _employeeRepository = employeeRepository;
    _configRepository = configRepository;
    _userManagement = userManagement;
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
    _shopifyRepository = shopifyRepository;
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateSaleChannelConfigCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      Employee? oEmployee = new Employee();
      var requestDictionry = request.InputParameters! != null && request.InputParameters!.Count > 0 ? Utils.ConvertKeysToCamelCase(request.InputParameters!) : new Dictionary<string, string>();
      string? saleChannelKey = null;

      string? settingConfig = null;
      if (request.SettingConfig != null && request.SettingConfig!.Count > 0)
      {
        settingConfig = JsonConvert.SerializeObject(request.SettingConfig!, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        var flattenedSettingConfig = UtilityHelper.FlattenSettingConfigToDictionary(settingConfig);
        foreach (var kvp in flattenedSettingConfig)
        {
          requestDictionry[kvp.Key] = kvp.Value;
        }
      }

      var saleChannelJsonData = JsonConvert.SerializeObject(requestDictionry, Formatting.Indented);
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      if (!string.IsNullOrEmpty(request.SaleChannelName))
      {
        request.SaleChannelName = await _SaleChannelConfigRepository.GetUniqueSaleChannelNameAsync(request.SaleChannelName, _currentUser.ClientId!); 
      }

      string? saleChannelUserName = null;
      string? saleChannelPassword = null;
      bool isAvailable = true; //if we need to check salechannel key then make it false and follow function else no need 
      // create user name password for sale channel for sync orders etc.
      if (request!.SaleChannelLookupId == (int)EnumSaleChannelLookup.SalePerson)
      {
        var salePersonName = Utils.GetValueFromDictionryByKey("salePersonName", requestDictionry);
        var phoneNo = Utils.GetValueFromDictionryByKey("phoneNo", requestDictionry);
        var email = Utils.GetValueFromDictionryByKey("email", requestDictionry);
        var password = Utils.GetValueFromDictionryByKey("password", requestDictionry);
        var address = Utils.GetValueFromDictionryByKey("fullAddress", requestDictionry);
        var username = Utils.GetValueFromDictionryByKey("username", requestDictionry);

        var isPreverifyEmail = bool.Parse(Utils.GetValueFromDictionryByKey("isPreverifyEmail", requestDictionry));



        if (!string.IsNullOrEmpty(email) && !Utility.IsValidEmail(email.ToLower()))
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Invalid Sale Person email");
        }
        else
        {
          saleChannelKey = email = email.ToLower();
        }
        if (string.IsNullOrEmpty(saleChannelKey))
        {
          saleChannelKey = GetSaleChannelFromDic(requestDictionry);
        }
        isAvailable = await ExistingSaleChannelKeyCheck(saleChannelKey);

        if (!string.IsNullOrEmpty(phoneNo) && !phoneNo.Contains("+"))
        {
          phoneNo = "+" + phoneNo;
        }
        #region Creating Cognito User and Employee
        var oUser = await _userManagement.SignupEmployeeAsync(email!, username, phoneNo, address, _currentUser.ClientIdStr, password, password, (int)EnumUserRole.SalePerson, isPreverifyEmail, mcconfig?.Value!);

        if (!string.IsNullOrEmpty(oUser))
        {
          //create client user
          var cognitoUser = JsonConvert.DeserializeObject<AuthResponseModel<SignupResult>>(oUser);
          if (cognitoUser!.isSuccess && cognitoUser!.result != null)
          {
            var requestResult = cognitoUser!.result;
            var employeeId = new Guid(cognitoUser.result!.userId!);
            var employeeCode = username;

            var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
            #region get and create user role 
            var oClientUserRole = await _permissionRepository.GetClientUserRoleByName(ApplicationConstants.SalePerson, _currentUser.ClientId!);
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
              oClientUserRole = await _permissionRepository.GetClientUserRoleByName(ApplicationConstants.SalePerson, _currentUser.ClientId!);
            }
            #endregion
            int genderId = int.Parse(Utils.GetValueFromDictionryByKey("genderId", requestDictionry));
            string dateOfBirthStr = Utils.GetValueFromDictionryByKey("dateOfBirth", requestDictionry);

            DateTime? dateOfBirth = null;
            if (DateTime.TryParse(dateOfBirthStr, out var parsedDate))
            {
              dateOfBirth = parsedDate;
            }
            var employeeTypeId = int.Parse(Utils.GetValueFromDictionryByKey("employeeTypeId", requestDictionry));


            oEmployee = await _employeeRepository.CreateEmployee(Employee.CreateEmployee(new EmployeeId(employeeId), _currentUser.ClientId!, employeeCode, salePersonName, genderId, dateOfBirth, phoneNo, phoneNo, email, "", oClientUserRole!.ClientUserRoleId, (int)EnumUserType.SalePerson, _currentUser.EmployeeId!, employeeTypeId));

            #region create employee address
            var countryId = int.Parse(Utils.GetValueFromDictionryByKey("countryId", requestDictionry));
            var cityId = int.Parse(Utils.GetValueFromDictionryByKey("cityId", requestDictionry));
            var areaId = int.Parse(Utils.GetValueFromDictionryByKey("areaId", requestDictionry));
            var streetAddress = Utils.GetValueFromDictionryByKey("streetAddress", requestDictionry);
            var streetAddress2 = Utils.GetValueFromDictionryByKey("streetAddress2", requestDictionry);
            var houseNo = Utils.GetValueFromDictionryByKey("houseNo", requestDictionry);
            var buildingName = Utils.GetValueFromDictionryByKey("buildingName", requestDictionry);
            var landmark = Utils.GetValueFromDictionryByKey("landmark", requestDictionry);
            var provinceId = int.Parse(Utils.GetValueFromDictionryByKey("provinceId", requestDictionry));
            var pinCodeId = int.Parse(Utils.GetValueFromDictionryByKey("pinCodeId", requestDictionry));
            var stateId = int.Parse(Utils.GetValueFromDictionryByKey("stateId", requestDictionry));
            var fullAddress = Utils.GetValueFromDictionryByKey("fullAddress", requestDictionry);
            var zip = Utils.GetValueFromDictionryByKey("zip", requestDictionry);
            var latitude = decimal.Parse(Utils.GetValueFromDictionryByKey("latitude", requestDictionry));
            var longitude = decimal.Parse(Utils.GetValueFromDictionryByKey("longitude", requestDictionry));

            bool isAdded = await CreateEmployeeAddress(oEmployee.EmployeeId,
                                                       countryId,
                                                       cityId,
                                                       areaId,
                                                       streetAddress,
                                                       streetAddress2,
                                                       houseNo,
                                                       buildingName,
                                                       landmark,
                                                       provinceId,
                                                       pinCodeId,
                                                       stateId,
                                                       fullAddress,
                                                       zip,
                                                       (int)EnumAddressType.Shipping,
                                                       latitude,
                                                       longitude);
            #endregion
          }
          else
          {
            serviceResult.Errors = cognitoUser.errors;
            serviceResult.IsSuccess = cognitoUser.isSuccess;
            return serviceResult;
          }
        }
        #endregion
      }
      else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
      {
        var shopName = Utils.GetValueFromDictionryByKey("shop", requestDictionry);
        if (!string.IsNullOrEmpty(shopName) && !Utility.IsValidShopifyUrl(shopName.ToLower()))
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Invalid Shop Please enter valid shop e.g. shipra.myshopify.com");
        }
        else
        {
          saleChannelKey = shopName.ToLower();
        }
        isAvailable = await ExistingSaleChannelKeyCheck(saleChannelKey);
      }
      else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.WooCommerce)
      {
        var shopURL = Utils.GetValueFromDictionryByKey("shop", requestDictionry);
        //if (!string.IsNullOrEmpty(shopURL) && !Utility.IsValidWooCommerceUrl(shopURL.ToLower()))
        //{
        //  throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Invalid Shop Please enter valid Shop with https e.g. https://shipra.woocommerce.com");
        //}
        //else
        //{
        //  saleChannelKey = shopURL.ToLower();
        //}

        saleChannelKey = shopURL.ToLower();

        isAvailable = await ExistingSaleChannelKeyCheck(saleChannelKey);
      }  
      else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.BigCommerce)
      {
        var shopURL = Utils.GetValueFromDictionryByKey("shop", requestDictionry);
        saleChannelKey = shopURL.ToLower();
        isAvailable = await ExistingSaleChannelKeyCheck(saleChannelKey);
      }
      else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.Wix)
      {
        // Primary domain:
        //https://mybusiness.wixsite.com
        // Custom domain:
        //https://www.mybusiness.com

        var shopURL = Utils.GetValueFromDictionryByKey("shop", requestDictionry); 
        if (!string.IsNullOrEmpty(shopURL) && !Utility.IsValidWixUrl(shopURL.ToLower()))
        {
          throw new ShipraApplicationException(
              System.Net.HttpStatusCode.BadRequest,
              "Invalid Shop. Please enter a valid Shop with https, e.g., https://mybusiness.wixsite.com or https://www.mybusiness.com"
          );
        }
        else
        {
          saleChannelKey = shopURL.ToLower();
        }
      }
      else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.OpenCart)
      {
        var shopURL = Utils.GetValueFromDictionryByKey("shop", requestDictionry);
        saleChannelKey = shopURL.ToLower();
        isAvailable = await ExistingSaleChannelKeyCheck(saleChannelKey);
      }
      else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.Noon)
      {
        //saleChannelKey = request.SaleChannelName!.ToLower();
        saleChannelKey = GenerateSaleChannelKey(request.SaleChannelName!);
        isAvailable = await ExistingSaleChannelKeyCheck(saleChannelKey ?? string.Empty);
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Please enter valid sale channel.");
      }

      //#region create user for auto syncing purpose for all sales channel except sale person
      //if (request.SaleChannelLookupId != (int)EnumSaleChannelLookup.SalePerson)
      //{
      //  var target = await _storeRepository.GetStoreById(request.StoreId, _currentUser.ClientId!);
      //  if (target == null)
      //  {
      //    throw new EntityNotFoundException("Store ", request.StoreId);
      //  }

      //  var oStoreAddress = await _storeRepository.GetStoreAddressById(target.StoreId);
      //  if (oStoreAddress is null)
      //  {
      //    throw new EntityNotFoundException("Store Address ", request.StoreId);
      //  }
      //  saleChannelUserName = request?.SaleChannelName?.Trim().Replace(" ", string.Empty) ?? string.Empty;
      //  saleChannelPassword = Utility.GenerateRandomPassword(8);
      //  var email = saleChannelUserName + "@salechannel.com";
      //  #region Creating Cognito User and Employee
      //  if (!string.IsNullOrEmpty(target.CustomerServiceNo) && !target.CustomerServiceNo.Contains("+"))
      //  {
      //    target.CustomerServiceNo = "+" + target.CustomerServiceNo;
      //  }
      //  var oUser = await _userManagement.SignupEmployeeAsync(email!, saleChannelUserName, target.CustomerServiceNo, oStoreAddress.FullAddress!, _currentUser.ClientIdStr, saleChannelPassword, saleChannelPassword, (int)EnumUserRole.SalesCoordinator, true, mcconfig?.Value!);

      //  if (!string.IsNullOrEmpty(oUser))
      //  {
      //    //create client user
      //    var cognitoUser = JsonConvert.DeserializeObject<AuthResponseModel<SignupResult>>(oUser);
      //    if (cognitoUser!.isSuccess && cognitoUser!.result != null)
      //    {
      //      var requestResult = cognitoUser!.result;
      //      var employeeId = new Guid(cognitoUser.result!.userId!);
      //      var employeeCode = saleChannelUserName;

      //      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      //      #region get and create user role 
      //      var oClientUserRole = await _permissionRepository.GetClientUserRoleByName(ApplicationConstants.SaleChannel, _currentUser.ClientId!);
      //      //get user role by name if not exist then create for existing users
      //      if (oClientUserRole is null)
      //      {
      //        #region Permission lookups
      //        var allUserRoleList = await _permissionRepository.GetAllUserRole();
      //        foreach (var oUserRole in allUserRoleList)
      //        {
      //          ClientUserRole clientUserRole = ClientUserRole.Create(oUserRole.RoleName, oUserRole.RoleDescription, _currentUser.ClientId!, true);
      //          await _permissionRepository.CreateClientUserRole(clientUserRole);

      //          var allGroupPermission = await _permissionRepository.GetAllRolePermissionGroupDefaultsByRoleId(oUserRole.RoleId);
      //          foreach (var oRolePermissionGroup in allGroupPermission)
      //          {
      //            var oClientRolePermissionGroup = ClientRolePermissionGroup.Create(clientUserRole.ClientUserRoleId, oRolePermissionGroup.RolePermissionGroupId, _currentUser.ClientId!);
      //            await _permissionRepository.CreateClientRolePermissionGroup(oClientRolePermissionGroup);
      //          }
      //        }
      //        #endregion
      //        oClientUserRole = await _permissionRepository.GetClientUserRoleByName(ApplicationConstants.SaleChannel, _currentUser.ClientId!);
      //      }
      //      #endregion  
      //      DateTime? dateOfBirth = null;
      //      if (!string.IsNullOrEmpty(target.CustomerServiceNo) && !target.CustomerServiceNo.Contains("+"))
      //      {
      //        target.CustomerServiceNo = "+" + target.CustomerServiceNo;
      //      }

      //      oEmployee = await _employeeRepository.CreateEmployee(Employee.CreateEmployee(new EmployeeId(employeeId), _currentUser.ClientId!, employeeCode, request?.SaleChannelName, (int)EnumGender.Unknown, dateOfBirth, target.CustomerServiceNo, target.CustomerServiceNo, email, "", oClientUserRole!.ClientUserRoleId, (int)EnumEmployeeType.SaleChannel, _currentUser.EmployeeId!));

      //      #region create employee address
      //      var countryId = oStoreAddress.CountryId;
      //      var cityId = oStoreAddress.CityId;
      //      var areaId = oStoreAddress.AreaId;
      //      var streetAddress = oStoreAddress.StreetAddress;
      //      var streetAddress2 = oStoreAddress.StreetAddress2;
      //      var houseNo = oStoreAddress.HouseNo;
      //      var buildingName = oStoreAddress.BuildingName;
      //      var landmark = oStoreAddress.Landmark;
      //      var provinceId = oStoreAddress.ProvinceId;
      //      var pinCodeId = oStoreAddress.PinCodeId;
      //      var stateId = oStoreAddress.StateId;
      //      var fullAddress = oStoreAddress.FullAddress!;
      //      var zip = oStoreAddress.Zip;
      //      var latitude = oStoreAddress.Latitude;
      //      var longitude = oStoreAddress.Longitude;

      //      bool isAdded = await CreateEmployeeAddress(oEmployee.EmployeeId,
      //                                                 countryId,
      //                                                 cityId,
      //                                                 areaId,
      //                                                 streetAddress,
      //                                                 streetAddress2,
      //                                                 houseNo,
      //                                                 buildingName,
      //                                                 landmark,
      //                                                 provinceId,
      //                                                 pinCodeId,
      //                                                 stateId,
      //                                                 fullAddress,
      //                                                 zip,
      //                                                 (int)EnumAddressType.Shipping,
      //                                                 latitude,
      //                                                 longitude);

      //      #endregion
      //    }
      //    else
      //    {
      //      serviceResult.Errors = cognitoUser.errors;
      //      serviceResult.IsSuccess = cognitoUser.isSuccess;
      //      return serviceResult;
      //    }
      //  }
      //  #endregion
      //}

      //#endregion

      if (isAvailable)
      {
        var oSaleChannelConfig = await _SaleChannelConfigRepository.CreateSaleChannelConfig(SaleChannelConfig.CreateSaleChannelConfig(request!.StoreId, request.SaleChannelLookupId, saleChannelJsonData, request.SaleChannelName!, _currentUser.ClientId, request.IsAllowToDisplayInSaleChannel.GetValueOrDefault(), _currentUser.EmployeeId, settingConfig!, saleChannelKey, saleChannelUserName, saleChannelPassword));
        if (oSaleChannelConfig is not null)
        {
          if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
          {
            var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(oSaleChannelConfig.SaleChannelConfigId, _currentUser.ClientId!);
            if (oShopifyConfig is not null)
            {
              oShopifyConfig.UpdateShopifyConfig(request.AccessToken!, _currentUser.EmployeeId!);
              oShopifyConfig = await _shopifyRepository.UpdateShopifyConfig(oShopifyConfig);
            }
            else
            {
              oShopifyConfig = await _shopifyRepository.CreateShopifyConfig(ShopifyConfig.CreateShopifyConfig(request.AccessToken!, oSaleChannelConfig.SaleChannelKey!.ToString(), oSaleChannelConfig.SaleChannelConfigId, oSaleChannelConfig.ClientId!, _currentUser.EmployeeId!));

              oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(_currentUser.EmployeeId!);
              await _SaleChannelConfigRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
            }
          }
          if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.SalePerson)
          {
            oEmployee.UpdateSaleChannelConfigId(oSaleChannelConfig.SaleChannelConfigId, _currentUser.EmployeeId!);
            await _employeeRepository.UpdateEmployee(oEmployee);
          }
          serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
        }
        else
        {
          serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
          serviceResult.Errors?.Add("SaleChannelConfig", new[] { "Sale Channel Config not saved" });
        }
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  private string GenerateSaleChannelKey(string saleChannelName)
  {
    var name = saleChannelName.Trim()
                       .Replace(" ", "")
                       .ToLowerInvariant();

    var randomKey = Random.Shared.Next(1000, 9999);

    return $"{name}{randomKey}";
  }
  private string GetSaleChannelFromDic(Dictionary<string, string> requestDictionry)
  {
    var shopURL = Utils.GetValueFromDictionryByKey("shop", requestDictionry);

    return shopURL;
  }

  private async Task<bool> ExistingSaleChannelKeyCheck(string saleChannelKey)
  {
    await Task.Delay(1);
    //var oSaleChannelKey = await _SaleChannelConfigRepository.GetSaleChannelConfigByKey(saleChannelKey!);

    //if (oSaleChannelKey != null)
    //{
    //  throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed,
    //      $"The sale channel record already exists with the name of {saleChannelKey}.");
    //}

    return true; // Indicating that no existing record was found.
  }
  private async Task<bool> CreateEmployeeAddress(EmployeeId? employeeId, int? countryId, int? cityId, int? areaId, string? streetAddress, string? streetAddress2, string? houseNo, string? buildingName, string? landmark, int? provinceId, int? pinCodeId, int? stateId, string fullAddress, string? zip, int shipping, decimal? latitude, decimal? longitude)
  {
    EmployeeAddress oOrderAddress = EmployeeAddress.CreateEmployeeAddress(employeeId,
                                                                                  countryId,
                                                                                  cityId,
                                                                                  areaId,
                                                                                  streetAddress,
                                                                                  streetAddress2,
                                                                                  houseNo,
                                                                                  buildingName,
                                                                                  landmark,
                                                                                  provinceId,
                                                                                  pinCodeId,
                                                                                  stateId,
                                                                                  fullAddress,
                                                                                  zip,
                                                                                  (int)EnumAddressType.Shipping,
                                                                                  latitude,
                                                                                  longitude);

    bool isAdded = await _employeeRepository.CreateEmployeeAddress(oOrderAddress);
    return isAdded;
  }
}
