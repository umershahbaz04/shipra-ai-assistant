//using DocumentFormat.OpenXml.EMMA;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using Shipra.Backend.API.Application.Common;
//using Shipra.Backend.API.Application.Common.Constants;
//using Shipra.Backend.API.Application.Common.Exceptions;
//using Shipra.Backend.API.Application.DTOs;
//using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
//using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
//using Shipra.Backend.API.Application.Helpers;
//using Shipra.Backend.API.Core.CarrierAggregate;
//using Shipra.Backend.API.Core.ClientAggregate;
//using Shipra.Backend.API.Core.EmployeeAggregate;
//using Shipra.Backend.API.Core.Enum;
//using Shipra.Backend.API.Core.Interfaces;
//using Shipra.Backend.API.Core.ProductAggregate;
//using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
//using Shipra.Backend.API.Core.StoresAggregate;
//using Shipra.Backend.API.SharedKernel.Interfaces;
//using Shipra.Backend.API.SharedKernel.Models;

//namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateClient;
//public class CreateClientCommandHandler_old : RequestHandlerBase<CreateClientCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
//{
//  private readonly IPermissionRepository _permissionRepository;
//  private readonly ICommonLookupRepository _commonLookupRepository;
//  private readonly IShipmentRepository _shipmentRepository;
//  private readonly ISharedStripeRepository _sharedStripeRepository;
//  private readonly ICountryRepository _countryRepository;
//  private readonly IProductStationRepository _productStationRepository;
//  private readonly IStoreRepository _storeRepository;
//  private readonly IConfigRepository _configRepository;
//  private readonly ISharedUserManagement _userManagement;
//  private readonly IProductCategoryRepository _categoryRepository;
//  private readonly IClientRepository _clientRepository;
//  private readonly ICarrierRepository _carrierRepository;
//  private readonly IExpenseCategoryRepository _expenseCategoryRepository;
//  private readonly IDriverRepository _driverRepository;
//  private readonly IEmployeeRepository _employeeRepository;
//  public CreateClientCommandHandler_old(IPermissionRepository permissionRepository, ICommonLookupRepository commonLookupRepository, IShipmentRepository shipmentRepository, ISharedStripeRepository sharedStripeRepository, ICountryRepository countryRepository, IProductStationRepository productStationRepository, IStoreRepository storeRepository, IConfigRepository configRepository, ISharedUserManagement userManagement, IProductCategoryRepository categoryRepository, IClientRepository clientRepository, ICarrierRepository carrierRepository, IExpenseCategoryRepository expenseCategoryRepository, IDriverRepository driverRepository, IEmployeeRepository employeeRepository, IServiceProvider serviceProvider, ILogger<CreateClientCommandHandler_old> logger) : base(serviceProvider, logger)
//  {
//    _permissionRepository = permissionRepository;
//    _commonLookupRepository = commonLookupRepository;
//    _shipmentRepository = shipmentRepository;
//    _sharedStripeRepository = sharedStripeRepository;
//    _countryRepository = countryRepository;
//    _productStationRepository = productStationRepository;
//    _storeRepository = storeRepository;
//    _configRepository = configRepository;
//    _userManagement = userManagement;
//    _categoryRepository = categoryRepository;
//    _clientRepository = clientRepository;
//    _carrierRepository = carrierRepository;
//    _expenseCategoryRepository = expenseCategoryRepository;
//    _driverRepository = driverRepository;
//    _employeeRepository = employeeRepository;
//  }

//  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(CreateClientCommand request, CancellationToken cancellationToken)
//  {
//    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
//    List<string> oResultLog = new List<string>();

//    try
//    {
//      /// <summary>
//      /// 1. we need cognito solution url
//      /// 2. create user on cognito solution
//      /// 3. get last client identifier
//      /// 4. create store and get id 
//      /// 5. get from station lookup by region and create product station
//      /// 6. get productCategoryName from product category lookup and create productCategory
//      /// 7. create client in shipra db
//      /// </summary>
//      ///      <summmary>
//      /// 1.  get cognito mcconfig from ApplicationConstants.CognitoKey and signup user on cognito
//      /// 2.  get the last client by client identifier
//      /// 3.  create store for this client using the same clientId
//      /// 4.  get the stationId using the clients regionId requested from frontend 
//      ///     and create product station for this client using the same clientId
//      /// 5.  get the default productCategoryName from product category lookup 
//      ///     and create productCategory for this client using the same clientId
//      /// 6.  get the ongftypeId from ONGFTypeLookup and create client 
//      /// 7.  get default shipment dashboard setting and create entry
//      ///     </summmary>


//      #region actual code 
//      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.CognitoKey);
//      if (mcconfig is null)
//      {
//        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
//      }

//      #region Create Cognito Client
//      var oCognitoResult = await _userManagement.SignupAsync(request.Email!, request.UserName!.ToLower().Trim(), request.Mobile, request.StreetAddress, request.Password, request.Password, (int)EnumUserRole.Admin, mcconfig?.Value!);
//      #endregion

//      if (!string.IsNullOrEmpty(oCognitoResult))
//      {
//        //Deserialize Cognito client user
//        var cognitoUser = JsonConvert.DeserializeObject<AuthResponseModel<SignupResult>>(oCognitoResult);
//        if (cognitoUser!.isSuccess && cognitoUser!.result != null)
//        {
//          oResultLog.Add("1. Cognito User created successfully. ");
//          var requestResult = cognitoUser!.result;
//          _currentUser.ClientIdStr = cognitoUser.result!.userId!;
//          var cognitoUserId = new Guid(cognitoUser.result!.userId!);
//          _currentUser.ClientId = new ClientId(cognitoUserId);
//          _currentUser.EmployeeId = new EmployeeId(cognitoUserId);
//          string stripeCustomerId = string.Empty;

//          #region Client identifier
//          int clientIdentifier = 0;
//          Client? lastClient = await _clientRepository.GetLastClient();
//          if (lastClient is null || lastClient.ClientIdentifier == null)
//          {
//            clientIdentifier = ClientIdentifierGeneration.NextIdentifier(0);
//          }
//          else
//          {
//            clientIdentifier = ClientIdentifierGeneration.NextIdentifier(lastClient!.ClientIdentifier!);
//          }
//          #endregion

//          #region Create Client in Shipra
//          //var data = _countryRepository.GetAllRegionTimeZone();
//          var keyModel = new KeyModel()
//          {
//            ClientId = _currentUser.ClientId,
//            PublicKey = Utility.GetPublicKey(),
//          };
//          var client = Client.CreateClient(_currentUser.ClientId, request!.ClientName, request.ClientImage, request.UserName!.ToLower().Trim(), request.CountryId, request.RegionId, request.CityId, request.ClientCompanyName, request.Mobile, request.Phone, request.Email,
//          request.StreetAddress, _currentUser.EmployeeId, clientIdentifier, (int)EnumONGFType.AppDefault, request.Zip, (int)EnumRegionTimeZone.GulfRegion, keyModel.PublicKey, Utility.GetSecretKey(), string.Empty);
//          client = await _clientRepository.CreateClient(client);

//          if (client is not null)
//          {
//            oResultLog.Add("2. Shipra Client's created successfully. ");

//            #region Client Address
//            ClientAddress clientAddress = ClientAddress.CreateClientAddress(_currentUser.ClientId, request.CountryId, request.RegionId, request.CityId, request.StreetAddress, request.Zip, (int)EnumAddressType.Shipping, request.Latitude, request.Longitude);
//            ClientAddress oClientAddress = await _clientRepository.CreateClientAddress(clientAddress);
//            if (oClientAddress is not null)
//            {
//              oResultLog.Add("3. Shipra Client's Address created successfully. ");
//            }
//            #endregion
//            #region Permission lookups
//            var allUserRoleList = await _permissionRepository.GetAllUserRole();
//            foreach (var oUserRole in allUserRoleList)
//            {
//              ClientUserRole clientUserRole = ClientUserRole.Create(oUserRole.RoleName, oUserRole.RoleDescription, _currentUser.ClientId!, true);
//              await _permissionRepository.CreateClientUserRole(clientUserRole);

//              var allGroupPermission = await _permissionRepository.GetAllRolePermissionGroupDefaultsByRoleId(oUserRole.RoleId);
//              foreach (var oRolePermissionGroup in allGroupPermission)
//              {
//                var oClientRolePermissionGroup = ClientRolePermissionGroup.Create(clientUserRole.ClientUserRoleId, oRolePermissionGroup.RolePermissionGroupId, _currentUser.ClientId!);
//                await _permissionRepository.CreateClientRolePermissionGroup(oClientRolePermissionGroup);
//              }
//            }
//            #endregion
//            #region Create Employee
//            var oClientUserRole = await _permissionRepository.GetClientUserRoleByName(ApplicationConstants.SuperAdmin, _currentUser.ClientId!);

//            var employeeCode = await _employeeRepository.GetEmployeeNextCode(_currentUser.ClientId!);
//            var oEmployee = Employee.CreateEmployee(_currentUser.EmployeeId, _currentUser.ClientId, employeeCode, request.ClientName, null, null, request.Phone, request.Mobile, request.Email, request.CountryId, request.RegionId, request.CityId, request.StreetAddress, null, request.ClientImage, request.Zip, null, null, oClientUserRole?.ClientUserRoleId, (int)EnumEmployeeType.SuperAdmin, _currentUser.EmployeeId, true);
//            await _employeeRepository.CreateEmployee(oEmployee);
//            if (oEmployee is not null)
//            {
//              oResultLog.Add("4. Shipra Client's as Employee created successfully. ");
//            }
//            #endregion

//            #region Create Client Store

//            var placeholderImg = ApplicationConstants.StoreImagePlaceHolder;
//            string storeAddress = await _countryRepository.GetFullAddress(request.StreetAddress, request.CityId, request.RegionId, request.CountryId);
//            var storeCode = await _storeRepository.GetClientNextStoreCode(_currentUser.ClientId);

//            var store = Store.CreateStore(_currentUser.ClientId, request?.ClientName, storeCode, storeAddress, request?.ClientCompanyName, request?.CountryId, request?.RegionId, request?.CityId, request?.Mobile, request?.Phone, request?.Email, "", placeholderImg, "", _currentUser.EmployeeId, request!.StreetAddress, null, request.Zip, request.Latitude, request.Longitude, true);
//            var oStore = await _storeRepository.CreateStore(store);
//            if (oStore is not null)
//            {
//              oResultLog.Add("5. Client's Store created successfully. ");

//              client.UpdateClientDefaultStore(oStore.StoreId, _currentUser.EmployeeId);
//              var oclient = await _clientRepository.UpdateClient(client);
//              if (oclient is not null)
//              {
//                oResultLog.Add("5.1 Client's Store info updated in Shipra. ");
//              }
//            }
//            else
//            {
//              oResultLog.Add("Error while creating Shipra Store. ");
//            }
//            #endregion

//            #region Product Relevant

//            #region Create Product Station
//            var stationLookup = await _productStationRepository.GetProductStationLookupByRegionId(request!.RegionId);
//            if (stationLookup != null)
//            {
//              var stationCode = await _productStationRepository.GetNextProductStationCode(client.ClientId!);
//              var productStation = ProductStation.CreateProductStation(stationCode, stationLookup!.Sname, _currentUser.ClientId, _currentUser.EmployeeId, true);
//              var oProductStation = await _productStationRepository.CreateProductStation(productStation);
//              if (oProductStation is not null)
//              {
//                oResultLog.Add("6. Client's default Product Station created successfully. ");
//                client.UpdateClientDefaultProductStation(oProductStation?.ProductStationId!, _currentUser.EmployeeId);
//                var oclient = await _clientRepository.UpdateClient(client);
//                if (oclient is not null)
//                {
//                  oResultLog.Add("6.1 Client's Product Station info updated in Shipra. ");
//                }
//              }
//              else
//              {
//                oResultLog.Add("Error while creating client default Product Station. ");
//              }
//            }
//            else
//            {
//              //Set default Dubai
//              client.UpdateClientDefaultProductStation((int)EnumStationLookup.Dubai, _currentUser.EmployeeId);
//              var oclient = await _clientRepository.UpdateClient(client);
//              if (oclient is not null)
//              {
//                oResultLog.Add("6. Client's default Station info updated in Shipra. ");
//              }
//            }

//            #endregion

//            #region Create Product Category

//            var productCategory = ProductCategory.CreateProductCategory(EnumProductCategoryLookupHelper.GetEnumString(EnumProductCategoryLookup.General), _currentUser.ClientId, _currentUser.EmployeeId, true);
//            productCategory = await _categoryRepository.CreateProductCategory(productCategory);
//            if (productCategory is not null)
//            {
//              oResultLog.Add("7. Client's default Product Category created successfully. ");

//              client.UpdateClientDefaultProductCategory(productCategory.ProductCategoryId, _currentUser.EmployeeId);
//              await _clientRepository.UpdateClient(client);

//              oResultLog.Add("7.1 Client's default Product Category Update successfully. ");
//            }
//            else
//            {
//              oResultLog.Add("Error while creating Product Category. ");
//            }
//            #endregion

//            #endregion

//            //#region Create Default Carrier
//            //List<Carrier> allClientCarriers = await _carrierRepository.GetAllClientCarrier(_currentUser.ClientIdStr);
//            ////get last client id 
//            //var carrierId = Carrier.GetNextMyCarrierClientId(allClientCarriers.LastOrDefault());

//            //var oCarrier = Carrier.CreateDefaultCarrierForClient(carrierId, request.ClientName, request.CountryId, _currentUser.EmployeeId);
//            //var oCarrierResult = await _carrierRepository.CreateCarrier(oCarrier);
//            //if (oCarrierResult is not null)
//            //{
//            //  var oActiveCarrier = ActiveCarrier.CreateActiveCarrier(oCarrierResult.CarrierId!, _currentUser.ClientId, _currentUser.EmployeeId, null, false);
//            //  var oActiveCarrierResult = await _carrierRepository.CreateActiveCarrier(oActiveCarrier);
//            //  if (oActiveCarrierResult is not null)
//            //  {
//            //    oResultLog.Add("8. Client's default Carrier Activated successfully. ");
//            //    client.UpdateClientDefaultCarrier(oCarrier.CarrierId, _currentUser.EmployeeId);
//            //    var oclient = await _clientRepository.UpdateClient(client);
//            //    if (oclient is not null)
//            //    {
//            //      oResultLog.Add("8.1 Client's Carrier info updated in Shipra. ");
//            //    }
//            //  }
//            //  else
//            //  {
//            //    oResultLog.Add("Error while creating Carrier Activated. ");
//            //  }
//            //}
//            //#endregion

//            #region General Setting
//            var isExpenseCategorySaved = await _expenseCategoryRepository.CreateExpenseCategoryForGeneralSetting(_currentUser.ClientId);
//            if (isExpenseCategorySaved > 0)
//            {
//              oResultLog.Add("9. Client's default Expense Category added successfully. ");
//            }
//            else
//            {
//              oResultLog.Add("Error while creating Expense Category. ");
//            }

//            var isDefaultDriverCTSSaved = await _driverRepository.CreateDriverDefaultCTSSetting(_currentUser.ClientId, _currentUser.EmployeeId);
//            if (isDefaultDriverCTSSaved > 0)
//            {
//              oResultLog.Add("10. Client's default Driver Carrier Tracking Status Setting added successfully. ");
//            }
//            else
//            {
//              oResultLog.Add("Error while creating Driver Carrier Tracking Status Setting. ");
//            }

//            #region create default shipment dashboard tabs
//            List<DefaultShipmentDashboard>? defaultShipmentDashboards = await _shipmentRepository.GetDefaultShipmentDashboard();
//            int displayOrder = 1;
//            foreach (var item in defaultShipmentDashboards!)
//            {
//              // create main tab text entry
//              var isDefaultStatusTab = true; //default value set (we dont delete it if its exist)
//              var oShipmentGridColumn = ShipmentGridColumn.Create(item.DashboardStatusName!, displayOrder, item.IsFetchAllPendingStatus, item.IsCompleted, _currentUser.ClientId, _currentUser.EmployeeId!, isDefaultStatusTab);
//              bool isCreateShipmentGridColumn = await _shipmentRepository.CreateShipmentGridColumn(oShipmentGridColumn);

//              if (isCreateShipmentGridColumn)
//              {
//                oResultLog.Add("11. Client's Shipment dashboard Grid Column created successfully. ");
//                // create list vlue entry 
//                bool isCreateoShipmentGridClientSetting = await _shipmentRepository.CreateShipmentGridClientSetting(ShipmentGridClientSetting.Create(oShipmentGridColumn.ShipmentGridColumnId!, item.DashboardStatusValue, _currentUser.ClientId!, _currentUser.EmployeeId!));
//                if (isCreateoShipmentGridClientSetting)
//                {
//                  oResultLog.Add("11. Client's shipment grid client setting created successfully. ");
//                }
//              }
//              displayOrder = displayOrder + 1;
//            }
//            #endregion

//            #region create default client carrier status
//            List<ClientCarrierTrackingStatus> listOfClientCarrier = new();
//            var allDefaultCarrierStatus = await _commonLookupRepository.GetAllCarrierTrackingStatusLookup();
//            foreach (var item in allDefaultCarrierStatus!)
//            {
//              var oClientCarrierTrackingStatus = ClientCarrierTrackingStatus.Create(item.CarrierTrackingStatusId, item.TrackingStatus!, item.TrackingStatusAr!, _currentUser.ClientId!);
//              listOfClientCarrier.Add(oClientCarrierTrackingStatus);
//            }
//            bool isAdded = await _clientRepository.CreateBatchClientCarrierTrackingStatus(listOfClientCarrier);
//            #endregion
//            #endregion
//            #region Create Stripe Customer
//            try
//            {
//              #region Create Stripe Customer

//              var mcconfigTenantAdmin = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey);
//              if (mcconfigTenantAdmin is null)
//              {
//                throw new EntityNotFoundException("Mcconfig", "Admin ControlPan Value");
//              }

//              var stripeResponse = await _sharedStripeRepository.CreateTenant(cognitoUser.result!.userId!, request.Email, request.ClientName, request.Mobile, mcconfigTenantAdmin.Value!);
//              if (!string.IsNullOrEmpty(stripeResponse))
//              {

//                var result = JsonConvert.DeserializeObject<ShipraControlPaneResponseModel<StripeCustomerResponseModel>>(stripeResponse);
//                if (result != null && result!.isSuccess)
//                {
//                  oResultLog.Add("12. Client's Stripe Customer Id created successfully. ");
//                  client.UpdateClientStripeInfo(result.result?.stripeCustomerId!, _currentUser.EmployeeId);
//                  var oclient = await _clientRepository.UpdateClient(client);
//                  if (oclient is not null)
//                  {
//                    oResultLog.Add("12.1 Client's Stripe Customer info updated in Shipra. ");
//                  }
//                }
//                else
//                {
//                  oResultLog.Add("Error while creating Stripe Customer. ");
//                }
//              }
//              #endregion
//            }
//            catch (Exception)
//            {
//              oResultLog.Add("Error while creating stripe");
//            }
//            #endregion
//            response = new ServiceResultDTOWithTypeModel<BaseResponseDto>(new BaseResponseDto { Data = client.ClientId?.Value.ToString(), Message = string.Join(',', oResultLog) });
//            response.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
//          }
//          else
//          {
//            oResultLog.Add("Error while creating Shipra Client");
//            response = new ServiceResultDTOWithTypeModel<BaseResponseDto>(new BaseResponseDto { Data = null, Message = NotificationConstants.Error });
//            response.CreateSuccessResponse();
//          }
//          #endregion Create CLient in Shipra
//        }
//        else
//        {
//          oResultLog.Add($"Error: {cognitoUser.errors} - Success: {cognitoUser.isSuccess}");
//          response.Errors = cognitoUser.errors;
//          response.IsSuccess = cognitoUser.isSuccess;
//        }
//      }
//      else
//      {
//        response = new ServiceResultDTOWithTypeModel<BaseResponseDto>(new BaseResponseDto { Data = null, Message = NotificationConstants.Error });
//        response.CreateErrorResponse(new Exception("Error while creating Cognito Client"));
//      }

//      #endregion
//      return response;
//    }
//    catch (Exception ex)
//    {
//      response.CreateErrorResponse(ex);
//      return response;
//    }
//  }
//}
