using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Core.ClientAggregate;
public class ClientAddress
{
  public ClientAddressId? ClientAddressId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public int? CountryId { get; set; } 
  public int? CityId { get; set; } 
  public int? AreaId { get; set; } 
  public string? StreetAddress { get; set; } 
  public string? StreetAddress2 { get; set; } 
  public string? HouseNo { get; set; } 
  public string? BuildingName { get; set; } 
  public string? Landmark { get; set; } 
  public int? PinCodeId { get; set; } 
  public int? ProvinceId { get; set; }  
  public int? StateId { get; set; }  
  public string? FullAddress { get; set; } 
  public string? Zip { get; set; } 
  public int? AddressTypeId { get; set; } 
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }

  public static ClientAddress CreateClientAddress(ClientId clientId,int? countryId, int? cityId, int? areaId, string? streetAddress, string? streetAddress2, string? houseNo, string? buildingName, string? landmark, int? provinceId, int? pinCodeId,int? stateId, string? fullAddress, string? zip, int? addressTypeId, decimal? latitude, decimal? longitude)
  {
    return new ClientAddress()
    {
      ClientAddressId = ClientAddressId.New,
      ClientId = clientId,
      CountryId = countryId,
      AreaId = areaId,
      CityId = cityId,
      StreetAddress = streetAddress,
      StreetAddress2 = streetAddress2,
      HouseNo = houseNo,
      BuildingName = buildingName,
      Landmark = landmark,
      PinCodeId = pinCodeId,
      ProvinceId = provinceId,
      StateId = stateId,
      FullAddress = fullAddress,
      Zip = zip,
      AddressTypeId = addressTypeId,
      Latitude = UtilityHelper.TrimToPrecision(latitude),
      Longitude = UtilityHelper.TrimToPrecision(longitude)
    };
  }

  public void UpdateClientAddress(int? countryId, int? cityId, int? areaId, string? streetAddress, string? streetAddress2, string? houseNo, string? buildingName, string? landmark, int? provinceId, int? pinCodeId, int? stateId, string? fullAddress, string? zip, int? addressTypeId, decimal? latitude, decimal? longitude)
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
    PinCodeId = pinCodeId;
    StateId = stateId;
    FullAddress = fullAddress;
    AddressTypeId = addressTypeId;
    Zip = zip;
    Latitude = UtilityHelper.TrimToPrecision(latitude);
    Longitude = UtilityHelper.TrimToPrecision(longitude);

  }
}
public sealed record ClientAddressId(Guid Value)
{
  public static ClientAddressId New => new(Guid.NewGuid());
}
