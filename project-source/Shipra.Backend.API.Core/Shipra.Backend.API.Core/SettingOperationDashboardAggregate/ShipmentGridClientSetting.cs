using DocumentFormat.OpenXml.Wordprocessing;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
public class ShipmentGridClientSetting
{
  public ShipmentGridClientSettingId? ShipmentGridClientSettingId { get; set; }
  public int? ShipmentGridColumnId { get; private set; }
  public string? DashboardStatusValue { get; private set; }
  public ClientId? ClientId { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public bool? Active { get; private set; }

  public static ShipmentGridClientSetting Create(int? shipmentGridColumnId, string? dashboardStatusValue, ClientId? clientId, EmployeeId? createdBy)
  {
    return new ShipmentGridClientSetting()
    {
      ShipmentGridClientSettingId = ShipmentGridClientSettingId.New,
      ShipmentGridColumnId = shipmentGridColumnId,
      DashboardStatusValue = dashboardStatusValue,
      ClientId = clientId,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }

  public void UpdateDashboardStatusValue(string dashboardStatusValue, EmployeeId? updatedBy)
  {
    DashboardStatusValue = dashboardStatusValue;
    UpdatedOn = DateTime.UtcNow;
    UpdatedBy = updatedBy;
  }
}
public sealed record ShipmentGridClientSettingId(Guid Value)
{
  public static ShipmentGridClientSettingId New => new(Guid.NewGuid());
}
