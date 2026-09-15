using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.ExpenseAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IDriverAccountRepository
{
  #region DriverExpense  
  Task<dynamic> CreateExpense(Expense expense);
  Task<dynamic> UpdateExpense(Expense expense);
  Task<dynamic> DeleteExpense(Expense expense);
  Task DeleteExpensesByDeliveryNoteId(DeliveryNoteId deliveryNoteId);
  Task<Expense?> GetExpenseById(ExpenseId expenseId);
  Task<dynamic?> GetAllDriverExpense(string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? driverId, int? expenseCategoryId);
  #endregion

  #region DriverReceiveable  
  Task<dynamic> CreateDriverReceivable(DriverReceivable driverReceivable);
  Task<dynamic> UpdateDriverReceivable(DriverReceivable driverReceivable);
  Task<dynamic> DeleteDriverReceivable(DriverReceivable driverReceivable);
  Task<DriverReceivable?> GetDriverReceivableById(DriverReceivableId driverReceivableById);
  Task<dynamic?> GetAllIDriverReceivables(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sorDir, string clientId, List<Guid>? driverIds);
  #endregion
  #region driveraccountbalance
  Task<dynamic> GetDriverAccountBalanceById(string? driverId,string clientId);
  Task<dynamic> GetAllCODClearedByDriverId(string? driverId,string clientId);
  #endregion
}
