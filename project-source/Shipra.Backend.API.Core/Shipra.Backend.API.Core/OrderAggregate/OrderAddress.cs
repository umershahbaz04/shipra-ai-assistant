using Org.BouncyCastle.Asn1.Ocsp;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderAddress
{
  public long OrderAddressId { get; private set; }
  public string? CustomerName { get; private set; }
  public string? CustomerFullAddress { get; private set; }
  public string? Email { get; private set; }
  public string? Mobile1 { get; private set; }
  public string? Mobile2 { get; private set; }
  public int? CountryId { get; private set; }
  public int? CityId { get; private set; }
  public int? AreaId { get; private set; }
  public string? StreetAddress { get; private set; }
  public string? StreetAddress2 { get; private set; }
  public string? HouseNo { get; private set; }
  public string? BuildingName { get; private set; }
  public string? Landmark { get; private set; }
  public int? ProvinceId { get; private set; }
  public int? PinCodeId { get; private set; }
  public int? StateId { get; private set; }
  public decimal? Latitude { get; private set; }
  public decimal? Longitude { get; private set; }
  public string? StripeCustomerId { get; private set; }
  public string? EntityAddressDataJson { get; set; }
  public int? SelectedCarrierId { get; set; }


  public static OrderAddress CreateOrderAddress(string? customerName, string? customerFullAddress, string? email, string? mobile1, string? mobile2, int? countryId, int? cityId, int? areaId, string? streetAddress, decimal? latitude, decimal? longitude, string? streetAddress2 = null, string? houseNo = null, string? buildingName = null, string? landmark = null, int? provinceId = null, int? pincodeId = null, int? stateId = null, int? selectedCarrierId = null, string? entityAddressDataJson =null)
  {
    return new OrderAddress()
    {
      CustomerName = customerName,
      CustomerFullAddress = customerFullAddress,
      Email = email,
      Mobile1 = UtilityHelper.CleanPhoneNumber(mobile1),
      Mobile2 = UtilityHelper.CleanPhoneNumber(mobile2),
      CountryId = countryId,
      AreaId = areaId,
      CityId = cityId,
      StreetAddress = streetAddress,
      StreetAddress2 = streetAddress2,
      HouseNo = houseNo,
      BuildingName = buildingName,
      Landmark = landmark,
      ProvinceId = provinceId,
      PinCodeId = pincodeId,
      StateId = stateId,
      EntityAddressDataJson = entityAddressDataJson,
      Latitude = UtilityHelper.TrimToPrecision(latitude),
      Longitude = UtilityHelper.TrimToPrecision(longitude),
      SelectedCarrierId = selectedCarrierId
    };
  }

  public void UpdateCustomerEmail(string? email, EmployeeAggregate.EmployeeId? userId, string? customerName = null, string? mobile1 = null, string? mobile2 = null)
  {
    if (email != null) Email = email;
    if (customerName != null) CustomerName = customerName;
    if (mobile1 != null) Mobile1 = UtilityHelper.CleanPhoneNumber(mobile1);
    if (mobile2 != null) Mobile2 = UtilityHelper.CleanPhoneNumber(mobile2);
  }

  public void UpdateLatLng(decimal? latitude, decimal? longitude, EmployeeAggregate.EmployeeId? employeeId)
  {
    Latitude = UtilityHelper.TrimToPrecision(latitude);
    Longitude = UtilityHelper.TrimToPrecision(longitude);

  }

  public void UpdateOrderAddress(string? customerName, string? customerFullAddress, string? email, string? mobile1, string? mobile2, int? countryId, int? cityId, int? areaId, string? streetAddress, decimal? latitude, decimal? longitude, string? streetAddress2 = null, string? houseNo = null, string? buildingName = null, string? landmark = null, int? provinceId = null, int? pincodeId = null, int? stateId = null, int? selectedCarrierId = null, string? entityAddressDataJson = null)
  {
    CustomerName = customerName;
    CustomerFullAddress = customerFullAddress;
    Email = email;
    Mobile1 = UtilityHelper.CleanPhoneNumber(mobile1);
    Mobile2 = UtilityHelper.CleanPhoneNumber(mobile2);
    CountryId = countryId;
    AreaId = areaId;
    CityId = cityId;
    StreetAddress = streetAddress;
    StreetAddress2 = streetAddress2;
    HouseNo = houseNo;
    BuildingName = buildingName;
    Landmark = landmark;
    ProvinceId = provinceId;
    PinCodeId = pincodeId;
    StateId = stateId;
    EntityAddressDataJson = entityAddressDataJson;
    Latitude = UtilityHelper.TrimToPrecision(latitude);
    Longitude = UtilityHelper.TrimToPrecision(longitude);
    SelectedCarrierId = selectedCarrierId;
  }

  public void UpdateValidatedAddressForCarrier(string fullAddress, int? cityId, int? areaId, int? provinceId, int? pinCodeId, int? stateId, int? selectedCarrierId, string? entityAddressDataJson,string? street)
  {
    CustomerFullAddress = fullAddress;
    AreaId = areaId;
    CityId = cityId;
    ProvinceId = provinceId;
    PinCodeId = pinCodeId;
    StateId = stateId;
    EntityAddressDataJson = entityAddressDataJson;
    SelectedCarrierId = selectedCarrierId;
    StreetAddress = street;
  } 
  public void UpdateAddressFromDeliveryTask(string fullAddress, int? cityId, int? areaId, int? provinceId, int? pinCodeId, int? stateId, string? entityAddressDataJson,string? street)
  {
    CustomerFullAddress = fullAddress;
    AreaId = areaId;
    CityId = cityId;
    ProvinceId = provinceId;
    PinCodeId = pinCodeId;
    StateId = stateId;
    EntityAddressDataJson = entityAddressDataJson; 
    StreetAddress = street;
  }
}
