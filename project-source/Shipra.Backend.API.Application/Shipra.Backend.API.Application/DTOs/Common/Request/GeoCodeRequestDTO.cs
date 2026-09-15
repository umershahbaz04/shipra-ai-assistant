namespace Shipra.Backend.API.Application.DTOs.Common.Request;

public class GeoCodeRequestDTO : BaseCreateRequestDto
{
  public decimal Longitude { get; set; }
  public decimal Latitude { get; set; }
  public decimal Geography { get; set; }
}
