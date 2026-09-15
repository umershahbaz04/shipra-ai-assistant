using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Core.EmployeeAggregate;

public class EmployeeAddress
{
  public long? EmployeeAddressId { get;private set; } 
  public EmployeeId? EmployeeId { get;private set; }
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

  public static EmployeeAddress CreateEmployeeAddress(EmployeeId? employeeId, int? countryId, int? cityId, int? areaId, string? streetAddress, string? streetAddress2, string? houseNo, string? buildingName, string? landmark , int? provinceId, int? pinCode,int? stateId, string? fullAddress, string? zip, int? addressTypeId, decimal? latitude, decimal? longitude)
  {
    return new EmployeeAddress
    { 
      EmployeeId = employeeId,
      CountryId = countryId,
      CityId = cityId,
      AreaId = areaId,
      StreetAddress = streetAddress,
      StreetAddress2 = streetAddress2,
      HouseNo = houseNo, 
      BuildingName = buildingName,
      Landmark = landmark,
      ProvinceId = provinceId,
      PinCodeId = pinCode,
      StateId = stateId,
      FullAddress = fullAddress,
      Zip = zip,
      AddressTypeId = addressTypeId,
      Latitude = UtilityHelper.TrimToPrecision(latitude),
      Longitude = UtilityHelper.TrimToPrecision(longitude)
    };
  }
  
  public void UpdateEmployeeAddress(int? countryId, int? cityId, int? areaId, string? streetAddress, string? streetAddress2, string? houseNo, string? buildingName, string? landmark, int? provinceId, int? pinCode,int? stateId, string? fullAddress, string? zip , int? addressTypeId, decimal? latitude, decimal? longitude)
  {
    CountryId = countryId;
    CityId = cityId;
    AreaId = areaId;
    StreetAddress = streetAddress;
    StreetAddress2 = streetAddress2;
    HouseNo = houseNo;
    BuildingName = buildingName;
    Landmark = landmark; 
    ProvinceId = provinceId;
    PinCodeId = pinCode;
    StateId = stateId;
    FullAddress = fullAddress;
    Zip = zip;
    AddressTypeId = addressTypeId;
    Latitude = UtilityHelper.TrimToPrecision(latitude);
    Longitude = UtilityHelper.TrimToPrecision(longitude);
  }
} 
