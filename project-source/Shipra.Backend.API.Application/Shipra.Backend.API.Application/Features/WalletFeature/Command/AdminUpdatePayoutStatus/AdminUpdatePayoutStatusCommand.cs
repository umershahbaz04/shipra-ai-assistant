using Amazon.Runtime.Internal.Util;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Command.AdminUpdatePayoutStatus;
public class AdminUpdatePayoutStatusCommand : IRequest<ServiceResultDTO>
{
  public string? PayoutId { get; set; }
  public int PayoutStatusId { get; set; }
  public string? UpdatedByName { get; set; }
  public string? TransactionRef { get; set; }
  public string? Comment { get; set; }
  public string? FilePath { get; set; }
  public decimal? TransactionCharges { get; set; }
}
public class AdminUpdatePayoutStatusCommandHandler : RequestHandlerBase<AdminUpdatePayoutStatusCommand, ServiceResultDTO>
{
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IWalletRepository _walletRepository;

  public AdminUpdatePayoutStatusCommandHandler(IEmployeeRepository employeeRepository, IClientRepository clientRepository, IWalletRepository walletRepository, IServiceProvider serviceProvider, ILogger<AdminUpdatePayoutStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _employeeRepository = employeeRepository;
    _clientRepository = clientRepository;
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AdminUpdatePayoutStatusCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);

      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!);
      }

      Payout? payout = await _walletRepository.CheckTransactionRef(request.TransactionRef, _currentUser.ClientId);
      if (payout is not null)
      {
        serviceResult.Errors = new Dictionary<string, string[]>();
        serviceResult.CreateError("AlreadyExist", new string[] { $"Payout already exist with Transaction ref.{request.TransactionRef}" });
        return serviceResult;
      }


      #region payout,history
      payout = await _walletRepository.GetPayoutById(request.PayoutId, _currentUser.ClientId!);
      if (payout is null)
      {
        throw new EntityNotFoundException("Payout ", request!.PayoutId!);
      }

      if (payout is not null && payout!.PayoutStatusId == (int)EnumPayoutStatus.Completed)
      {
        serviceResult.Errors = new Dictionary<string, string[]>();
        serviceResult.CreateError("AlreadyPaid", new string[] { $"Payout already paid aginst Transaction ref:{request.TransactionRef}" });
        return serviceResult;
      }
      var tranSactionCharges = request.TransactionCharges.GetValueOrDefault(payout!.TransactionCharges.GetValueOrDefault());

      if (tranSactionCharges == 0 && request!.PayoutStatusId == (int)EnumPayoutStatus.Completed)
      {
        serviceResult.Errors = new Dictionary<string, string[]>();
        serviceResult.CreateError("TransactionChargesRequired", new string[] { $"Please add transaction charges." });
        return serviceResult;
      }


      if (payout!.PayoutStatusId == request.PayoutStatusId)
      {
        serviceResult.Errors = new Dictionary<string, string[]>();
        serviceResult.CreateError("AlreadySameStatus", new string[] { $"Cannot be update same status." });
        return serviceResult;
      }
      #region upload payout file
      if (!string.IsNullOrEmpty(request.FilePath))
      {
        PayoutFile payoutFile = PayoutFile.Create(payout.PayoutId, request.FilePath);
        bool isC = await _walletRepository.CreatePayoutFile(payoutFile);
      }
      #endregion
      payout.UpdatePayoutStatus(request.PayoutStatusId, request.TransactionRef!);
      await _walletRepository.UpdatePayout(payout);
      #region create history
      //string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

      PayoutStatusHistory payoutStatusHistory = PayoutStatusHistory.CreatePayoutStatusHistory(new PayoutId(new Guid(request.PayoutId!)), request!.PayoutStatusId, request.Comment, request.UpdatedByName);
      await _walletRepository.CreatePayoutStatusHistory(payoutStatusHistory);
      #endregion
      #endregion

      //get only paid links which already not payout
      #region if completed then update release data
      if (request.PayoutStatusId == (int)EnumPayoutStatus.Completed)
      {
        //update payout amount 

        payout.UpdateTransactionChargesAndDeductFromAmount(tranSactionCharges);
        await _walletRepository.UpdatePayout(payout);

        payoutStatusHistory = PayoutStatusHistory.CreatePayoutStatusHistory(new PayoutId(new Guid(request.PayoutId!)), null, $"Transaction charges of {tranSactionCharges} have been added.", request.UpdatedByName);
        await _walletRepository.CreatePayoutStatusHistory(payoutStatusHistory);


        List<PaymentLink> paymentLinks = await _walletRepository.GetPaymentLinksByPayoutId(request.PayoutId, _currentUser.ClientId);
        if (paymentLinks.Count > 0)
        {
          #region update paymentlink
          foreach (var paymentLink in paymentLinks)
          {
            if (paymentLink != null)
            {
              paymentLink.UpdatePayoutPaymentReleaseDate();
              await _walletRepository.UpdatePaymentLink(paymentLink);
            }
          }

          #endregion
        }
      }
      #endregion
      serviceResult = new ServiceResultDTO(new BaseResponseDto
      {
        Message = "Payout udpated successfully."
      });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class AdminUpdatePayoutStatusCommandValidator : AbstractValidator<AdminUpdatePayoutStatusCommand>
{
  public AdminUpdatePayoutStatusCommandValidator()
  {
    RuleFor(x => x.PayoutId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(x => x.PayoutStatusId).NotNull().NotEmpty().GreaterThanOrEqualTo(0);
    RuleFor(x => x.UpdatedByName).NotNull().NotEmpty();
    When(v => v.PayoutStatusId == (int)EnumPayoutStatus.Completed, () =>
    {
      RuleFor(x => x.TransactionRef).NotNull().NotEmpty();
    });
    RuleFor(x => x.FilePath).NotNull().NotEmpty();
  }
}
