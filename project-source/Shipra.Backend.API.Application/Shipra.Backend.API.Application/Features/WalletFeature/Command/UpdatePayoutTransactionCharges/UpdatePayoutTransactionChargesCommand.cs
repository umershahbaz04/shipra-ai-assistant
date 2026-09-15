using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Command.UpdatePayoutTransactionCharges;
public class UpdatePayoutTransactionChargesCommand : IRequest<ServiceResultDTO>
{
  public string? PayoutId { get; set; }
  public string? Comment { get; set; }
  public string? UpdatedByName { get; set; }
  public decimal? TransactionCharges { get; set; }
}
public class UpdatePayoutTransactionChargesCommandHandler : RequestHandlerBase<UpdatePayoutTransactionChargesCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IWalletRepository _walletRepository;

  public UpdatePayoutTransactionChargesCommandHandler(IClientRepository clientRepository, IWalletRepository walletRepository, IServiceProvider serviceProvider, ILogger<UpdatePayoutTransactionChargesCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _walletRepository = walletRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdatePayoutTransactionChargesCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      //var client = await _clientRepository.GetClientById(_currentUser.ClientId!);

      //if (client is null)
      //{
      //  throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!);
      //}

      #region payout,history
      var payout = await _walletRepository.GetPayoutById(request.PayoutId, _currentUser.ClientId!);
      if (payout is null)
      {
        throw new EntityNotFoundException("Payout ", request!.PayoutId!);
      }
      if (payout!.PayoutStatusId == (int)EnumPayoutStatus.Completed)
      {
        serviceResult.Errors = new Dictionary<string, string[]>();
        serviceResult.CreateError("AlreadyPaid", new string[] { $"Transaction charges cannot be updated for a payout that has already been paid" });
        return serviceResult;
      }
      #region payout

      #endregion
      payout.UpdateTransactionCharges(request.TransactionCharges);
      await _walletRepository.UpdatePayout(payout);
      #region create history
      //string? createdByName = await _employeeRepository.GetEmployeeNameById(_currentUser.EmployeeId);

      PayoutStatusHistory payoutStatusHistory = PayoutStatusHistory.CreatePayoutStatusHistory(new PayoutId(new Guid(request.PayoutId!)), null, request.Comment, request.UpdatedByName);
      await _walletRepository.CreatePayoutStatusHistory(payoutStatusHistory);
      #endregion
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
