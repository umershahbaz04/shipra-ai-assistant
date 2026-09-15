using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shipra.Backend.API.Application.Services.Interfaces;
using Exception = System.Exception;

namespace Shipra.Backend.API.Application.Services.Implementation;

public partial class EmailHandler : IEmailHandler
{
  private readonly IConfiguration _configuration;

  public EmailHandler(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  /// <summary>
  /// SendEmailWithTemplate used to send emails using template
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  //public bool SendEmailWithTemplate(EmailTemplateDto model)
  //{
  //    var sendGridClient = new SendGridClient(_configuration["SendGrid:ApiKey"]);

  //    var sendGridMessage = new SendGridMessage();

  //    sendGridMessage.SetFrom(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:SenderName"]);

  //    sendGridMessage.AddTo(model.PatientEmail, model.PatientName);

  //    sendGridMessage.SetTemplateId(_configuration["SendGrid:TemplateId"]);

  //    sendGridMessage.SetTemplateData(new EmailTemplateDto
  //    {
  //        AppointmentDate = model.AppointmentDate,
  //        AppointmentTime = model.AppointmentTime,
  //        AppointmentProvider = model.AppointmentProvider,
  //        AppointmentOfferLink = model.AppointmentOfferLink
  //    });

  //    var response = sendGridClient.SendEmailAsync(sendGridMessage).Result;

  //    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
  //    {
  //        return true;
  //    }

  //    return false;
  //}

  /// <summary>
  /// SendEmailWithTemplates
  /// </summary>
  /// <param name="model"></param>
  /// <returns></returns>
  public Task<Response> SendEmailWithTemplates(dynamic model)
  {
    var sendGridClient = new SendGridClient(_configuration["SendGrid:ApiKey"]);

    var sendGridMessage = new SendGridMessage();
    sendGridMessage.SetFrom(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:SenderName"]);
    sendGridMessage.AddTo(model.Email, model.Name);
    sendGridMessage.SetTemplateId(model.TemplateId);
    sendGridMessage.SetTemplateData(model);
    return sendGridClient.SendEmailAsync(sendGridMessage);
    //if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
    //{
    //    return true;
    //}
  }

  /// <summary>
  /// SendEmail is used to send emails without template
  /// </summary>
  /// <param name="subject"></param>
  /// <param name="message"></param>
  /// <param name="to"></param>
  /// <param name="CC"></param>
  /// <returns></returns>
  public bool SendEmail(string subject, string message, string to, string CC)
  {
    var apikey = _configuration["SendGrid:ApiKey"];

    var emails = new List<string>();

    emails.Add(to);

    try
    {
      Execute(apikey!, subject, message, emails);
    }
    catch
    {

    }
    return true;
  }


  #region Factory methods

  /// <summary>
  /// Factory method for SendEmail
  /// </summary>
  /// <param name="apiKey"></param>
  /// <param name="subject"></param>
  /// <param name="message"></param>
  /// <param name="emails"></param>
  /// <returns></returns>
  public Task Execute(string apiKey, string subject, string message, List<string> emails)
  {
    var client = new SendGridClient(apiKey);
    var msg = new SendGridMessage()
    {
      From = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:SenderName"]),
      Subject = subject,
      PlainTextContent = message,
      HtmlContent = message
    };

    var list = new List<EmailAddress>();

    if (emails != null)
    {
      foreach (var email in emails)
      {
        list.Add(new EmailAddress(email));
      }
    }

    msg.AddTos(list);

    // Disable click tracking.
    // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
    msg.SetClickTracking(false, false);

    var response = client.SendEmailAsync(msg);
    return response;
  }

  #endregion

}
