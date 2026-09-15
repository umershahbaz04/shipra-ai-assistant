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
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.WalletFeature.Command.UpdatePaymentProcessingCharges;
public class UpdateClientPaymentProcessingChargesCommand : IRequest<ServiceResultDTO>
{
  public decimal? PaymentProcessingCharges { get; set; }
  public int? PayoutScheduleSettings { get; set; }
}
public class UpdateClientPaymentProcessingChargesHandler : RequestHandlerBase<UpdateClientPaymentProcessingChargesCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public UpdateClientPaymentProcessingChargesHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<UpdateClientPaymentProcessingChargesHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientPaymentProcessingChargesCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);

      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientIdStr!);
      }
      client.UpdatePaymentProcessingCharges(request.PaymentProcessingCharges.GetValueOrDefault(),request.PayoutScheduleSettings);
      await _clientRepository.UpdateClient(client);

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
