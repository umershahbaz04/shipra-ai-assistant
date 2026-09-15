namespace Shipra.Backend.API.Application.DTOs.SettingOperationDashboardUseCase.Response;
public class SettingOperationDashboardResponseModel
{
  public int SettingOperationDashboardId { get; set; }
  public int? DashboardStatusId { get; set; }
  public string? DashboardStatusName { get; set; }
  public string? DashboardStatusValue { get; set; }
  public bool? IsDisplay { get; set; }
  public int? DisplayOrder { get; set; }
  public bool? Active { get; set; }
}
