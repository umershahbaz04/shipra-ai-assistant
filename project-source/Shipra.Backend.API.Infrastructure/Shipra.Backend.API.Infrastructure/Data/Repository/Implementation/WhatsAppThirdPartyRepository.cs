using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class WhatsAppThirdPartyRepository : IWhatsAppThirdPartyRepository
{
  private readonly string messageUrl = "v2/message";
  public async Task<WhatsAppResponse> SendMessageAsync(object jsonPayload, string config)
  {
    var configData = JsonConvert.DeserializeObject<WhatsAppConfig>(config);

    using var httpClient = new HttpClient();

    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", configData!.ApiKey);

    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    var json = JsonConvert.SerializeObject(jsonPayload);

    var content = new StringContent(json, Encoding.UTF8, "application/json");

    try
    {
      var response = await httpClient.PostAsync($"{configData!.BaseUrl}/v1/public/message/", content);

      var responseContent = await response.Content.ReadAsStringAsync();

      var deserialized =
          JsonConvert.DeserializeObject<ResponseModel>(responseContent);

      if (response.IsSuccessStatusCode && deserialized != null && deserialized.result == true)
      {
        return new WhatsAppResponse
        {
          result = true,
          message = deserialized?.message
        };
      }

      return new WhatsAppResponse
      {
        result = false,
        message = deserialized?.message ?? responseContent
      };
    }
    catch (Exception ex)
    {
      return new WhatsAppResponse
      {
        result = false,
        message = ex.Message
      };
    }
  }
  #region Register number
  public async Task<WhatsAppRegisterResponse> Registernumber(string config, OrderAddress address)
  {
    var configData = JsonConvert.DeserializeObject<WhatsAppConfig>(config);
    var Baseurl = configData!.BaseUrl;
    var apiKey = configData!.ApiKey;
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    var jsonPayload = new
    {
      phoneNumber = address.Mobile1,
      countryCode = "",
      traits = new
      {
        name = address.CustomerName,
        merchant_location = "",
        account_owner_email_crm = "",
        lead_status_crm = ""
      },
      add_to_sales_cycle = true,
      createdAt = "",
      tags = new[] { "sample-tag-1", "sample-tag-2" }
    };
    var json = System.Text.Json.JsonSerializer.Serialize(jsonPayload);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    try
    {
      var response = await httpClient.PostAsync($"{Baseurl}/v1/public/track/users/", content);
      var responseContent = await response.Content.ReadAsStringAsync();
      var deserialized = JsonConvert.DeserializeObject<WhatsAppRegisterResponse>(responseContent);
      if (deserialized != null && deserialized.result == true)
      {
        return new WhatsAppRegisterResponse
        {
          result = deserialized!.result,
          message = deserialized?.message,
        };
      }
      return new WhatsAppRegisterResponse
      {
        result = false,
        message = deserialized?.message ?? "Registration failed or response invalid"
      };
    }

    catch (Exception ex)
    {
      return new WhatsAppRegisterResponse
      {
        result = false,
        message = $"Exception occurred: {ex.Message}"
      };
    }
  }
  #endregion


  #region Zoko WhatsApp 
  public ZokoReponseMessage? SendLocationMsgByZoko(TemplateArgs whatsAppModel, string config)
  {
    ZokoReponseMessage? reponse = null;
    ZokoWhatsappModel model = new ZokoWhatsappModel();
    model.recipient = whatsAppModel.recipient;
    model.templateArgs = whatsAppModel.templateArgs;
    model.type = whatsAppModel.type;
    model.templateId = whatsAppModel.templateId;
    model.templateLanguage = whatsAppModel.templateLanguage;
    model.message = whatsAppModel.message;

    var configData = JsonConvert.DeserializeObject<WhatsAppConfig>(config);
    try
    {
      HttpClient _httpClient = new HttpClient();
      if (_httpClient.BaseAddress == null)
      {
        _httpClient.BaseAddress = new Uri(configData!.BaseUrl!);
      }
      _httpClient.DefaultRequestHeaders.Clear();
      _httpClient.DefaultRequestHeaders.ConnectionClose = true;
      var json = JsonConvert.SerializeObject(model);
      var content = new StringContent(json, null, "application/json");

      var requestMessage = new HttpRequestMessage(HttpMethod.Post, messageUrl);
      requestMessage.Headers.Add("apikey", $"{configData!.ApiKey}");
      requestMessage.Content = content;

      //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
      var task = _httpClient.SendAsync(requestMessage);
      var response = task.Result;

      var responseStringRS = response.Content.ReadAsStringAsync().Result;
      response.EnsureSuccessStatusCode();
      if (response.StatusCode == HttpStatusCode.OK)
      {
        var responseString = response.Content.ReadAsStringAsync().Result;

        var data = JsonConvert.DeserializeObject<ZokoReponseMessage>(responseString);

        reponse = data;
      }
    }
    catch (Exception ex)
    {
      // Handle the exception here, e.g., log it or take appropriate action.
      // You can also throw a custom exception or rethrow the original exception if needed.
      Console.WriteLine("An error occurred: " + ex.Message);
    }

    return reponse;
  }
  #endregion

}

#region Response 
public class ResponseModel
{
  public bool? result { get; set; }
  public string? message { get; set; }
  public string? id { get; set; }
}
#endregion

#region Whatsappconfig 
public class WhatsAppConfig
{
  public string? ApiKey { get; set; }
  public string? BaseUrl { get; set; }
}
#endregion
