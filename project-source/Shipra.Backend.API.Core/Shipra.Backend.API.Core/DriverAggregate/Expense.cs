using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.DriverAggregate;
public class Expense
{
  public ExpenseId? ExpenseId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public DriverId? DriverId { get; private set; }
  public DriverReceivableId? DriverReceivableId { get; private set; }
  public DeliveryNoteId? DeliveryNoteId { get; private set; }
  public decimal Amount { get; private set; }
  public DateTime ExpenseDate { get; private set; }
  public int ExpenseCategoryId { get; private set; }
  public string? Details { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public bool? Active { get; private set; }

  public static Expense CreateExpense(ClientId clientId, DriverId? driverId, DriverReceivableId driverReceivableId, DeliveryNoteId deliveryNoteId, decimal amount, DateTime expenseDate, int expenseCategoryId, string? details, EmployeeId createdBy)
  {
    return new Expense()
    {
      ExpenseId = ExpenseId.New,
      ClientId = clientId,
      DriverId = driverId,
      DriverReceivableId = driverReceivableId,
      DeliveryNoteId = deliveryNoteId,
      ExpenseCategoryId = expenseCategoryId,
      Amount = amount,
      ExpenseDate = expenseDate,
      Details = details,
      CreatedBy = createdBy,
      CreatedOn = DateTime.UtcNow,
      Active = true,
    };
  }

  public void UpdateExpense(ClientId clientId, DriverId? driverId, decimal amount, DateTime expenseDate, int expenseCategoryId, string? details, EmployeeId updatedBy)
  {
    ClientId = clientId;
    DriverId = driverId;
    Amount = amount;
    ExpenseDate = expenseDate;
    Details = details;
    ExpenseCategoryId = expenseCategoryId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
  }
}
public sealed record ExpenseId(Guid Value)
{
  public static ExpenseId New => new(Guid.NewGuid());
}
