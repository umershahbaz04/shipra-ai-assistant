using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.DeleteProductStation;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.DeleteClient;
public class DeleteClientCommandHandler : RequestHandlerBase<DeleteClientCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IClientRepository _clientRepository;
  public DeleteClientCommandHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<DeleteClientCommand> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(DeleteClientCommand request, CancellationToken cancellationToken)
  {
    var response = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      Guid guidID;
      var hasGUID = Guid.TryParse(request!.ClientId!, out guidID);
      if (!hasGUID)
      {
        throw new InvalidIdTypeException(request!.ClientId!);
      }
      var clientId = new ClientId(new Guid(request!.ClientId!));
      var client = await _clientRepository.GetClientById(clientId);
      if (client == null)
      {
        throw new EntityNotFoundException("Client", clientId.Value);
      }
      response.IsSuccess = await _clientRepository.DeleteClient(client);

      return response;
    }
    catch (Exception ex)
    {
      response.CreateErrorResponse(ex);
      throw;
    }
  }
}
