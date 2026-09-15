using System.Dynamic;
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class StoreRepository : IStoreRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public StoreRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<dynamic?> CreateStore(Store store)
  {
    await _context.Stores.AddAsync(store);
    await _context.SaveChangesAsync();
    return store;
  }
  public async Task<bool> CreateStoreAddress(StoreAddress oStoreAddress)
  {
    await _context.StoreAddresses.AddAsync(oStoreAddress);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> UpdateStoreAddress(StoreAddress oStoreAddress)
  {
    _context.StoreAddresses.Update(oStoreAddress);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> DisableStore(Store store)
  {
    _context.Stores.Update(store);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> EnableStore(Store store)
  {
    _context.Stores.Update(store);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<StoreUploadSampleFile>> GetSampleExcelFileForStoreUpload(int countryId)
  {
    return await _context.StoreUplaodSampleFiles
                         .Where(x => x.CountryId == countryId)
                         .ToListAsync();
  }

  public async Task<List<Store>?> GetAllStoresByClient(ClientId clientId)
  {
    return await _context.Stores.Where(x => x.ClientId == clientId).ToListAsync();
  }
  public async Task<dynamic?> GetAllStore(string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               CASE
                                   WHEN s.IsDefault = 1 THEN
                                       s.StoreName + ' (Default)'
                                   ELSE
                                       s.StoreName
                               END AS StoreName,
                               s.StoreId,
                               s.StoreCode,
                               s.StoreImage,
                               sa.FullAddress,
                               s.StoreCompany,
                               s.Active,
                               (
                                   SELECT COUNT(*)
                                   FROM dbo.SaleChannelConfig AS scc
                                   WHERE scc.StoreId = s.StoreId
                               ) AS SaleChannelConfigCount,
                               cn.Name AS CountryName,
                               sa.StreetAddress,
                               sa.Zip,
                               s.CustomerServiceNo,
                               s.Phone,
                               s.Email,
                               s.URLs,
                               s.IsDefault,
                               FORMAT(s.CreatedOn, 'dd-MM-yyyy') CreatedOn
                        FROM dbo.Stores AS s
                            INNER JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = s.StoreId
                            INNER JOIN dbo.Client AS cl
                                ON cl.ClientId = s.ClientId
                            INNER JOIN dbo.Country AS cn
                                ON cn.CountryId = sa.CountryId   ";

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
        whereStart += "And (s.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("s.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("s.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "s.StoreId");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;


        foreach (var item in dataList)
        {
          var saleChannel = await GetSaleChannelConfByStoreIdAsync(item?.StoreId, clientId);
          item!.SalesChannel = saleChannel!;
        }

      }

      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }

  private async Task<dynamic> GetSaleChannelConfByStoreIdAsync(int storeId, string clientId)
  {
    ClientId clId = new ClientId(new Guid(clientId));
    var data = await _context.SaleChannelConfigs.Where(x => x.StoreId == storeId && x.ClientId == clId).Select(x => new { x.SaleChannelName, x.SaleChannelKey }).ToListAsync();
    return data;
  }

  public async Task<List<Store>?> GetStoresForSelection(ClientId? clientId)
  {
    return await _context.Stores.Where(x => x.ClientId == clientId && x.Active == true).ToListAsync();
  }

  public async Task<Store?> GetStoreById(int storeid, ClientId? clientId)
  {
    return await _context.Stores.FirstOrDefaultAsync(x => x.StoreId! == storeid && x.ClientId == clientId);
  }
  public async Task<StoreAddress?> GetStoreAddressById(int storeid)
  {
    return await _context.StoreAddresses.FirstOrDefaultAsync(x => x.StoreId! == storeid);
  }
  public async Task<Store?> UpdateStore(Store store)
  {
    _context.Stores.Update(store);
    await _context.SaveChangesAsync();
    return store;
  }

  public async Task<Store> CheckStoreExistAginstClientByStoreName(string? storeName, ClientId? clientId)
  {
    var data = await _context.Stores.FirstOrDefaultAsync(x => x.ClientId == clientId && x.Active == true && x.StoreName!.Trim().ToLower() == storeName!.Trim().ToLower());
    return data!;
  }
  #region store product

  public async Task<StoreProduct> CheckExistProductOnStore(int? storeId, ProductId? productId)
  {
    var data = await _context.StoreProducts.FirstOrDefaultAsync(x => x.StoreId == storeId && x.ProductId == productId);
    return data!;
  } 
  public async Task<StoreProduct> GetProductStoreById(StoreProductId? storeProductId)
  {
    var data = await _context.StoreProducts.FirstOrDefaultAsync(x => x.StoreProductId == storeProductId);
    return data!;
  }
  public async Task<bool> CreateStoreProduct(StoreProduct storeProduct)
  {
    var data = await _context.StoreProducts.AddAsync(storeProduct);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> UpdateStoreProduct(StoreProduct storeProduct)
  {
    _context.StoreProducts.Update(storeProduct);
    return await _context.SaveChangesAsync() > 0;
  }

  #endregion
  //public async Task<string> GetClientNextStoreCode(ClientId? clientId)
  //{
  //  int storeCount = 0;
  //  int? clientIdentifier = 0;
  //  var client = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync();
  //  if (client is not null)
  //  {
  //    clientIdentifier = client.ClientIdentifier;
  //    storeCount = await _context.Stores.CountAsync(x => x.ClientId == clientId);
  //    storeCount++;

  //  }
  //  string clientStoreCode = $"ST{clientIdentifier}{storeCount}"; //ST1001
  //  return clientStoreCode;
  //}
  public async Task<string> GetClientNextStoreCode(ClientId? clientId)
  {
    var client = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync();

    if (client is null)
      throw new InvalidOperationException($"Client with Id {clientId} not found or inactive.");

    int clientIdentifier = client.ClientIdentifier ?? 0;
    int storeCount = await _context.Stores.CountAsync(x => x.ClientId == clientId);
    string clientStoreCode;

    do
    {
      storeCount++;
      clientStoreCode = $"ST{clientIdentifier}{storeCount}";
    }
    while (await _context.Stores.AnyAsync(x => x.StoreCode == clientStoreCode));

    return clientStoreCode;
  }


  public async Task<dynamic> GetStoreAddressByStoreId(int storeId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"	SELECT s.StoreName,
                               s.StoreImage,
                               ISNULL(s.CustomerServiceNo, '') AS Phone,
                               sa.Latitude,
                               sa.Longitude,
                               sa.FullAddress AS StoreAddress
                        FROM dbo.Stores AS s
                            INNER JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = s.StoreId
                            INNER JOIN dbo.Country AS c
                                ON c.CountryId = sa.CountryId ";
      string whereStart = "WHERE ( s.Active=1 ";
      string whereEnd = ")";

      if (storeId > 0)
      {
        dynamicParams.Add("@storeId", storeId);
        whereStart += "And (s.StoreId = @storeId) ";
      }
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.FirstOrDefault()!;
    }
  }

  //GetStoreid

  public async Task<List<Store>?> GetAllStoreForid(ClientId clientId)
  {
    return await _context.Stores.Where(x => x.ClientId == clientId && x.Active == true).ToListAsync();
  }
}
