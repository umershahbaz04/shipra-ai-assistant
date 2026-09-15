using System.Net;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientById;
public class GetClientByIdQueryHandler : RequestHandlerBase<GetClientByIdQuery, ServiceResultDTOWithTypeModel<ClientResponseModel>>
{
  private readonly IClientRepository _clientRepository;
  public GetClientByIdQueryHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<GetClientByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<ClientResponseModel>> HandleRequest(GetClientByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTOWithTypeModel<ClientResponseModel> serviceResult = new ServiceResultDTOWithTypeModel<ClientResponseModel>();
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
      if (client is null)
      {
        throw new EntityNotFoundException("Client", clientId.Value);
      }

      var clientAddress = await _clientRepository.GetClientAddresByClientId(clientId);
      if (clientAddress is null)
      {
        throw new EntityNotFoundException("Client Address", clientId.Value);
      }

      var model = _mapper.Map<ClientResponseModel>(client);
      var oAddressMap = _mapper.Map<AddressResponseDTO>(clientAddress);
      model.Address = oAddressMap;
      serviceResult = new ServiceResultDTOWithTypeModel<ClientResponseModel>(model);
      serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
