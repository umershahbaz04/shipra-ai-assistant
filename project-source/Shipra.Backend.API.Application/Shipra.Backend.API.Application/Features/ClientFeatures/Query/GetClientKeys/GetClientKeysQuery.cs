using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientKeys;
public class GetClientKeysQuery : IRequest<ServiceResultDTO>
{
}
public class GetClientKeysQueryHandler : RequestHandlerBase<GetClientKeysQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public GetClientKeysQueryHandler(IClientRepository clientRepository,IServiceProvider serviceProvider, ILogger<GetClientKeysQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientKeysQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    { 
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientIdStr!);
      }
       
      var model = _mapper.Map<ClientKeysResponseModel>(client); 
      serviceResult = new ServiceResultDTO(model); 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
