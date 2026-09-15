namespace Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
public class DefaultShipmentDashboard
{
  public int DefaultShipmentDashboardId { get; set; }
  public int? DashboardStatusId { get; set; }
  public string? DashboardStatusName { get; set; }
  public string? DashboardStatusValue { get; set; }
  public bool? IsDisplay { get; set; }
  public int? DisplayOrder { get; set; }
  public bool? IsFetchAllPendingStatus { get; private set; }
  public bool? IsCompleted { get; private set; }
  public bool? Active { get; set; }
}
