using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Infrastructure.Helpers;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class AwsCongnitoRepository : IAwsCongnitoRepository
{
  private readonly string _connectionString;
  private readonly IMemoryCache _cache;
  private readonly IOptions<CacheSettings> _cacheSettings;
  private readonly MemoryCacheEntryOptions _cacheOptions;

  public AwsCongnitoRepository(IMemoryCache cache, IConfiguration configuration, IOptions<CacheSettings> cacheSettings)
  {
    _connectionString = configuration!.GetConnectionString("AwsCognitoConnection")!;
    _cache = cache;
    _cacheSettings = cacheSettings;
    _cacheOptions = new MemoryCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheSettings.Value.AbsoluteExpirationInMinutes),
      SlidingExpiration = TimeSpan.FromMinutes(cacheSettings.Value.SlidingExpirationInMinutes)
    };
  }

  public async Task<UserPoolClientResponseModel?> GetUserPoolClientByClientId(string clientId)
  {
    var alllClients = await GetAllUserPoolClients();
    var config = alllClients!.FirstOrDefault(c => string.Equals(c.TenantClientId!.Value!.ToString(), clientId, StringComparison.OrdinalIgnoreCase));


    if (config == null)
    {
      // If alllClients is null, attempt to clear cache and reload
      alllClients = await GetAllUserPoolClients(clearCacheOnRequest: true);
      config = alllClients!.FirstOrDefault(c => string.Equals(c.TenantClientId!.Value!.ToString(), clientId, StringComparison.OrdinalIgnoreCase));
    }

    return config;
  }
  public async Task<List<UserPoolClientResponseModel>?> GetAllUserPoolClients(bool? clearCacheOnRequest = false)
  {
    // Define the cache key
    const string cacheKey = CacheKeys.UserPoolClientsCacheKey;

    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(cacheKey);
    }

    // Try to get data from cache
    if (_cache.TryGetValue(cacheKey, out List<UserPoolClientResponseModel>? cachedData))
    {
      return cachedData; // Return cached data if available
    }

    using (var connection = new SqlConnection(_connectionString))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT upc.UserPoolClientId,
                                     upc.ClientName,
                                     upc.UserPoolId,
                                     upc.PoolClientId,
                                     upc.UserPoolClientSecret,
                                     upc.TenantClientId,
                                     upc.Region,
                                     upc.IsCodeConfirm
                              FROM dbo.UserPoolClient AS upc ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";
      string where = whereStart + whereEnd;
      string queryData = query + where;

      var data = await connection.QueryAsync<UserPoolClientResponseModel>(queryData, dynamicParams);
      var resultList = data.ToList();

   

      // Store the result in the cache
      _cache.Set(cacheKey, resultList, _cacheOptions);

      return resultList;
    }
  }

}
