namespace Shipra.Backend.API.Application.DTOs.UnMappedOrderUseCase;
public class UnMappedOrderAddressModel
{
  public long OrderAddressId { get; set; }
  public string? CustomerName { get; set; }
  public string? Email { get; set; }
  public string? Mobile1 { get; set; }
  public string? Mobile2 { get; set; }
  public string? EntityAddressDataJson { get; set; }
  public int? SelectedCarrierId { get; set; }
  public string? CustomerFullAddress { get; set; }
  public string? Country { get; set; }
  public string? CountryCodeISO3 { get; set; }
  public string? City { get; set; }
  public string? Area { get; set; }
  public string? StreetAddress { get; set; }
  public string? StreetAddress2 { get; set; }
  public string? HouseNo { get; set; }
  public string? BuildingName { get; set; }
  public string? Landmark { get; set; }
  public string? Province { get; set; }
  public string? PinCodeId { get; set; }
  public string? State { get; set; }
  public string? Zip { get; set; }
  public int? AddressTypeId { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
}
