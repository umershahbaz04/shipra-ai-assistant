using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ClientRepository : IClientRepository
{
  private readonly AppDbContext _context;
  private readonly DapperAppDbContext? _dapperAppDbContext;
  private readonly IDbContextService _dbContextService;

  public ClientRepository(AppDbContext context, DapperAppDbContext? dapperAppDbContext, IDbContextService dbContextService)
  {
    _context = context;
    _dapperAppDbContext = dapperAppDbContext;
    _dbContextService = dbContextService;
  }

  //public ClientRepository(AppDbContext context) : this(context, null)
  //{
  //  _context = context;
  //}

  public async Task<dynamic> CreateClient(Client client)
  {
    await _context.Clients.AddAsync(client);
    await _context.SaveChangesAsync();
    return client;
  }
  public async Task<ClientAddress> CreateClientAddress(ClientAddress clientAddress)
  {
    await _context.ClientAddresses.AddAsync(clientAddress);
    await _context.SaveChangesAsync();
    return clientAddress;
  }
  public async Task<dynamic> UpdateClient(Client client)
  {
    _context.Clients.Update(client);
    await _context.SaveChangesAsync();
    return client;
  }
  public async Task<dynamic> DeleteClient(Client client)
  {
    _context.Remove(client);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic?> GetAllClients(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {

    using (var connection = _dapperAppDbContext!.CreateConnection())
    {
      var dynamicParams = new DynamicParameters();
      string TotalCount = "Select COUNT(c.ClientId) ";
      TotalCount += @"FROM dbo.Client AS c 
                    LEFT JOIN dbo.Country AS cn ON c.CountryId = cn.CountryId
                    LEFT JOIN dbo.Region AS r ON c.RegionId = c.RegionId
                    LEFT JOIN dbo.City AS ct ON ct.CityId = c.CityId ";
      string query = @"SELECT 
                    ROW_NUMBER() OVER (ORDER BY (SELECT 1)) As RowNum,
                    COUNT(*) OVER () AS TotalCount,
                    c.ClientId,
                    c.ClientName,
                    c.ClientCode,
                    c.ClientImage,
                    c.ClientCompanyName,
                    c.Mobile,
                    ISNULL(c.Phone,'') AS Phone,
                    c.Email,
                    c.LicenseNo,
                    c.ClientIdentifier,
                    c.TRNNo,
                    c.CreatedOn,
                    cn.Name AS CountryName,
                    r.Name AS RegionName,
                    ct.Name AS CityName 
                    FROM dbo.Client AS c
                    LEFT JOIN dbo.Country AS cn ON c.CountryId = cn.CountryId
                    LEFT JOIN dbo.Region AS r ON c.RegionId = c.RegionId
                    LEFT JOIN dbo.City AS ct ON ct.CityId = c.CityId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( c.ClientId in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += "And (CAST(c.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += "And (CAST(c.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "c.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      result.list = data.ToList();
      return result;
    }
  }
  public async Task<Client?> GetLastClient()
  {
    return await _context.Clients
                    .OrderByDescending(p => p.CreatedOn)
                    .FirstOrDefaultAsync();
  }
  public async Task<Client?> GetClientById(ClientId clientid)
  {
    return await _context.Clients.FirstOrDefaultAsync(x => x.ClientId! == clientid);
  } 
  public async Task<int> GetClientRegionMinutes(string? clientId)
  {
    var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
    return regionMinuts;
  }
  public async Task<Client?> GetClientByIdWithConnectionstring(ClientId clientid)
  {
    using (var _ctx = _dbContextService!.GetAppDbContext(clientid.Value.ToString()))
    {
      return await _ctx.Clients.FirstOrDefaultAsync(x => x.ClientId! == clientid);
    }
  }
  public async Task<ClientAddress?> GetClientAddress(ClientId? clientId)
  {
    using (var _ctx = _dbContextService!.GetAppDbContext(clientId!.Value.ToString()))
    {
      return await _ctx.ClientAddresses.FirstOrDefaultAsync(x => x.ClientId! == clientId);
    }
  }

  public async Task<dynamic> GetUserProfileInfo(string clientId)
  {
    using (var connection = _dapperAppDbContext!.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT  c.ClientId,
                               c.ClientName,
                               c.ClientCode,
                               c.ClientImage,
                               c.ClientCompanyName,
                               c.Mobile,
                               ISNULL(c.Phone, '') AS Phone,
                               c.Email,
                               c.LicenseNo,
                               c.ClientIdentifier,
                               c.TRNNo,
                               c.CreatedOn,
                               c.Active,
                               c.IsPaymentVerified,
                               ca.StreetAddress,
                               ca.Zip,
                               ca.AddressTypeId,
                               ca.Latitude,
                               ca.Longitude,
                               cn.Name AS CountryName,
                               r.Name AS RegionName,
                               ct.Name AS CityName,
                               atl.AddressTypeName
                        FROM dbo.Client AS c
                            INNER JOIN dbo.ClientAddress AS ca
                                ON ca.ClientId = c.ClientId
                            INNER JOIN dbo.Country AS cn
                                ON ca.CountryId = cn.CountryId
                            INNER JOIN dbo.Region AS r
                                ON ca.RegionId = r.RegionId
                            LEFT JOIN dbo.City AS ct
                                ON ca.CityId = ct.CityId
                            INNER JOIN dbo.AddressTypeLookup AS atl
                                ON atl.AddressTypeId = ca.AddressTypeId  ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " AND ( c.ClientId = @ClientId ) ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data!;
    }
  }

  public async Task<dynamic> UpdateClientAddress(ClientAddress clientAddress)
  {
    _context.ClientAddresses.Update(clientAddress);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<ClientAddress?> GetClientAddresByClientId(ClientId? clientId)
  {
    return await _context.ClientAddresses.FirstOrDefaultAsync(x => x.ClientId == clientId);
  }

  public async Task<dynamic?> GetActiveClients(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {
    using (var connection = _dapperAppDbContext!.CreateConnection())
    {
      var dynamicParams = new DynamicParameters();
      string TotalCount = "Select COUNT(c.ClientId) ";
      TotalCount += @"FROM dbo.Client AS c 
                    LEFT JOIN dbo.Country AS cn ON c.CountryId = cn.CountryId
                    LEFT JOIN dbo.Region AS r ON c.RegionId = c.RegionId
                    LEFT JOIN dbo.City AS ct ON ct.CityId = c.CityId ";
      string query = @"SELECT 
                    c.ClientId,
                    c.ClientName,
                    c.ClientCode,
                    c.ClientImage,
                    c.ClientCompanyName,
                    c.Mobile,
                    ISNULL(c.Phone,'') AS Phone,
                    c.Email,
                    c.LicenseNo,
                    c.ClientIdentifier,
                    c.TRNNo,
                    c.CreatedOn,
                    cn.Name AS CountryName,
                    r.Name AS RegionName,
                    ct.Name AS CityName 
                    FROM dbo.Client AS c
                    LEFT JOIN dbo.Country AS cn ON c.CountryId = cn.CountryId
                    LEFT JOIN dbo.Region AS r ON c.RegionId = c.RegionId
                    LEFT JOIN dbo.City AS ct ON ct.CityId = c.CityId ";
      string whereStart = "WHERE ( c.Active=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( c.ClientId in (select value from STRING_SPLIT(@search,',')))) ";
      }

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += "And (CAST(c.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += "And (CAST(c.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "c.CreatedOn");
      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      result.list = data.ToList();
      return result;
    }
  }

  public async Task<Client?> CheckEmailExists(string? email)
  {
    return await _context.Clients.FirstOrDefaultAsync(x => x.Email == email);
  }

  public async Task<Client?> GetClientByKey(string? tenantUsername, string? publicKey, string? clientId)
  {
    //First call Catalog DB context with client to get the connection string
    using (var _ctx = _dbContextService!.GetAppDbContext(clientId!))
    {
      return await _ctx.Clients.FirstOrDefaultAsync(x => x.Username!.ToLower() == tenantUsername!.ToLower().Trim() && x.PublicKey == publicKey);
    }
  }

  public async Task<dynamic> GetClientProfileById(string clientId)
  {
    using (var connection = _dapperAppDbContext!.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT c.clientId,
                               c.clientName,
                               c.clientCode,
                               c.clientImage,
                               c.clientCompanyName,
                               s.storeName,
                               co.name AS CountryName,
                               ca.countryId AS country,
                               ca.cityId AS city,
                               ca.stateId AS state,
                               ca.provinceId AS province,
                               ca.areaId AS area,
                               ca.pinCodeId AS pinCode,
                               ca.streetAddress,
                               ca.streetAddress2,
                               ca.houseNo,
                               ca.buildingName,
                               ca.landmark,
                               ca.fullAddress,
                               ca.latitude,
                               ca.longitude,
                               ca.zip,
                               ps.StationCode AS stationName,
                               c.mobile,
                               c.phone,
                               c.email,
                               c.clientIdentifier,
                               c.licenseNo,
                               c.regionTimeZoneId
                        FROM dbo.Client AS c
                            INNER JOIN dbo.ClientAddress AS ca
                                ON ca.ClientId = c.ClientId
                            INNER JOIN dbo.Stores AS s
                                ON c.DefaultStoreId = s.StoreId
                            INNER JOIN dbo.Country AS co
                                ON co.CountryId = ca.CountryId
                            LEFT JOIN dbo.ProductStation AS ps
                                ON ps.ClientId = c.ClientId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@clientId", clientId);
        whereStart += " AND ( c.ClientId = @clientId ) ";
      }
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.FirstOrDefault()!;
    }
  }
  public async Task<Client?> GetClientByClientId(string clientId, string connectionString)
  {
    using (var connection = _dapperAppDbContext!.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT  c.ClientId,
                                c.ClientCode,
                                c.ClientName,
                                c.ClientImage,
                                c.CountryId,
                                c.RegionId,
                                c.CityId,
                                c.StreetAddress,
                                c.Zip,
                                c.ClientCompanyName,
                                c.Mobile,
                                c.Phone,
                                c.Email,
                                c.LicenseNo,  
                                c.ClientIdentifier, 
                                c.DefaultCarrierId,  
                                c.Username,  
                                c.PublicKey FROM dbo.Client AS c ";
      string whereStart = $"WHERE ( c.Active=1 AND ( c.ClientId = '{clientId}' ) ";
      string whereEnd = ")";

      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync<Client>(queryData, dynamicParams);
      return data.FirstOrDefault();
    }
  }
  public async Task<ClientGenericSetting> GetGenericSettingByClientIdAsync(ClientId clientId)
  {
    var data = await _context.ClientGenericSettings.FirstOrDefaultAsync(x => x.ClientId == clientId);
    return data!;
  }
  #region client status
  public async Task<ClientCarrierTrackingStatus> CreateClientCarrierTrackingStatus(ClientCarrierTrackingStatus oClientCarrierTrackingStatus)
  {
    await _context.ClientCarrierTrackingStatuses.AddAsync(oClientCarrierTrackingStatus);
    await _context.SaveChangesAsync();
    return oClientCarrierTrackingStatus;
  }
  public async Task<bool> CreateBatchClientCarrierTrackingStatus(List<ClientCarrierTrackingStatus> listOfClientCarrier)
  {
    await _context.ClientCarrierTrackingStatuses.AddRangeAsync(listOfClientCarrier);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> UpdateClientCarrierTracking(ClientCarrierTrackingStatus oClientCarrierTrackingStatus)
  {
    _context.ClientCarrierTrackingStatuses.Update(oClientCarrierTrackingStatus);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> DeleteClientCarrierTrackingStatusById(ClientCarrierTrackingStatus oClientCarrierTrackingStatus)
  {
    _context.ClientCarrierTrackingStatuses.Remove(oClientCarrierTrackingStatus);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<List<ClientCarrierTrackingStatus>?> GetAllClientCarrierTrackingStatus(ClientId clientId)
  {
    return await _context.ClientCarrierTrackingStatuses.Where(x => x.ClientId == clientId).ToListAsync();
  }

  public async Task<ClientGenericSetting> ClientGenericSettingById(ClientId clientId)
  {
    var data = await _context.ClientGenericSettings.FirstOrDefaultAsync(x => x.ClientId == clientId);
    return data!;
  }

  public async Task<bool> CreateClientGenericSetting(ClientGenericSetting model)
  {
    await _context.ClientGenericSettings.AddAsync(model);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> UpdateClientGenericSetting(ClientGenericSetting model)
  {
    _context.ClientGenericSettings.Update(model);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<ClientGenericSettingLookup> GetClientGenericSettingLookup()
  {
    var data = await _context.ClientGenericSettingLookups.FirstOrDefaultAsync();
    return data!;
  }

  public async Task<ClientConfigSetting> GetClientConfigSetting(ClientId clientId)
  {
    var data = await _context.ClientConfigSettings.FirstOrDefaultAsync(x => x.ClientId == clientId);
    return data!;
  }

  public async Task<bool> UpdateClientConfigSetting(ClientConfigSetting clientSettingConfig)
  {
    _context.ClientConfigSettings.Update(clientSettingConfig);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> CreateClientConfigSetting(ClientConfigSetting clientSettingConfig)
  {
    await _context.ClientConfigSettings.AddAsync(clientSettingConfig);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> WipeOutClientData(ClientId clientId, Shipra.Backend.API.Core.Enum.EnumWipeOutSection section)
  {
    var clientIdStr = clientId.Value.ToString();
    var sql = "";

    if (section == Shipra.Backend.API.Core.Enum.EnumWipeOutSection.All)
    {
      sql = $@"
        IF OBJECT_ID('tempdb..#TempClientOrders') IS NOT NULL DROP TABLE #TempClientOrders;
        IF OBJECT_ID('tempdb..#TempClientProducts') IS NOT NULL DROP TABLE #TempClientProducts;
        IF OBJECT_ID('tempdb..#TempClientPayouts') IS NOT NULL DROP TABLE #TempClientPayouts;
        IF OBJECT_ID('tempdb..#TempClientReturns') IS NOT NULL DROP TABLE #TempClientReturns;
        IF OBJECT_ID('tempdb..#TempClientInventoryBalances') IS NOT NULL DROP TABLE #TempClientInventoryBalances;

        BEGIN TRANSACTION;
        BEGIN TRY
            DECLARE @ClientId UNIQUEIDENTIFIER = '{clientIdStr}';

            CREATE TABLE #TempClientOrders (OrderId UNIQUEIDENTIFIER PRIMARY KEY);
            INSERT INTO #TempClientOrders (OrderId) SELECT OrderId FROM dbo.[Order] WHERE ClientId = @ClientId;

            CREATE TABLE #TempClientProducts (ProductId UNIQUEIDENTIFIER PRIMARY KEY);
            INSERT INTO #TempClientProducts (ProductId) SELECT ProductId FROM dbo.Product WHERE ClientId = @ClientId;

            CREATE TABLE #TempClientPayouts (PayoutId UNIQUEIDENTIFIER PRIMARY KEY);
            INSERT INTO #TempClientPayouts (PayoutId) SELECT PayoutId FROM dbo.Payout WHERE ClientId = @ClientId;

            CREATE TABLE #TempClientReturns (ReturnId UNIQUEIDENTIFIER PRIMARY KEY);
            INSERT INTO #TempClientReturns (ReturnId) SELECT ReturnId FROM dbo.[Return] WHERE ClientId = @ClientId;

            CREATE TABLE #TempClientInventoryBalances (InventoryBalanceId BIGINT PRIMARY KEY);
            INSERT INTO #TempClientInventoryBalances (InventoryBalanceId) 
            SELECT ib.InventoryBalanceId FROM dbo.InventoryBalance AS ib INNER JOIN dbo.ProductVariant pv ON pv.ProductVariantId = ib.ProductVariantId WHERE pv.ProductId IN (SELECT ProductId FROM #TempClientProducts);

            -- PHASE 1: DELETE INDIRECT CHILD TABLES FIRST
            DELETE FROM dbo.OrderItem WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderTrackingHistory WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderTax WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderPODFiles WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderNote WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderBox WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.DeliveryNoteDetail WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);

            DELETE FROM dbo.ReturnActivityLog WHERE ReturnId IN (SELECT ReturnId FROM #TempClientReturns);
            DELETE FROM dbo.ReturnProduct WHERE ReturnId IN (SELECT ReturnId FROM #TempClientReturns);
            DELETE FROM dbo.ReturnTrackingHistory WHERE ReturnId IN (SELECT ReturnId FROM #TempClientReturns);

            DELETE FROM dbo.InventoryTransaction WHERE InventoryBalanceId IN (SELECT InventoryBalanceId FROM #TempClientInventoryBalances);
            DELETE ib FROM dbo.InventoryBalance ib INNER JOIN dbo.ProductVariant pv ON pv.ProductVariantId = ib.ProductVariantId WHERE pv.ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.ProductFile WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.ProductInventory WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.ProductMedia WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.ProductOptions WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.TransferProduct WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);

            DELETE FROM dbo.PayoutFile WHERE PayoutId IN (SELECT PayoutId FROM #TempClientPayouts);
            DELETE FROM dbo.PayoutStatusHistory WHERE PayoutId IN (SELECT PayoutId FROM #TempClientPayouts);

            -- PHASE 2: DELETE DIRECT CLIENT TABLES
            DELETE FROM dbo.MetaField 
            WHERE ClientId = @ClientId
               OR EntityId IN (SELECT CAST(OrderId AS VARCHAR(50)) FROM #TempClientOrders)
               OR EntityId IN (SELECT CAST(ProductId AS VARCHAR(50)) FROM #TempClientProducts);

            DELETE FROM dbo.ClientWebhookEvent WHERE ClientId = @ClientId;
            DELETE FROM dbo.ClientPayoutBank WHERE ClientId = @ClientId;
            DELETE FROM dbo.ClientReturnReason WHERE ClientId = @ClientId; 
            DELETE FROM dbo.LeadGridClientSetting WHERE ClientId = @ClientId;
            DELETE FROM dbo.ShipraContractClientCarrier WHERE ClientId = @ClientId;

            DELETE FROM dbo.SaleChannelProduct WHERE ClientId = @ClientId;
            DELETE FROM dbo.SaleChannelOrder WHERE ClientId = @ClientId;
            DELETE FROM dbo.ShopifyConfig WHERE ClientId = @ClientId;
            DELETE FROM dbo.SMSActivate WHERE ClientId = @ClientId;
            DELETE FROM dbo.WhatsappActivate WHERE ClientId = @ClientId;

            DELETE FROM dbo.ActiveCarrierPickupLocation WHERE ClientId = @ClientId;
            DELETE FROM dbo.ActiveCarrier WHERE ClientId = @ClientId;

            DELETE FROM dbo.ProductLinkToken WHERE ClientId = @ClientId;
            DELETE FROM dbo.ProductCategory WHERE ClientId = @ClientId; 
            DELETE FROM dbo.Product WHERE ClientId = @ClientId;
            
            DELETE FROM dbo.LeadGridColumn WHERE ClientId = @ClientId;
            DELETE FROM dbo.Leads WHERE ClientId = @ClientId;

            DELETE FROM dbo.PaymentLink WHERE ClientId = @ClientId;
            DELETE FROM dbo.[Return] WHERE ClientId = @ClientId;
            DELETE FROM dbo.OrderDraft WHERE ClientId = @ClientId;
            DELETE FROM dbo.OrderArchive WHERE ClientId = @ClientId;
            DELETE FROM dbo.OrderDeleted WHERE ClientId = @ClientId;
            DELETE FROM dbo.[Order] WHERE ClientId = @ClientId;
            
            DELETE FROM dbo.[Transaction] WHERE ClientId = @ClientId;
            DELETE FROM dbo.Payout WHERE ClientId = @ClientId;

            DELETE FROM dbo.DeliveryTask WHERE ClientId = @ClientId;
            DELETE FROM dbo.ActivityLog WHERE ClientId = @ClientId;
            DELETE FROM dbo.WebhookEventLog WHERE ClientId = @ClientId;
            DELETE FROM dbo.NotificationConfig WHERE ClientId = @ClientId;
            DELETE FROM dbo.ImageGallery WHERE ClientId = @ClientId;
            DELETE FROM dbo.Expense WHERE ClientId = @ClientId;

            DELETE FROM dbo.DriverReceivable 
            WHERE DeliveryNoteId IN (SELECT DeliveryNoteId FROM dbo.DeliveryNote WHERE ClientId = @ClientId);

            DELETE FROM dbo.DeliveryNote WHERE ClientId = @ClientId;

            IF OBJECT_ID('tempdb..#TempClientOrders') IS NOT NULL DROP TABLE #TempClientOrders;
            IF OBJECT_ID('tempdb..#TempClientProducts') IS NOT NULL DROP TABLE #TempClientProducts;
            IF OBJECT_ID('tempdb..#TempClientPayouts') IS NOT NULL DROP TABLE #TempClientPayouts;
            IF OBJECT_ID('tempdb..#TempClientReturns') IS NOT NULL DROP TABLE #TempClientReturns;
            IF OBJECT_ID('tempdb..#TempClientInventoryBalances') IS NOT NULL DROP TABLE #TempClientInventoryBalances;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;

            IF OBJECT_ID('tempdb..#TempClientOrders') IS NOT NULL DROP TABLE #TempClientOrders;
            IF OBJECT_ID('tempdb..#TempClientProducts') IS NOT NULL DROP TABLE #TempClientProducts;
            IF OBJECT_ID('tempdb..#TempClientPayouts') IS NOT NULL DROP TABLE #TempClientPayouts;
            IF OBJECT_ID('tempdb..#TempClientReturns') IS NOT NULL DROP TABLE #TempClientReturns;
            IF OBJECT_ID('tempdb..#TempClientInventoryBalances') IS NOT NULL DROP TABLE #TempClientInventoryBalances;

            THROW;
        END CATCH;
      ";
    }
    else if (section == Shipra.Backend.API.Core.Enum.EnumWipeOutSection.Products)
    {
      sql = $@"
        IF OBJECT_ID('tempdb..#TempClientProducts') IS NOT NULL DROP TABLE #TempClientProducts;
        IF OBJECT_ID('tempdb..#TempClientInventoryBalances') IS NOT NULL DROP TABLE #TempClientInventoryBalances;

        BEGIN TRANSACTION;
        BEGIN TRY
            DECLARE @ClientId UNIQUEIDENTIFIER = '{clientIdStr}';

            CREATE TABLE #TempClientProducts (ProductId UNIQUEIDENTIFIER PRIMARY KEY);
            INSERT INTO #TempClientProducts (ProductId) SELECT ProductId FROM dbo.Product WHERE ClientId = @ClientId;

            CREATE TABLE #TempClientInventoryBalances (InventoryBalanceId BIGINT PRIMARY KEY);
            INSERT INTO #TempClientInventoryBalances (InventoryBalanceId) 
            SELECT ib.InventoryBalanceId FROM dbo.InventoryBalance ib INNER JOIN dbo.ProductVariant pv ON pv.ProductVariantId = ib.ProductVariantId WHERE pv.ProductId IN (SELECT ProductId FROM #TempClientProducts);

            -- DELETE PRODUCT CHILDREN
            DELETE FROM dbo.InventoryTransaction WHERE InventoryBalanceId IN (SELECT InventoryBalanceId FROM #TempClientInventoryBalances);
            DELETE ib FROM dbo.InventoryBalance ib INNER JOIN dbo.ProductVariant pv ON pv.ProductVariantId = ib.ProductVariantId WHERE pv.ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.ProductFile WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.ProductInventory WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.ProductMedia WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.ProductOptions WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);
            DELETE FROM dbo.TransferProduct WHERE ProductId IN (SELECT ProductId FROM #TempClientProducts);

            -- DELETE PRODUCT DIRECTS
            DELETE FROM dbo.ProductLinkToken WHERE ClientId = @ClientId;
            DELETE FROM dbo.ProductCategory WHERE ClientId = @ClientId;
            DELETE FROM dbo.ProductStation WHERE ClientId = @ClientId;
            DELETE FROM dbo.Product WHERE ClientId = @ClientId;

            IF OBJECT_ID('tempdb..#TempClientProducts') IS NOT NULL DROP TABLE #TempClientProducts;
            IF OBJECT_ID('tempdb..#TempClientInventoryBalances') IS NOT NULL DROP TABLE #TempClientInventoryBalances;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#TempClientProducts') IS NOT NULL DROP TABLE #TempClientProducts;
            IF OBJECT_ID('tempdb..#TempClientInventoryBalances') IS NOT NULL DROP TABLE #TempClientInventoryBalances;
            THROW;
        END CATCH;
      ";
    }
    else if (section == Shipra.Backend.API.Core.Enum.EnumWipeOutSection.Orders)
    {
      sql = $@"
        IF OBJECT_ID('tempdb..#TempClientOrders') IS NOT NULL DROP TABLE #TempClientOrders;

        BEGIN TRANSACTION;
        BEGIN TRY
            DECLARE @ClientId UNIQUEIDENTIFIER = '{clientIdStr}';

            CREATE TABLE #TempClientOrders (OrderId UNIQUEIDENTIFIER PRIMARY KEY);
            INSERT INTO #TempClientOrders (OrderId) SELECT OrderId FROM dbo.[Order] WHERE ClientId = @ClientId;

            -- DELETE ORDER CHILDREN
            DELETE FROM dbo.OrderItem WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderTrackingHistory WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderTax WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderPODFiles WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderNote WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.OrderBox WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);
            DELETE FROM dbo.DeliveryNoteDetail WHERE OrderId IN (SELECT OrderId FROM #TempClientOrders);

            -- DELETE ORDER DIRECTS
            DELETE FROM dbo.OrderDraft WHERE ClientId = @ClientId;
            DELETE FROM dbo.OrderArchive WHERE ClientId = @ClientId;
            DELETE FROM dbo.OrderDeleted WHERE ClientId = @ClientId;
            DELETE FROM dbo.[Order] WHERE ClientId = @ClientId;

            IF OBJECT_ID('tempdb..#TempClientOrders') IS NOT NULL DROP TABLE #TempClientOrders;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#TempClientOrders') IS NOT NULL DROP TABLE #TempClientOrders;
            THROW;
        END CATCH;
      ";
    }
    else if (section == Shipra.Backend.API.Core.Enum.EnumWipeOutSection.Delivery)
    {
      sql = $@"
        BEGIN TRANSACTION;
        BEGIN TRY
            DECLARE @ClientId UNIQUEIDENTIFIER = '{clientIdStr}';

            DELETE FROM dbo.DeliveryTask WHERE ClientId = @ClientId;

            DELETE FROM dbo.DriverReceivable 
            WHERE DeliveryNoteId IN (SELECT DeliveryNoteId FROM dbo.DeliveryNote WHERE ClientId = @ClientId);

            DELETE FROM dbo.DeliveryNote WHERE ClientId = @ClientId;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW;
        END CATCH;
      ";
    }
    else if (section == Shipra.Backend.API.Core.Enum.EnumWipeOutSection.Returns)
    {
      sql = $@"
        IF OBJECT_ID('tempdb..#TempClientReturns') IS NOT NULL DROP TABLE #TempClientReturns;

        BEGIN TRANSACTION;
        BEGIN TRY
            DECLARE @ClientId UNIQUEIDENTIFIER = '{clientIdStr}';

            CREATE TABLE #TempClientReturns (ReturnId UNIQUEIDENTIFIER PRIMARY KEY);
            INSERT INTO #TempClientReturns (ReturnId) SELECT ReturnId FROM dbo.[Return] WHERE ClientId = @ClientId;

            -- DELETE RETURN CHILDREN
            DELETE FROM dbo.ReturnActivityLog WHERE ReturnId IN (SELECT ReturnId FROM #TempClientReturns);
            DELETE FROM dbo.ReturnProduct WHERE ReturnId IN (SELECT ReturnId FROM #TempClientReturns);
            DELETE FROM dbo.ReturnTrackingHistory WHERE ReturnId IN (SELECT ReturnId FROM #TempClientReturns);

            -- DELETE RETURN DIRECTS
            DELETE FROM dbo.PaymentLink WHERE ClientId = @ClientId;
            DELETE FROM dbo.[Return] WHERE ClientId = @ClientId;

            IF OBJECT_ID('tempdb..#TempClientReturns') IS NOT NULL DROP TABLE #TempClientReturns;

            COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            IF OBJECT_ID('tempdb..#TempClientReturns') IS NOT NULL DROP TABLE #TempClientReturns;
            THROW;
        END CATCH;
      ";
    }
    else
    {
      throw new ArgumentException($"Invalid section parameter: '{section}'");
    }

    await _context.Database.ExecuteSqlRawAsync(sql);
    return true;
  }
  #endregion
}
