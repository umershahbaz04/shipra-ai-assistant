namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.CreateCarrierPaymentSettlement;

public class CreateCarrierPaymentSettlementRequestModel
{
  public string? OrderNo { get; set; }
  public decimal? Amount { get; set; }
  public string? PaymentRef { get; set; }
  public DateTime? PaymentDate { get; set; } 
}
