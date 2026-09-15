using SendGrid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Services.Interfaces;

public interface IEmailHandler
{
  Task Execute(string apiKey, string subject, string message, List<string> emails);
  bool SendEmail(string subject, string message, string to, string CC);
  //bool SendEmailWithTemplate(EmailHandler.EmailTemplateDto model);
  Task<Response> SendEmailWithTemplates(dynamic model);
}
