using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.Services;

namespace Shipra.Backend.API.Infrastructure.Services.Sms;

public class CountrySMSService : ISmsProvider
{
  public async Task Send(Message message, string? jsonConfig)
  {
    //var model = new OrderSmsSendDataModel
    //{
    //  RecipientMobile1 = "1563798893",
    //  TrackingStatusID = (int)EnumCarrierTrackingStatus.Delivered,
    //  Barcode = "01804170",
    //  DriverMobile = "0547029717",
    //  Language = "English",
    //  TrackingStatus = "Delivered",
    //  DriverName = "karam",
    //  Recipient_Name = "hamza",
    //  TrackingStatus_Ar = "تم التسليم"
    //};

    await SendSMSUpdate(message, jsonConfig!);
  }


  public async Task SendSMSUpdate(Message message, string jsonConfig)
  {

    var configModel = JsonConvert.DeserializeObject<CountrySMSGatewaysConfigModel>(jsonConfig);
     
    SMSCountry? oSMSGateway = new SMSCountry()
    {
      Message = message.Msg,
      Mtype = "LNG",
      DR = "Y",
      BaseUrl = configModel?.BaseUrl,
      User = configModel?.User,
      Passwd = configModel?.Password,
      SID = configModel?.SID,

      mobilenumber = FormatPhoneNumber(message.RecipientMobile!)
    };
    var messageID = await SentSMS(oSMSGateway);
  }

  #region country sms
  private async Task<string> SentSMS(SMSCountry value)
  {
    try
    {
      string[]? result = null;
      var dataID = string.Empty;

      var client = new HttpClient();
      var request = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = new Uri(value?.BaseUrl + "?user=" + value!.User + "&passwd=" + value.Passwd + "&mobilenumber=" + value.mobilenumber + "&message=" + value.Message + "&sid=" + value.SID + "&mtype=" + value.Mtype + "&DR=" + value.DR),
      };

      var response = await client.SendAsync(request);
      response.EnsureSuccessStatusCode();
      var responseBody = await response.Content.ReadAsStringAsync();
      if (!string.IsNullOrEmpty(responseBody))
      {
        result = responseBody.Split(':');
        if (result.Count() > 1)
        {
          dataID = result[1];
        }
      }

      return dataID;
    }
    catch (Exception)
    {
      throw;
    }
  }

  public class CountrySMSGatewaysConfigModel
  {
    public string? BaseUrl { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? SID { get; set; }
  }
  public class SMSCountry
  {
    public string? MessageID { get; set; }
    public string? User { get; set; }
    public string? Passwd { get; set; }
    public string? SID { get; set; }
    public string? BaseUrl { get; set; }
    public string? mobilenumber { get; set; }
    public string? Message { get; set; }
    public string? Mtype { get; set; }
    public string? DR { get; set; }
  }
  private string FormatPhoneNumber(string mobile1)
  {
    if (!mobile1.StartsWith("971"))
    {
      mobile1 = mobile1.Substring(1, mobile1.Length - 1);

      mobile1 = "971" + mobile1;
    }

    return Regex.Replace(mobile1, @"\s+", "");
  }

  private string GetMessageText(OrderSmsSendDataModel value)
  {
    var text = "Order ";

    var msg = "";

    if (value.TrackingStatusID == (int)EnumCarrierTrackingStatus.Delivered)
    {

      msg = value.Tracking_no + " " + value.TrackingStatus;

    }
    else
    {
      msg = text + value.Tracking_no + " " + value.Recipient_Name + " " + value.RecipientMobile1 + " " + value.TrackingStatus + " " + value.DriverMobile;
    }

    return msg;

  }

  #endregion 
}
