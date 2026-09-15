using System.Data;
using Amazon.Runtime.Internal.Util;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.CatalougeAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Infrastructure.Data;
using Shipra.Backend.API.Infrastructure.Helpers;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Services.Implementation;
public class DbContextService : IDbContextService
{
  private readonly ShipraMasterDbContext _shipraMasterDbContext;
  private readonly IKeyGeneratorService _keyGeneratorService;
  private readonly IMemoryCache _cache;
  private readonly MemoryCacheEntryOptions _cacheOptions;
  private readonly IOptions<CacheSettings> _cacheSettings;

  public DbContextService(ShipraMasterDbContext shipraMasterDbContext, IKeyGeneratorService keyGeneratorService, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
  {
    _shipraMasterDbContext = shipraMasterDbContext;
    _keyGeneratorService = keyGeneratorService;
    _cache = cache;
    _cacheSettings = cacheSettings;
    _cacheOptions = new MemoryCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheSettings.Value.AbsoluteExpirationInMinutes),
      SlidingExpiration = TimeSpan.FromMinutes(cacheSettings.Value.SlidingExpirationInMinutes)
    };

  }

  #region old
  public IDbConnection GetDapperDbConection(string clientId)
  {
    return new SqlConnection(GetConnectionString(clientId));
  }

  public AppDbContext GetAppDbContext(string clientId)
  {
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseSqlServer(GetConnectionString(clientId));
    return new AppDbContext(optionsBuilder.Options, null);
  }
  public AppDbContext GetAppDbContextWithConnectionString(string connectionString)
  {
    connectionString = _keyGeneratorService.DecryptString(connectionString!) ?? string.Empty; 
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseSqlServer(connectionString);
    return new AppDbContext(optionsBuilder.Options, null);
  }

  public OperationStatusResponseModel GetConnectionWithOperationaStatus(string clientId)
  {
    OperationStatusResponseModel operationStatus = new OperationStatusResponseModel();
    #region cataloug (cehck value from cache if not exsit then refresh)
    var catalouges = GetAllCacheCatalogues();
    var oCatalogues = catalouges.FirstOrDefault(x => x.ClientId!.ToLower() == clientId.ToLower());

    if (oCatalogues is null)
    {
      catalouges = GetAllCacheCatalogues(clearCacheOnRequest: true);
      oCatalogues = catalouges.FirstOrDefault(x => x.ClientId!.ToLower() == clientId.ToLower());

    }
    operationStatus.Catalogue = oCatalogues;
    #endregion


    if (oCatalogues is not null)
    {
      #region CatalougeDatabases (cehck value from cache if not exsit then refresh)

      var allCatalougeDatabases = GetAllCacheCatalogueDatabase();
      var oCatalogueDatabases = allCatalougeDatabases.FirstOrDefault(x => x.DatabaseId == oCatalogues!.DatabaseId && x.Active == true);
      if (oCatalogueDatabases is null)
      {
        allCatalougeDatabases = GetAllCacheCatalogueDatabase();
        oCatalogueDatabases = allCatalougeDatabases.FirstOrDefault(x => x.DatabaseId == oCatalogues!.DatabaseId && x.Active == true);

      }
      operationStatus.CatalogueDatabase = oCatalogueDatabases;
      operationStatus.ConnectionString = "Server=13.205.115.20,1245; initial catalog=App_Shipra; MultipleActiveResultSets= True; user id=sa;password=devshipra@123;multipleactiveresultsets=True;TrustServerCertificate=True";//_keyGeneratorService.DecryptString(oCatalogueDatabases?.ConnectionString!) ?? string.Empty;
      #endregion

    }
    return operationStatus;
  }

  public string GetConnectionString(string clientId)
  {
    string connectionString = string.Empty;
    if (!string.IsNullOrEmpty(clientId))
    {
      clientId = clientId.Trim();
      #region cataloug (cehck value from cache if not exsit then refresh)
      var catalouges = GetAllCacheCatalogues();
      var oCatalogues = catalouges.FirstOrDefault(x => x.ClientId!.ToLower() == clientId.ToLower());

      if (oCatalogues is null)
      {
        catalouges = GetAllCacheCatalogues(clearCacheOnRequest: true);
        oCatalogues = catalouges.FirstOrDefault(x => x.ClientId!.ToLower() == clientId.ToLower());
      }
      #endregion


      if (oCatalogues is not null)
      {
        #region CatalougeDatabases (cehck value from cache if not exsit then refresh)

        var allCatalougeDatabases = GetAllCacheCatalogueDatabase();
        var oCatalogueDatabases = allCatalougeDatabases.FirstOrDefault(x => x.DatabaseId == oCatalogues!.DatabaseId && x.Active == true);
        if (oCatalogueDatabases is null)
        {
          allCatalougeDatabases = GetAllCacheCatalogueDatabase();
          oCatalogueDatabases = allCatalougeDatabases.FirstOrDefault(x => x.DatabaseId == oCatalogues!.DatabaseId && x.Active == true);
        }
        connectionString = "Server=13.205.115.20,1245; initial catalog=App_Shipra; MultipleActiveResultSets= True; user id=sa;password=devshipra@123;multipleactiveresultsets=True;TrustServerCertificate=True";//_keyGeneratorService.DecryptString(oCatalogueDatabases?.ConnectionString!) ?? string.Empty;
        #endregion

      }
    }
    return connectionString;
  }

  #region cachec data 
  public List<Catalogue> GetAllCacheCatalogues(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.Catalogues);
    }

    if (!_cache.TryGetValue(CacheKeys.Catalogues, out List<Catalogue>? catalouges))
    {
      catalouges = _shipraMasterDbContext?.Catalogues.ToList();
      _cache.Set(CacheKeys.Catalogues, catalouges, _cacheOptions);
    }

    return catalouges!;
  }
  public List<CatalogueDatabase> GetAllCacheCatalogueDatabase(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.CatalogueDatabases);
    }

    if (!_cache.TryGetValue(CacheKeys.CatalogueDatabases, out List<CatalogueDatabase>? catalougesDatabase))
    {
      catalougesDatabase = _shipraMasterDbContext?.CatalogueDatabases.ToList();
      _cache.Set(CacheKeys.CatalogueDatabases, catalougesDatabase, _cacheOptions);
    }

    return catalougesDatabase!;
  }


  #endregion


  #endregion
}
