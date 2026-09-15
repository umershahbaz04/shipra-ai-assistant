using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SMSProcessAggregate;
using Shipra.Backend.API.Core.WhatsappAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;

public class SMSProcessRepository : ISMSProcessRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public SMSProcessRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<SMSActivate> CreateSMSActivate(SMSActivate sMSActivate)
  {
    await _context.SMSActivates.AddAsync(sMSActivate);
    await _context.SaveChangesAsync();
    return sMSActivate;
  }

  public async Task<dynamic> DeleteSMSActivate(SMSActivate sMSActivate)
  {
    _context.SMSActivates.Update(sMSActivate);
    await _context.SaveChangesAsync();
    return sMSActivate;
  }

  public async Task<dynamic> GetAllSMSActivate(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) As RowNum,
                      COUNT(*) OVER () AS TotalCount, 
                       sa.SMSActivateId,
                       sa.IsDefault,
                       sa.CreatedOn,
                       sl.ServiceName,
                       sa.Config,
                       sa.Active
                       FROM dbo.SMSActivate AS sa INNER JOIN dbo.SMSLookup AS sl ON sa.SMSLookupId = sl.SMSLookupId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( sl.SMSName in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (sa.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("sa.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("sa.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "sa.CreatedOn");


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

  public async Task<dynamic?> GetSMSActivateForSelection(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = @"SELECT sa.SMSActivateId,sl.ServiceName,sa.IsDefault 
                          FROM dbo.SMSActivate AS sa
                              INNER JOIN dbo.SMSLookup AS sl
                                  ON sl.SMSLookupId = sa.SMSLookupId ";

      string whereStart = "WHERE ( 1=1 AND ( sa.Active = 1 ) ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (sa.ClientId = @ClientId) ";
      }

      string where = whereStart + whereEnd;
       
      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }
  public async Task<SMSActivate?> GetSMSActivateById(int sMSActivateId, ClientId clientId)
  {
    return await _context.SMSActivates.Where(x => x.SMSActivateId == sMSActivateId && x.ClientId == clientId).FirstOrDefaultAsync();
  }
  public async Task<dynamic> UpdateSMSActivate(SMSActivate sMSActivate)
  {
    _context.SMSActivates.Update(sMSActivate);
    await _context.SaveChangesAsync();
    return sMSActivate;
  }
  public async Task<SMSLookup?> GetSMSLookupById(int sMSLookupId)
  {
    return await _context.SMSlookups.Where(x => x.SMSLookupId == sMSLookupId).FirstOrDefaultAsync();
  }
  public async Task<List<SMSLookup>?> GetAllSMSLookupForSelection()
  {
    return await _context.SMSlookups.ToListAsync();
  }

  public async Task<SMSActivate?> GetSMSActivateBySMSLookupId(int sMSLookupId, ClientId clientId)
  {
    return await _context.SMSActivates.Where(x => x.SMSLookupId == sMSLookupId && x.ClientId == clientId).FirstOrDefaultAsync();
  }

  public async Task<SMSActivate?> GetClientDefaultSMSActivateById(ClientId clientId)
  {
    return await _context.SMSActivates.Where(x => x.ClientId == clientId && x.IsDefault == true).FirstOrDefaultAsync();
  }
  public async Task<List<SMSActivate>?> GetAllDefaultSmsActivated(ClientId clientId)
  {
    return await _context.SMSActivates.Where(x => x.ClientId == clientId && x.IsDefault == true).ToListAsync();
  }
  public async Task<List<SMSActivate>?> GetAllSmsActivated(ClientId clientId)
  {
    return await _context.SMSActivates.Where(x => x.ClientId == clientId).ToListAsync();
  }
  #region Whatsapp
  public async Task<WhatsappActivate?> GetWhatsappActivateById(int sMSActivateId, ClientId clientId)
  {
    return await _context.WhatsappActivates.Where(x => x.WhatsAppActivateId == sMSActivateId && x.ClientId == clientId).FirstOrDefaultAsync();
  }
  public async Task<WhatsappLookup?> GetWhatsappLookupById(int WhatsappLookupId)
  {
    return await _context.WhatsappLookups.Where(x => x.WhatsAppLookupId == WhatsappLookupId).FirstOrDefaultAsync();
  }
  public async Task<WhatsappActivate> CreateWhatsappActivate(WhatsappActivate WhatsappActivate)
  {
    await _context.WhatsappActivates.AddAsync(WhatsappActivate);
    await _context.SaveChangesAsync();
    return WhatsappActivate;
  }
  public async Task<dynamic> UpdateWhatsappActivate(WhatsappActivate whatsappActivate)
  {
    _context.WhatsappActivates.Update(whatsappActivate);
    await _context.SaveChangesAsync();
    return whatsappActivate;
  }
  public async Task<List<WhatsappActivate>?> GetAllDefaultWhatsappActivated(ClientId clientId)
  {
    return await _context.WhatsappActivates.Where(x => x.ClientId == clientId && x.IsDefault == true).ToListAsync();
  }
  public async Task<List<WhatsappActivate>?> GetAllWhatsappActivated(ClientId clientId)
  {
    return await _context.WhatsappActivates.Where(x => x.ClientId == clientId).ToListAsync();
  }

  #region WhatsApp Button Handler
  public async Task<WhatsappActivate?> GetWhatsappActivationById(int sMSActivateId, ClientId clientId)
  {
    return await _context.WhatsappActivates.Where(x => x.WhatsAppLookupId == sMSActivateId && x.ClientId == clientId).FirstOrDefaultAsync();
  }
  #endregion

  #region Query
  public async Task<dynamic> GetAllWhatsappActivate(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) As RowNum,
                      COUNT(*) OVER () AS TotalCount, 
                       sa.WhatsAppActivateId,
                       sa.IsDefault,
                       sa.CreatedOn,
                       sl.ServiceName,
                       sa.Config,
                       sa.Active
                       FROM dbo.WhatsAppActivate AS sa INNER JOIN dbo.WhatsappLookup AS sl ON sa.WhatsAppLookupId = sl.WhatsAppLookupId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( sl.SMSName in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (sa.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("sa.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("sa.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "sa.CreatedOn");


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
  public async Task<List<WhatsappLookup>?> GetAllWhatsappLookupForSelection()
  {
    return await _context.WhatsappLookups.ToListAsync();
  }
  #endregion
  #endregion
}

