namespace Shipra.Backend.API.Core.Models;

public class LeadDashboardResponseModel
{
    public int LeadGridColumnId { get; set; }
    public string? DashboardStatusName { get; set; }
    public string? DashboardStatusNameForKey { get; set; }
    public string? DashboardStatusValue { get; set; }
    public bool IsDisplay { get; set; }
    public bool Active { get; set; }
    public bool IsDefaultStatusTab { get; set; }
    public int DisplayOrder { get; set; }
    public string? ClientId { get; set; }
}
