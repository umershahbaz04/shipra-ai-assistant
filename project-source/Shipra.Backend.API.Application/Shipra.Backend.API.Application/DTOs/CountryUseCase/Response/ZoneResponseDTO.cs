using Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;

namespace Shipra.Backend.API.Application.DTOs.CountryUseCase.Response;
public class ZoneResponseModel
{
  public long ZoneId { get; set; }

  public string? Code { get; set; }

  public string? Name { get; set; }

  public string? NameArabic { get; set; }

  public string? CityId { get; set; }
  //public List<ZoneRequestModel>? Zones { get; set; } = new();
  public List<AreaRequestModel>? Areas { get; set; } = new();
}
