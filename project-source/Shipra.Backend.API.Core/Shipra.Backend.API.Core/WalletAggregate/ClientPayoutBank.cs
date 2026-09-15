using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.WalletAggregate;

public class ClientPayoutBank
{
  public ClientPayoutBankId? ClientPayoutBankId { get; set; }
  public string? BankName { get; set; }
  public string? AccountTitle { get; set; }
  public string? Iban { get; set; }
  public string? SwiftCode { get; set; }
  public string? BranchName { get; set; }
  public ClientId? ClientId { get; set; }

  public static ClientPayoutBank CreateClientPayoutBank(ClientId clientId,string? bankName, string? accountTitle, string? iban, string? swiftCode, string? branchName)
  {
    return new ClientPayoutBank()
    {
      ClientPayoutBankId = ClientPayoutBankId.New,
      ClientId = clientId,
      BankName = bankName,
      AccountTitle = accountTitle,
      Iban = iban,
      SwiftCode = swiftCode,
      BranchName = branchName,
    };
  }
  public void UpdateClientPayoutBank(string? bankName, string? accountTitle, string? iban, string? swiftCode, string? branchName)
  {
    BankName = bankName;
    AccountTitle = accountTitle;
    Iban = iban;
    SwiftCode = swiftCode;
    BranchName = branchName;
  }

}
public sealed record ClientPayoutBankId(Guid Value)
{
  public static ClientPayoutBankId New => new(Guid.NewGuid());
}
