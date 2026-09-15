namespace Shipra.Backend.API.Application.DTOs.Common.Response;

public class AddressResponseDTO
{
  public long AddressId { get; set; }
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
  public string? FullAddress { get; set; }
  public string? Zip { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
}
