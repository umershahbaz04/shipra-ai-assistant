using DocumentFormat.OpenXml.Spreadsheet;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.PaymentProcessAggregate;
public class Ppactivate
{
  /// <summary>
  /// PaymentProcessActivateId
  /// </summary>
  public int PpactivateId { get; set; }
  public int? PplookupId { get; set; }
  public bool? IsDefault { get; set; }
  public string? Config { get; set; }
  public ClientId? ClientId { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdateOn { get; set; }
  public bool? Active { get; set; }

  public static Ppactivate CreatePpactivate(int pplookupId, string jsonStr, ClientId? clientId, EmployeeId? userId, bool? isDefault = false)
  {
    return new Ppactivate()
    {
      PplookupId = pplookupId,
      IsDefault = isDefault,
      Config = jsonStr,
      ClientId = clientId,
      CreatedBy = userId,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public void DeletePpactivate(EmployeeId? userId)
  {
    UpdatedBy = userId;
    UpdateOn = DateTime.UtcNow;
    Active = false;
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

  public void UpdatePpactivate(string jsonStr, EmployeeId? userId, bool? isActive = true, bool? isDefault = false)
  {
    Config = jsonStr;
    UpdatedBy = userId;
    UpdateOn = DateTime.UtcNow;
    Active = isActive;
  }
}
