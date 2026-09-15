using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.DeliveryNoteAggregate;
public class DeliveryNoteDetail
{
  public DeliveryNoteDetailId? DeliveryNoteDetailId { get; set; }
  public DeliveryNoteId? DeliveryNoteId { get; set; }
  public OrderId? OrderId { get; set; }
  public int? DeliveryNoteDetailStatusId { get; set; }
  public int? DriverLastUpdatedStatusId { get; set; }
  public bool? IsInProcess { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public bool? Active { get; set; }
  public static DeliveryNoteDetail CreateDeliveryNoteDetail(DeliveryNoteId deliveryNoteId, OrderId orderId, EmployeeId createdBy)
  {
    var deliveryNoteDetail = new DeliveryNoteDetail()
    {
      DeliveryNoteDetailId = DeliveryNoteDetailId.New,
      DeliveryNoteId = deliveryNoteId,
      OrderId = orderId,
      DeliveryNoteDetailStatusId = (int)EnumDeliveryNoteDetailStatusLookup.Pending,
      IsInProcess = true,
      Active = true,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow
    };
    return deliveryNoteDetail;
  }
  public void UpdateDeliveryNoteDetailStatus(int deliveryNoteDetailStatusId, EmployeeId employeeId)
  {
    DeliveryNoteDetailStatusId = deliveryNoteDetailStatusId;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateDeliveryNoteDetailStatusForRevert(EmployeeId employeeId)
  {
    DeliveryNoteDetailStatusId = (int)EnumDeliveryNoteDetailStatusLookup.Pending;
    DriverLastUpdatedStatusId = null;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateDeliveryNoteDetailStatusByDriverForCompletion(EmployeeId employeeId)
  {
    DeliveryNoteDetailStatusId = (int)EnumDeliveryNoteDetailStatusLookup.Completed;
    DriverLastUpdatedStatusId = (int)EnumCarrierTrackingStatus.Delivered;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void ReassignToDeliveryNote(DeliveryNoteId deliveryNoteId, EmployeeId updatedBy)
  {
    DeliveryNoteId = deliveryNoteId;
    DeliveryNoteDetailStatusId = (int)EnumDeliveryNoteDetailStatusLookup.Pending;
    DriverLastUpdatedStatusId = null;
    IsInProcess = true;
    Active = true;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
}
public sealed record DeliveryNoteDetailId(Guid value)
{
  public static DeliveryNoteDetailId New => new(Guid.NewGuid());
}
