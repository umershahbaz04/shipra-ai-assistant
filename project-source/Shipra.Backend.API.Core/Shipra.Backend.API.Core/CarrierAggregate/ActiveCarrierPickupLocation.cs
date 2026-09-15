using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.OrderAggregate;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Shipra.Backend.API.Core.CarrierAggregate;
public class ActiveCarrierPickupLocation
{
  public int ActiveCarrierPickupLocationId { get; set; }
  public int ActiveCarrierId { get; set; }
  public int CarrierId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? LocationName { get; set; }
  public int? CountryId { get; set; }
  public int? CityId { get; set; }
  public int? AreaId { get; set; }
  public string? CustomerServiceNo { get; set; }
  public string? Phone { get; set; }
  public string? AreaName { get; set; }
  public string? StreetAddress { get; set; }
  public string? StreetAddress2 { get; set; }
  public string? HouseNo { get; set; }
  public string? BuildingName { get; set; }
  public string? Landmark { get; set; }
  public int? ProvinceId { get; set; }
  public int? PinCodeId { get; set; }
  public int? StateId { get; set; }
  public string? FullAddress { get; set; }
  public string? Zip { get; set; }
  public int? AddressTypeId { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? EntityAddressDataJson { get; set; }
  public string? LocationCode { get; set; }


  public static ActiveCarrierPickupLocation CreatePickupLocation(string fullAddress, int? countryId, int? cityId, int? areaId, string? streetAddress, decimal? latitude, decimal? longitude, string? streetAddress2, string? houseNo, string? buildingName, string? landmark, int? provinceId, int? pinCodeId, int? stateId, int carrierId, int activeCarrierId, string? customerServiceNo, string? entityAddressDataJson, ClientId clientId, string? locationName, string? phone, string? locationCode)
  {
    return new ActiveCarrierPickupLocation()
    {
      FullAddress = fullAddress,
      CustomerServiceNo = UtilityHelper.CleanPhoneNumber(customerServiceNo),
      CountryId = countryId,
      AreaId = areaId,
      CityId = cityId,
      StreetAddress = streetAddress,
      StreetAddress2 = streetAddress2,
      HouseNo = houseNo,
      BuildingName = buildingName,
      Landmark = landmark,
      ProvinceId = provinceId,
      PinCodeId = pinCodeId,
      StateId = stateId,
      EntityAddressDataJson = entityAddressDataJson,
      Latitude = UtilityHelper.TrimToPrecision(latitude),
      Longitude = UtilityHelper.TrimToPrecision(longitude),
      CarrierId = carrierId,
      ActiveCarrierId = activeCarrierId,
      ClientId = clientId,
      Phone = phone,
      LocationName = locationName,
      LocationCode = locationCode,

    };

  }
  public void UpdatepickLocation(
    string? locationName,
    string? phone,
    int? countryId,
    int? cityId,
    int? areaId,
    string? streetAddress,
    string? streetAddress2,
    decimal? longitude,
    decimal? latitude,
    string? customerServiceNo,
    string? houseNo,
    string? buildingName,
    string? landmark,
    string? zip, string? locationCode,string? fullAddress,string? entityAddressDataJson)
  {
    LocationName = locationName;
    Phone = phone;
    CountryId = countryId;
    CityId = cityId;
    AreaId = areaId;
    StreetAddress = streetAddress;
    StreetAddress2 = streetAddress2;
    Longitude = longitude;
    Latitude = latitude;
    CustomerServiceNo = UtilityHelper.CleanPhoneNumber(customerServiceNo);
    HouseNo = houseNo;
    BuildingName = buildingName;
    Landmark = landmark;
    Zip = zip;
    FullAddress = fullAddress;
    EntityAddressDataJson = entityAddressDataJson;
    if (string.IsNullOrEmpty(LocationCode))
    {
      LocationCode = locationCode;
    }
  }

  public static string GetCode(string locationName)
  {
    if (string.IsNullOrWhiteSpace(locationName))
      return "";

    // Take first 3 letters (uppercased), pad if shorter
    string prefix = new string(locationName
        .Where(char.IsLetter)
        .Take(3)
        .ToArray())
        .ToUpper()
        .PadRight(3, 'X'); // Fill with X if less than 3 letters

    // Generate 3 random digits
    Random rnd = new Random();
    string suffix = rnd.Next(0, 1000).ToString("D3"); // Always 3 digits

    return prefix + suffix;
  }



}
