using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.WalletAggregate;

public class Payout
{
  public PayoutId? PayoutId { get; set; }
  public ClientId? ClientId { get; set; }
  public string? PayoutRef { get; set; }
  public decimal? Amount { get; set; }
  public decimal? TransactionCharges { get; set; }
  public decimal? ServiceCharges { get; set; }
  public decimal? Outstanding { get; set; }
  public string? Currency { get; set; }
  public int? PayoutStatusId { get; set; }
  public string? TransactionRef { get; set; }
  public DateTime? CreatedOn { get; set; }
  public string? BankName { get; set; }
  public string? AccountTitle { get; set; }
  public string? Iban { get; set; }
  public string? SwiftCode { get; set; }
  public string? BranchName { get; set; }

  public static Payout CreatePayout(string? payoutRef, decimal? amount, string? currency, int? payoutStatusId, string? bankName, string? accountTitle, string? iban, string? swiftCode, string? branchName, ClientId? clientId, decimal paymentProcessingChargesPer)
  {
    decimal serviceCharges = (amount ?? 0) * paymentProcessingChargesPer / 100;
    decimal? outstandingAmount = (amount ?? 0) - serviceCharges;

    return new Payout()
    {
      PayoutId = PayoutId.New,
      PayoutRef = payoutRef,
      Currency = currency,
      PayoutStatusId = payoutStatusId,
      CreatedOn = DateTime.UtcNow,
      BankName = bankName,
      AccountTitle = accountTitle,
      Iban = iban,
      SwiftCode = swiftCode,
      BranchName = branchName,
      ClientId = clientId,
      Amount = amount,
      TransactionCharges = 0,
      ServiceCharges = serviceCharges,
      Outstanding = outstandingAmount
    };
  }
  public static string GetRefNo()
  {
    var random = new Random();
    return random.Next(0, 1000000000).ToString(); // Generate a random integer between 0 and 999,999,999
  }

  public void UpdatePayoutStatus(int payoutStatusId, string transactionRef)
  {
    PayoutStatusId = payoutStatusId;
    TransactionRef = transactionRef;
  }

  public void UpdateTransactionChargesAndDeductFromAmount(decimal? transactionCharges)
  { 
    Outstanding = (Outstanding ?? 0) - transactionCharges;  
    TransactionCharges = transactionCharges;
  }
  public void UpdateTransactionCharges(decimal? transactionCharges)
  {
    TransactionCharges = transactionCharges;
  }
}
public sealed record PayoutId(Guid Value)
{
  public static PayoutId New => new(Guid.NewGuid());
}
