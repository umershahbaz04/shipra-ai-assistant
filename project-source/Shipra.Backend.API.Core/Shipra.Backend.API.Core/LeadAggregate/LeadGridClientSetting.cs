using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.LeadAggregate;

/// <summary>
/// Maps lead status IDs to a client's tab column (mirrors ShipmentGridClientSetting)
/// </summary>
public class LeadGridClientSetting
{
    public LeadGridClientSettingId? LeadGridClientSettingId { get; set; }
    public int? LeadGridColumnId { get; set; }
    /// <summary>Comma-separated LeadStatusId values belonging to this tab</summary>
    public string? DashboardStatusValue { get; set; }
    public ClientId? ClientId { get; set; }
    public EmployeeId? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public EmployeeId? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool? Active { get; set; }

    public static LeadGridClientSetting Create(int? leadGridColumnId, string? dashboardStatusValue, ClientId? clientId, EmployeeId? createdBy)
    {
        return new LeadGridClientSetting()
        {
            LeadGridClientSettingId = LeadAggregate.LeadGridClientSettingId.New,
            LeadGridColumnId = leadGridColumnId,
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

public sealed record LeadGridClientSettingId(Guid Value)
{
    public static LeadGridClientSettingId New => new(Guid.NewGuid());
}
