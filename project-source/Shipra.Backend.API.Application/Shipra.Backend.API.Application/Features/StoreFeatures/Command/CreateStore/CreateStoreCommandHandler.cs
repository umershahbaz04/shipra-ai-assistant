using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.CreateStore;
public class CreateStoreCommandHandler : RequestHandlerBase<CreateStoreCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly ICountryRepository _countryRepository;
  private readonly IStoreRepository _storeRepository;

  public CreateStoreCommandHandler(ICountryRepository countryRepository, IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<CreateStoreCommandHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(CreateStoreCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    try
    {
      ///    <summary>
      /// 1. get the full address for the store by concatenating countryname,
      ///    cityname,regionname using countryid,cityid,regionid
      /// 2. get the total stores for creating store code
      /// 3. get the generated store code 
      /// 4. createstore in store table using all above information and those
      ///    entered from frontend.
      ///   </summary>
      var existStore = await _storeRepository.CheckStoreExistAginstClientByStoreName(request.StoreName, _currentUser.ClientId);
      if (existStore is not null)
      {
        response.Errors?.Add("AlreadyExistStore", new string[] { "Store already exist against name: " + request.StoreName });
        response.IsSuccess = false;
        return response;
      }
      if (string.IsNullOrEmpty(request.StoreImage))
      {
        request.StoreImage = ApplicationConstants.StoreImagePlaceHolder;
      }
      var objAddress = request.StoreAddress!;
      #region update street address 2
      var modifyStreet = objAddress.StreetAddress;

      if (!string.IsNullOrEmpty(objAddress.StreetAddress2))
      {
        modifyStreet = objAddress.StreetAddress + "," + objAddress.StreetAddress2;
      }
      #endregion
      var storeFullAddress = await _countryRepository.GetFullAddress(modifyStreet, objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId);

      var storeCode = await _storeRepository.GetClientNextStoreCode(_currentUser.ClientId!);
      var store = Store.CreateStore(_currentUser.ClientId!, request?.StoreName, storeCode, request?.StoreCompany, request?.CustomerServiceNo, request?.Phone, request?.Email, request?.Urls, request?.StoreImage,request?.LicenseNo, _currentUser.EmployeeId);
      var createdStore = await _storeRepository.CreateStore(store) as Store;
      if (store.StoreId > 0)
      { 
        #region store address
        var oStoreAddress = StoreAddress.CreateStoreAddress(store.StoreId, objAddress?.CountryId, objAddress?.CityId, objAddress!.AreaId, objAddress.StreetAddress, objAddress.StreetAddress2, objAddress.HouseNo, objAddress.BuildingName, objAddress.Landmark, objAddress.ProvinceId, objAddress.PinCodeId,objAddress.StateId, storeFullAddress, objAddress.Zip, (int)EnumAddressType.Shipping, objAddress.Latitude, objAddress.Longitude);
        var isAddedAdd = await _storeRepository.CreateStoreAddress(oStoreAddress);

        #endregion
      }
      var dt = new BaseResponseDto()
      {
        Data = createdStore!.StoreId,
        Message = NotificationConstants.SavedSuccess
      };

      response = new ServiceResultDTOWithTypeModel<BaseResponseDto>(dt);
      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;

    }
  }
}

