namespace Shipra.Backend.API.Core.WalletAggregate;

public class PayoutStatusHistory
{
  public PayoutStatusHistoryId? PayoutStatusHistoryId { get; set; }
  public PayoutId? PayoutId { get; set; }
  public int? PayoutStatusId { get; set; }
  public string? Comment { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? CreatedBy { get; set; }

  public static PayoutStatusHistory CreatePayoutStatusHistory(PayoutId? payoutId, int? payoutStatusId, string? comment, string? createdBy)
  {
    return new PayoutStatusHistory()
    {
      PayoutStatusHistoryId = PayoutStatusHistoryId.New,
      PayoutStatusId = payoutStatusId,
      Comment = comment,
      PayoutId = payoutId,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy
    };
  }
}
public sealed record PayoutStatusHistoryId(Guid Value)
{
  public static PayoutStatusHistoryId New => new(Guid.NewGuid());
}
