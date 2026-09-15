using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Base.Response;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Command.CreatePayoutRequest;
public class CreatePayoutRequestCommand : IRequest<ServiceResultDTO>
{
  //schedual date
  public DateTime? CreatedFrom { get; set; }
  public DateTime? CreatedTo { get; set; }
  public string? CreatedByName { get; set; }
}
public class CreatePayoutRequestCommandHandler : RequestHandlerBase<CreatePayoutRequestCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IClientRepository _clientRepository;

  public CreatePayoutRequestCommandHandler( IEmployeeRepository employeeRepository, IOrderRepository orderRepository, IWalletRepository walletRepository, IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<CreatePayoutRequestCommandHandler> logger) : base(serviceProvider, logger)
  { 
    _employeeRepository = employeeRepository;
    _orderRepository = orderRepository;
    _walletRepository = walletRepository;
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreatePayoutRequestCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!);
      }
      ClientPayoutBank? clientPayoutBank = await _walletRepository.GetClientPayoutBank(_currentUser.ClientId!);
      if (clientPayoutBank is null)
      {
        throw new EntityNotFoundException("ClientPayoutBank ", _currentUser.ClientIdStr!);
      } 
      DateTime? scehdualFrom = request.CreatedFrom;
      DateTime? scehdualTo = request.CreatedTo;

      //set null
      request.CreatedFrom = null;
      request.CreatedTo = null;

      #region create wallet
      var oWallet = await _walletRepository.GetWalletByClientId(_currentUser.ClientId!);
      if (oWallet is null)
      {
        throw new EntityNotFoundException("Wallet ", _currentUser.ClientIdStr!);
      }
      #endregion
      var trackingPageUrl = _configuration.GetValue<string>("TrackingPageUrl");

      //get only paid links which already not payout
      List<AllOrderPaymentLinkResponseModel> paymentLinks = await _orderRepository.GetAllOrderPaymentLinks(request.CreatedFrom, request.CreatedTo, 0, 10000000, "", 0, "desc", _currentUser.ClientIdStr!, (int)EnumPaymentLinkStatus.Paid, true, trackingPageUrl!, scehdualFrom, scehdualTo);

      if (paymentLinks.Count > 0)
      {
        decimal? totalAmmount = paymentLinks.Sum(x => x.Amount);

        var paymentProcessingCharges = client.PaymentProcessingCharges.GetValueOrDefault(ApplicationConstants.ClientDefaultTransactionCharges); //default charges for client

        Payout payout = Payout.CreatePayout(Payout.GetRefNo(), totalAmmount, "AED", (int)EnumPayoutStatus.Pending, clientPayoutBank.BankName, clientPayoutBank.AccountTitle, clientPayoutBank.Iban, clientPayoutBank.SwiftCode, clientPayoutBank.BranchName, _currentUser.ClientId!, paymentProcessingCharges);
        bool ispayoutCreaded = await _walletRepository.CreatePayout(payout);
        #region update paymentlink
        foreach (var item in paymentLinks)
        {
          var paymentLink = await _walletRepository.GetPaymentLinkByPaymentLinkId(new PaymentLinkId(new Guid(item.PaymentLinkId!)));
          if (paymentLink != null)
          {
            paymentLink.UpdatePayout(payout.PayoutId);
            await _walletRepository.UpdatePaymentLink(paymentLink);
          }
        }

        #region create history
        string? createdByName = string.IsNullOrEmpty(request.CreatedByName) ? await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId) : request.CreatedByName;

        PayoutStatusHistory payoutStatusHistory = PayoutStatusHistory.CreatePayoutStatusHistory(payout.PayoutId, (int)EnumPayoutStatus.Pending, "Payout request generated.", createdByName);
        await _walletRepository.CreatePayoutStatusHistory(payoutStatusHistory);
        #endregion

        #region update wallet and create history  
        oWallet.DeductBalanceOnPayoutRequest(totalAmmount.GetValueOrDefault());
        await _walletRepository.UpdateWallet(oWallet);

        #region create transactio
        string transNo = Transaction.GetTransactionNo(client.ClientIdentifier);
        Transaction transaction = Transaction.CreateTransaction(transNo, $"The wallet has been debited with an amount of {totalAmmount} against the payout reference: {payout.PayoutRef}.", _currentUser.ClientId, oWallet.WalletId, (int)EnumTransactionTypeLookup.PayoutRequestDebit, 0, totalAmmount.GetValueOrDefault());

        await _walletRepository.CreateTransaction(transaction);
        #endregion
        #endregion

        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Message = "Payout request created successfully"
        });
        #endregion
      }
      else
      {
        serviceResult.CreateError("NotFound", new string[] { "No order links found" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
