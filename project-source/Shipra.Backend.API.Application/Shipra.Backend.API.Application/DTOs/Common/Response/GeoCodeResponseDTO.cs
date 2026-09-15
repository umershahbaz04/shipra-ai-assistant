using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs.Common.Response;

public class GeoCodeResponseDTO : BaseGetResponseDTO
{
  public decimal Longitude { get; set; }
  public decimal Latitude { get; set; }
  public decimal Geography { get; set; }
}
