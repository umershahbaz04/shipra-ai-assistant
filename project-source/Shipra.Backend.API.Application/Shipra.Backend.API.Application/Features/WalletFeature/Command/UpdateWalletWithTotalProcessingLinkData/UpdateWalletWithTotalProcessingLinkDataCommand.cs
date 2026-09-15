using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Command.UpdateWalletWithTotalProcessingLinkData;
public class UpdateWalletWithTotalProcessingLinkDataCommand : IRequest<ServiceResultDTO>
{
  public decimal Ammount { get; set; }
  public string? OrderNo { get; set; }
}

public class UpdateWalletWithTotalProcessingLinkDataCommandHandler : RequestHandlerBase<UpdateWalletWithTotalProcessingLinkDataCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IWalletRepository _walletRepository;

  public UpdateWalletWithTotalProcessingLinkDataCommandHandler(IClientRepository clientRepository, IOrderRepository orderRepository, IWalletRepository walletRepository, IServiceProvider serviceProvider, ILogger<UpdateWalletWithTotalProcessingLinkDataCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _orderRepository = orderRepository;
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateWalletWithTotalProcessingLinkDataCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
      }

      #region get order by no and update payment link
      var oOrder = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (oOrder is null)
      {
        throw new EntityNotFoundException("Order ", request.OrderNo!);
      }
      ///check if payment is already paid or not if paid then return else add into wallet 
      PaymentLink? paymentLink = await _walletRepository.GetPaymentLinkByOrderId(oOrder.OrderId!);
      if (paymentLink is null)
      {
        throw new EntityNotFoundException("PaymentLink ", request.OrderNo!);
      }
      if (paymentLink.PaymentLinkStatusId == (int)EnumPaymentLinkStatus.Unpaid)
      {
        DateTime schedualDate = DateTime.UtcNow.AddDays(client.PayoutScheduleSettings.GetValueOrDefault(7));
        paymentLink.UpdatePaymentSuccess((int)EnumPaymentLinkStatus.Paid, schedualDate);
        await _walletRepository.UpdatePaymentLink(paymentLink);
      }
      else
      { 
        serviceResult = new ServiceResultDTO(new
        {
          Message = "Amount with link already paid."
        });
        serviceResult.IsSuccess = true;
        return serviceResult;
      }

      #endregion
      #region update order
      oOrder.TotalProcessingUpdateOrderWithPrepaidStatusAndPaid();
      var updatedOrder = await _orderRepository.UpdateOrder(oOrder); 
      #endregion
      #region create wallet
      var oWallet = await _walletRepository.GetWalletByClientId(_currentUser.ClientId!);
      var isCreated = oWallet != null;
      if (oWallet is null)
      {
        oWallet = Wallet.CreateWalletWithTotalProcessing(_currentUser.ClientId!, request.Ammount);
        isCreated = await _walletRepository.CreateWallat(oWallet);
      }
      else
      {
        oWallet.UpdatePayemntSuccessBalance(request.Ammount);
        await _walletRepository.UpdateWallet(oWallet); 
      }
      #endregion

      if (isCreated)
      {
        #region create transactio
        string transNo = Transaction.GetTransactionNo(client.ClientIdentifier);
        Transaction transaction = Transaction.CreateTransaction(transNo, $"{oOrder.OrderNo}, Paid via link amount {request.Ammount}.", _currentUser.ClientId, oWallet.WalletId, (int)EnumTransactionTypeLookup.Paymentlinkcredit, request.Ammount, 0);

        isCreated = await _walletRepository.CreateTransaction(transaction);
        #endregion

        if (isCreated)
        {
          serviceResult = new ServiceResultDTO(new BaseResponseDto
          {
            Data = oWallet.WalletId!.Value!.ToString(),
            Message = "Wallet update successfully."
          });
        }
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
