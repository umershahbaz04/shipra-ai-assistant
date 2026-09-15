using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.PaymentProcessAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class PaymentProcessRepository : IPaymentProcessRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public PaymentProcessRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<Ppactivate> CreatePPActivate(Ppactivate ppactivate)
  {
    await _context.Ppactivates.AddAsync(ppactivate);
    await _context.SaveChangesAsync();
    return ppactivate;
  }

  public async Task<dynamic> DeletePPActivate(Ppactivate ppactivate)
  {
    _context.Ppactivates.Update(ppactivate);
    await _context.SaveChangesAsync();
    return ppactivate;
  }

  public async Task<dynamic> GetAllPPActivate(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(pa.PPActivateId) ";
      TotalCount += @"FROM dbo.PPActivate AS pa INNER JOIN dbo.PPLookup AS pl ON pa.PPLookupId = pl.PPLookupId ";

      string query = @" SELECT pa.PPActivateId,
       pa.IsDefault,
       pa.CreatedOn,
       pl.PPName,
       pa.Config,
       pa.Active 
       FROM dbo.PPActivate AS pa INNER JOIN dbo.PPLookup AS pl ON pa.PPLookupId = pl.PPLookupId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( pl.PPName in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (pa.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("pa.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("pa.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "pa.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      result.list = data.ToList();
      return result;
    }

  }

  public async Task<Ppactivate?> GetPPActivateByPPActivateId(int ppactivateId, ClientId clientId)
  {
    return await _context.Ppactivates.Where(x => x.PpactivateId == ppactivateId && x.ClientId == clientId).FirstOrDefaultAsync();
  }

  public async Task<dynamic> UpdatePPActivate(Ppactivate ppactivate)
  {
    _context.Ppactivates.Update(ppactivate);
    await _context.SaveChangesAsync();
    return ppactivate;
  }
  public async Task<Pplookup?> GetPPLookupById(int pPLookupId)
  {
    return await _context.Pplookups.Where(x => x.PplookupId == pPLookupId).FirstOrDefaultAsync();
  }
  public async Task<List<Pplookup>?> GetAllPPLookupForSelection()
  {
    return await _context.Pplookups.ToListAsync();
  }
  public async Task<List<Ppactivate>?> GetAllDefaultPPActivated(ClientId clientId)
  {
    return await _context.Ppactivates.Where(x => x.ClientId == clientId && x.IsDefault == true).ToListAsync();
  }

  public async Task<Ppactivate?> GetPPActivateByPPLookupId(int pplookupId, ClientId clientId)
  {
    return await _context.Ppactivates.Where(x => x.PplookupId == pplookupId && x.ClientId == clientId).FirstOrDefaultAsync();
  }

  public async Task<Ppactivate?> GetClientDefaultPaymentProcessById(ClientId clientId)
  {
    return await _context.Ppactivates.Where(x => x.ClientId == clientId && x.IsDefault == true).FirstOrDefaultAsync();
  }
}
