using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.DTOs.SmsUseCase;
public class SMSGateway
{
  public string? MessageID { get; set; }
  public string? Username { get; set; }
  public string? Password { get; set; }
  public string? SenderID { get; set; }
  public String? ToNumber { get; set; }
  public string? Message { get; set; }
  public SmsTextType MessageTextType { get; set; }
  public DateTime Time { get; set; }
}

public enum SmsTextType
{
  text,
  unicode
}
