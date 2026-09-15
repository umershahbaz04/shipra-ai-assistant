using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeUpdateSubscriptionRemoveTrial;

public class StripeUpdateSubscriptionRemoveTrialCommand : IRequest<ServiceResultDTO>
{
  public string? StripeSubscriptionId { get; set; }
}

public class StripeUpdateSubscriptionRemoveTrialCommandHandler : RequestHandlerBase<StripeUpdateSubscriptionRemoveTrialCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedStripeRepository _sharedStripeRepository;

  public StripeUpdateSubscriptionRemoveTrialCommandHandler(IConfigRepository configRepository, ISharedStripeRepository sharedStripeRepository, IServiceProvider serviceProvider, ILogger<StripeUpdateSubscriptionRemoveTrialCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _sharedStripeRepository = sharedStripeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(StripeUpdateSubscriptionRemoveTrialCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      var stripeSubscriptionId = await _sharedStripeRepository.UpdateSubscriptionRemoveTrialAndCharge(request.StripeSubscriptionId!, mcconfig.Value!);
      if (string.IsNullOrEmpty(stripeSubscriptionId))
      {
        serviceResult.Errors!["StripeSubscriptionFailed"] = new string[] { "Update Subscription Remove Trial is Failed: " + request.StripeSubscriptionId };
        serviceResult.CreateErrorResponse(HttpStatusCode.BadRequest);
      }
      else
      {
        serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
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
