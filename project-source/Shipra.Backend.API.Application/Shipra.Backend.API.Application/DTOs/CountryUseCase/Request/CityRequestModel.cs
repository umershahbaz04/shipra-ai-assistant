namespace Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;
public class CityRequestModel
{
  public long CityId { get; set; }

  public string? Code { get; set; }

  public string? Name { get; set; }

  public string? NameArabic { get; set; }

  public long? CountryId { get; set; }

  public long? ExtendId { get; set; }
}
