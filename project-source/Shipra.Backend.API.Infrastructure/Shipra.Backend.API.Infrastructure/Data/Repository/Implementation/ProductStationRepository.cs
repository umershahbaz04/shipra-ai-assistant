using Dapper;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using NPOI.SS.Formula.Functions;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ProductStationRepository : IProductStationRepository
{
  private readonly AppDbContext _context;
  private readonly DapperAppDbContext _dapperAppDbContext;

  public ProductStationRepository(AppDbContext context, DapperAppDbContext dapperAppDbContext)
  {
    _context = context;
    _dapperAppDbContext = dapperAppDbContext;
  }

  public async Task<ProductStation> CreateProductStation(ProductStation model)
  {
    await _context.ProductStations.AddAsync(model);
    await _context.SaveChangesAsync();
    return model;
  }

  public async Task<bool> IsProductStationExist(string name, ClientId clientId)
  {
    return await _context.ProductStations.AnyAsync(x => x.Name!.Trim().ToLower() == name.Trim().ToLower() && x.ClientId == clientId);
  }
  public async Task<string> GetNextProductStationCode(ClientId clientId)
  {
    int count = 0;
    int? clientIdentifier = 0;
    var client = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync();
    if (client is not null)
    {
      clientIdentifier = client.ClientIdentifier;
      count = await _context.ProductStations.CountAsync(x => x.ClientId == clientId);
       
    //  count++;
    }
    string clientStoreCode = await GenerateUniqueStationCodeAsync(clientIdentifier, clientId, count);//$"PS{clientIdentifier}{count}"; //ST1001

    return clientStoreCode;

  }
  private async Task<string> GenerateUniqueStationCodeAsync(int? clientIdentifier, ClientId clientId, int count)
  {
    count++;
    string newCode = $"PS{clientIdentifier}{count}";

    var orderExist = await _context.ProductStations
        .AnyAsync(x => x.ClientId == clientId && x.StationCode == newCode);

    if (orderExist)
    {
      // If the new order number still exists, recursively call itself until a unique order number is found.
      return await GenerateUniqueStationCodeAsync(clientIdentifier, clientId, count);
    }

    return newCode;
  }

  public async Task<dynamic?> GetAllProductStations(int start, int length, string search, DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string query = @"SELECT  
                      ROW_NUMBER() OVER (ORDER BY (SELECT 1)) As RowNum,
                      COUNT(*) OVER () AS TotalCount, 
					            ps.Active,
                      ps.CreatedBy,
                      ps.CreatedOn,
                      ps.IsDefault, 
                      ps.Name,
                      ps.ProductStationId,
                      ps.StationCode FROM dbo.ProductStation AS ps   ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        //dynamicParams.Add("@search", search);
        //whereStart += "And ( ( s.storename like '%@search%' )) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (ps.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ps.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ps.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "ps.ProductStationId");


      string queryData = query + where + " ORDER BY " + keyValuePairs[0] + " " + "desc" + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

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

  public async Task<dynamic?> GetProductStationLookupByRegionId(int? regionId)
  {
    return await _context.StationLookups.FirstOrDefaultAsync(x => x.CityId == regionId);
  }

  public async Task<ProductStation?> GetProductStationById(int productStationid)
  {
    return await _context.ProductStations.FirstOrDefaultAsync(x => x.ProductStationId == productStationid);
  }

  public async Task<dynamic> UpdateProductStation(ProductStation model)
  {
    _context.ProductStations.Update(model);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<ProductStation?> GetDefaultProductStation(ClientId clientId)
  {
    return await _context.ProductStations.FirstOrDefaultAsync(x => x.IsDefault == true && x.ClientId == clientId);
  }
  public async Task<List<ProductStation>?> GetAllProductStations(ClientId clientId)
  {
    return await _context.ProductStations.Where(x => x.ClientId == clientId).ToListAsync();
  }
}
