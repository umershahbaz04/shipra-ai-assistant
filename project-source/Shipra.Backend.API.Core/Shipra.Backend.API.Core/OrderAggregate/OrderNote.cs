using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderNote
{
  public OrderNoteId? OrderNoteId { get; set; }
  public OrderId? OrderId { get; set; }
  public string? NoteDescription { get; set; }
  public EmployeeId? CreatedBy { get; set; }
  public DateTime? CreatedOn { get; set; }
  public EmployeeId? UpdatedBy { get; set; }
  public DateTime? UpdatedOn { get; set; }
  public bool? Active { get; set; }
  public static OrderNote CreateOrderNote(OrderId orderId, string? NoteDescription, EmployeeId createdBy)
  {
    return new OrderNote()
    {
      OrderNoteId = OrderNoteId.New,
      OrderId = orderId,
      NoteDescription = NoteDescription,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true
    };
  }
  public void UpdateOrderNote(string? note, EmployeeId updatedBy)
  {
    NoteDescription = note;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
}

public sealed record OrderNoteId(Guid Value)
{
  public static OrderNoteId New => new(Guid.NewGuid());
}
