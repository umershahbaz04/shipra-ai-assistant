using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;
using Shipra.Backend.API.Infrastructure.Data.Repository;

namespace Shipra.Backend.API.Infrastructure.Services;
public class interaktService : IinteraktService
{
  private readonly IOrderRepository _orderRepository;
  private readonly ISMSProcessRepository _sMSProcessRepository;
  private readonly IWhatsAppThirdPartyRepository _whatsAppRepo;
  private readonly IConfiguration _configuration;
  private readonly INotificationRepository _notificationRepository;

  public interaktService(IOrderRepository orderRepository, ISMSProcessRepository sMSProcessRepository, IWhatsAppThirdPartyRepository whatsAppRepo, INotificationRepository notificationRepository, IConfiguration configuration)
  {
    _orderRepository = orderRepository;
    _sMSProcessRepository = sMSProcessRepository;
    _whatsAppRepo = whatsAppRepo;
    _notificationRepository = notificationRepository;
    _configuration = configuration;
  }
  public async Task<WhatsAppResponse> SendShipmentNotificationAsync(Order oOrder, OrderAddress orderAddress, ClientId clientId)
  {
    var whatsappLookup = await _sMSProcessRepository.GetWhatsappLookupById((int)EnumWhatsappLookup.WhatsAppSms);

    var trackingPageUrl = _configuration.GetValue<string>("TrackingPageUrl");

    var param = $"{clientId.Value}_{oOrder.OrderNo}";
    var url = $"{trackingPageUrl}/{param}";

    var config = await _notificationRepository.GetSingleNotificationConfigByClientId(clientId);

    var bodyValues = GetBodyValues(config.Text!, oOrder, orderAddress);

    var whatsappActivationConfig = await _sMSProcessRepository.GetWhatsappActivationById(whatsappLookup!.WhatsAppLookupId, clientId);

    if (whatsappActivationConfig == null)
    {
      return new WhatsAppResponse
      {
        result = false,
        message = "WhatsApp configuration not found."
      };
    }

    var jsonPayload = new
    {
      userId = "",
      fullPhoneNumber = orderAddress.Mobile1,
      callbackData = "some_callback_data",
      type = "Template",
      template = new
      {
        name = "shipra_embellish_01",
        languageCode = "en",
        bodyValues = bodyValues,
        buttonValues = new Dictionary<string, List<string>>
            {
                { "0", new List<string> { url } }
            }
      }
    };

    var registerResult = await _whatsAppRepo.Registernumber(whatsappActivationConfig.Config!, orderAddress);

    if (!registerResult.result)
    {
      return new WhatsAppResponse
      {
        result = false,
        message = registerResult.message
      };
    }

    var sendResult = await _whatsAppRepo.SendMessageAsync(jsonPayload, whatsappActivationConfig.Config!);

    if (!sendResult.result)
    {
      return new WhatsAppResponse
      {
        result = false,
        message = sendResult.message
      };
    }

    return new WhatsAppResponse
    {
      result = true,
      message = sendResult.message ?? "WhatsApp notification sent successfully."
    };
  }

  #region Get Whatsapp msg template values
  private List<string> GetBodyValues(string? text, Order oOrder, OrderAddress orderAddress)
  {
    var values = new List<string>();
    if (string.IsNullOrEmpty(text))
      return values;
    //values.Add(orderAddress.CustomerName ?? "");
    values.Add(oOrder.OrderNo ?? "");
    return values;
  }
  #endregion

}




