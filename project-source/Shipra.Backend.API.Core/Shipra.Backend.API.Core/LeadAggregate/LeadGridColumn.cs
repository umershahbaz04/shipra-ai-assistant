using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.LeadAggregate;

/// <summary>
/// Per-client lead tab column configuration (mirrors ShipmentGridColumn for shipments)
/// </summary>
public class LeadGridColumn
{
    public int LeadGridColumnId { get; set; }
    public string? ColumnName { get; set; }
    public ClientId? ClientId { get; set; }
    public bool? IsDefaultStatusTab { get; set; }
    public bool? IsCompleted { get; set; }
    public int DisplayOrder { get; set; }
    public bool? IsDisplay { get; set; }
    public DateTime? CreatedOn { get; set; }
    public EmployeeId? CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public EmployeeId? UpdatedBy { get; set; }
    public bool? Active { get; set; }

    public static LeadGridColumn Create(string columnName, int displayOrder, bool? isCompleted, ClientId clientId, EmployeeId employeeId, bool? isDefaultStatusTab = false)
    {
        return new LeadGridColumn()
        {
            ColumnName = columnName,
            DisplayOrder = displayOrder,
            ClientId = clientId,
            IsDefaultStatusTab = isDefaultStatusTab,
            IsCompleted = isCompleted,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = employeeId,
            IsDisplay = true,
            Active = true
        };
    }
}
