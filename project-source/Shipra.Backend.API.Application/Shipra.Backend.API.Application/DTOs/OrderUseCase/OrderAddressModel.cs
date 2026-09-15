using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;

public class OrderAddressValidateAgainstCarrierModel : AddressRequestDTO
{
  public string? TrackingNo { get; set; }
  public string? EntityAddressDataJson { get; set; }
}
public class OrderAddressValidateAgainstCarrierWithNameModel  
{
  public string? OrderNo { get; set; }
  public string? Country { get; set; }
  public string? City { get; set; }
  public string? Area { get; set; }   
  public string? Province { get; set; }
  public string? PinCode { get; set; }
  public string? State { get; set; } 
  public string? StreetAddress { get; set; } 
}
public class OrderAddressModel : AddressRequestDTO
{
  public long OrderAddressId { get; set; }
  public string? CustomerName { get; set; }
  public string? Email { get; set; }
  public string? Mobile1 { get; set; }
  public string? Mobile2 { get; set; }
  public string? EntityAddressDataJson { get; set; }
  public int? SelectedCarrierId { get; set; }
  public string? CustomerFullAddress { get; set; }
}
public class OrderAddressResponseModel
{
  public long OrderAddressId { get; set; }
  public string? CustomerName { get; set; }
  public string? CustomerFullAddress { get; set; }
  public string? Email { get; set; }
  public string? Mobile1 { get; set; }
  public string? Mobile2 { get; set; }
  public int? Country { get; set; }
  public int? City { get; set; }
  public int? Area { get; set; }
  public string? StreetAddress { get; set; }
  public string? StreetAddress2 { get; set; }
  public string? HouseNo { get; set; }
  public string? BuildingName { get; set; }
  public string? Landmark { get; set; }
  public int? Province { get; set; }
  public int? PinCode { get; set; }
  public int? State { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
}
public class OrderAddressWithEntityResponseModel
{
  public long OrderAddressId { get; set; }
  public string? CustomerName { get; set; }
  public string? CustomerFullAddress { get; set; }
  public string? Email { get; set; }
  public string? Mobile1 { get; set; }
  public string? Mobile2 { get; set; }
  public int? Country { get; set; }
  public string? City { get; set; }
  public string? Area { get; set; }
  public string? StreetAddress { get; set; }
  public string? StreetAddress2 { get; set; }
  public string? HouseNo { get; set; }
  public string? BuildingName { get; set; }
  public string? Landmark { get; set; }
  public string? Province { get; set; }
  public string? PinCode { get; set; }
  public string? State { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
  public string? EntityAddressDataJson { get; set; }
  public int? SelectedCarrierId {  get; set; }
}
