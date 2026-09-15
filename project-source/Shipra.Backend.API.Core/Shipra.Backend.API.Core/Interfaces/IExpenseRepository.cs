using Shipra.Backend.API.Core.ExpenseAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IExpenseRepository
{
  Task<ExpenseCategory> CreateExpenseCategory(ExpenseCategory expenseCategory);
  Task<dynamic> GetAllExpenseAccount(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string v);
}
