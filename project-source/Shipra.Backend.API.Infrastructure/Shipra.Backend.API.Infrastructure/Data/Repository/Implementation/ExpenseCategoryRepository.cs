using Dapper;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ExpenseCategoryRepository : IExpenseCategoryRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public ExpenseCategoryRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<int> CreateExpenseCategoryForGeneralSetting(ClientId? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!.Value.ToString()))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"INSERT INTO dbo.ExpenseCategory(ExpenceName,ClientId,Active) SELECT ecl.ExpenseCategoryName,'{clientId?.Value}',1 FROM dbo.ExpenseCategoryLookup AS ecl;";     

      var IsSaved = await connection.ExecuteAsync(query);     
      return IsSaved;
    }
  }
}
