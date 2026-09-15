namespace Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;
public class ZoneRequestModel
{
  public long ZoneId { get; set; }

  public string? Code { get; set; }

  public string? Name { get; set; }

  public string? NameArabic { get; set; }

  public long? CityId { get; set; }
}
