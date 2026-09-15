using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.Services.Interfaces;

namespace Shipra.Backend.API.Application.Services;
public class ExceptionHandlerService
{
  private readonly IEmailServiceProvider _emailServiceProvider;

  public ExceptionHandlerService(IEmailServiceProvider emailServiceProvider)
  {
    _emailServiceProvider = emailServiceProvider;
  }

  public async Task<string> GetRequestBodyAsync(HttpContext context)
  {
    context.Request.EnableBuffering(); // Enable rewinding of the request body stream
    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
    {
      string body = await reader.ReadToEndAsync();
      context.Request.Body.Position = 0; // Reset the stream position for further use
      return body;
    }
  }

  public void SendEmail(string? tenantId, string? requestBody, Exception? ex = null)
  {
    tenantId = string.IsNullOrEmpty(tenantId) ? "Unknown Tenant" : tenantId;   
    var emailBody = new StringBuilder();

    // Build the email body
    emailBody.AppendLine("<h3>An error occurred in the application</h3>");
    // Serialize the request body
    try
    {
      var serializedRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(
          requestBody,
          Newtonsoft.Json.Formatting.Indented,
          new Newtonsoft.Json.JsonSerializerSettings
          {
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
          });
      emailBody.AppendLine($"<p><strong>Request Body:</strong></p><pre>{serializedRequestBody}</pre>");
    }
    catch (Exception serializationEx)
    {
      _ = serializationEx.Message;
      emailBody.AppendLine($"<p><strong>Request Body:</strong> Could not serialize due to an error.</p>");
    }
    emailBody.AppendLine($"<p><strong>Tenant ID:</strong> {tenantId}</p>");
    if (ex != null)
    {
      emailBody.AppendLine($"<p><strong>Message:</strong> {ex.Message}</p>");
      if (ex.InnerException != null)
      {
        emailBody.AppendLine($"<p><strong>Inner Exception:</strong> {ex.InnerException.Message}</p>");
      }
      emailBody.AppendLine($"<p><strong>Stack Trace:</strong></p><pre>{ex.StackTrace}</pre>");
    }
    emailBody.AppendLine($"<p><strong>Occurred At:</strong> {DateTime.UtcNow}</p>");

    // Send the email
    try
    {
      _emailServiceProvider.SendExceptionEmailAsync("Exception Alert: " + tenantId, emailBody.ToString()).GetAwaiter().GetResult();
    }
    catch (Exception emailEx)
    {
      _ = emailEx.Message;
      // Log email exception if necessary
    }
  }

  public string GetClientIdFromRequest(string? clientId, string? userName)
  {
    StringBuilder builder = new StringBuilder();
    if (!string.IsNullOrEmpty(clientId))
    {
      builder.Append(clientId);
    }
    if (!string.IsNullOrEmpty(userName))
    {
      if (builder.Length > 0)
      {
        builder.Append(" - "); // Separator between clientId and userName
      }
      builder.Append(userName);
    }
    return builder.Length > 0 ? builder.ToString() : "Client not found";
  }
}
