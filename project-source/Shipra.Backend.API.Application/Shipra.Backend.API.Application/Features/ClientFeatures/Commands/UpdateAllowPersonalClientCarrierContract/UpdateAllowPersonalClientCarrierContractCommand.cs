using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateAllowPersonalClientCarrierContract;
public class UpdateAllowPersonalClientCarrierContractCommand : IRequest<ServiceResultDTO>
{
  public bool? IsAllow { get; set; }
}
public class UpdateAllowPersonalClientCarrierContractCommandHandler : RequestHandlerBase<UpdateAllowPersonalClientCarrierContractCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public UpdateAllowPersonalClientCarrierContractCommandHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<UpdateAllowPersonalClientCarrierContractCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateAllowPersonalClientCarrierContractCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientIdStr!);
      }
      client.UpdateAllowPersonalClientCarrier(request.IsAllow);
      await _clientRepository.UpdateClient(client);
      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = true, Message = "Update successfully" });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
