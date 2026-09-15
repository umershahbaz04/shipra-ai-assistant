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
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UpdateAllowWithoutBalance;
public class UpdateAllowWithoutBalanceCommand : IRequest<ServiceResultDTO>
{
  public bool? IsAllow { get; set; }
}
public class UpdateAllowWithoutBalanceCommandHandler : RequestHandlerBase<UpdateAllowWithoutBalanceCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public UpdateAllowWithoutBalanceCommandHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<UpdateAllowWithoutBalanceCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateAllowWithoutBalanceCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientIdStr!);
      }
      client.UpdateAllowWithoutBalance(request.IsAllow);
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
