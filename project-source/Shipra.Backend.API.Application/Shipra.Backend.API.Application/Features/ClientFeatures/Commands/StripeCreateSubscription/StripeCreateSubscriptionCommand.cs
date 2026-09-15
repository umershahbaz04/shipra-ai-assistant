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

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeCreateSubscription;
public class StripeCreateSubscriptionCommand : IRequest<ServiceResultDTO>
{
  public int ProductId { get; set; }
}

public class StripeCreateSubscriptionCommandHandler : RequestHandlerBase<StripeCreateSubscriptionCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedStripeRepository _sharedStripeRepository;

  public StripeCreateSubscriptionCommandHandler(IConfigRepository configRepository, ISharedStripeRepository sharedStripeRepository, IServiceProvider serviceProvider, ILogger<StripeCreateSubscriptionCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _sharedStripeRepository = sharedStripeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(StripeCreateSubscriptionCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      var result = await _sharedStripeRepository.CreateSubscription(_currentUser.ClientIdStr!, request.ProductId!, mcconfig.Value!);
      var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<StripeSubscriptionResponseModel>>(result);
      if (deseralisedResponse!.isSuccess)
      {
        serviceResult = new ServiceResultDTO(deseralisedResponse.result!);
        serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
        return serviceResult;
      }
      else
      {
        serviceResult.Errors = deseralisedResponse.errors;
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  public class StripeSubscriptionResponseModel { }
}
