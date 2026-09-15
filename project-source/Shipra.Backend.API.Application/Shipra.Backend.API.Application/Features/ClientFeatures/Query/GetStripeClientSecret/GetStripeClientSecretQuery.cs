using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetStripeClientSecret;
public class GetStripeClientSecretQuery : IRequest<ServiceResultDTO>
{
}

public class GetClientStripePaymentSecretQueryHandler : RequestHandlerBase<GetStripeClientSecretQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly ISharedStripeRepository _sharedStripeRepository;
  private readonly IConfigRepository _configRepository;

  public GetClientStripePaymentSecretQueryHandler(IClientRepository clientRepository, ISharedStripeRepository sharedStripeRepository, IConfigRepository configRepository, IServiceProvider serviceProvider, ILogger<GetClientStripePaymentSecretQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _sharedStripeRepository = sharedStripeRepository;
    _configRepository = configRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetStripeClientSecretQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientId!.Value);
      }

      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Integration Value");
      }
      dynamic result = await _sharedStripeRepository.CreatePaymentIntent(_currentUser.ClientIdStr!, mcconfig.Value!);
      var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<StripeClientSecretResponseModel>>(result);
      if (deseralisedResponse!.isSuccess)
      {
        if (!string.IsNullOrEmpty(deseralisedResponse.result.ClientSecret))
        {
          serviceResult = new ServiceResultDTO(new { deseralisedResponse.result.ClientSecret });
          serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
          return serviceResult;
        }
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
