namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;

public class ClientOrderBoxResponseModel
{
  public int ClientOrderBoxId { get; set; }
  public string? BoxName { get; set; }
  public decimal? Length { get; set; }
  public decimal? Width { get; set; }
  public decimal? Height { get; set; }
  public decimal? Volume { get; set; }
  public bool? Active { get; set; }
  public bool? IsDefault { get; set; }
}
