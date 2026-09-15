using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure;

public class FakeEmailSender : IEmailSender
{
  public Task SendEmailAsync(string to, string from, string subject, string body)
  {
    return Task.CompletedTask;
  }
}
