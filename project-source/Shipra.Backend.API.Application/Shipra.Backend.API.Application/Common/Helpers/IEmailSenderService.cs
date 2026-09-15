using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Common.Helpers;

public interface IEmailSenderService
{
  Task SendEmailAsync(string email, string subject, string htmlMessage, string cc = "");
}
