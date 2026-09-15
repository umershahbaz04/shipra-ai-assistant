using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.SMSProcessAggregate;
public class SMSActivate
{
  /// <summary>
  /// SMSActivate
  /// </summary>
  public int SMSActivateId { get; set; }
  public int? SMSLookupId { get; set; }
  public bool? IsDefault { get; set; }
  public string? Config { get; set; }
  public ClientId? ClientId { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdateOn { get; set; }
  public bool? Active { get; set; }

  public static SMSActivate CreateSMSASctivate(int sMSLookupId, string jsonStr, ClientId? clientId, EmployeeId? userId, bool? isDefault = false)
  {
    return new SMSActivate()
    {
      SMSLookupId = sMSLookupId,
      IsDefault = isDefault,
      Config = jsonStr,
      ClientId = clientId,
      CreatedBy = userId,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public void DeleteSMSActivate(EmployeeId? userId)
  {
    UpdatedBy = userId;
    UpdateOn = DateTime.UtcNow;
    Active = false;
  }

  public void UpdateSMSActivate(string jsonStr, EmployeeId? userId, bool? isActive, bool? isDefault = false)
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
