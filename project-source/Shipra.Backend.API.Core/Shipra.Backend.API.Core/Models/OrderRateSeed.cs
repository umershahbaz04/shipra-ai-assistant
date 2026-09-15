namespace Shipra.Backend.API.Core.Models;
public sealed class OrderRateSeed
{
  public string OrderNo { get; set; } = "";
  public decimal Weight { get; set; }
  public decimal Amount { get; set; }
  public int? OriginTypeId { get; set; }
  public int? From { get; set; }
  public int? To { get; set; }

  public StoreAddressDto? StoreAddress { get; set; } = new();
  public OrderAddressDto? OrderAddress { get; set; } = new();
}
public class OrderAddressDto
{
  public long OrderAddressId { get; set; }
  public int? CountryId { get; set; }
  public int? CityId { get; set; }
  public int? AreaId { get; set; }
  public int? ProvinceId { get; set; }
  public int? PinCodeId { get; set; }
  public int? StateId { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? EntityAddressDataJson { get; set; }
}
public class StoreAddressDto
{
  public int? StoreAddressId { get; set; }
  public int? CountryId { get; set; }
  public int? CityId { get; set; }
  public int? AreaId { get; set; }
  public int? ProvinceId { get; set; }
  public int? PinCodeId { get; set; }
  public int? StateId { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
}
