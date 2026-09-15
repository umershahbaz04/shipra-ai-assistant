using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeCancelSubscription;

public class StripeCancelSubscriptionCommand : IRequest<ServiceResultDTO>
{
  public string? StripeSubscriptionId { get; set; }
  public string? Feedback { get; set; }
  public string? Comments { get; set; }
}

public class StripeCancelSubscriptionCommandHandler : RequestHandlerBase<StripeCancelSubscriptionCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedStripeRepository _sharedStripeRepository;

  public StripeCancelSubscriptionCommandHandler(IConfigRepository configRepository, ISharedStripeRepository sharedStripeRepository, IServiceProvider serviceProvider, ILogger<StripeCancelSubscriptionCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _sharedStripeRepository = sharedStripeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(StripeCancelSubscriptionCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      var stripeSubscriptionId = await _sharedStripeRepository.CancelSubscription(request.StripeSubscriptionId!, request.Feedback!, request.Comments!, mcconfig.Value!);
      if (string.IsNullOrEmpty(stripeSubscriptionId))
      {
        serviceResult.Errors!["StripeSubscriptionFailed"] = new string[] { "Cancel Stripe Subscription is Failed: " + request.StripeSubscriptionId };
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
