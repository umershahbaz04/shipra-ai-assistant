using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrierPickupLocation;
using Shipra.Backend.API.Core.Helper;

namespace Shipra.Backend.API.Application.Common.CustomBinder;
public static class AddressConversionHelper
{
  // Converts client-side address model to server-side address model
  public static OrderAddressModel? ConvertToOrderAddressModel(OrderAddressClientSideModel? source)
  {
    if (source == null)
      return null;

    return new OrderAddressModel
    {
      OrderAddressId = source.OrderAddressId,
      CountryId = source.CountryId,
      CityId = ExtractFirstElement(source.CityId),
      AreaId = ExtractFirstElement(source.AreaId),
      ProvinceId = ExtractFirstElement(source.ProvinceId),
      StateId = ExtractFirstElement(source.StateId),
      PinCodeId = ExtractFirstElement(source.PinCodeId),

      StreetAddress = source.StreetAddress,
      StreetAddress2 = source.StreetAddress2,
      HouseNo = source.HouseNo,
      BuildingName = source.BuildingName,
      Landmark = source.Landmark,
      Zip = source.Zip,
      AddressTypeId = 0, // Default address type, can be modified as per requirement
      Latitude = source.Latitude,
      Longitude = source.Longitude,

      CustomerName = source.CustomerName,
      Email = source.Email,
      Mobile1 = source.Mobile1,
      Mobile2 = source.Mobile2,
      EntityAddressDataJson = GetAddressJson(source),
      SelectedCarrierId = source.SelectedCarrierId
    };
  }

  public static List<OrderAddressValidateAgainstCarrierModel>? ConvertToOrderAddressValidateCarrierModel(List<ValidateCarrierOrderAddressClientSideModel>? source)
  {
    if (source == null)
      return null;

    return source.Select(item => new OrderAddressValidateAgainstCarrierModel
    {
      TrackingNo = item.TrackingNo,
      CountryId = item.CountryId,
      CityId = ExtractFirstElement(item.CityId),
      AreaId = ExtractFirstElement(item.AreaId),
      ProvinceId = ExtractFirstElement(item.ProvinceId),
      StateId = ExtractFirstElement(item.StateId),
      PinCodeId = ExtractFirstElement(item.PinCodeId),
      EntityAddressDataJson = GetAddressJson(new OrderAddressClientSideModel()
      {
        CityId = item.CityId,
        AreaId = item.AreaId,
        ProvinceId = item.ProvinceId,
        StateId = item.StateId,
        PinCodeId = item.PinCodeId,
      })
    }).ToList();
  }

  public static CreateActiveCarrierPickupLocationCommand? PickupLocationAddressModel(PickupLocationAddressClientSideModel? source)
  {
    if (source == null)
      return null;

    var dt = new OrderAddressClientSideModel()
    {
      CityId = source.CityId,
      AreaId = source.AreaId,
      ProvinceId = source.ProvinceId,
      StateId = source.StateId,
      PinCodeId = source.PinCodeId,
    };
    return new CreateActiveCarrierPickupLocationCommand
    {
      //OrderAddressId = source.OrderAddressId,
      CountryId = source.CountryId,
      CityId = ExtractFirstElement(source.CityId),
      AreaId = ExtractFirstElement(source.AreaId),
      ProvinceId = ExtractFirstElement(source.ProvinceId),
      StateId = ExtractFirstElement(source.StateId),
      PinCodeId = ExtractFirstElement(source.PinCodeId),

      StreetAddress = source.StreetAddress,
      StreetAddress2 = source.StreetAddress2,
      HouseNo = source.HouseNo,
      BuildingName = source.BuildingName,
      Landmark = source.Landmark,
      Zip = source.Zip,
      AddressTypeId = 0, // Default address type, can be modified as per requirement
      Latitude = source.Latitude,
      Longitude = source.Longitude,

      CustomerServiceNo = source.CustomerServiceNo,
      CarrierId = source.CarrierId,
      ActiveCarrierId = source.ActiveCarrierId,
      EntityAddressDataJson = GetAddressJson(dt),
      LocationName = source.LocationName,
      Phone = source.Phone,
      ActiveCarrierPickupLocationId= source.ActiveCarrierPickupLocationId

    };
  }

  // Extracts the first part of the string, checks if it's > 0, and returns 0 or -1
  public static int? ExtractFirstElement(string? value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      var firstPart = value.Contains("_") ? value.Split('_')[0] : value; // Get first part before "_"
      if (int.TryParse(firstPart, out int number))
      {
        return number > 0 ? number : -1;
      }
    }
    return null; // Default return if parsing fails
  }

  // Serializes order address details into JSON format
  public static string GetAddressJson(OrderAddressClientSideModel? orderAddress)
  {
    var jsonList = new List<Dictionary<string, object>>();

    CivilEntityHelper.AddToJsonList(jsonList, "CityId", orderAddress?.CityId);
    CivilEntityHelper.AddToJsonList(jsonList, "AreaId", orderAddress?.AreaId);
    CivilEntityHelper.AddToJsonList(jsonList, "ProvinceId", orderAddress?.ProvinceId);
    CivilEntityHelper.AddToJsonList(jsonList, "StateId", orderAddress?.StateId);
    CivilEntityHelper.AddToJsonList(jsonList, "PinCodeId", orderAddress?.PinCodeId);

    return Newtonsoft.Json.JsonConvert.SerializeObject(jsonList);
  }
}
