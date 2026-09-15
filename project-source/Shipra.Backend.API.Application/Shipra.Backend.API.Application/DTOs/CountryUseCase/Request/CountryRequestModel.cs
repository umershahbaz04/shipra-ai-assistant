namespace Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;
public class CountryRequestModel
{
  public long CountryId { get; set; }

  public string? Code { get; set; }

  public string? Name { get; set; }

  public string? NameArabic { get; set; }
}
