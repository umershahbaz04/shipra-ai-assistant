namespace Shipra.Backend.API.Application.DTOs.CarrierUseCase;
public class CarrierFeatureResponseModel
{
  public string CarrierFeatureId { get; set; } = null!;
  public int? CarrierId { get; set; }
  public string? Feature { get; set; }
  public bool? Active { get; set; }
}
