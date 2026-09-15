using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.WebhookEventAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class WebHookEventRepository : IWebHookEventRepository
{
  private readonly AppDbContext _context;
  private readonly DapperAppDbContext _dapperAppDbContext;

  public WebHookEventRepository(AppDbContext context, DapperAppDbContext dapperAppDbContext)
  {
    _context = context;
    _dapperAppDbContext = dapperAppDbContext;
  }

  #region webhook event

  public async Task<bool?> CreateClientWebhookEvent(ClientWebhookEvent model)
  {
    await _context.ClientWebhookEvents.AddAsync(model);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<ClientWebhookEvent?> IsClientWebhookEventExist(ClientWebhookEvent model)
  {
    return await _context.ClientWebhookEvents.FirstOrDefaultAsync(x => x.WebhookEventLookupId == model.WebhookEventLookupId && x.ClientId == model.ClientId && x.WebhookUrl == model.WebhookUrl);
  }
  public async Task<bool> UpdateClientWebhookEvent(ClientWebhookEvent model)
  {
    _context.ClientWebhookEvents.Update(model);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<WebhookEventLookup>> GetAllWebhookEventLookups()
  {
    return await _context.WebhookEventLookups.Where(x => x.Active == true).ToListAsync();
  }

  public async Task<ClientWebhookEvent?> GetClientWebhookEventById(int clientWebhookEventId)
  {
    return await _context.ClientWebhookEvents.FirstOrDefaultAsync(x => x.ClientWebhookEventId == clientWebhookEventId);
  }


  public async Task<bool?> DeleteWebhookEvent(ClientWebhookEvent model)
  {
    _context.ClientWebhookEvents.Remove(model);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<ClientWebhookEvent?> GetClientWebhookEventById(long clientWebhookEventId)
  {
    return await _context.ClientWebhookEvents.FirstOrDefaultAsync(x => x.ClientWebhookEventId == clientWebhookEventId);
  }
  public async Task<dynamic> GetAllWebhookEventLogByClient(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      var query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                           COUNT(*) OVER () AS TotalCount,
                           wel.WebhookEventLogId,
                           wel.IsSuccess,
                           wel.Request,
                           wel.Response,
                           wel.StatusCode,
                           wel.StatusName,
                           wel.ErrorMessage,
                           wel.CreatedOn,
                           wel.Active,
                           wel2.WebhookEventLookupId,
                           wel2.EventName,
                           wel2.EventKey,
                           cwe.WebhookUrl
                    FROM dbo.WebhookEventLog AS wel
                        INNER JOIN dbo.WebhookEventLookup AS wel2
                            ON wel2.WebhookEventLookupId = wel.WebhookEventLookupId
                        LEFT JOIN dbo.ClientWebhookEvent AS cwe
                            ON cwe.ClientId = wel.ClientId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@Search", search);
        whereStart += @"And ( (wel2.EventName in (select value from STRING_SPLIT(@Search,',')))) 
                        OR ( ( wel2.EventKey in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (wel.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("wel.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("wel.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "wel.CreatedOn");


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
  public async Task<dynamic> GetAllClientWebhookEvents(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      var query = @"SELECT  ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                            COUNT(*) OVER () AS TotalCount,
                           cwe.ClientWebhookEventId,
                           cwe.WebhookUrl,
                           wel.EventName,
                           cwe.CreatedOn,
                           cwe.UpdatedOn
                    FROM dbo.ClientWebhookEvent AS cwe
                        INNER JOIN dbo.WebhookEventLookup AS wel
                            ON wel.WebhookEventLookupId = cwe.WebhookEventLookupId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (cwe.ClientId = @ClientId) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "cwe.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[0] + " " + " DESC ";

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
  public async Task<List<ClientWebhookEvent>> GetAllClientWebhookEvents(ClientId clientId)
  {
    var target = await _context.ClientWebhookEvents.Where(x => x.ClientId == clientId).ToListAsync();
    return target!;
  }

  public async Task<dynamic>? GetAllClientWebhookEndpoints(string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      var query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                           COUNT(*) OVER () AS TotalCount,
                           cwe.ClientWebhookEndpointId,
                           cwe.WebhookUrl,
                           cwe.CreatedOn,
                           COUNT(cwe.ClientWebhookEndpointId) AS EventCount
                    FROM dbo.ClientWebhookEndpoint AS cwe
                        INNER JOIN dbo.ClientWebhookEvent AS cwe2
                            ON cwe2.ClientWebhookEndpointId = cwe.ClientWebhookEndpointId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (cwe.ClientId = @ClientId) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "cwe.CreatedOn");

      string groupBy = @"GROUP BY cwe.ClientWebhookEndpointId,
                                   cwe.WebhookUrl,
                                   cwe.CreatedOn";

      string queryData = query + where + " " + groupBy;

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

  public async Task<WebhookEventLookup?> GetWebHookEventsByName(string? eventName)
  {
    var target = await _context.WebhookEventLookups.FirstOrDefaultAsync(x => x.EventKey!.Trim().ToLower() == eventName!.Trim().ToLower());
    return target!;
  }

  public async Task<bool?> CreateWebHookEventlog(WebhookEventLog model)
  {
    await _context.WebhookEventLogs.AddAsync(model);
    return await _context.SaveChangesAsync() > 0;
  }

  #endregion
}
