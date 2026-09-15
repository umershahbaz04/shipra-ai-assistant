using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class DriverAccountRepository : IDriverAccountRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public DriverAccountRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  #region DriverExpense
  public async Task<dynamic> CreateExpense(Expense expense)
  {
    await _context.Expenses.AddAsync(expense);
    await _context.SaveChangesAsync();
    return expense;
  }
  public async Task<dynamic> DeleteExpense(Expense expense)
  {
    _context.Remove(expense);
    await _context.SaveChangesAsync();
    return expense;
  }
  public async Task DeleteExpensesByDeliveryNoteId(DeliveryNoteId deliveryNoteId)
  {
    var expenses = await _context.Expenses.Where(x => x.DeliveryNoteId == deliveryNoteId).ToListAsync();
    if (expenses.Any())
    {
      _context.Expenses.RemoveRange(expenses);
      await _context.SaveChangesAsync();
    }
  }
  public async Task<dynamic?> GetAllDriverExpense(string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? driverId, int? expenseCategoryId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();
            var query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY e.CreatedOn)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               e.ExpenseId,
                               e.Active,
                               e.Details,
                               e.DriverId,
                               d.DriverCode,
                               emp.EmployeeCode,
                               ISNULL(emp.EmployeeName, '') AS DriverName,
                               dn.NoteNo,
                               e.ExpenseDate,
                               ISNULL(ec.ExpenceName, '') AS ExpenseCategoryName,
                               e.ExpenseCategoryId,
                               e.CreatedOn,
                               dn.DeliveryNoteId,
                               e.Amount,
	                           ISNULL(c.ClientCompanyName,c.ClientName) AS ClientName
                        FROM dbo.Expense AS e
                            INNER JOIN dbo.ExpenseCategory AS ec
                                ON ec.ExpenseCategoryId = e.ExpenseCategoryId
                            LEFT JOIN dbo.Driver AS d
                                ON d.DriverId = e.DriverId
                            LEFT JOIN dbo.Client AS c
                                ON c.ClientId = e.ClientId
                            LEFT JOIN dbo.Employee AS emp
                                ON d.EmployeeId = emp.EmployeeId
                            INNER JOIN dbo.DeliveryNote AS dn
                                ON dn.DeliveryNoteId = e.DeliveryNoteId
                            INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                ON dnd.DeliveryNoteId = dn.DeliveryNoteId ";

      string whereStart = "WHERE ( e.Active=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @" And ( d.DriverCode in (select value from STRING_SPLIT(@search,',')))
                              OR	
                            ( emp.EmployeeName in (select value from STRING_SPLIT(@search,',')))
                             OR
                            ( dn.NoteNo in (select value from STRING_SPLIT(@search,',')))
                            OR
                            ( ec.ExpenceName in (select value from STRING_SPLIT(@search,','))) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (e.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("e.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("e.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }

      //driverreceiveable
      whereStart += @"And (e.DriverReceivableId IS NOT NULL) ";

      if (!string.IsNullOrEmpty(driverId) && driverId != "0")
      {
        dynamicParams.Add("@driverId", driverId);
        whereStart += "And (e.DriverId = @driverId) ";
      }
      if (expenseCategoryId != 0)
      {
        dynamicParams.Add("@expenseCategoryId", expenseCategoryId);
        whereStart += "And (e.ExpenseCategoryId = @expenseCategoryId) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "e.ExpenseId");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }
  public async Task<Expense?> GetExpenseById(ExpenseId expenseId)
  {
    return await _context.Expenses.FirstOrDefaultAsync(x => x.ExpenseId! == expenseId);
  }
  public async Task<dynamic> UpdateExpense(Expense expense)
  {
    _context.Expenses.Update(expense);
    await _context.SaveChangesAsync();
    return expense;
  }
  #endregion

  #region MyRegion
  public async Task<dynamic> CreateDriverReceivable(DriverReceivable driverReceivable)
  {
    await _context.DriverReceivables.AddAsync(driverReceivable);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> UpdateDriverReceivable(DriverReceivable driverReceivable)
  {
    _context.DriverReceivables.Update(driverReceivable);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> DeleteDriverReceivable(DriverReceivable driverReceivable)
  {
    _context.Remove(driverReceivable);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<DriverReceivable?> GetDriverReceivableById(DriverReceivableId driverReceivableById)
  {
    return await _context.DriverReceivables.FirstOrDefaultAsync(x => x.DriverReceivableId! == driverReceivableById && x.Active == true);
  }
  public async Task<dynamic?> GetAllIDriverReceivables(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, List<Guid>? driverIds)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      var query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY dr.CreatedOn)) AS RowNum,
                                         COUNT(*) OVER () AS TotalCount,
                                         dr.DriverReceivableId,
                                         dr.DriverId,
                                         dr.DeliveryNoteId,
                                         dr.ReceiveDate,
                                         dr.Expense,
                                         dr.Cash,
                                         dr.Total,
                                         dr.OtherCollections,
                                         (SELECT COUNT(*) FROM dbo.DeliveryNoteDetail dnd WHERE dnd.DeliveryNoteId = dr.DeliveryNoteId) AS TotalShipment,
                                         dr.Comment,
                                         dr.CreatedOn,
                                         dr.CreatedBy,
                                         dr.Active,
                                         dr.DriverReceivableNo,
                                         dr.DriverPaidStatusId,
                                         ISNULL(e.EmployeeName, '') AS DriverName,
                                         ISNULL(e.MobileNo, '') AS Mobile
                                  FROM dbo.DriverReceivable AS dr
                                      INNER JOIN dbo.Driver AS d
                                          ON dr.DriverId = d.DriverId
                                      LEFT JOIN dbo.DriverPaidStatus AS dps
                                              ON dps.DriverPaidStatusId = dr.DriverPaidStatusId
                                      LEFT JOIN dbo.Employee AS e
                                          ON d.EmployeeId = e.EmployeeId ";

      string whereStart = $"WHERE ( dr.Active=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( dr.DriverReceivableNo in (select value from STRING_SPLIT(@search,',')) )) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (d.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dr.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dr.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      if (driverIds != null && driverIds.Any())
      {
        dynamicParams.Add("@DriverIds", driverIds);
        whereStart += "And (dr.DriverId IN @DriverIds) ";
      }

      string where = whereStart + whereEnd;
      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "dr.DriverReceivableId");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }
  #endregion

  #region driveracountbalance
  public async Task<dynamic> GetDriverAccountBalanceById(string? driverId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT ISNULL(
                                                 (
                                                     SELECT SUM(o.Amount) AS CashCollected
                                                     FROM dbo.DeliveryNote AS dn
                                                         INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                                             ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                                                         INNER JOIN dbo.[Order] AS o
                                                             ON o.OrderId = dnd.OrderId
                                                     WHERE ISNULL(dn.IsCompleted, 0) = {(int)EnumDeliveryNoteStatusLookup.Completed}
                                                           AND o.PaymentMethodId = {(int)EnumPaymentMethod.COD}
                                                           AND CAST(dn.CompletedOn AS DATE) = CAST(GETDATE() AS DATE)
                                                           AND dn.DriverId = '{driverId}'
                                                 ),
                                                 0
                                                       ) AS CashCollected,
                                                 ISNULL(
                                                 (
                                                     SELECT SUM(o.Amount) AS ClearancePending
                                                     FROM dbo.DeliveryNote AS dn
                                                         INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                                             ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                                                         INNER JOIN dbo.[Order] AS o
                                                             ON o.OrderId = dnd.OrderId
                                                     WHERE ISNULL(dn.IsCompleted, 0) = {(int)EnumDeliveryNoteStatusLookup.Pending}
                                                           AND o.PaymentMethodId = {(int)EnumPaymentMethod.COD}
                                                           AND dn.DriverId = '{driverId}'
                                                 ),
                                                 0
                                                       ) AS ClearancePending,
                                                 ISNULL(
                                                 (
                                                     SELECT SUM(e.Amount) AS Expenses
                                                     FROM dbo.DeliveryNote AS dn
                                                         INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                                             ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                                                         INNER JOIN dbo.Expense AS e
                                                             ON e.DeliveryNoteId = dn.DeliveryNoteId
                                                     WHERE ISNULL(dn.IsCompleted, 0) =  {(int)EnumDeliveryNoteStatusLookup.Pending}
                                                           AND dn.DriverId = '{driverId}'
                                                     GROUP BY dn.DriverId
                                                 ),
                                                 0
                                                       ) AS Expenses;";


      var data = await connection.QueryAsync(query, dynamicParams);
      return data;
    }
  }

  public async Task<dynamic> GetAllCODClearedByDriverId(string? driverId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT DISTINCT
                               dn.DeliveryNoteId,
                               dn.NoteNo,
                               dn.ShipmentCount AS TotalShipment,
                               dn.CompletedOn AS NoteCompletionDate,
                               dr.ReceiveDate AS CODDate,
                               dr.Expense AS TotalExpense,
                               dr.Cash AS TotalCOD,
                               dr.Total AS TotalAmount,
                               sb.Delivered
                        FROM dbo.DeliveryNote AS dn
                            INNER JOIN dbo.DriverReceivable AS dr
                                ON dr.DeliveryNoteId = dn.DeliveryNoteId
                            INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                            LEFT JOIN
                            (
                                SELECT dn2.DeliveryNoteId,
                                       COUNT(o2.CarrierTrackingStatusId) AS Delivered
                                FROM dbo.[Order] AS o2
                                    INNER JOIN dbo.DeliveryNoteDetail AS dnd2
                                        ON dnd2.OrderId = o2.OrderId
                                    INNER JOIN dbo.DeliveryNote AS dn2
                                        ON dn2.DeliveryNoteId = dnd2.DeliveryNoteId
                                WHERE o2.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered}
                                GROUP BY dn2.DeliveryNoteId
                            ) AS sb
                                ON sb.DeliveryNoteId = dn.DeliveryNoteId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(driverId))
      {
        dynamicParams.Add("@DriverId", driverId);
        whereStart += "And (dn.DriverId = @DriverId) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }
  #endregion
}
