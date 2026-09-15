using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;

namespace Shipra.Backend.API.Core.ExpenseAggregate;
public class ExpenseCategory
{
  public int ExpenseCategoryId { get; set; }

  public string? ExpenceName { get; set; }

  public ClientId? ClientId { get; set; }

  public DateTime? CreatedOn { get; set; }

  public EmployeeId? CreatedBy { get; set; }

  public DateTime? UpdatedOn { get; set; }

  public EmployeeId? UpdatedBy { get; set; }

  public bool? Active { get; set; }

  public static ExpenseCategory CreateExpenseCategory(ClientId clientId, string expenseName, EmployeeId createdBy)
  {
    return new ExpenseCategory()
    {
      ClientId = clientId,
      ExpenceName = expenseName,
      CreatedOn = DateTime.UtcNow,
      CreatedBy = createdBy,
      Active = true
    };
  }
}
