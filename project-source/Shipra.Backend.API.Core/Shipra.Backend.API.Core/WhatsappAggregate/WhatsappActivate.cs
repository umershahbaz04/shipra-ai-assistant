using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.SMSProcessAggregate;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Shipra.Backend.API.Core.WhatsappAggregate;
public class WhatsappActivate
{
  public int WhatsAppActivateId { get; set; }
  public int? WhatsAppLookupId { get; set; }
  public bool? IsDefault { get; set; }
  public string? Config { get; set; }
  public ClientId? ClientId { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdateOn { get; set; }
  public bool? Active { get; set; }
  public static WhatsappActivate CreateWhatsappActivate(int WhatsappLookupId, string jsonStr, ClientId? clientId, EmployeeId? userId, bool? isDefault = false)
  {
    return new WhatsappActivate()
    {
      WhatsAppLookupId = WhatsappLookupId,
      IsDefault = isDefault,
      Config = jsonStr,
      ClientId = clientId,
      CreatedBy = userId,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }
  public void UpdateWhatsappActivate(string jsonStr, EmployeeId? userId, bool? isActive, bool? isDefault = false)
  {
    Config = jsonStr;
    UpdatedBy = userId;
    UpdateOn = DateTime.UtcNow;
    Active = isActive;
  }
  public void RemoveDefault(EmployeeId? userId)
  {
    IsDefault = false;
    UpdatedBy = userId;
    UpdateOn = DateTime.UtcNow;
  }
  public void MakeDefault(EmployeeId? userId)
  {
    IsDefault = true;
    UpdatedBy = userId;
    UpdateOn = DateTime.UtcNow;
  }
}

