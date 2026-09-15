using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class SaleChannelConfigRepository : ISaleChannelConfigRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public SaleChannelConfigRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<SaleChannelConfig> CreateSaleChannelConfig(SaleChannelConfig SaleChannelConfig)
  {
    await _context.SaleChannelConfigs.AddAsync(SaleChannelConfig);
    await _context.SaveChangesAsync();
    return SaleChannelConfig;
  }
  public async Task<SaleChannelConfig> UpdateSaleChannelConfig(SaleChannelConfig SaleChannelConfig)
  {
    _context.SaleChannelConfigs.Update(SaleChannelConfig);
    await _context.SaveChangesAsync();
    return SaleChannelConfig;
  }
  public async Task<bool> DeleteSaleChannelConfig(SaleChannelConfig SaleChannelConfig)
  {
    _context.SaleChannelConfigs.Update(SaleChannelConfig);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> GetAllSaleChannelConfig(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(scc.SaleChannelConfigId) ";
      TotalCount += @"FROM dbo.SaleChannelConfig AS scc INNER JOIN dbo.SaleChannelLookup AS scl ON scc.SaleChannelLookupId = scl.SaleChannelLookupId ";

      string query = @" SELECT scc.SaleChannelConfigId,
                               scc.SaleChannelKey,
                               CASE
                                   WHEN scl.SaleChannelName = 'Sale Person'
                                        AND emp.EmployeeName IS NOT NULL THEN
                                       emp.EmployeeName + ' (' + scl.SaleChannelName + ') '
                                   ELSE
                                       scc.SaleChannelName + ' (' + scl.SaleChannelName + ') '
                               END as SaleChannelName,
                               scl.ImageUrl,
                               s.StoreName,
                               scc.ClientId,
                               scc.Config,
                               scc.CreatedOn,
                               scc.Active
                        FROM dbo.SaleChannelConfig AS scc
                            INNER JOIN dbo.SaleChannelLookup AS scl
                                ON scc.SaleChannelLookupId = scl.SaleChannelLookupId
                            INNER JOIN dbo.Stores AS s ON s.StoreId = scc.StoreId 
                            OUTER APPLY (
                                SELECT TOP 1 e.EmployeeName
                                FROM dbo.Employee AS e
                                WHERE e.SaleChannelConfigId = scc.SaleChannelConfigId
                                    AND e.Active = 1
                            ) AS emp ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( scc.SaleChannelName in (select value from STRING_SPLIT(@search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (scc.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("scc.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("scc.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "scc.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      result.list = data.ToList();
      return result;
    }
  }
  public async Task<List<SaleChannelLookup>?> GetAllSaleChannelLookupForSelection()
  {
    return await _context.SaleChannelLookups.ToListAsync();
  }
  public async Task<SaleChannelConfig?> GetClientDefaultSaleChannelConfigById(ClientId clientId)
  {
    return await _context.SaleChannelConfigs.Where(x => x.ClientId == clientId).FirstOrDefaultAsync();
  }
  public async Task<SaleChannelConfig?> GetSaleChannelConfigById(int SaleChannelConfigId, ClientId clientId)
  {
    return await _context.SaleChannelConfigs.FirstOrDefaultAsync(x => x.SaleChannelConfigId == SaleChannelConfigId && x.ClientId == clientId && x.Active == true);
  }
  public async Task<SaleChannelConfig?> GetSaleChannelConfigForUpdateById(int SaleChannelConfigId, ClientId clientId)
  {
    return await _context.SaleChannelConfigs.FirstOrDefaultAsync(x => x.SaleChannelConfigId == SaleChannelConfigId && x.ClientId == clientId);
  }
  public async Task<List<SaleChannelConfig>?> GetSaleChannelConfigBySaleChannelLookupId(int? SaleChannelLookupId, ClientId clientId)
  {
    return await _context.SaleChannelConfigs.Where(x => x.SaleChannelLookupId == SaleChannelLookupId && x.ClientId == clientId).ToListAsync();
  }
  public async Task<SaleChannelLookup?> GetSaleChannelLookupById(int SaleChannelLookupId)
  {
    return await _context.SaleChannelLookups.Where(x => x.SaleChannelLookupId == SaleChannelLookupId).FirstOrDefaultAsync();
  }
  public async Task<SaleChannelConfig?> GetSaleChannelConfigByKey(string saleChannelKey)
  {
    return await _context.SaleChannelConfigs.Where(x => x.SaleChannelKey == saleChannelKey).FirstOrDefaultAsync();
  }
  public async Task<dynamic> GetSaleChannelByStoreIdForSelection(int storeId, string? clientId, int? roleId = 0)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT scc.SaleChannelConfigId AS id,
                               scc.SaleChannelName,
                               CASE
                                   WHEN scl.SaleChannelName = 'Sale Person'
                                        AND emp.EmployeeName IS NOT NULL THEN
                                       emp.EmployeeName + ' (' + scl.SaleChannelName + ')'
                                   ELSE
                                       scc.SaleChannelName + ' (' + scl.SaleChannelName + ')'
                               END AS [text]
                        FROM dbo.SaleChannelConfig AS scc
                            INNER JOIN dbo.Stores AS s
                                ON s.StoreId = scc.StoreId
                            INNER JOIN dbo.SaleChannelLookup AS scl
                                ON scl.SaleChannelLookupId = scc.SaleChannelLookupId
                            LEFT JOIN dbo.ShopifyConfig AS sc
                                ON sc.SaleChannelConfigId = scc.SaleChannelConfigId
                                   AND sc.AccessToken IS NOT NULL

                            -- ✅ pick only ONE employee per SaleChannelConfigId (prevents extra rows)
                            OUTER APPLY
                        (
                            SELECT TOP 1
                                   e.EmployeeName
                            FROM dbo.Employee e
                            WHERE e.SaleChannelConfigId = scc.SaleChannelConfigId
                                  AND e.EmployeeName IS NOT NULL
                            ORDER BY e.EmployeeId DESC -- change ordering if you want ""latest""
                        ) emp  ";

      string whereStart = "WHERE ( 1=1 AND (scc.Active = 1 ) AND ( scc.SaleChannelName IS NOT NULL ) ";
      string whereEnd = ")";
      if (roleId > 0 && roleId == (int)EnumUserRole.SalePerson)
      {
        whereStart += $" And (scl.SaleChannelLookupId <> {(int)EnumSaleChannelLookup.SalePerson}) ";
      }
      #region storeId
      if (storeId > 0)
      {
        dynamicParams.Add("@storeId", storeId);
        whereStart += " And (scc.StoreId = @storeId ) ";
      }
      #endregion

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@clientId", clientId);
        whereStart += "And (scc.ClientId = @clientId) ";
      }
      string where = whereStart + whereEnd;
      var data = await connection.QueryAsync(query + where, dynamicParams);
      return data.ToList();
    }
  }
  public async Task<dynamic> GetSaleChannelInforByConfigId(int? saleChannelConfigId, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"
                      SELECT e.EmployeeId,
                             e.EmployeeName,
                             e.EmployeeCode,
                             e.EmployeeImage,
                             e.MobileNo as Mobile ,
                             e.WorkEmail as Email,
                             c.ClientIdentifier,
                             e.CreatedOn,
                             ea.FullAddress,
        
                             c4.Name AS CountryName
                      FROM dbo.Employee AS e
                          LEFT JOIN dbo.EmployeeAddress AS ea
                              ON ea.EmployeeId = e.EmployeeId
                          LEFT JOIN dbo.Client AS c
                              ON c.ClientId = e.ClientId
                          LEFT JOIN dbo.Country AS c4
                              ON c4.CountryId = ea.CountryId
		                      LEFT JOIN dbo.ClientConfigSetting AS ccs ON ccs.ClientId = c.ClientId ";

      string whereStart = "WHERE ( 1=1 AND (e.Active = 1 ) ";
      string whereEnd = ")";

      #region storeId
      if (saleChannelConfigId.GetValueOrDefault() > 0)
      {
        dynamicParams.Add("@saleChannelConfigId", saleChannelConfigId);
        whereStart += " And (e.SaleChannelConfigId = @saleChannelConfigId ) ";
      }
      #endregion

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@clientId", clientId);
        whereStart += "And (e.ClientId = @clientId) ";
      }
      string where = whereStart + whereEnd;

      var data = await connection.QueryAsync(query + where, dynamicParams);
      var first  = data.FirstOrDefault();
      return first!;
    }
  }

  public async Task<dynamic> GetAllSaleChannelByLookupIdForSelection(int saleChannelLookupId, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT scc.SaleChannelConfigId AS id,
                               scc.SaleChannelName + ' (' + scl.SaleChannelName + ') ' AS [text]
                        FROM dbo.SaleChannelConfig AS scc
                            INNER JOIN dbo.SaleChannelLookup AS scl
                                ON scl.SaleChannelLookupId = scc.SaleChannelLookupId
                            LEFT JOIN dbo.ShopifyConfig AS sc
                                ON sc.SaleChannelConfigId = scc.SaleChannelConfigId
                                   AND sc.AccessToken IS NOT NULL ";

      string whereStart = "WHERE ( 1=1 AND (scc.Active = 1 ) ";
      string whereEnd = ")";
      whereStart += $" And (scl.SaleChannelLookupId <> {(int)EnumSaleChannelLookup.SalePerson}) ";
      #region SaleChannelLookup
      if (saleChannelLookupId > 0)
      {
        dynamicParams.Add("@SaleChannelLookupId", saleChannelLookupId);
        whereStart += " And (scl.SaleChannelLookupId = @SaleChannelLookupId ) ";
      }
      #endregion

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@clientId", clientId);
        whereStart += "And (scc.ClientId = @clientId) ";
      }
      string where = whereStart + whereEnd;
      var data = await connection.QueryAsync(query + where, dynamicParams);
      return data.ToList();
    }
  }

  public async Task<dynamic> GetAllSaleChannelForSelection(string? storeIds, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT scc.SaleChannelConfigId AS id,
                               scc.SaleChannelName AS [text]
                        FROM dbo.SaleChannelConfig AS scc
                            INNER JOIN dbo.Stores AS s
                                ON s.StoreId = scc.StoreId
                            INNER JOIN dbo.SaleChannelLookup AS scl
                                ON scl.SaleChannelLookupId = scc.SaleChannelLookupId  ";

      string whereStart = "WHERE ( 1=1 AND (scc.Active = 1 ) ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@clientId", clientId);
        whereStart += "And (scc.ClientId = @clientId) ";
      }
      #region storeId
      if (!string.IsNullOrEmpty(storeIds))
      {
        dynamicParams.Add("@storeIds", storeIds);
        whereStart += "And ((scc.StoreId in (select value from STRING_SPLIT(@storeIds,',')) )) ";
      }
      #endregion
      string where = whereStart + whereEnd;
      var data = await connection.QueryAsync(query + where, dynamicParams);
      return data.ToList();
    }

  }

  public async Task<SaleChannelConfig?> GetSaleChannelNameValidate(string? saleChannelName, ClientId clientId)
  {
    return await _context.SaleChannelConfigs.FirstOrDefaultAsync(x => x.SaleChannelName == saleChannelName!.ToLower().Trim() && x.ClientId == clientId);
  }
  public async Task<string> GetUniqueSaleChannelNameAsync(string saleChannelName, ClientId clientId)
  {
    if (string.IsNullOrWhiteSpace(saleChannelName))
      throw new ArgumentException("SaleChannelName cannot be null or empty.");

    string trimmedName = saleChannelName.Trim();
    string uniqueName = trimmedName;
    int counter = 1;

    // Check case-insensitively while preserving original casing
    while (await _context.SaleChannelConfigs
               .AnyAsync(x => x.SaleChannelName!.ToLower() == uniqueName.ToLower() && x.ClientId == clientId))
    {
      uniqueName = $"{trimmedName}{counter}";
      counter++;
    }

    return uniqueName;
  }


}
