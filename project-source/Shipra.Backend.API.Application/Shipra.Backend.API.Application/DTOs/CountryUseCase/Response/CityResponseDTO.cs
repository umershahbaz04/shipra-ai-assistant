using Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;

namespace Shipra.Backend.API.Application.DTOs.CountryUseCase.Response;
public class CityResponseModel
{
  public int CityId { get; set; }

  public string? Code { get; set; }

  public string? Name { get; set; }

  public string? NameArabic { get; set; }

  public int? CountryId { get; set; }

  public long? ExtendId { get; set; }
  public List<ZoneRequestModel>? Zones { get; set; } = new();
}
