using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientByKey;


public class GetClientByKeyQueryHandler : RequestHandlerBase<GetClientByKeyQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  public GetClientByKeyQueryHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<GetClientByKeyQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetClientByKeyQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClient = await _clientRepository.GetClientByKey(request.TenantUsername, request.PublicKey, request.ClientId);
      if (oClient is not null)
      {
        var clientId = oClient.ClientId!.Value.ToString();
        serviceResult = new ServiceResultDTO(new { clientId, oClient.SecretKey });
        serviceResult.CreateSuccessResponse();
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Invalid Client object - Record not found");
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
