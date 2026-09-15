using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.X509;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Services;
public class ZokoService : IZokoService
{
  private readonly IOrderRepository _orderRepository;
  private readonly ISMSProcessRepository _sMSProcessRepository;
  private readonly IWhatsAppThirdPartyRepository _whatsAppRepo;
  private readonly IConfiguration _configuration;
  private readonly INotificationRepository _notificationRepository;

  public ZokoService(IOrderRepository orderRepository, ISMSProcessRepository sMSProcessRepository, IWhatsAppThirdPartyRepository whatsAppRepo, INotificationRepository notificationRepository, IConfiguration configuration)
  {
    _orderRepository = orderRepository;
    _sMSProcessRepository = sMSProcessRepository;
    _whatsAppRepo = whatsAppRepo;
    _notificationRepository = notificationRepository;
    _configuration = configuration;
  }
  public async Task<WhatsAppResponse> SendLocationNotificationAsync(Order oOrder, OrderAddress orderAddress, ClientId clientId)
  {
    var whatsappLookup = await _sMSProcessRepository.GetWhatsappLookupById((int)EnumWhatsappLookup.WhatsAppSms);

    //var trackingPageUrl = _configuration.GetValue<string>("TrackingPageUrl");
    //var param = $"{clientId.Value}_{oOrder.OrderNo}";
    //var url = $"{trackingPageUrl}/{param}";

    var whatsappActivationConfig = await _sMSProcessRepository.GetWhatsappActivationById(whatsappLookup!.WhatsAppLookupId, clientId);

    if (whatsappActivationConfig == null)
    {
      return new WhatsAppResponse
      {
        result = false,
        message = "WhatsApp configuration not found."
      };
    }

    TemplateArgs argr = new TemplateArgs();

    //Values to set in placeholders
    argr.templateArgs = new string[1];
    argr.templateArgs[0] = oOrder.OrderNo!;
    //argr.templateArgs[1] = url;

    argr.recipient = orderAddress.Mobile1;
    argr.templateLanguage = "en";

    argr.type = "template";
    argr.templateId = "03_07_26_template_3e0m";

    var sendResult = _whatsAppRepo.SendLocationMsgByZoko(argr, whatsappActivationConfig.Config!);

    if (sendResult == null || sendResult.status != "202" || !string.Equals(sendResult.statusText, "Accepted", StringComparison.OrdinalIgnoreCase))
    {
      return new WhatsAppResponse
      {
        result = false,
        message = sendResult?.statusText ?? "Failed to send WhatsApp message."
      };
    }

    return new WhatsAppResponse
    {
      result = true,
      message = sendResult.statusText ?? "WhatsApp notification sent successfully."
    };
  }
}
