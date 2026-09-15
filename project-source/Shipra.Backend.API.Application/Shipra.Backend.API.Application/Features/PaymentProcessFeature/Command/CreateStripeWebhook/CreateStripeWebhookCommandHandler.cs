using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Stripe;

namespace Shipra.Backend.API.Application.Features.PaymentProcessFeature.Command.CreateStripeWebhook;


public class CreateStripeWebhookCommandHandler : RequestHandlerBase<CreateStripeWebhookCommand, ServiceResultDTO>
{
  private readonly IPaymentProcessRepository _paymentProcessRepository;
  private readonly IConfigRepository _configRepository;
  private readonly IClientRepository _clientRepository;
  private readonly IStripeRepository _stripeRepository;

  public CreateStripeWebhookCommandHandler(IPaymentProcessRepository paymentProcessRepository, IConfigRepository configRepository, IClientRepository clientRepository, IStripeRepository stripeRepository, IServiceProvider serviceProvider, ILogger<CreateStripeWebhookCommandHandler> logger) : base(serviceProvider, logger)
  {
    _paymentProcessRepository = paymentProcessRepository;
    _configRepository = configRepository;
    _clientRepository = clientRepository;
    _stripeRepository = stripeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateStripeWebhookCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oPaymentProcess = await _paymentProcessRepository.GetPPActivateByPPLookupId((int)EnumPaymentProcessLookup.Stripe, _currentUser!.ClientId!);

      if (oPaymentProcess is not null)
      {
        //Deserialize Stripe Config values
        var stripeSettingJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(oPaymentProcess.Config!);

        //Convert Keys To CamelCase with Deserialize Stripe object
        var resultStripeSetting = Utils.ConvertKeysToCamelCase(stripeSettingJson!);

        //Stripe keys
        var secretKey = Utils.GetValueFromDictionryByKey("secretKey", resultStripeSetting);
        var publicKey = Utils.GetValueFromDictionryByKey("publicKey", resultStripeSetting);

        var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
        if (client is null)
        {
          throw new EntityNotFoundException("Client ", _currentUser.ClientId!.Value.ToString());
        }

        var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.API, _currentUser.EnvironmentTypeId);
        if (mcconfig is null)
        {
          throw new EntityNotFoundException("Mcconfig", "API Value");
        }
        List<string> EnabledEvents = new List<string>() { EventTypes.InvoicePaid };
        // Create & Retrieve the webhook secret from the created endpoint
        if (string.IsNullOrEmpty(client.StripeWebhookId))
        {
          //Case 01: Create New Stripe Webhook 
          dynamic oWebhook = await _stripeRepository.CreateStripeWebhookAsync(mcconfig.Value!, EnabledEvents, secretKey);
          client.UpdateClientStripeWebhookSecret(oWebhook.Id, oWebhook.Secret, _currentUser.EmployeeId!);
          await _clientRepository.UpdateClient(client);
          serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = true, Message = NotificationConstants.Success });
          return serviceResult;
        }
        else
        {
          dynamic oGetWebhook = await _stripeRepository.GetStripeWebhookAsync(client.StripeWebhookId, secretKey);
          if (!string.IsNullOrEmpty(oGetWebhook.Id))
          {
            serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = true, Message = NotificationConstants.Success });
            return serviceResult;
          }
          else
          {
            //Case 03: Create New Stripe Webhook when already in Shipra DB but not on Stripe Account
            dynamic oWebhook = await _stripeRepository.CreateStripeWebhookAsync(mcconfig.Value!, EnabledEvents, secretKey);
            client.UpdateClientStripeWebhookSecret(oWebhook.Id, oWebhook.Secret, _currentUser.EmployeeId!);
            await _clientRepository.UpdateClient(client);
            serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = true, Message = NotificationConstants.Success });
            return serviceResult;
          }         
        }       
      }
      else
      {
        throw new EntityNotFoundException("PaymentProcess", "Payment Process Entity Not Found.");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

