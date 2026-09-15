using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.CatalougeAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.Security.Service.IManagers;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateClient;
public class CreateClientCommandHandler : RequestHandlerBase<CreateClientCommand, ServiceResultDTO>
{ 
  private readonly ICatalogueRepository _catalogueRepository;
  private readonly IClientRepositoryInitializer _clientRepositoryInitializer;
  private readonly ISharedStripeRepository _sharedStripeRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IConfigRepository _configRepository;
  private readonly ISharedUserManagement _userManagement;
  private readonly IKeyGeneratorManager _keyGeneratorManager;
  public CreateClientCommandHandler(ICatalogueRepository catalogueRepository, IClientRepositoryInitializer clientRepositoryInitializer, ISharedStripeRepository sharedStripeRepository, ICountryRepository countryRepository, IConfigRepository configRepository, ISharedUserManagement userManagement, IKeyGeneratorManager keyGeneratorManager, IServiceProvider serviceProvider, ILogger<CreateClientCommandHandler> logger) : base(serviceProvider, logger)
  { 
    _catalogueRepository = catalogueRepository;
    _clientRepositoryInitializer = clientRepositoryInitializer;
    _sharedStripeRepository = sharedStripeRepository;
    _countryRepository = countryRepository;
    _configRepository = configRepository;
    _userManagement = userManagement;
    _keyGeneratorManager = keyGeneratorManager;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTO();
    List<string> oResultLog = new List<string>();
    string? customerSessionClientSecret = "";
    string? publishableKey = "";
    string? pricingTableId = "";
    bool? isPlannedSubscribed = false;
    try
    {
      /// <summary>
      /// 1. we need cognito solution url
      /// 2. create user on cognito solution
      /// 3. get last client identifier
      /// 4. create store and get id 
      /// 5. get from station lookup by region and create product station
      /// 6. get productCategoryName from product category lookup and create productCategory
      /// 7. create client in shipra db
      /// </summary>
      ///      <summmary>
      /// 1.  get cognito mcconfig from ApplicationConstants.CognitoKey and signup user on cognito
      /// 2.  get the last client by client identifier
      /// 3.  create store for this client using the same clientId
      /// 4.  get the stationId using the clients regionId requested from frontend 
      ///     and create product station for this client using the same clientId
      /// 5.  get the default productCategoryName from product category lookup 
      ///     and create productCategory for this client using the same clientId
      /// 6.  get the ongftypeId from ONGFTypeLookup and create client 
      /// 7.  get default shipment dashboard setting and create entry
      ///     </summmary>


      #region actual code 
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      var objAddress = request.ClientAddress;

      #region Create Cognito Client
      var oCognitoResult = await _userManagement.SignupAsync(request.Email!, request.UserName!.ToLower().Trim(), request.Mobile, objAddress!.StreetAddress, request.Password, request.Password, (int)EnumUserRole.Admin, mcconfig?.Value!,request.IsPreverifyEmail);
      #endregion

      if (!string.IsNullOrEmpty(oCognitoResult))
      {
        //Deserialize Cognito client user
        var cognitoUser = JsonConvert.DeserializeObject<AuthResponseModel<SignupResult>>(oCognitoResult);
        if (cognitoUser!.isSuccess && cognitoUser!.result != null)
        {
          oResultLog.Add("1. Cognito User created successfully. ");
          var requestResult = cognitoUser!.result;
          _currentUser.ClientIdStr = cognitoUser.result!.userId!;
          var cognitoUserId = new Guid(cognitoUser.result!.userId!);
          _currentUser.ClientId = new ClientId(cognitoUserId);
          _currentUser.EmployeeId = new EmployeeId(cognitoUserId);
          string stripeCustomerId = string.Empty;

          //algoritherm for create new catalog 
          #region create catalog
          var allDbs = await _catalogueRepository.GetAllCatalogueDatabases();
          if (allDbs.Count > 0)
          {
            var oCatalogueDatabase = allDbs.Where(x => x.MaxClientAllowed > x.QuantityUsed).FirstOrDefault();
            if (oCatalogueDatabase != null)
            {
              Catalogue catalogue = Catalogue.Create(oCatalogueDatabase.DatabaseId, _currentUser.ClientIdStr);
              await _catalogueRepository.CreateCatalogue(catalogue);

              if (catalogue.CatalogueId > 0)
              {
                var newQty = oCatalogueDatabase.QuantityUsed + 1;
                oCatalogueDatabase.UpdateUsedQty(newQty);
                await _catalogueRepository.UpdateCatalogueDatabase(oCatalogueDatabase);

                #region after creating cataloge 
                #region Client identifier
                int clientIdentifier = 0;
                Client? lastClient = await _clientRepositoryInitializer.GetLastClient(_currentUser.ClientIdStr);
                if (lastClient is null || lastClient.ClientIdentifier == null)
                {
                  clientIdentifier = ClientIdentifierGeneration.NextIdentifier(0);
                }
                else
                {
                  clientIdentifier = ClientIdentifierGeneration.NextIdentifier(lastClient!.ClientIdentifier!);
                }
                #endregion
                #region updateClinerIdentifier
                catalogue.UpdateClientIdentifier(clientIdentifier.ToString());
                #endregion
                #region Create Client in Shipra
                //var data = _countryRepository.GetAllRegionTimeZone();
                string? encryptedValue = string.Empty;
                var keyModel = new KeyModel()
                {
                  ClientId = _currentUser.ClientId,
                  PublicKey = Utility.GetPublicKey(), 
                  RandomKey = Utility.GetRandomKey(),
                  UserName = request.UserName,
                  Password = request.Password,
                };
                var content = JsonConvert.SerializeObject(keyModel);
                encryptedValue = _keyGeneratorManager.EncryptString(content);

                string fullAddress = await _countryRepository.GetFullAddress(objAddress.StreetAddress, objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId);

                #region client and address
                var client = Client.CreateClient(_currentUser.ClientId, request!.ClientName, request.ClientImage, request.UserName!.ToLower().Trim(), request.ClientCompanyName, request.Mobile, request.Phone, request.Email,
                       _currentUser.EmployeeId, clientIdentifier, (int)EnumONGFType.AppDefault, (int)EnumRegionTimeZone.GulfRegion, keyModel.PublicKey, Utility.GetSecretKey(), encryptedValue);

                #region Client Address
                ClientAddress clientAddress = ClientAddress.CreateClientAddress(_currentUser.ClientId, objAddress!.CountryId, objAddress!.CityId, objAddress!.AreaId, objAddress!.StreetAddress, objAddress.StreetAddress2, objAddress.HouseNo, objAddress.BuildingName, objAddress.Landmark, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId, fullAddress, objAddress!.Zip, (int)EnumAddressType.Shipping, request.Latitude, request.Longitude);

                client = await _clientRepositoryInitializer.CreateClient(client, clientAddress);

                #endregion
                if (client is not null)
                {
                  #endregion

                  #region Create Employee
                  var oClientUserRole = await _clientRepositoryInitializer.GetClientUserRoleByName(ApplicationConstants.SuperAdmin, _currentUser.ClientId!);

                  var employeeCode = await _clientRepositoryInitializer.GetEmployeeNextCode(_currentUser.ClientId!);
                  var oEmployee = Employee.CreateEmployee(_currentUser.EmployeeId, _currentUser.ClientId, employeeCode, request.ClientName, null, null, request.Phone, request.Mobile, request.Email, request.ClientImage, oClientUserRole?.ClientUserRoleId, (int)EnumUserType.SuperAdmin, _currentUser.EmployeeId,(int)EnumEmployeeType.Employee, true);
                  await _clientRepositoryInitializer.CreateEmployee(oEmployee);
                  if (oEmployee is not null)
                  {
                    #region create employee address
                    EmployeeAddress oOrderAddress = EmployeeAddress.CreateEmployeeAddress(oEmployee.EmployeeId, objAddress!.CountryId, objAddress!.CityId, objAddress!.AreaId, objAddress!.StreetAddress, objAddress!.StreetAddress2, objAddress!.HouseNo, objAddress!.BuildingName, objAddress!.Landmark, objAddress!.ProvinceId, objAddress!.PinCodeId, objAddress.StateId, fullAddress, objAddress!.Zip, (int)EnumAddressType.Shipping, objAddress!.Latitude, objAddress!.Longitude);

                    bool isAddedAddress = await _clientRepositoryInitializer.CreateEmployeeAddress(oOrderAddress, _currentUser.ClientIdStr);

                    #endregion

                    oResultLog.Add("4. Shipra Client's as Employee created successfully. ");
                  }
                  #endregion

                  #region Create Client Store

                  var placeholderImg = ApplicationConstants.StoreImagePlaceHolder;
                  var storeCode = await _clientRepositoryInitializer.GetClientNextStoreCode(_currentUser.ClientId);

                  var store = Store.CreateStore(_currentUser.ClientId, request?.ClientName, storeCode, request?.ClientCompanyName, request?.Mobile, request?.Phone, request?.Email, "", placeholderImg, "", _currentUser.EmployeeId, true);
                  var oStore = await _clientRepositoryInitializer.CreateStore(store);
                  if (oStore is not null)
                  {
                    #region store address
                    StoreAddress oStoreAddress = StoreAddress.CreateStoreAddress(store.StoreId, objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.StreetAddress, objAddress.StreetAddress2, objAddress.HouseNo, objAddress.BuildingName, objAddress.Landmark, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId, fullAddress, objAddress.Zip, (int)EnumAddressType.Shipping, objAddress.Latitude, objAddress.Longitude);
                    bool isAddedAdd = await _clientRepositoryInitializer.CreateStoreAddress(oStoreAddress, _currentUser.ClientIdStr);

                    #endregion


                    oResultLog.Add("5. Client's Store created successfully. ");

                    client.UpdateClientDefaultStore(oStore.StoreId, _currentUser.EmployeeId);
                    var oclient = await _clientRepositoryInitializer.UpdateClient(client);
                    if (oclient is not null)
                    {
                      oResultLog.Add("5.1 Client's Store info updated in Shipra. ");
                    }
                  }
                  else
                  {
                    oResultLog.Add("Error while creating Shipra Store. ");
                  }
                  #endregion

                  #region Product Relevant

                  #region Create Product Station
                  var stationLookup = await _clientRepositoryInitializer.GetProductStationLookupByCityId(objAddress!.CityId);
                  if (stationLookup != null)
                  {
                    var stationCode = await _clientRepositoryInitializer.GetNextProductStationCode(client.ClientId!);
                    var productStation = ProductStation.CreateProductStation(stationCode, stationLookup!.Sname, _currentUser.ClientId, _currentUser.EmployeeId, true);
                    var oProductStation = await _clientRepositoryInitializer.CreateProductStation(productStation);
                    if (oProductStation is not null)
                    {
                      oResultLog.Add("6. Client's default Product Station created successfully. ");
                      client.UpdateClientDefaultProductStation(oProductStation?.ProductStationId!, _currentUser.EmployeeId);
                      var oclient = await _clientRepositoryInitializer.UpdateClient(client);
                      if (oclient is not null)
                      {
                        oResultLog.Add("6.1 Client's Product Station info updated in Shipra. ");
                      }
                    }
                    else
                    {
                      oResultLog.Add("Error while creating client default Product Station. ");
                    }
                  }
                  else
                  {
                    //Set default Dubai
                    client.UpdateClientDefaultProductStation((int)EnumStationLookup.Dubai, _currentUser.EmployeeId);
                    var oclient = await _clientRepositoryInitializer.UpdateClient(client);
                    if (oclient is not null)
                    {
                      oResultLog.Add("6. Client's default Station info updated in Shipra. ");
                    }
                  }

                  #endregion

                  #region Create Product Category

                  var productCategory = ProductCategory.CreateProductCategory(EnumProductCategoryLookupHelper.GetEnumString(EnumProductCategoryLookup.General), _currentUser.ClientId, _currentUser.EmployeeId, true);
                  productCategory = await _clientRepositoryInitializer.CreateProductCategory(productCategory);
                  if (productCategory is not null)
                  {
                    oResultLog.Add("7. Client's default Product Category created successfully. ");

                    client.UpdateClientDefaultProductCategory(productCategory.ProductCategoryId, _currentUser.EmployeeId);
                    await _clientRepositoryInitializer.UpdateClient(client);

                    oResultLog.Add("7.1 Client's default Product Category Update successfully. ");
                  }
                  else
                  {
                    oResultLog.Add("Error while creating Product Category. ");
                  }
                  #endregion

                  #endregion

                  #region extract carriers for client from lookups table
                  bool dd = await _clientRepositoryInitializer.CreateCarrierFromLookup(_currentUser.ClientIdStr);

                  #endregion

                  #region Create Default Carrier
                  List<Carrier> allClientCarriers = await _clientRepositoryInitializer.GetAllCreatedClientCarrier(_currentUser.ClientIdStr!);
                  //get last client id 
                  var carrierId = Carrier.GetNextMyCarrierClientId(allClientCarriers.LastOrDefault());

                  var oCarrier = Carrier.CreateDefaultCarrierForClient(carrierId, request!.ClientName, objAddress.CountryId, _currentUser.EmployeeId);
                  var oCarrierResult = await _clientRepositoryInitializer.CreateCarrier(oCarrier, _currentUser.ClientIdStr!);
                  if (oCarrierResult is not null)
                  {
                    ActiveCarrier oActiveCarrier = ActiveCarrier.CreateActiveCarrier(oCarrierResult.CarrierId!, _currentUser.ClientId, _currentUser.EmployeeId, null, false);
                    var oActiveCarrierResult = await _clientRepositoryInitializer.CreateActiveCarrier(oActiveCarrier);
                    if (oActiveCarrierResult is not null)
                    {
                      oResultLog.Add("8. Client's default Carrier Activated successfully. ");
                      client.UpdateClientDefaultCarrier(oCarrier.CarrierId, _currentUser.EmployeeId);
                      var oclient = await _clientRepositoryInitializer.UpdateClient(client);
                      if (oclient is not null)
                      {
                        oResultLog.Add("8.1 Client's Carrier info updated in Shipra. ");
                      }
                    }
                    else
                    {
                      oResultLog.Add("Error while creating Carrier Activated. ");
                    }
                  }
                  #endregion

                  #region General Setting
                  var isExpenseCategorySaved = await _clientRepositoryInitializer.CreateExpenseCategoryForGeneralSetting(_currentUser.ClientId);
                  if (isExpenseCategorySaved > 0)
                  {
                    oResultLog.Add("9. Client's default Expense Category added successfully. ");
                  }
                  else
                  {
                    oResultLog.Add("Error while creating Expense Category. ");
                  }

                  var isDefaultDriverCTSSaved = await _clientRepositoryInitializer.CreateDriverDefaultCTSSetting(_currentUser.ClientId, _currentUser.EmployeeId);
                  if (isDefaultDriverCTSSaved > 0)
                  {
                    oResultLog.Add("10. Client's default Driver Carrier Tracking Status Setting added successfully. ");
                  }
                  else
                  {
                    oResultLog.Add("Error while creating Driver Carrier Tracking Status Setting. ");
                  }

                  #region create default shipment dashboard tabs
                  List<DefaultShipmentDashboard>? defaultShipmentDashboards = await _clientRepositoryInitializer.GetDefaultShipmentDashboard();
                  int displayOrder = 1;
                  foreach (var item in defaultShipmentDashboards!)
                  {
                    // create main tab text entry
                    var isDefaultStatusTab = true; //default value set (we dont delete it if its exist)
                    var oShipmentGridColumn = ShipmentGridColumn.Create(item.DashboardStatusName!, displayOrder, item.IsFetchAllPendingStatus, item.IsCompleted, _currentUser.ClientId, _currentUser.EmployeeId!, isDefaultStatusTab);
                    bool isCreateShipmentGridColumn = await _clientRepositoryInitializer.CreateShipmentGridColumn(oShipmentGridColumn);

                    if (isCreateShipmentGridColumn)
                    {
                      oResultLog.Add("11. Client's Shipment dashboard Grid Column created successfully. ");
                      // create list vlue entry 
                      bool isCreateoShipmentGridClientSetting = await _clientRepositoryInitializer.CreateShipmentGridClientSetting(ShipmentGridClientSetting.Create(oShipmentGridColumn.ShipmentGridColumnId!, item.DashboardStatusValue, _currentUser.ClientId!, _currentUser.EmployeeId!));
                      if (isCreateoShipmentGridClientSetting)
                      {
                        oResultLog.Add("11. Client's shipment grid client setting created successfully. ");
                      }
                    }
                    displayOrder = displayOrder + 1;
                  }
                  #endregion

                  #region create default client carrier status
                  List<ClientCarrierTrackingStatus> listOfClientCarrier = new();
                  var allDefaultCarrierStatus = await _clientRepositoryInitializer.GetAllCarrierTrackingStatusLookup();
                  foreach (var item in allDefaultCarrierStatus!)
                  {
                    var oClientCarrierTrackingStatus = ClientCarrierTrackingStatus.Create(item.CarrierTrackingStatusId, item.TrackingStatus!, item.TrackingStatusAr!, _currentUser.ClientId!);
                    listOfClientCarrier.Add(oClientCarrierTrackingStatus);
                  }
                  bool isAdded = await _clientRepositoryInitializer.CreateBatchClientCarrierTrackingStatus(listOfClientCarrier);
                  #endregion
                  #endregion

                  #region Create Stripe Customer
                  try
                  {
                    #region Create Stripe Customer

                    var mcconfigTenantAdmin = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, _currentUser.EnvironmentTypeId);
                    if (mcconfigTenantAdmin is null)
                    {
                      throw new EntityNotFoundException("Mcconfig", "Admin ControlPan Value");
                    }
                    #region country
                    var country = await _countryRepository.GetCountryById(objAddress.CountryId);
                    var iSOA2Code = country?.ISOA2Code ?? string.Empty;
                    #endregion
                    var stripeResponse = await _sharedStripeRepository.CreateTenant(cognitoUser.result!.userId!, request.Email, request.ClientName, request.Mobile,iSOA2Code,mcconfigTenantAdmin.Value!);
                    if (!string.IsNullOrEmpty(stripeResponse))
                    {

                      var result = JsonConvert.DeserializeObject<ShipraControlPaneResponseModel<StripeCustomerResponseModel>>(stripeResponse);
                      if (result != null && result!.isSuccess)
                      {
                        oResultLog.Add("12. Client's Stripe Customer Id created successfully. ");
                        client.UpdateClientStripeInfo(result.result?.stripeCustomerId!, _currentUser.EmployeeId);
                        var oclient = await _clientRepositoryInitializer.UpdateClient(client);
                        if (oclient is not null)
                        {

                          oResultLog.Add("12.1 Client's Stripe Customer info updated in Shipra. ");

                          oResultLog.Add("12.2 Get Client's Stripe Customer Secret for Shipra Subscription. ");
                          var data = await _sharedStripeRepository.GetCustomerSessionClientSecret(cognitoUser.result!.userId!, mcconfigTenantAdmin.Value!);
                          var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<StripeSubscriptionModel>>(data);
                          if (deseralisedResponse!.isSuccess)
                          {
                            customerSessionClientSecret = deseralisedResponse.result!.CustomerSessionClientSecret;
                            isPlannedSubscribed = deseralisedResponse.result!.IsPlannedSubscribed;
                            publishableKey = deseralisedResponse.result!.PublishableKey;
                            pricingTableId = deseralisedResponse.result!.PricingTableId;
                          }
                        }
                      }
                      else
                      {
                        oResultLog.Add("Error while creating Stripe Customer. ");
                      }
                    }
                    #endregion
                  }
                  catch (Exception)
                  {
                    oResultLog.Add("Error while creating stripe");
                  }
                  #endregion
                
                  #region send email
                  string userName = request.UserName!; // Replace with actual user name
                  string userPassword = request.Password!; // Replace with actual user password

                  string body = $@"
                                <div style='font-family: Lato, Arial, sans-serif; color: #333;'> 
                                    <h2 style='color: #563AD5;'>Welcome to Shipra, {userName}!</h2>
                                    <p>We are excited to have you on board.</p>
                                    <p><strong>Your login details are as follows:</strong></p>
                                    <table style='border: 1px solid #ddd; border-collapse: collapse;'>
                                        <tr>
                                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Username:</strong></td>
                                            <td style='padding: 8px; border: 1px solid #ddd;'>{userName}</td>
                                        </tr>
                                        <tr>
                                            <td style='padding: 8px; border: 1px solid #ddd;'><strong>Password:</strong></td>
                                            <td style='padding: 8px; border: 1px solid #ddd;'>{userPassword}</td>
                                        </tr>
                                    </table>
                                    <p>To get started, simply log in using your credentials.</p>
                                    <p>If you have any questions, feel free to reach out to our support team at <a href='mailto:{ApplicationConstants.ShipraSupportEmail}' style='color: #563AD5;'>{ApplicationConstants.ShipraSupportEmail}</a>.</p>
                                    <br />
                                    <p style='font-size: 12px; color: #888;'>Note: Please do not share your password with anyone. You can change your password after your first login.</p>
                                    <br />
                                    <p>Best regards,</p>
                                    <p><strong>Shipra Team</strong></p>
                                    <div style='text-align: left;'>
                                        <!-- Add your logo here -->
                                        <img src='{ApplicationConstants.ShipraLogo}' alt='Shipra Logo' style='width: 150px; height: auto; margin-bottom: 20px;' />
                                    </div>
                                </div>";


                  var result123 = await _emailServiceProvider.SendEmailAsync("Welcome to shipra", body, new string[] { request.Email! });
                  #endregion
                  response = new ServiceResultDTO(
                 new
                 {
                   ClientId = client.ClientId?.Value.ToString(),
                   CustomerSessionClientSecret = customerSessionClientSecret,
                   IsPlannedSubscribed = isPlannedSubscribed,
                   PublishableKey = publishableKey,
                   PricingTableId = pricingTableId,
                   Message = string.Join(',', oResultLog)
                 });
                }
                else
                {
                  oResultLog.Add("Error while creating Shipra Client");
                  response = new ServiceResultDTO(new BaseResponseDto { Data = null, Message = NotificationConstants.Error });
                  response.CreateSuccessResponse();
                }
                #endregion Create CLient in Shipra
                #endregion
              }

            }
          }
          #endregion

        }
        else
        {
          oResultLog.Add($"Error: {cognitoUser.errors} - Success: {cognitoUser.isSuccess}");
          response.Errors = cognitoUser.errors;
          response.IsSuccess = cognitoUser.isSuccess;
        }
      }
      else
      {
        response = new ServiceResultDTO(new BaseResponseDto { Data = null, Message = NotificationConstants.Error });
        response.CreateErrorResponse(new Exception("Error while creating Cognito Client"));
      }

      #endregion
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
