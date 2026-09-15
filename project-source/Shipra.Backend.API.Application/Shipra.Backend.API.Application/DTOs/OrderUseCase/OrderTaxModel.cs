namespace Shipra.Backend.API.Application.DTOs.OrderUseCase;

public class OrderTaxModel
{
  public string? OrderTaxId { get; set; }
  public int? ClientTaxId { get; set; }
  public decimal? TaxValue { get; set; }
}
