namespace Shipra.Backend.API.Application.Services.Interfaces;
public interface IEmailServiceProvider
{
  Task<bool> SendEmailUsingGoogle(string name, string? email, string? phone, string? message, string? primaryNeed, string? monthlyOrder, string? companyName);
  Task<bool> SendEmailForNewsSubscription(string? email);
  Task<bool> SendEmailToCustomer(string? email, string? body, string subject, string? shippinglink = null, byte[]? attachmentBytes = null, string? attachmentFileName = null);

  Task<bool> SendEmailAsync(string subject, string body, string[] toEmails, string? ccEmail = null, string? bccEmail = null); 
  Task<bool> SendExceptionEmailAsync(string subject, string body, string? ccEmail = null, string? bccEmail = null); 
}
