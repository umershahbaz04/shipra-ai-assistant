using System.Dynamic;
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class ClientRepositoryInitializer : IClientRepositoryInitializer
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly ShipraMasterDbContext _shipraMasterDbContext;
  private readonly IDbContextService _dbContextService;

  public ClientRepositoryInitializer(DapperAppDbContext dapperAppDbContext, ShipraMasterDbContext shipraMasterDbContext, IDbContextService dbContextService)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _shipraMasterDbContext = shipraMasterDbContext;
    _dbContextService = dbContextService;
  }

  public async Task<bool> CreatePrerequisiteClientData(string clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {

      await Task.Delay(1000);
      return true;
    }
  }
  public async Task<dynamic> CreateClient(Client client, ClientAddress clientAddress)
  {
    var clientId = client!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.Clients.AddAsync(client);
      var res = await _context.SaveChangesAsync() > 0;

      if (res)
      {
        await _context.ClientAddresses.AddAsync(clientAddress);
        await _context.SaveChangesAsync();
      }
    }
    return client;
  }
  public async Task<ClientUserRole?> GetClientUserRoleByName(string roleName, ClientId clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId.Value.ToString()))
    {
      var userRole = await _context.ClientUserRolees.FirstOrDefaultAsync(x => x.RoleName!.Trim()!.ToLower() == roleName!.Trim()!.ToLower() && x.ClientId == clientId);
      return userRole;
    }
  }
  public async Task<string> GetEmployeeNextCode(ClientId clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId.Value.ToString()))
    {
      int? clientIdentifier = 0;
      var client = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync();
      if (client is not null)
      {
        clientIdentifier = client.ClientIdentifier;
      }
      string employeeCode = await GenerateEmployeeCodeAsync(clientIdentifier, clientId.Value.ToString()); //EM1001

      return employeeCode;
    }
  }
  #region employee code

  private async Task<string> GenerateEmployeeCodeAsync(int? clientIdentifier, string clientId)
  {
    string employeeCode = $"{clientIdentifier}{GenerateNextEmployeeUserName()}";
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      var emp = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeCode == employeeCode);

      if (emp != null)
      {
        // Employee code already exists, generate a new one recursively
        return await GenerateEmployeeCodeAsync(clientIdentifier, clientId);
      }

      return employeeCode;
    }
  }
  #region em no
  private string? GenerateNextEmployeeUserName()
  {
    Guid myGuid = Guid.NewGuid();
    uint hashCode = (uint)myGuid.GetHashCode();
    string formattedHex = string.Format("{0:x}", hashCode).Substring(0, 6); ;

    return formattedHex;

  }

  #endregion
  #endregion
  public async Task<dynamic?> CreateStore(Store store)
  {
    var clientId = store!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.Stores.AddAsync(store);
      await _context.SaveChangesAsync();
      return store;
    }
  }
  public async Task<bool> CreateStoreAddress(StoreAddress oStoreAddress, string clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.StoreAddresses.AddAsync(oStoreAddress);
      return await _context.SaveChangesAsync() > 0;
    }
  }
  #region ProductLinkToken
  public async Task<ProductLinkToken> GetProductLinkTokenByToken(string? token)
  {
    var databaseId = UtilityHelper.GetLeadingInteger(token!);

    var oCatalogDatabase = await _shipraMasterDbContext.CatalogueDatabases.FirstOrDefaultAsync(x => x.DatabaseId == databaseId);

    using (var _context = _dbContextService.GetAppDbContextWithConnectionString(oCatalogDatabase!.ConnectionString!))
    {
      var data = await _context.ProductLinkTokens.FirstOrDefaultAsync(x => x.Token == token);
      var clientId = data?.ClientId;
      return data!;
    }
  }
  public async Task<dynamic> GetProductByIdForTakeOrder(string productId, int? storeId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      //Currency need to join table after that
      var query = @$"SELECT CAST(p.ProductId AS NVARCHAR(40)) AS ProductId,
                             p.ProductName,
                             p.FeatureImage,
                             p.SKU,
                             p.Price,
                             p.PurchasePrice,
                             p.[Description],
                             p.HaveOptions,
                             p.[Weight],
                             {storeId} AS StoreId,
                             c2.ClientId,
                             c.Code AS Currency,
	                         c2.DefaultProductStationId
                      FROM dbo.Product AS p 
                          INNER JOIN dbo.Currency AS c
                              ON c.CurrencyId = p.CurrencyId
                          LEFT JOIN dbo.ProductLinkToken AS plt
                              ON plt.ProductId = p.ProductId
                          INNER JOIN dbo.Client AS c2
                              ON c2.ClientId = p.ClientId ";
      string whereStart = "WHERE (1=1 AND (p.Active = 1) ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(productId))
      {
        dynamicParams.Add("@ProductId", productId);
        whereStart += "And (p.ProductId = @ProductId) ";
      }
      string where = whereStart + whereEnd;


      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic item = new ExpandoObject();
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        item = dataList.FirstOrDefault()!;

        if (!string.IsNullOrEmpty(productId))
        {
          var items = await GetProductStocksDetailByProductIdAsync(productId, clientId);
          item!.ProductStocks = items!;

          var media = await GetProductMediaByProductId(productId, clientId);
          item!.ProductMedias = media!;

          if (item.HaveOptions)
          {
            var options = await GetProductOptionsbYProductIdAsync(productId, clientId);
            item!.ProductOptions = options!;
          }
        }
      }
      return item!;
    }
  }
  public async Task<List<dynamic>?> GetProductStocksDetailByProductIdAsync(string productId, string clientId, int productStationId = 0)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      //Currency need to join table after that
      var query = @"SELECT ps.ProductStockId,
                           ps.SKU,
                           ps.VarientOption,
                           ps.Price,
                           ps2.ProductStationId,
                           '' as Image
                    FROM dbo.ProductStock AS ps
                        INNER JOIN dbo.Product AS p
                            ON p.ProductId = ps.ProductId
                        INNER JOIN dbo.ProductStation AS ps2
                            ON ps2.ProductStationId = ps.ProductStationId ";
      string whereStart = "WHERE (1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }

      if (!string.IsNullOrEmpty(productId))
      {
        dynamicParams.Add("@ProductId", productId);
        whereStart += "And (ps.ProductId = @ProductId) ";
      }
      if (productStationId > 0)
      {
        dynamicParams.Add("@productStationId", productStationId);
        whereStart += "And (ps.ProductStationId = @productStationId) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      var dataList = data.ToList();

      return dataList;
    }

  }
  public async Task<List<dynamic>?> GetProductMediaByProductId(string productId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      //Currency need to join table after that
      var query = @"SELECT pm.ImageGalleryId,ig.ImageUrl from Product as p
                    Inner join ProductMedia as pm
                    ON pm.ProductId = p.ProductId
                    Inner join ImageGallery as ig
                    ON ig.ImageGalleryId = pm.ImageGalleryId ";
      string whereStart = "WHERE (1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }

      if (!string.IsNullOrEmpty(productId))
      {
        dynamicParams.Add("@ProductId", productId);
        whereStart += "And (p.ProductId = @ProductId) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      var dataList = data.ToList();

      return dataList;
    }

  }
  public async Task<List<dynamic>?> GetProductOptionsbYProductIdAsync(string productId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      //Currency need to join table after that
      var query = @"SELECT pol.ProductOptionId,
                           pol.Name,
                           po.OptionValue 
                    FROM dbo.ProductOptions AS po
                        INNER JOIN dbo.ProductOptionLookup AS pol
                            ON po.OptionId = pol.ProductOptionId
                        INNER JOIN dbo.Product AS p
                            ON p.ProductId = po.ProductId
                        INNER JOIN dbo.Client AS c
                            ON c.ClientId = p.ClientId ";
      string whereStart = "WHERE (1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }

      if (!string.IsNullOrEmpty(productId))
      {
        dynamicParams.Add("@ProductId", productId);
        whereStart += "And (po.ProductId = @ProductId) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      var dataList = data.ToList();

      return dataList;
    }

  }

  #endregion
  public async Task<string> GetNextProductStationCode(ClientId clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId.Value.ToString()))
    {
      int count = 0;
      int? clientIdentifier = 0;
      var client = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync();
      if (client is not null)
      {
        clientIdentifier = client.ClientIdentifier;
        count = await _context.ProductStations.CountAsync(x => x.ClientId == clientId);
        count++;
      }
      string clientStoreCode = $"PS{clientIdentifier}{count}"; //ST1001
      return clientStoreCode;
    }
  }
  public async Task<ProductStation> CreateProductStation(ProductStation model)
  {
    var clientId = model!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.ProductStations.AddAsync(model);
      await _context.SaveChangesAsync();
      return model;
    }
  }
  public async Task<dynamic> CreateProductCategory(ProductCategory model)
  {
    var clientId = model!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.ProductCategories.AddAsync(model);
      await _context.SaveChangesAsync();
      return model;
    }
  }
  public async Task<bool> CreateCarrierFromLookup(string clientId)
  {
    bool isSave = false;
    #region get all data from master

    var allCarriers = await _shipraMasterDbContext.Carriers.ToListAsync();
    var allCarrierFeatures = await _shipraMasterDbContext.CarrierFeatures.ToListAsync();
    var allCarrierLocations = await _shipraMasterDbContext.CarrierLocations.ToListAsync();
    var allCarrierDeliveryService = await _shipraMasterDbContext.CarrierDeliveryServices.ToListAsync();
    var allDeliveryService = await _shipraMasterDbContext.DeliveryServices.ToListAsync();
    #endregion

    //insert into client db if no entry exist

    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      var clientDbCarriers = await _context.Carriers.ToListAsync();
      if (clientDbCarriers.Count == 0)
      {
        foreach (var car in allCarriers)
        {
          var carrier = Carrier.CreateCarrier(car.CarrierId, car.Name, car.CarrierImage, car.CarrierWebsite, car.Config, car.InputRequiredConfig, car.CountryId, car.SettingConfig, car.IsRateCheck, car.ValidateAddress, car.CreatedBy!);
          await _context.Carriers.AddAsync(carrier);
        }

        foreach (var item in allCarrierFeatures)
        {
          CarrierFeature cf = CarrierFeature.Create(item.CarrierId, item.Feature, item.Active.GetValueOrDefault(), new EmployeeId(new Guid(clientId)));
          await _context.CarrierFeatures.AddAsync(cf);
        }
        foreach (var item in allCarrierLocations)
        {
          CarrierLocation carrierLocation = CarrierLocation.Create(item.CarrierId, item.CountryId, item.Active, item.AddressingScheme);
          await _context.CarrierLocations.AddAsync(carrierLocation);
        }
        foreach (var item in allCarrierDeliveryService)
        {
          CarrierDeliveryService carrierDeliveryService = CarrierDeliveryService.Create(item.DeliveryServiceId, item.CarrierId, item.Active);
          await _context.CarrierDeliveryServices.AddAsync(carrierDeliveryService);
        }
        foreach (var item in allDeliveryService)
        {
          DeliveryService deliveryService = DeliveryService.Create(item.ServiceLogo, item.ServiceName);
          await _context.DeliveryServices.AddAsync(deliveryService);
        }

        await _context.SaveChangesAsync();
      }
    }

    return isSave;
  }
  public async Task<List<Carrier>> GetAllCreatedClientCarrier(string clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      var allCarriers = await _context.Carriers.Where(x => x.IsClientCarrier == true).ToListAsync();
      return allCarriers!;
    }
  }
  public async Task<dynamic> CreateCarrier(Carrier model, string clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      int newCarrierId = model.CarrierId;

      // Keep looping until we find a unique CarrierId
      do
      {
        bool exists = await _context.Carriers
            .AnyAsync(c => c.CarrierId == newCarrierId);

        if (!exists)
          break;

        newCarrierId++;
      }
      while (true);
      // Assign the unique CarrierId
      model.CarrierId = newCarrierId;

      await _context.Carriers.AddAsync(model);
      await _context.SaveChangesAsync();
      return model;
    }
  }
  public async Task<dynamic> CreateActiveCarrier(ActiveCarrier model)
  {
    var clientId = model!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.ActiveCarriers.AddAsync(model);
      await _context.SaveChangesAsync();
      return model;
    }
  }
  public async Task<int> CreateExpenseCategoryForGeneralSetting(ClientId? clientId)
  {
    using (var connection = _dbContextService.GetDapperDbConection(clientId!.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"INSERT INTO dbo.ExpenseCategory(ExpenceName,ClientId,Active) SELECT ecl.ExpenseCategoryName,'{clientId?.Value}',1 FROM dbo.ExpenseCategoryLookup AS ecl;";

      var IsSaved = await connection.ExecuteAsync(query);
      return IsSaved;
    }
  }
  public async Task<int> CreateDriverDefaultCTSSetting(ClientId? clientId, EmployeeId employeeId)
  {
    int isSaved = 0;
    using (var _context = _dbContextService.GetAppDbContext(clientId!.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      //Status Values
      int[] statusValues = new int[] { (int)EnumCarrierTrackingStatus.Delivered, (int)EnumCarrierTrackingStatus.Cancelled, (int)EnumCarrierTrackingStatus.LocationChanged, (int)EnumCarrierTrackingStatus.MobileNotAnswered, (int)EnumCarrierTrackingStatus.MobileSwitchedOff, (int)EnumCarrierTrackingStatus.Refunded, (int)EnumCarrierTrackingStatus.Exchanged };
      foreach (var statusId in statusValues)
      {
        var objDriverCTSSetting = DriverCTSSetting.Create(clientId, statusId, employeeId);
        await _context.DriverCTSSettings.AddAsync(objDriverCTSSetting);
        isSaved = await _context.SaveChangesAsync();
      }
      return isSaved;
    }
  }
  public async Task<List<DefaultShipmentDashboard>?> GetDefaultShipmentDashboard()
  {
    return await _shipraMasterDbContext.DefaultShipmentDashboards.ToListAsync();
  }
  public async Task<bool> CreateShipmentGridColumn(ShipmentGridColumn oShipmentGridColumn)
  {
    var clientId = oShipmentGridColumn!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.AddAsync(oShipmentGridColumn);
      return await _context.SaveChangesAsync() > 0;
    }
  }
  public async Task<bool> CreateShipmentGridClientSetting(ShipmentGridClientSetting oShipmentGridClientSetting)
  {
    var clientId = oShipmentGridClientSetting!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.AddAsync(oShipmentGridClientSetting);
      return await _context.SaveChangesAsync() > 0;
    }
  }
  public async Task<List<CarrierTrackingStatusLookup>?> GetAllCarrierTrackingStatusLookup()
  {
    return await _shipraMasterDbContext.CarrierTrackingStatusLookups.ToListAsync();
  }
  public async Task<bool> CreateBatchClientCarrierTrackingStatus(List<ClientCarrierTrackingStatus> listOfClientCarrier)
  {
    var clientId = listOfClientCarrier.FirstOrDefault()?.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      await _context.ClientCarrierTrackingStatuses.AddRangeAsync(listOfClientCarrier);
      return await _context.SaveChangesAsync() > 0;

    }
  }

  public async Task<Client?> GetLastClient(string clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      return await _context.Clients
                    .OrderByDescending(p => p.CreatedOn)
                    .FirstOrDefaultAsync();
    }
  }
  public async Task<Client?> GetClientById(string clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      var clientIdGuid = new ClientId(new Guid(clientId));
      return await _context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientIdGuid);
    }
  }
  public async Task<dynamic?> GetProductStationLookupByCityId(int? cityId)
  {
    return await _shipraMasterDbContext.StationLookups.FirstOrDefaultAsync(x => x.CityId == cityId);
  }
  public async Task<Employee> CreateEmployee(Employee employee)
  {
    var clientId = employee!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      await _context.Employees.AddAsync(employee);
      await _context.SaveChangesAsync();
      return employee;
    }
  }
  public async Task<bool> CreateEmployeeAddress(EmployeeAddress oOrderAddress, string? clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      await _context.EmployeeAddresses.AddAsync(oOrderAddress);
      return await _context.SaveChangesAsync() > 0;
    }
  }
  public async Task<dynamic> UpdateClient(Client client)
  {
    var clientId = client!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clientId))
    {
      _context.Clients.Update(client);
      await _context.SaveChangesAsync();
      return client;
    }
  }
  public async Task<string> GetClientNextStoreCode(ClientId? clientId)
  {
    var clientIdStr = clientId!.Value.ToString();

    using (var _context = _dbContextService.GetAppDbContext(clientIdStr))
    {
      var client = await _context.Clients
          .Where(x => x.ClientId == clientId && x.Active == true)
          .FirstOrDefaultAsync();

      if (client is null)
        throw new InvalidOperationException($"Client with ID {clientId} not found or inactive.");

      var clientIdentifier = client.ClientIdentifier ?? 0;

      int storeCount = await _context.Stores.CountAsync(x => x.ClientId == clientId);
      string clientStoreCode;
      bool exists;

      do
      {
        storeCount++;
        clientStoreCode = $"ST{clientIdentifier}{storeCount}";
        exists = await _context.Stores.AnyAsync(s => s.StoreCode == clientStoreCode);
      }
      while (exists);

      return clientStoreCode;
    }
  }

}
