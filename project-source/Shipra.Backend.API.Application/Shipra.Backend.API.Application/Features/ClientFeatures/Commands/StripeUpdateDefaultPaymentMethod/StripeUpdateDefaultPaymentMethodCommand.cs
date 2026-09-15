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

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.StripeUpdateDefaultPaymentMethod;
public class StripeUpdateDefaultPaymentMethodCommand : IRequest<ServiceResultDTO>
{
  public string? PaymentMethodId { get; set; }
}

public class StripeUpdateDefaultPaymentMethodCommandHandler : RequestHandlerBase<StripeUpdateDefaultPaymentMethodCommand, ServiceResultDTO>
{
  private readonly IConfigRepository _configRepository;
  private readonly ISharedStripeRepository _sharedStripeRepository;

  public StripeUpdateDefaultPaymentMethodCommandHandler(IConfigRepository configRepository, ISharedStripeRepository sharedStripeRepository, IServiceProvider serviceProvider, ILogger<StripeUpdateDefaultPaymentMethodCommandHandler> logger) : base(serviceProvider, logger)
  {
    _configRepository = configRepository;
    _sharedStripeRepository = sharedStripeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(StripeUpdateDefaultPaymentMethodCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.Admin, _currentUser.EnvironmentTypeId);
      if (mcconfig is null)
      {
        throw new EntityNotFoundException("Mcconfig", "Cognito Value");
      }
      var result = await _sharedStripeRepository.UpdateDefaultPaymentMethod(_currentUser.ClientIdStr!, request.PaymentMethodId!, mcconfig.Value!);
      if (!string.IsNullOrEmpty(result))
      {
        var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<StripeCustomerResponseModel>>(result);
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
      }
      else
      {
        serviceResult.Errors!["UpdateDefaultPaymentMethodFailed"] = new string[] { "Update Default PaymentMethod is Failed: " + request.PaymentMethodId! };
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
}
