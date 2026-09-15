using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.DeliveryTaskAggregate;
public class DeliveryTask
{
  public DeliveryTask() { }
  public DeliveryTaskId? DeliveryTaskId { get; set; }
  public string? JobCode { get; set; }
  public OrderId? OrderId { get; set; }
  public DriverId? DriverId { get; set; }
  public ClientId? ClientId { get; set; }
  public int? DriverPaid { get; set; }
  public DateTime? DriverAssignedDate { get; set; }
  public DateTime? DriverPaidDate { get; set; }
  public int? DeliveryTaskStatusId { get; set; }
  public DriverReceivableId? DriverReceivableId { get; set; }
  public int? LastStatusUpdateId { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public bool? Active { get; set; }
  public DeliveryNoteDetailId? DeliveryNoteDetailId { get; set; }
  public int? SortOrder { get; set; }
  public static DeliveryTask CreateDeliveryTask(ClientId clientId, OrderId? orderId, EmployeeId createdBy)
  {
    var deliveryTask = new DeliveryTask()
    {
      DeliveryTaskId = DeliveryTaskId.New,
      ClientId = clientId,
      OrderId = orderId,
      DeliveryTaskStatusId = (int)EnumDeliveryTaskStatusLookup.Unallocated,
      Active = true,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
    return deliveryTask;
  }

  public void AssignDriver(DateTime? assigningDate, DriverId driverId, EmployeeId updatedBy)
  {
    DeliveryTaskStatusId = (int)EnumDeliveryTaskStatusLookup.Allocated;
    DriverAssignedDate = assigningDate;
    DriverId = driverId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UnAssignDriver(EmployeeId updatedBy)
  {
    DeliveryTaskStatusId = (int)EnumDeliveryTaskStatusLookup.Unallocated;
    DriverAssignedDate = null;
    DriverId = null;
    DeliveryNoteDetailId = null;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateDeliveryTask(string? jobCode, DriverId? driverId, int? driverPaid, int? deliveryTaskStatusId, DriverReceivableId? driverReceivableId, int? lastStatusUpdateId, bool? active, int? sortOrder)
  {
    JobCode = jobCode;
    DriverId = driverId;
    DriverPaid = driverPaid;
    DeliveryTaskStatusId = deliveryTaskStatusId;
    DriverReceivableId = driverReceivableId;
    LastStatusUpdateId = lastStatusUpdateId;
    Active = active;
    SortOrder = sortOrder;
  }

  public void UpdateDeliveryTaskStatus(int? deliveryTaskStatusId, EmployeeId employeeId)
  {
    DeliveryTaskStatusId = deliveryTaskStatusId;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateDeliveryTaskForCompleted(EmployeeId employeeId)
  {
    DriverId = null;
    DeliveryTaskStatusId = (int)EnumDeliveryTaskStatusLookup.Completed;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
}
public sealed record DeliveryTaskId(Guid Value)
{
  public static DeliveryTaskId New => new(Guid.NewGuid());
}
