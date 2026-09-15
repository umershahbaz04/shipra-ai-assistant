using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
public class ShipmentGridColumn
{
  public int ShipmentGridColumnId { get; private set; }
  public string? ColumnName { get; private set; }
  public ClientId? ClientId { get; private set; } 
  public bool? IsDefaultStatusTab { get; private set; }
  public bool? IsFetchAllPendingStatus { get; private set; }
  public bool? IsCompleted { get; private set; }
  public int DisplayOrder { get; private set; } 
  public bool? IsDisplay { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public bool? Active { get; private set; }

  public static ShipmentGridColumn Create(string columnName, int displayOrder,bool? isFetchAllPendingStatus, bool? isCompleted, ClientId clientId, EmployeeId employeeId, bool? isDefaultStatusTab = false)
  {
    return new ShipmentGridColumn()
    {
      ColumnName = columnName,
      DisplayOrder = displayOrder,
      ClientId = clientId,
      IsDefaultStatusTab = isDefaultStatusTab,
      IsCompleted = isCompleted,
      IsFetchAllPendingStatus = isFetchAllPendingStatus,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = employeeId,
      IsDisplay = true,
      Active = true
    };
  }

  public static int GetNextDisplayOrder(ShipmentGridColumn? lstShipmentGridColumn)
  {
    //order start from
    int displayOrder = 1;
    if (lstShipmentGridColumn != null)
    {
      displayOrder = lstShipmentGridColumn.DisplayOrder + 1;
    }
    return displayOrder;
  }

  public void UpdateColumnName(string? columnName,EmployeeId employeeId)
  {
    ColumnName = columnName;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateDisplayOrder(int displayOrder, EmployeeId employeeId)
  {
    DisplayOrder = displayOrder;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
}
