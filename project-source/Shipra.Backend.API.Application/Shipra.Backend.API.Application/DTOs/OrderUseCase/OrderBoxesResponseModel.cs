namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;

public class OrderBoxesResponseModel
{
  public string? OrderBoxId { get; set; }
  public int? ClientOrderBoxId { get; set; }
  public string? OrderId { get; set; }
  public decimal? Length { get; set; }
  public decimal? Width { get; set; }
  public decimal? Height { get; set; }
  public decimal? Volume { get; set; }
}
