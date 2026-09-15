using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CompleteDeliveryNote;
public class CompleteDeliveryNoteCommand : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteId { get; set; }
  public decimal? TotalAmount { get; set; }
  public List<ExpenseModel>? ExpenseList { get; set; } = new();
  public List<DebriefItemUpdateModel>? DebriefItems { get; set; } = new();
}
public class ExpenseModel
{
  public int ExpenseCategoryId { get; set; }
  public decimal Amount { get; set; }
  public DateTime ExpenseDate { get; set; }
  public string? Detail { get; set; }
}
public class DebriefItemUpdateModel
{
  public string? DeliveryNoteDetailId { get; set; }
  public string? OrderId { get; set; }
  public decimal? Amount { get; set; }
  public int? StatusId { get; set; } // 1 = InOperation, 2 = PendingForReturn, 3 = Delivered
}
