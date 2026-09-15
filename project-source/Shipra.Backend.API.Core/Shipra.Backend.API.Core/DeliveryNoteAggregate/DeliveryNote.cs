using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Core.DeliveryNoteAggregate;
public class DeliveryNote
{
  public DeliveryNote() { }
  public DeliveryNoteId? DeliveryNoteId { get; set; }
  public string? NoteNo { get; set; }
  public DriverId? DriverId { get; set; }
  public ClientId? ClientId { get; set; }
  public int? ShipmentCount { get; set; }
  public int? DeliveryNoteStatusId { get; set; }
  public bool? IsCompleted { get; set; }
  public DateTime? CompletedOn { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public bool? Active { get; set; }
  public static DeliveryNote CreateDeliveryNote(string nextNoteNumber, DriverId driverId, int shipmentCount, ClientId clientId, EmployeeId createdBy)
  {
    var deliveryNote = new DeliveryNote()
    {
      DeliveryNoteId = DeliveryNoteId.New,
      NoteNo = nextNoteNumber,
      DriverId = driverId,
      ShipmentCount = shipmentCount,
      ClientId = clientId,
      DeliveryNoteStatusId = (int)EnumDeliveryNoteStatusLookup.InProgress,
      IsCompleted = false,
      Active = true,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
    return deliveryNote;
  }

  public void AddToExisting(int totalShipments, EmployeeId? employeeId)
  {
    ShipmentCount = totalShipments;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void RemoveDriver(EmployeeId? employeeId)
  {
    DriverId = null;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateDeliveryNoteInOperation(EmployeeId employeeId)
  {
    DriverId = null;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateDeliveryNoteStatusForCompletion(EmployeeId employeeId)
  {
    DeliveryNoteStatusId = (int)EnumDeliveryNoteStatusLookup.Completed;
    IsCompleted = true;
    CompletedOn = DateTime.UtcNow;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
}
public sealed record DeliveryNoteId(Guid Value)
{
  public static DeliveryNoteId New => new(Guid.NewGuid());
}
