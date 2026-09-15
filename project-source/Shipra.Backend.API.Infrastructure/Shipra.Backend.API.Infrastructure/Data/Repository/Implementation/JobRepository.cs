using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.Infrastructure.Services.Interface;
using Shipra.Backend.API.SharedKernel.Models;
using Order = Shipra.Backend.API.Core.OrderAggregate.Order;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class JobRepository : IJobRepository
{ 
  private readonly IDbContextService _dbContextService;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly ICatalogueRepository _catalogueRepository;

  public JobRepository(IDbContextService dbContextService, DapperAppDbContext dapperAppDbContext, ICatalogueRepository catalogueRepository)
  {
    _dbContextService = dbContextService;
    _dapperAppDbContext = dapperAppDbContext;
    _catalogueRepository = catalogueRepository;
  }

  public async Task<Order> CreateOrder(Order order, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.Orders.AddAsync(order);
      await _context.SaveChangesAsync();
      return order;

    }
  }

  public async Task<OrderAddress> CreateOrderAddress(OrderAddress orderAddress, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.OrderAddresses.AddAsync(orderAddress);
      await _context.SaveChangesAsync();
      return orderAddress;
    }
  }

  public async Task<OrderNote> CreateOrderNote(OrderNote orderNote, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.OrderNotes.AddAsync(orderNote!);
      await _context.SaveChangesAsync();
      return orderNote!;
    }
  }

  public async Task<bool> CreateOrderTax(OrderTax orderTax, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.OrderTaxes.AddAsync(orderTax);
      return await _context.SaveChangesAsync() > 0;
    }
  }


  public async Task<OrderTrackingHistory> CreateOrderTrackingHistory(OrderTrackingHistory orderHistory, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.OrderTrackingHistories.AddAsync(orderHistory!);
      await _context.SaveChangesAsync();
      return orderHistory!;
    }
  }

  public async Task<List<SaleChannelConfigResponseModel>> GetAllAutoSyncSaleChannelsForJob(List<ClientSummaryResponseModel> clientSummaryResponseModels)
  {
    int autoSync = 1;
    var catalogueDatabases = await _catalogueRepository.GetAllCatalogueDatabases();
    var catalogues = await _catalogueRepository.GetAllCatalogues();
    List<SaleChannelConfigResponseModel> saleChannelConfigs = new List<SaleChannelConfigResponseModel>();
    var groupClientsDatabase = clientSummaryResponseModels.GroupBy(x => x.DatabaseId).ToList();
    foreach (var clientDatabase in groupClientsDatabase)
    {
      try
      {
        var allclientIds = string.Join(',', clientDatabase.Select(x => x.ClientId!.Value.ToString()).ToList());

        var catalogDb = catalogueDatabases.FirstOrDefault(x => x.DatabaseId == clientDatabase.Key);
        if (catalogDb is not null)
        {
          using (var connection = _dapperAppDbContext.CreateConnectionByEncryptedConString(catalogDb.ConnectionString!))
          {
            var dynamicParams = new DynamicParameters();
            string query = @"SELECT scc.SaleChannelConfigId,
                                     scc.SaleChannelLookupId,
                                     scc.SaleChannelKey,
                                     scc.SaleChannelName,
                                     scc.StoreId,
                                     scc.Config,
                                     scc.ClientId,
                                     scc.IsSaleChannelActivate,
                                     scc.IsAllowToDisplayInSaleChannel,
                                     scc.SettingConfig AS SCSettingConfig, 
                                     scc.CreatedBy,
                                     scc.UserName,
                                     scc.Password,
                                     scc.CreatedOn,
                                     scc.UpdatedBy,
                                     scc.UpdateOn,
                                     scc.Active,
                                     scl.SaleChannelLookupId,
                                     scl.SaleChannelName,
                                     scl.InputRequiredConfig,
                                     scl.ImageUrl,
                                     scl.SettingConfig AS SCLSettingConfig,
                                     scl.AppUrl
                              FROM dbo.SaleChannelConfig AS scc
                                  INNER JOIN dbo.SaleChannelLookup AS scl
                                      ON scl.SaleChannelLookupId = scc.SaleChannelLookupId ";

            string whereStart = $"WHERE ( 1=1 AND (scc.Active = 1) AND (scl.AutoSync = {autoSync}) ";
            string whereEnd = ")";


            dynamicParams.Add("@ClientId", allclientIds);
            whereStart += "And ( ( scc.ClientId in (select value from STRING_SPLIT(@ClientId,',')))) ";

            string where = whereStart + whereEnd;

            Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
            keyValuePairs.Add(0, "scc.CreatedOn");

            string queryData = query + where;// + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

            var data = await connection.QueryAsync<SaleChannelConfigResponseModel>(queryData, dynamicParams);

            saleChannelConfigs.AddRange(data);

          }

        }
      }
      catch (Exception ex)
      {
        _ = ex.Message;
        continue;
      }
    }

    return saleChannelConfigs;

  }

  public async Task<Client?> GetClientById(ClientId clientid)
  {
    var clid = clientid.Value!.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      return await _context.Clients.FirstOrDefaultAsync(x => x.ClientId! == clientid);
    }
  }

  #region tracking no
  public async Task<string> GetClientNextOrderNo(ClientId? clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      int orderCount = 0;
      int? clientIdentifier = 0;

      var client = await _context.Clients
          .Where(x => x.ClientId == clientId && x.Active == true)
          .FirstOrDefaultAsync();

      if (client is not null)
      {
        clientIdentifier = client.ClientIdentifier;
        orderCount = await _context.Orders.CountAsync(x => x.ClientId == clientId);
        orderCount++;
      }

      // Get new order no of client
      string clientNextOrderNo = AppDefaultNextOrderNumber(clientIdentifier, orderCount); // ST1001

      // Duplicate order number check, if exists then create a new one using recursion
      if (!string.IsNullOrEmpty(clientNextOrderNo))
      {
        var orderExist = await _context.Orders
            .FirstOrDefaultAsync(x => x.ClientId == clientId && x.OrderNo == clientNextOrderNo);

        if (orderExist is not null)
        {
          // If the order number already exists, generate a new unique order number recursively.
          clientNextOrderNo = await GenerateUniqueOrderNoAsync(clientIdentifier, clientId!, orderCount);
        }
      }

      return clientNextOrderNo;
    }
  }

  private async Task<string> GenerateUniqueOrderNoAsync(int? clientIdentifier, ClientId clientId, int orderCount)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      orderCount++;
      string newOrderNo = AppDefaultNextOrderNumber(clientIdentifier, orderCount);

      var orderExist = await _context.Orders
          .AnyAsync(x => x.ClientId == clientId && x.OrderNo == newOrderNo);

      if (orderExist)
      {
        // If the new order number still exists, recursively call itself until a unique order number is found.
        return await GenerateUniqueOrderNoAsync(clientIdentifier, clientId, orderCount);
      }

      return newOrderNo;
    }
  }



  private static string AppDefaultNextOrderNumber(int? clientIdentifier, int? clientOrderCount)
  {
    string trNo = string.Empty;
    int length = clientOrderCount.ToString()!.ToCharArray().Length;

    if (length <= 1)
    {
      trNo = clientIdentifier + "0000" + clientOrderCount.ToString();
    }

    else if (length <= 2)
    {

      trNo = clientIdentifier + "000" + clientOrderCount.ToString();
    }

    else if (length <= 3)
    {
      trNo = clientIdentifier + "00" + clientOrderCount.ToString();
    }

    else if (length <= 4)
    {
      trNo = clientIdentifier + "0" + clientOrderCount.ToString();
    }
    else

    {
      trNo = clientIdentifier + clientOrderCount.ToString();
    }
    return trNo;

  }

  #endregion

  public async Task<string?> GetEmployeeNameById(EmployeeId? employeeId, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      string name = string.Empty;
      var oEmployee = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
      if (oEmployee is not null)
      {
        name = $"{oEmployee.EmployeeName}";
      }
      return name;
    }
  }

  public async Task<List<SaleChannelOrder>?> GetSaleChannelOrderListByOrderIds(string? orderIds, int saleChannelLookUpId, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      var list = await _context.SaleChannelOrders.Where(x => orderIds!.Contains(x.OrderId!) && x.OrderId != "" && x.SaleChannelLookupId == saleChannelLookUpId && x.ClientId == clientId).ToListAsync();
      return list;
    }
  }
  public async Task<ShopifyConfig?> GetShopifyConfigByClientId(int? SaleChannelConfigId, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      return await _context.ShopifyConfigs.Where(x => x.ClientId == clientId && x.SaleChannelConfigId == SaleChannelConfigId && x.Active == true).FirstOrDefaultAsync();
    }
  }
  public async Task<Store?> GetStoreById(int storeid, ClientId? clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      return await _context.Stores.FirstOrDefaultAsync(x => x.StoreId! == storeid && x.ClientId == clientId);
    }
  }

  public async Task<bool> CreateSaleChannelOrder(SaleChannelOrder oSaleChannelOrder, ClientId? clientId)
  {

    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.SaleChannelOrders.AddAsync(oSaleChannelOrder);
      return await _context.SaveChangesAsync() > 0;
    }
  }

  public async Task<SaleChannelOrder> GetSaleChannelOrderBySaleChannelNoByClient(string? scOderNo, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      var target = await _context.SaleChannelOrders.FirstOrDefaultAsync(x => x.OrderNo! == scOderNo && x.ClientId == clientId);
      return target!;
    }
  }

  public async Task<List<SaleChannelProduct>> GetAllSaleChannelProduct(ClientId clientId, int? saleChannelLookupId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      return await _context.SaleChannelProducts.Where(x => x.SaleChannelLookupId == saleChannelLookupId && x.ClientId == clientId).ToListAsync();
    }
  }
  public async Task<List<SaleChannelProduct>?> GetSaleChannelProductListByProductIds(string? productIds, int? saleChannelLookUpId, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      var list = await _context.SaleChannelProducts.Where(x => productIds!.Contains(x.ProductId!) && x.ProductId != "" && x.SaleChannelLookupId == saleChannelLookUpId && x.ClientId == clientId).ToListAsync();
      return list;
    }
  }

  public async Task<SaleChannelProduct> CreateSaleChannelProduct(SaleChannelProduct saleChannelProduct, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.SaleChannelProducts.AddAsync(saleChannelProduct);
      await _context.SaveChangesAsync();
      return saleChannelProduct;
    }
  }

  public async Task<dynamic> CreateProductOptionAsync(Core.ProductAggregate.ProductOption option, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.ProductOptions.AddAsync(option);
      return await _context.SaveChangesAsync() > 0;
    }
  }

  public async Task<dynamic> CreateProductStockHistory(Core.ProductAggregate.ProductStockHistory stockHistory, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      _context.ProductStockHistories.Add(stockHistory);
      await _context.SaveChangesAsync();
      return stockHistory;
    }
  }

  public async Task<Core.ProductAggregate.ProductStock> CreateProductStockAsync(Core.ProductAggregate.ProductStock productStock, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      _context.ProductStocks.Add(productStock);
      await _context.SaveChangesAsync();
      return productStock;
    }
  }

  public async Task<dynamic> CreateProductAsync(Core.ProductAggregate.Product product)
  {
    var clid = product.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      _context.Products.Add(product);
      return await _context.SaveChangesAsync() > 0;
    }
  }
  public async Task<List<dynamic>?> GetAllProductStocksForSaleChannelInventorySync(string? clientId, string? productSKUs)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT  pv.SKU,
                               ib.QuantityAvailable,
                               CAST(p.ProductId AS NVARCHAR(40)) AS ProductId,
                               ib.ProductStockId,
                               pv.SaleChannelVariantId
                        FROM dbo.ProductStock AS ib
                            INNER JOIN dbo.ProductVariant AS pv ON pv.ProductVariantId = ib.ProductVariantId
                            INNER JOIN dbo.Product AS p
                                ON p.ProductId = pv.ProductId ";

      string whereStart = "WHERE ( 1=1 AND p.Active = 1 AND pv.Active = 1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(productSKUs))
      {
        dynamicParams.Add("@ProductSKUs", productSKUs);
        whereStart += @" And ( ( pv.SKU in (select value from STRING_SPLIT(@ProductSKUs,',')))) ";
      }

      dynamicParams.Add("@ClientId", clientId);
      whereStart += "And (p.ClientId = @ClientId) ";

      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.AsList();
    }
  }

  public async Task<Core.ProductAggregate.ProductStock?> GetProductStockBySKUAsync(string sku, ClientId? clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      var result = await _context.ProductStocks.FirstOrDefaultAsync(p => p.Sku == sku);
      return result;
    }
  }

  public async Task<Core.ProductAggregate.ProductStock> UpdateProductStockAsync(Core.ProductAggregate.ProductStock productStock, ClientId? clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      _context.ProductStocks.Update(productStock);
      await _context.SaveChangesAsync();
      return productStock;
    }
  }

  public async Task<OrderItem> CreateOrderItem(OrderItem orderItem, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      await _context.OrderItems.AddAsync(orderItem);
      await _context.SaveChangesAsync();
      return orderItem;
    }
  }
  public async Task<Core.ProductAggregate.ProductStock> GetProductStockByIdAsync(long productStockId, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      return (await _context.ProductStocks.FirstOrDefaultAsync(x => x.ProductStockId == productStockId))!;
    }
  }

  public async Task<Core.ProductAggregate.Product?> GetProductByIdAsync(ProductId productId, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      return await _context.Products.FirstOrDefaultAsync(x => x.ProductId! == productId && x.ClientId == clientId);
    }
  }
  public async Task<dynamic> UpdateOrder(Order order)
  {
    var clid = order!.ClientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      _context.Orders.Update(order);
      return await _context.SaveChangesAsync() > 0;
    }
  }
  public async Task<List<Order>> GetOrdersWithOrderNos(string? orderNos, ClientId clientId)
  {
    var clid = clientId!.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(clid))
    {
      var list = await _context.Orders.Where(x => orderNos!.Contains(x.OrderNo!) && x.OrderNo != "" && x.ClientId == clientId).ToListAsync();
      return list;
    }
  }
}
