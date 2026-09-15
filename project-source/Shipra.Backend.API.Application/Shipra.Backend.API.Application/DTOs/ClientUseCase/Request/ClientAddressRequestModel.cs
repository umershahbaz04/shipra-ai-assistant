using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.ClientUseCase.Request;
public class ClientAddressRequestModel
{
  public int? AddressId { get; set; }
  public int? CountryId { get; set; }
  public int? CityId { get; set; }
  public int? AreaId { get; set; }
  public string? StreetAddress { get; set; }
  public string? StreetAddress2 { get; set; }
  public string? HouseNo { get; set; }
  public string? BuildingName { get; set; }
  public string? Landmark { get; set; }
  public string? WorkEmail { get; set; }
  public int? ProvinceId { get; set; }
  public int? PinCodeId { get; set; }
  public int? StateId { get; set; }
  public string? FullAddress { get; set; }
  public string? Zip { get; set; }
  public int? AddressTypeId { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
}
