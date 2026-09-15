using Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;

namespace Shipra.Backend.API.Application.DTOs.CountryUseCase.Response;
public class CountryResponseModel
{
  public int CountryId { get; set; } 
  public string? Code { get; set; } 
  public string? Name { get; set; } 
  public string? NameArabic { get; set; } 
}
