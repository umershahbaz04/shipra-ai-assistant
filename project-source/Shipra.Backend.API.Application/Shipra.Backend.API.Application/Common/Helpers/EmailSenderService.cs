using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Common.Helpers;

public class EmailSenderService : IEmailSenderService
{
  public IConfiguration _config { get; set; }
  public EmailSenderService(IConfiguration configuration)
  {
    _config = configuration;
  }

  public Task SendEmailAsync(string email, string subject, string htmlMessage, string cc = "")
  {
    try
    {
      if (email == null)
      {
        throw new ArgumentNullException("email cannot be null.");
      }
      else if (subject == null)
      {
        throw new ArgumentNullException("subject cannot be null.");
      }
      var message = new MailMessage(_config["Smtp:Email"]!, email, subject, htmlMessage);
      message.Sender = new MailAddress(_config["Smtp:Email"]!);
      message.Priority = MailPriority.High;
      message.IsBodyHtml = true;
      if (!string.IsNullOrEmpty(cc))
      {
        message.CC.Add(cc);
      }

      var client = new SmtpClient(_config["Smtp:Host"]);
      client.Port = int.Parse(_config["Smtp:Port"]!);
      client.Credentials = new NetworkCredential(_config["Smtp:Email"], _config["Smtp:Password"]);
      client.DeliveryMethod = SmtpDeliveryMethod.Network;
      client.EnableSsl = true;
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      message.IsBodyHtml = true;
      client.Send(message);
      return Task.CompletedTask;
    }
    catch
    {
      return Task.CompletedTask;
    }
  }
}
