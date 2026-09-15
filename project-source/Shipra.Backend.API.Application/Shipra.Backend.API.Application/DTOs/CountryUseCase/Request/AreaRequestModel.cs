namespace Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;
public class AreaRequestModel
{
  public long AreaId { get; set; }

  public string? Code { get; set; }

  public string? Name { get; set; }

  public string? NameArabic { get; set; }

  public long ZoneId { get; set; }

  public long CityId { get; set; }

  public decimal? Latitude { get; set; }

  public decimal? Longitude { get; set; }

  public long? ExtendId { get; set; }
}
