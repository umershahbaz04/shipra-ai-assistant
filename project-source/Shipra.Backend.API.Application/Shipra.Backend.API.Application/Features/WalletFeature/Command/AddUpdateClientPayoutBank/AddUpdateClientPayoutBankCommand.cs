using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Command.AddUpdateClientPayoutBank;
public class AddUpdateClientPayoutBankCommand : IRequest<ServiceResultDTO>
{
  public string? ClientPayoutBankId { get; set; }
  public string? BankName { get; set; }
  public string? AccountTitle { get; set; }
  public string? Iban { get; set; }
  public string? SwiftCode { get; set; }
  public string? BranchName { get; set; } 
}
public class AddUpdateClientPayoutBankCommandHandler : RequestHandlerBase<AddUpdateClientPayoutBankCommand, ServiceResultDTO>
{
  private readonly IWalletRepository _walletRepository;

  public AddUpdateClientPayoutBankCommandHandler(IWalletRepository walletRepository,IServiceProvider serviceProvider, ILogger<AddUpdateClientPayoutBankCommandHandler> logger) : base(serviceProvider, logger)
  {
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AddUpdateClientPayoutBankCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      ClientPayoutBank? oClientPayoutBank = await _walletRepository.GetClientPayoutBank(_currentUser.ClientId!);
      var isCreated = false;
      if (oClientPayoutBank is null)
      {
        oClientPayoutBank = ClientPayoutBank.CreateClientPayoutBank(_currentUser.ClientId!,request.BankName,request.AccountTitle,request.Iban,request.SwiftCode,request.BranchName);
        isCreated = await _walletRepository.CreateClientPayoutBank(oClientPayoutBank);

        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = oClientPayoutBank.ClientPayoutBankId!.Value!.ToString(),
          Message = "Payout bank created successfully."
        });
      }
      else
      {
        oClientPayoutBank.UpdateClientPayoutBank(request.BankName, request.AccountTitle, request.Iban, request.SwiftCode, request.BranchName);
        isCreated = await _walletRepository.UpdateClientPayoutBank(oClientPayoutBank);

        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Data = oClientPayoutBank.ClientPayoutBankId!.Value!.ToString(),
          Message = "Payout bank update successfully."
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
public class AddUpdateClientPayoutBankCommandValidator : AbstractValidator<AddUpdateClientPayoutBankCommand>
{
  public AddUpdateClientPayoutBankCommandValidator()
  {
    RuleFor(x => x.BankName).NotNull().NotEmpty();
    RuleFor(x => x.AccountTitle).NotNull().NotEmpty();
    RuleFor(x => x.Iban).NotNull().NotEmpty(); 
  } 
}
