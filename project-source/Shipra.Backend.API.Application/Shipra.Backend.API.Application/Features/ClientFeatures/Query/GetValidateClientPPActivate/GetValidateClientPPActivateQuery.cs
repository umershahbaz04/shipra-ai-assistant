using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetValidateClientPPActivate;

public class GetValidateClientPPActivateQuery : IRequest<ServiceResultDTO>
{
}
public class GetValidateClientPPActivateQueryHandler : RequestHandlerBase<GetValidateClientPPActivateQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IPaymentProcessRepository _paymentProcessRepository;
  private readonly IStripeRepository _stripeRepository;

  public GetValidateClientPPActivateQueryHandler(IClientRepository clientRepository, IStripeRepository stripeRepository, IPaymentProcessRepository paymentProcessRepository, IServiceProvider serviceProvider, ILogger<GetValidateClientPPActivateQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _stripeRepository = stripeRepository;
    _paymentProcessRepository = paymentProcessRepository;

  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetValidateClientPPActivateQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      bool isValidate = false;
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client ", _currentUser.ClientIdStr!);
      }

      var oPaymentProcess = await _paymentProcessRepository.GetClientDefaultPaymentProcessById(_currentUser.ClientId!);
      if (oPaymentProcess is not null)
      {
        #region Stripe Payment Process
        //The Below region is for Stripe Payment
        if (oPaymentProcess.PplookupId == (int)EnumPaymentProcessLookup.Stripe)
        {
          //Deserialize Stripe Config values
          var stripeSettingJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(oPaymentProcess.Config!);

          //Convert Keys To CamelCase with Deserialize Stripe object
          var resultStripeSetting = Utils.ConvertKeysToCamelCase(stripeSettingJson!);
          //Stripe keys
          var stripeSecretKey = Utils.GetValueFromDictionryByKey("secretKey", resultStripeSetting);
          var stripePublicKey = Utils.GetValueFromDictionryByKey("publicKey", resultStripeSetting);
          if (!string.IsNullOrEmpty(stripeSecretKey))
          {
            var accountStripe = await _stripeRepository.ValidateStripeAccount(stripeSecretKey);
            if (!string.IsNullOrEmpty(accountStripe))
            {
              isValidate = true;
            }            
          }
        }
        #endregion
      }
      serviceResult = new ServiceResultDTO(new { isValidate });
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
