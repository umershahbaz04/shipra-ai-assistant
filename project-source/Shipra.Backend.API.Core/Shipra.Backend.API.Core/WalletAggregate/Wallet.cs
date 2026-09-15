using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;

namespace Shipra.Backend.API.Core.WalletAggregate;
public class Wallet
{
  public WalletId? WalletId { get; set; } 
  public ClientId? ClientId { get; set; } 
  public decimal AvailableBalance { get; set; } 
  public decimal CurrentBalance { get; set; }

  public static Wallet CreateWallet(ClientId clientId, decimal balance)
  {
    return new Wallet()
    {
      WalletId = WalletId.New,
      ClientId = clientId,
      AvailableBalance = balance,
      CurrentBalance = balance,
    };
  } 
  
  public static Wallet CreateWalletWithTotalProcessing(ClientId clientId, decimal balance)
  {
    return new Wallet()
    {
      WalletId = WalletId.New,
      ClientId = clientId,
      AvailableBalance = 0,
      CurrentBalance = balance,
    };
  }
  public void AddBalance(decimal balance)
  {
    AvailableBalance += balance;
    CurrentBalance += balance;
  }

  public void DeductBalanceOnPayoutRequest(decimal balance)
  { 
    CurrentBalance = CurrentBalance - balance;
  } 
  public void DeductBalanceOnAssignOrder(decimal balance)
  {
    AvailableBalance = AvailableBalance - balance;
    CurrentBalance = CurrentBalance - balance;
  }

  public void UpdatePayemntSuccessBalance(decimal balance)
  { 
    CurrentBalance += balance;
  }
}
public sealed record WalletId(Guid Value)
{
  public static WalletId New => new(Guid.NewGuid());
}
