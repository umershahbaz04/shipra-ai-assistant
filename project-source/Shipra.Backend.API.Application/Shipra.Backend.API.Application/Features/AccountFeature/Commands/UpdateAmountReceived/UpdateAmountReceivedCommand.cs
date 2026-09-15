using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.AccountFeature.Commands.UpdateAmountReceived;
public class UpdateAmountReceivedCommand : IRequest<ServiceResultDTO>
{
  public string? CarrierPaymentSettlementId { get; set; }
  public decimal Amount { get; set; }
}
public class UpdateAmountReceivedCommandHandler : RequestHandlerBase<UpdateAmountReceivedCommand, ServiceResultDTO>
{
  private readonly IAccountRepository _accountRepository;

  public UpdateAmountReceivedCommandHandler(IAccountRepository accountRepository,IServiceProvider serviceProvider, ILogger<UpdateAmountReceivedCommandHandler> logger) : base(serviceProvider, logger)
  {
    _accountRepository = accountRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateAmountReceivedCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var target = await _accountRepository.GetCarrierPaymentSettlementById(new CarrierPaymentSettlementId(new Guid(request.CarrierPaymentSettlementId!)) );
      if (target is null)
      {
        throw new EntityNotFoundException("CarrierPaymentSettlement", request.CarrierPaymentSettlementId!);
      }
      target.UpdateAmountReceived(request.Amount, _currentUser.EmployeeId!);


      var upData = await _accountRepository.UpdateCarrierPaymentSettlement(target);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
