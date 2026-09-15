using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientProfile;
public class GetClientProfileQueryHandler : RequestHandlerBase<GetClientProfileQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  public GetClientProfileQueryHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<GetClientProfileQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetClientProfileQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oClientProfile = await _clientRepository.GetClientProfileById(_currentUser.ClientId!.Value.ToString());
      if (oClientProfile is not null)
      {
        serviceResult = new ServiceResultDTO(oClientProfile);
        serviceResult.CreateSuccessResponse();
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
