using Newtonsoft.Json;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.SmsUseCase;
using Shipra.Backend.API.Application.DTOs.SmsUseCase.Smart;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.Services;

namespace Shipra.Backend.API.Infrastructure.Services.Sms;
public class SmartSMSService : ISmsProvider
{
  public async Task Send(Message message, string? jsonConfig)
  {
    var model = new OrderSmsSendDataModel
    {
      RecipientMobile1 = "1563798893",
      TrackingStatusID = (int)EnumCarrierTrackingStatus.Delivered,
      Barcode = "01804170",
      DriverMobile = "0547029717",
      Language = "English",
      TrackingStatus = "Delivered",
      DriverName = "karam",
      Recipient_Name = "hamza",
      TrackingStatus_Ar = "تم التسليم"
    };

    await SendSMSTrackingUpdate("orderId", model, jsonConfig!);
  }

  public async Task SendSMSTrackingUpdate(string? orderId, OrderSmsSendDataModel model, string jsonConfig)
  {
    if (!string.IsNullOrEmpty(orderId))
    {
      SMSGateway? oSMSGateway = null;
      oSMSGateway = new SMSGateway()
      {
        Message = GetMessageText(model),
        MessageTextType = SmsTextType.unicode,
        ToNumber = FormatPhoneNumber(model.RecipientMobile1!)
      };

      var configModel = JsonConvert.DeserializeObject<SmartSMSGatewaysConfigModel>(jsonConfig);

      var messageID = await SentSMS(oSMSGateway, configModel);
    }
  }
  #region smart sms gateway 
  private async Task<string> SentSMS(SMSGateway value, SmartSMSGatewaysConfigModel? configModel)
  {
    try
    {
      var authId = configModel?.Username;
      var authToken = configModel?.Password;
      var senderNumber = configModel?.SenderID;

      var client = new HttpClient();
      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = new Uri(configModel?.BaseUrl + "?username=" + authId + "&password=" + authToken + "&senderid=" + senderNumber + "&to=" + value.ToNumber + "&text=" + value.Message + "&type=" + value.MessageTextType + ""),
      };

      var response = await client.SendAsync(request);
      response.EnsureSuccessStatusCode();
      var responseBody = await response.Content.ReadAsStringAsync();
      var oSmartSmsGateway = JsonConvert.DeserializeObject<SmartSmsGateway>(responseBody);

      return oSmartSmsGateway!.data!.id!;
    }
    catch (Exception)
    {
      throw;
    }
  }
  private string FormatPhoneNumber(string mobile1)
  {
    if (!mobile1.StartsWith("971"))
    {
      if (mobile1.StartsWith("0"))
      {
        mobile1 = "971" + mobile1.Substring(1, mobile1.Length - 1);
      }
      else
      {
        mobile1 = "971" + mobile1;
      }
    }

    return mobile1;
  }

  private string GetMessageText(OrderSmsSendDataModel value)
  {
    var text = "";

    var textEng = "";

    var leftToRight = ((char)0x200E).ToString();

    var RightToLeft = ((char)0x200F).ToString();

    var msg = "";

    if (value.Language != "English")
    {


      if (value.TrackingStatusID == (int)EnumCarrierTrackingStatus.Delivered)
      {
        //msg = leftToRight + " (" + RightToLeft + value.STATUS_AR + leftToRight + "," + leftToRight + value.ReceiverNo + "," + RightToLeft + value.Receiver_Name + leftToRight + "," + value.Barcode + ")";

        msg = leftToRight + " (" + value.Barcode + "," + RightToLeft + value.Recipient_Name + leftToRight + "," + value.RecipientMobile1 + "," + RightToLeft + value.TrackingStatus_Ar + leftToRight + ")";

      }
      else
      {
        msg = leftToRight + " (" + value.Barcode + "," + RightToLeft + value.Recipient_Name + leftToRight + "," + value.RecipientMobile1 + "," + RightToLeft + value.TrackingStatus_Ar + leftToRight + ")" + RightToLeft + text + " " + leftToRight + value.DriverMobile;
      }
    }
    else
    {

      if (value.TrackingStatusID == (int)EnumCarrierTrackingStatus.Delivered)
      {
        //msg = leftToRight + " (" + RightToLeft + value.STATUS_AR + leftToRight + "," + leftToRight + value.ReceiverNo + "," + RightToLeft + value.Receiver_Name + leftToRight + "," + value.Barcode + ")";

        msg = textEng + " (" + value.Barcode + "," + value.Recipient_Name + "," + value.RecipientMobile1 + "," + value.TrackingStatus + ")";

      }
      else
      {
        msg = textEng + " " + value.DriverMobile + " (" + value.Barcode + "," + value.Recipient_Name + "," + value.RecipientMobile1 + "," + value.TrackingStatus + ")";
      }
    }

    return msg;

  }

  public class SmartSMSGatewaysConfigModel
  {
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? SenderID { get; set; }
    public string? BaseUrl { get; set; }
  }

  #endregion 
}
