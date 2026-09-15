using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.WalletAggregate;

public class PaymentLink
{
  public PaymentLinkId? PaymentLinkId { get; set; }
  public string? PaymentLinkUrl { get; set; }
  public string? PaymentLinkPdf { get; set; }
  public string? ServiceUUId { get; set; }
  public OrderId? OrderId { get; set; }
  public int? PaymentLinkStatusId { get; set; }
  public decimal? Amount { get; set; }
  public DateTime? ScheduledPayoutDate { get; set; }
  public DateTime? PaidOn { get; set; }
  public DateTime CreatedOn { get; set; }
  public DateTime? PaymentReleaseDate { get; set; }
  public PayoutId? PayoutId { get; set; }
  public ClientId? ClientId { get; set; }
  public static PaymentLink CreatePaymentLink(string? paymentLinkUrl, string? serviceId, string? paymentLinkPdf, OrderId? orderId, int? paymentLinkStatusId, ClientId clientId,decimal? amount)
  {
    return new PaymentLink()
    {
      PaymentLinkId = PaymentLinkId.New,
      PaymentLinkUrl = paymentLinkUrl,
      PaymentLinkPdf = paymentLinkPdf,
      ServiceUUId = serviceId,
      OrderId = orderId,
      ClientId = clientId,
      PaymentLinkStatusId = paymentLinkStatusId,
      CreatedOn = DateTime.UtcNow,
      Amount = amount

    };
  }

  public void UpdatePaymentSuccess(int? paymentLinkStatusId, DateTime? scheduledPayoutDate)
  {
    PaymentLinkStatusId = paymentLinkStatusId;
    ScheduledPayoutDate = scheduledPayoutDate;
    PaidOn = DateTime.UtcNow;
  }

  public void UpdatePayout(PayoutId? payoutId)
  {
    PayoutId = payoutId;
  } 
  public void UpdatePayoutPaymentReleaseDate()
  {
    PaymentReleaseDate = DateTime.UtcNow;
  }
}
public sealed record PaymentLinkId(Guid Value)
{
  public static PaymentLinkId New => new(Guid.NewGuid());
}
