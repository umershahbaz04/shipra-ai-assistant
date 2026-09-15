using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class EmailServiceProvider : IEmailServiceProvider
{
  private readonly SmtpSettings? _smtpSettings;
  private readonly SmtpSettingsForException? _SmtpSettingsForException;

  public EmailServiceProvider(IConfiguration configuration)
  {
    // Bind the SMTP settings from the appsettings.json to the SmtpSettings object
    _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
    _SmtpSettingsForException = configuration.GetSection("SmtpSettingsForException").Get<SmtpSettingsForException>();
  }
  public async Task<bool> SendExceptionEmailAsync(string subject, string body, string? ccEmail = null, string? bccEmail = null)
  {
    try
    {
      string smtpAddress = _SmtpSettingsForException!.SmtpAddress!;// "smtp.gmail.com";
      int portNumber = _SmtpSettingsForException.PortNumber!;//587;
      bool enableSSL = _SmtpSettingsForException.EnableSSL!;//true;
      string emailFromAddress = _SmtpSettingsForException.EmailFromAddress!;//"shipraio.technobatch@gmail.com";
      string password = _SmtpSettingsForException.Password!;//"jbaaczxjllbskxnh";
      var toEmails = new string[] { _SmtpSettingsForException.ErrorNotificationEmail! };
      MailMessage message = new MailMessage();
      message.From = new MailAddress(emailFromAddress);

      // Add multiple recipients
      foreach (var toEmail in toEmails)
      {
        message.To.Add(new MailAddress(toEmail));
      }
      // Add CC email if provided
      if (!string.IsNullOrEmpty(ccEmail))
      {
        message.CC.Add(new MailAddress(ccEmail));
      }

      // Add BCC email if provided
      if (!string.IsNullOrEmpty(bccEmail))
      {
        message.Bcc.Add(new MailAddress(bccEmail));
      }

      message.Subject = subject;
      message.IsBodyHtml = true; // Make the body HTML

      // Set the body of the email
      message.Body = body;

      using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
      {
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
        smtp.EnableSsl = enableSSL;
        await smtp.SendMailAsync(message);
      }

      return true;
    }
    catch (Exception ex)
    {
      // Log or handle the error
      var msg = ex.Message;
      return false;
    }
  }
  public async Task<bool> SendEmailAsync(string subject, string body, string[] toEmails, string? ccEmail = null, string? bccEmail = null)
  {
    try
    {
      string smtpAddress = _smtpSettings!.SmtpAddress!;// "smtp.gmail.com";
      int portNumber = _smtpSettings.PortNumber!;//587;
      bool enableSSL = _smtpSettings.EnableSSL!;//true;
      string emailFromAddress = _smtpSettings.EmailFromAddress!;//"shipraio.technobatch@gmail.com";
      string password = _smtpSettings.Password!;//"jbaaczxjllbskxnh";

      MailMessage message = new MailMessage();
      message.From = new MailAddress(emailFromAddress);

      // Add multiple recipients
      foreach (var toEmail in toEmails)
      {
        message.To.Add(new MailAddress(toEmail));
      }
      // Add CC email if provided
      if (!string.IsNullOrEmpty(ccEmail))
      {
        message.CC.Add(new MailAddress(ccEmail));
      }

      // Add BCC email if provided
      if (!string.IsNullOrEmpty(bccEmail))
      {
        message.Bcc.Add(new MailAddress(bccEmail));
      }

      message.Subject = subject;
      message.IsBodyHtml = true; // Make the body HTML

      // Set the body of the email
      message.Body = body;

      using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
      {
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
        smtp.EnableSsl = enableSSL;
        await smtp.SendMailAsync(message);
      }

      return true;
    }
    catch (Exception ex)
    {
      // Log or handle the error
      var msg = ex.Message;
      return false;
    }
  }

  public async Task<bool> SendEmailToCustomer(string? email, string? body, string subject, string? shippinglink = null, byte[]? attachmentBytes = null, string? attachmentFileName = null)
  {
    try
    {
      string smtpAddress = _smtpSettings!.SmtpAddress!;// "smtp.gmail.com";
      int portNumber = _smtpSettings.PortNumber!;//587;
      bool enableSSL = _smtpSettings.EnableSSL!;//true;
      string emailFromAddress = _smtpSettings.EmailFromAddress!;//"shipraio.technobatch@gmail.com";
      string password = _smtpSettings.Password!;//"jbaaczxjllbskxnh";
      string? toEmail = email;

      MailMessage message = new MailMessage();
      message.From = new MailAddress(emailFromAddress);
      message.To.Add(new MailAddress(toEmail!));
      message.Subject = subject;
      message.IsBodyHtml = true; //to make message body as html
      message.Body = "<p>" + body + "</p>";
      if (attachmentBytes is not null)
      {
        MemoryStream memoryStream = new MemoryStream(attachmentBytes);

        Attachment attachment = new Attachment(memoryStream, attachmentFileName, "application/pdf");
        message.Attachments.Add(attachment);
      }
      if (shippinglink is not null)
      {
        message.Body = "<p> Shipping LinkWooComereceModal " + shippinglink + "</p>";
      }
      using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
      {
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
        smtp.EnableSsl = enableSSL;
        await smtp.SendMailAsync(message);
      }
      await Task.Delay(1);
      return true;
    }
    catch (Exception ex)
    {
      var msg = ex.Message;
      return false;
    }
  }

  public async Task<bool> SendEmailUsingGoogle(string name, string? email, string? phone, string? messageDesc, string? primaryNeed, string? monthlyOrder, string? companyName)
  {

    try
    {
      string smtpAddress = _smtpSettings!.SmtpAddress!;// "smtp.gmail.com";
      int portNumber = _smtpSettings.PortNumber!;//587;
      bool enableSSL = _smtpSettings.EnableSSL!;//true;
      string emailFromAddress = _smtpSettings.EmailFromAddress!;//"shipraio.technobatch@gmail.com";
      string password = _smtpSettings.Password!;//"jbaaczxjllbskxnh";
      string toEmail = "app.shipra@gmail.com";

      MailMessage message = new MailMessage();
      message.From = new MailAddress(emailFromAddress);
      message.To.Add(new MailAddress(toEmail!));
      message.Subject = "Contact Information";
      message.IsBodyHtml = true; //to make message body as html  

      string body = "<div style='line-height:inherit;font-family:Avenir,Helvetica,sans-serif;box-sizing:border-box;direction:ltr;text-align:left;'>";
      body += "<br style='line-height:inherit;'><b style='font-size: 16px;'>Contact Information</b>";
      body += "<br style='line-height:inherit;'><b>Name:</b> " + name;
      //body += "<br style='line-height:inherit;'><b>Subject:</b> " + " Client";
      body += "<br style='line-height:inherit;'><b>Email Address:</b> " + email;
      body += "<br style='line-height:inherit;'><b>Message:</b> " + messageDesc;
      body += "<br style='line-height:inherit;'><b>PrimaryNeed:</b> " + primaryNeed;
      body += "<br style='line-height:inherit;'><b>MonthlyOrder:</b> " + monthlyOrder;
      body += "<br style='line-height:inherit;'><b>CompanyName:</b> " + companyName;
      body += "<br style='line-height: inherit;'>";
      body += "<b>Shipra</b> App <br style = 'line-height: inherit;' >";
      body += "<b>Note:</b> This is an electronic message.Please do not reply to this email.</div>";

      message.Body = "<p>" + body + "</p>";

      using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
      {
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
        smtp.EnableSsl = enableSSL;
        await smtp.SendMailAsync(message);
      }
      await Task.Delay(1);
      return true;
    }
    catch (Exception ex)
    {

      var msg = ex.Message;
      return false;
    }

  }
  //public async Task SendEmail()
  //{
  //  try
  //  {
  //    // Use your Zoho SMTP settings
  //    string smtpAddress = _smtpSettings!.SmtpAddress!;// "smtp.gmail.com";
  //    int portNumber = _smtpSettings.PortNumber!;//587;
  //    bool enableSSL = _smtpSettings.EnableSSL!;//true;
  //    string emailFromAddress = _smtpSettings.EmailFromAddress!;//"shipraio.technobatch@gmail.com";
  //    string password = _smtpSettings.Password!;//"jbaaczxjllbskxnh";
  //    string? toEmail = email;

  //    MailMessage message = new MailMessage();
  //    message.From = new MailAddress(emailFromAddress);
  //    message.To.Add(new MailAddress(toEmail));
  //    message.Subject = "Shipra Subscriber Updates!";
  //    message.IsBodyHtml = true; // Set body as HTML

  //    // Build the email body
  //    string body = "test body";
  //    message.Body = body;

  //    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
  //    {
  //      smtp.UseDefaultCredentials = false;
  //      smtp.Credentials = new NetworkCredential(emailFromAddress, password);
  //      smtp.EnableSsl = enableSSL;

  //      // Send email asynchronously
  //      await smtp.SendMailAsync(message);
  //    }

  //    //return true;
  //  }
  //  catch (Exception ex)
  //  {
  //    // Log or handle the error (you can log ex.Message for debugging)
  //    Console.WriteLine($"Email sending failed: {ex.Message}");
  //    //return false;
  //  }
  //}

  public async Task<bool> SendEmailForNewsSubscription(string? email)
  {

    try
    {
      string smtpAddress = _smtpSettings!.SmtpAddress!;// "smtp.gmail.com";
      int portNumber = _smtpSettings.PortNumber!;//587;
      bool enableSSL = _smtpSettings.EnableSSL!;//true;
      string emailFromAddress = _smtpSettings.EmailFromAddress!;//"shipraio.technobatch@gmail.com";
      string password = _smtpSettings.Password!;//"jbaaczxjllbskxnh";
      string toEmail = "app.shipra@gmail.com";

      MailMessage message = new MailMessage();
      message.From = new MailAddress(emailFromAddress);
      message.To.Add(new MailAddress(toEmail!));
      message.Subject = "Shipra Subscriber Updates!";
      message.IsBodyHtml = true; //to make message body as html  

      string body = "<div style='line-height:inherit;font-family:Avenir,Helvetica,sans-serif;box-sizing:border-box;direction:ltr;text-align:left;'>";
      body += "<br style='line-height:inherit;'><b style='font-size: 16px;'>Shipra Subscriber Information</b>";
      body += "<br style='line-height: inherit;'>";
      body += "<br style='line-height: inherit;'>";
      body += "<br style='line-height:inherit;'><b>Subscriber Email :</b> " + email;
      body += "<br style='line-height: inherit;'>";
      body += "<b>Note:</b> This is an electronic message.Please do not reply to this email.</div>";

      message.Body = "<p>" + body + "</p>";

      using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
      {
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(emailFromAddress, password);
        smtp.EnableSsl = enableSSL;
        await smtp.SendMailAsync(message);
      }
      await Task.Delay(1);
      return true;
    }
    catch (Exception ex)
    {
      var msg = ex.Message;
      return false;
    }
  }
}
