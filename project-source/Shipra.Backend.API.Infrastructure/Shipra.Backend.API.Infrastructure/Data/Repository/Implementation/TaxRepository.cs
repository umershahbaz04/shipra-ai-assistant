using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.TaxAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class TaxRepository : ITaxRepository
{
  private readonly AppDbContext _context;
  private readonly DapperAppDbContext _dapperAppDbContext;

  public TaxRepository(AppDbContext context, DapperAppDbContext dapperAppDbContext)
  {
    _context = context;
    _dapperAppDbContext = dapperAppDbContext;
  }

  public async Task<ClientTax?> CreateClientTax(ClientTax model)
  {
    await _context.ClientTaxes.AddAsync(model);
    await _context.SaveChangesAsync();
    return model;
  }

  public async Task<dynamic> GetAllClientTaxes(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               ct.ClientTaxId,
                               ttl.Name,
                               ct.Percentage,
                               ct.CreatedOn,
                               ct.UpdateOn,
                               ct.Active
                        FROM dbo.ClientTax AS ct
                            INNER JOIN dbo.TaxTypeLookup AS ttl
                                ON ttl.TaxId = ct.TaxId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( ttl.Name in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (ct.ClientId = @ClientId) ";
      }

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ct.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ct.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }


      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "ct.CreatedOn");


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

  public async Task<List<TaxTypeLookup>> GetAllTaxTypeLookup()
  {
    return await _context.TaxTypeLookups.Where(x => x.Active == true).ToListAsync();

  }

  public async Task<ClientTax?> GetClientTaxById(int clientTaxId, ClientId clientId)
  {
    return await _context.ClientTaxes.FirstOrDefaultAsync(x => x.ClientTaxId == clientTaxId && x.ClientId == clientId);
  }
  public async Task<ClientTax?> GetAddedTaxById(int taxId, ClientId clientId)
  {
    return await _context.ClientTaxes.FirstOrDefaultAsync(x => x.TaxId == taxId && x.ClientId == clientId);
  }

  public async Task<TaxTypeLookup?> GetTaxTypeLookupById(int TaxId)
  {
    return await _context.TaxTypeLookups.FirstOrDefaultAsync(x => x.TaxId == TaxId && x.Active == true);
  }

  public async Task<bool> UpdateClientTax(ClientTax model)
  {
    _context.ClientTaxes.Update(model);
    return await _context.SaveChangesAsync() > 0;
  }
}
