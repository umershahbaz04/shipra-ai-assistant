using System.Dynamic;
using Dapper;
using Shipra.Backend.API.Core.ExpenseAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ExpenseRepository : IExpenseRepository
{
  private readonly AppDbContext _context;
  private readonly DapperAppDbContext _dapperAppDbContext;

  public ExpenseRepository(AppDbContext context, DapperAppDbContext dapperAppDbContext)
  {
    _context = context;
    _dapperAppDbContext = dapperAppDbContext;
  }

  public async Task<ExpenseCategory> CreateExpenseCategory(ExpenseCategory expenseCategory)
  {
    await _context.ExpenseCategories.AddAsync(expenseCategory);
    await _context.SaveChangesAsync();
    return expenseCategory;
  }
  public async Task<dynamic> GetAllExpenseAccount(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();
      var query = @"SELECT
                    ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY e.CreatedOn)) AS RowNum,
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
                     e.Amount
                  FROM dbo.Expense AS e
                  LEFT JOIN dbo.ExpenseCategory AS ec
                      ON ec.ExpenseCategoryId = e.ExpenseCategoryId
                  LEFT JOIN dbo.Driver AS d
                      ON d.DriverId = e.DriverId
                  LEFT JOIN dbo.Employee AS emp
                      ON d.EmployeeId = emp.EmployeeId
				          INNER JOIN DeliveryNote AS dn
					            ON dn.DeliveryNoteId = e.DeliveryNoteId ";

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
}
