using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Core.StoresAggregate;

public class StoreAddress
{
  public int StoreAddressId { get;private set; }
  public int StoreId { get;private set; }
  public int? CountryId { get;private set; }
  public int? CityId { get;private set; }
  public int? AreaId { get;private set; }
  public string? StreetAddress { get;private set; }
  public string? StreetAddress2 { get;private set; }
  public string? HouseNo { get;private set; }
  public string? BuildingName { get;private set; }
  public string? Landmark { get;private set; }
  public int? ProvinceId { get;private set; }
  public int? PinCodeId { get;private set; } 
  public int? StateId { get;private set; } 
  public string? FullAddress { get;private set; }
  public string? Zip { get;private set; }
  public int? AddressTypeId { get;private set; }
  public decimal? Latitude { get;private set; }
  public decimal? Longitude { get;private set; }

  public static StoreAddress CreateStoreAddress(int storeId, int? countryId, int? cityId, int? areaId, string? streetAddress, string? streetAddress2, string? houseNo, string? buildingName, string? landmark, int? provinceId, int? pinCodeId,int? stateId, string? fullAddress, string? zip, int? addressTypeId, decimal? latitude, decimal? longitude)
  {
    return new StoreAddress
    {
      StoreId = storeId,
      CountryId = countryId,
      CityId = cityId,
      AreaId = areaId,
      StreetAddress = streetAddress,
      StreetAddress2 = streetAddress2,
      HouseNo = houseNo,
      BuildingName = buildingName,
      Landmark = landmark,
      ProvinceId = provinceId,
      PinCodeId = pinCodeId,
      FullAddress = fullAddress,
      StateId = stateId,
      Zip = zip,
      AddressTypeId = addressTypeId,
      Latitude = UtilityHelper.TrimToPrecision(latitude),
      Longitude = UtilityHelper.TrimToPrecision(longitude)
    };
  }
  public void UpdateOrderAddress(int? countryId, int? cityId, int? areaId, string? streetAddress, string? streetAddress2, string? houseNo, string? buildingName, string? landmark, int? provinceId, int? pinCodeId,int? stateId, string? fullAddress, string? zip, int? addressTypeId, decimal? latitude, decimal? longitude)
  {
    CountryId = countryId;
    CityId = cityId;
    AreaId = areaId;
    StreetAddress = streetAddress;
    StreetAddress2 = streetAddress2;
    HouseNo = houseNo;
    BuildingName = buildingName;
    Landmark = landmark;
    StateId = stateId;
    ProvinceId = provinceId;
    PinCodeId = pinCodeId;
    FullAddress = fullAddress;
    Zip = zip;
    AddressTypeId = addressTypeId;
    Latitude = UtilityHelper.TrimToPrecision(latitude);
    Longitude = UtilityHelper.TrimToPrecision(longitude);
  }

}
