using System.Globalization;
using System.Transactions;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.WalletAggregate;

public class Transaction
{
  public static Transaction CreateTransaction(string transactionNo, string? description, ClientId? clientId, WalletId? walletId, int transactionTypeId, decimal credit, decimal debit)
  {
    return new Transaction
    {
      TransactionId = TransactionId.New,
      TransactionNo = transactionNo,
      Description = description,
      ClientId = clientId,
      WalletId = walletId,
      Credit = credit,
      Debit = debit,
      TransactionTypeId = transactionTypeId,
      CreatedOn = DateTime.UtcNow
    };
  }

  public TransactionId? TransactionId { get; set; }
  public string TransactionNo { get; set; } = null!;
  public string? Description { get; set; }
  public ClientId? ClientId { get; set; }
  public WalletId? WalletId { get; set; }
  public int TransactionTypeId { get; set; }
  public decimal Debit { get; set; }
  public decimal Credit { get; set; }
  public DateTime CreatedOn { get; set; }

  public static string GetTransactionNo(int? clientIdentifier)
  { 
    string rNo = clientIdentifier + DateTime.Now.ToString("MMddfff");
    return rNo;
  }
}
public sealed record TransactionId(Guid Value)
{
  public static TransactionId New => new(Guid.NewGuid());
}
