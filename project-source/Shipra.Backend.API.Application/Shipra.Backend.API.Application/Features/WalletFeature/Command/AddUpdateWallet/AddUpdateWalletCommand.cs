using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Command.AddUpdateWallet;
public class AddUpdateWalletCommand : IRequest<ServiceResultDTO>
{
  public decimal Balance { get; set; }
}
public class AddUpdateWalletCommandHandler : RequestHandlerBase<AddUpdateWalletCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IWalletRepository _walletRepository;

  public AddUpdateWalletCommandHandler(IClientRepository clientRepository,IWalletRepository walletRepository, IServiceProvider serviceProvider, ILogger<AddUpdateWalletCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AddUpdateWalletCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!.ToString());
      }

      var oWallet = await _walletRepository.GetWalletByClientId(_currentUser.ClientId!);
      var isCreated = false;
      if (oWallet is null)
      {
        oWallet = Wallet.CreateWallet(_currentUser.ClientId!, request.Balance);
        isCreated = await _walletRepository.CreateWallat(oWallet);
         
      }
      else
      {
        oWallet.AddBalance(request.Balance);
        isCreated = await _walletRepository.UpdateWallet(oWallet);
      }

      if (isCreated)
      {
        #region create transactio
        string transNo = Transaction.GetTransactionNo(client.ClientIdentifier);
        Transaction transaction = Transaction.CreateTransaction(transNo, $"Top up with amount {request.Balance}.", _currentUser.ClientId, oWallet.WalletId, (int)EnumTransactionTypeLookup.CustomerCredit, request.Balance, 0);

        isCreated = await _walletRepository.CreateTransaction(transaction);
        #endregion


        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = oWallet.WalletId!.Value!.ToString(),
          Message = "Wallet update successfully."
        });
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
public class AddUpdateWalletCommandValidator : AbstractValidator<AddUpdateWalletCommand>
{
  public AddUpdateWalletCommandValidator()
  {
    RuleFor(x => x.Balance).NotNull().NotEmpty().GreaterThan(0);
  }
}
