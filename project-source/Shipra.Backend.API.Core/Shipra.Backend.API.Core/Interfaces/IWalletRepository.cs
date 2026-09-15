using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IWalletRepository
{
  Task<bool> CreatePaymentLink(PaymentLink paymentLink);
  Task<PaymentLink?> GetPaymentLinkByPaymentLinkId(PaymentLinkId paymentLinkId);
  Task<PaymentLink?> GetPaymentLinkByOrderId(OrderId orderId);
  Task<List<PaymentLinkStatusLookup>?> GetAllPaymentLinkStatusLookup();
  Task<Wallet?> GetWalletByClientId(ClientId clientId);
  Task<bool> CreateWallat(Wallet oWallet);
  Task<bool> UpdateWallet(Wallet oWallet);
  Task<ClientPayoutBank?> GetClientPayoutBank(ClientId clientId);
  Task<bool> CreateClientPayoutBank(ClientPayoutBank oWallet);
  Task<bool> UpdateClientPayoutBank(ClientPayoutBank oWallet);
  Task<dynamic> GetAllClientPayoutBank(string clientId);
  Task<dynamic> GetAllClientWallets(string clientIdv);
  Task<dynamic> GetAllPayouts(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, int? PayoutStatusId = 0);
  Task<dynamic> GetAllPayoutStatusHistory(string clientId, string? payoutId);
  Task<bool> UpdatePaymentLink(PaymentLink paymentLink);
  Task<bool> CreateTransaction(Transaction transaction);
  Task<bool> CreatePayout(Payout payout);
  Task<bool> UpdatePayout(Payout payout);
  Task<bool> CreatePayoutStatusHistory(PayoutStatusHistory payoutStatusHistory);
  Task<List<PaymentLink>> GetPaymentLinksByPayoutId(string? payoutId, ClientId? clientId);
  Task<Payout?> GetPayoutById(string? payoutId, ClientId? clientId);
  Task<Payout> CheckTransactionRef(string? transactionRef, ClientId? clientId);
  Task<bool> CreatePayoutFile(PayoutFile payoutFile);
  Task<List<PayoutFile>> GetAllPayoutFiles(PayoutId payoutId, ClientId clientId);
  Task<dynamic> GetAllTransaction(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, int? transactionTypeId);
  Task<dynamic> GetCalucaltedBalance(string? clientIdStr);
  Task<dynamic> GetAllOrdersByPayoutId(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string sortDir, string clientId, string? payoutId);
  //Task<decimal?> GetCalucaltedBalance(string? clientIdStr);
}
