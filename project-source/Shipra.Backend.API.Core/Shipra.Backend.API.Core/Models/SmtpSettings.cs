using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.Models;
public class SmtpSettings
{
  public string? SmtpAddress { get; set; }
  public int PortNumber { get; set; }
  public bool EnableSSL { get; set; }
  public string? EmailFromAddress { get; set; }
  public string? Password { get; set; }
  public string? ErrorNotificationEmail { get; set; }
}
public class SmtpSettingsForException
{
  public string? SmtpAddress { get; set; }
  public int PortNumber { get; set; }
  public bool EnableSSL { get; set; }
  public string? EmailFromAddress { get; set; }
  public string? Password { get; set; }
  public string? ErrorNotificationEmail { get; set; }
}
