using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.UpdateStoreCommand;
public class UpdateStoreCommandHandler : RequestHandlerBase<UpdateStoreCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly ICountryRepository _countryRepository;
  private readonly IStoreRepository _storeRepository;
  public UpdateStoreCommandHandler(ICountryRepository countryRepository,IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<UpdateStoreCommandHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
    _storeRepository = storeRepository;
  }
  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(UpdateStoreCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<BaseResponseDto> serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      Store? store = await _storeRepository.GetStoreById(request.StoreId, _currentUser.ClientId!);
      if (store is not null)
      {
        if (store.StoreName != request.StoreName)
        {
          Store existStore = await _storeRepository.CheckStoreExistAginstClientByStoreName(request.StoreName, _currentUser.ClientId);
          if (existStore is not null)
          {
            serviceResult.Errors?.Add("AlreadyExistStore", new string[] { "Store already exist against name: " + request.StoreName });
            serviceResult.IsSuccess = false;
            return serviceResult;
          }
        }

        store!.UpdateStore(request.StoreName, request.StoreCompany, request.CustomerServiceNo, request.Phone, request.Email, request.Urls, request.StoreImage, request.LicenseNo, _currentUser.EmployeeId);
        await _storeRepository.UpdateStore(store);

        StoreAddress? oStoreAddress = await _storeRepository.GetStoreAddressById(store.StoreId);
        if (oStoreAddress is null)
        {
          serviceResult.Errors?.Add("NotFound", new string[] { "Store Address not found" });
          serviceResult.IsSuccess = false;
        } 
        var objAddress = request.Address!;
        #region update street address 2
        string? modifyStreet = objAddress.StreetAddress;

        if (!string.IsNullOrEmpty(objAddress.StreetAddress2))
        {
          modifyStreet = objAddress.StreetAddress + "," + objAddress.StreetAddress2;
        }
        #endregion
        string storeFullAddress = await _countryRepository.GetFullAddress(modifyStreet, objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.ProvinceId, objAddress.PinCodeId, objAddress.StateId);

        oStoreAddress!.UpdateOrderAddress(objAddress.CountryId, objAddress.CityId, objAddress.AreaId, objAddress.StreetAddress, objAddress.StreetAddress2, objAddress.HouseNo, objAddress.BuildingName, objAddress.Landmark, objAddress.ProvinceId, objAddress.PinCodeId,objAddress.StateId, storeFullAddress, objAddress.Zip, (int)EnumAddressType.Shipping, objAddress.Latitude, objAddress.Longitude);

        await _storeRepository.UpdateStoreAddress(oStoreAddress);

        serviceResult.CreateSuccessResponse();
      }
      else
      {
        {
          serviceResult.Errors?.Add("NotFound", new string[] { "Store not found" });
          serviceResult.IsSuccess = false;
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
}
