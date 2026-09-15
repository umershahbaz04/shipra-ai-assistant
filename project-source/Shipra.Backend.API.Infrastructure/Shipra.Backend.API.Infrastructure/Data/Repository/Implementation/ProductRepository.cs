using System.Dynamic;
using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ProductRepository : IProductRepository
{
  private readonly ICatalogueRepository _catalogueRepository;
  private readonly IConfiguration _configuration;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;
  private readonly IDbContextService _dbContextService;

  public ProductRepository(ICatalogueRepository catalogueRepository, IConfiguration configuration, DapperAppDbContext dapperAppDbContext, AppDbContext context, IDbContextService dbContextService)
  {
    _catalogueRepository = catalogueRepository;
    _configuration = configuration;
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
    _dbContextService = dbContextService;
  }

  public async Task<dynamic> CreateProductAsync(Product product)
  {
    _context.Products.Add(product);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> CreateProductOptionsAsync(List<ProductOption> productOptions)
  {
    _context.ProductOptions.AddRange(productOptions);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> CreateProductStocksAsync(List<ProductStock> productStocks)
  {
    _context.ProductStocks.AddRange(productStocks);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> CreateProductOptionAsync(ProductOption option)
  {
    await _context.ProductOptions.AddAsync(option);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<Product>?> GetAllProductAsync(ClientId? clientId)
  {
    var productStock = await _context.ProductStocks.ToListAsync();

    return await _context.Products.Where(x => x.ClientId == clientId).ToListAsync();
  }

  public async Task<List<Product>?> GetAllProductByProductIdsAsync(List<string> productIds)
  {
    List<Product>? productList = new List<Product>();
    // Ensure the productIds list is not null and contains elements
    if (productIds == null || !productIds.Any())
    {
      return new List<Product>(); // or return null based on your preference
    }
    foreach (var item in productIds)
    {
      var oProduct = await _context.Products.Where(p => p.ProductId == new ProductId(new Guid(item))).FirstOrDefaultAsync();

      productList.Add(oProduct!);
    }
    return productList;
  }
  public async Task<List<Product>?> GetAllProductsByIdsAsync(string productid, string clientid)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientid))
    {
      var productIds = productid.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => new ProductId(Guid.Parse(x.Trim()))).ToList();
      var oProduct = await _context.Products.Where(p => productIds.Contains(p.ProductId!)
             && p.ClientId == new ClientId(Guid.Parse(clientid))).ToListAsync();
      return oProduct;
    }
  }
  public async Task<(List<ProductStock> Stock, List<ProductOption> Options, List<ProductMedia> ProductMedia)> GetAllProductStockAndOptionsByIdAsync(Product model, string clientid)
  {
    using var _context = _dbContextService.GetAppDbContext(clientid);




    var stocks = await _context.ProductStocks.Where(p => p.ProductId! == model.ProductId).ToListAsync();

    var options = await _context.ProductOptions.Where(p => p.ProductId! == model.ProductId).ToListAsync();

    var productMedia = await _context.ProductMedias.Where(p => p.ProductId == model.ProductId).ToListAsync();

    return (stocks, options, productMedia);
  }
  public async Task<Product?> GetProductByIdAsync(ProductId productId, ClientId? clientId)
  {
    return await _context.Products.FirstOrDefaultAsync(x => x.ProductId! == productId && x.ClientId == clientId);
  }
  public async Task<Product?> GetProductBySKUAsync(string sku, ClientId? clientId)
  {
    return await _context.Products.FirstOrDefaultAsync(x => x.Sku!.Trim().ToLower() == sku!.Trim().ToLower() && x.ClientId == clientId);
  }

  public async Task<ProductStock?> GetProductStockBySaleChannelVariantIdAsync(long? saleChannelVariantId)
  {
    return await _context.ProductStocks.FirstOrDefaultAsync(x => x.SaleChannelVariantId! == saleChannelVariantId);
  }
  public async Task<List<ProductOptionLookup>?> GetProductOptionLookups()
  {
    return await _context.ProductOptionLookups.ToListAsync();
  }

  public async Task<List<ProductOption>?> GetProductOptionByProductIdAsync(ProductId productId)
  {
    return await _context.ProductOptions.Where(x => x.ProductId! == productId).ToListAsync();
  }

  public async Task<Product?> CheckUniqueProductSku(string sku, ClientId? clientId)
  {
    return await _context.Products.FirstOrDefaultAsync(x => x.Sku! == sku && x.ClientId == clientId);
  }

  
  public async Task<List<dynamic>> GetProductVariantsAndBalancesAsync(ProductId productId, string clientId)
  {
      using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
      {
          var query = @"
            SELECT 
                ib.InventoryBalanceId,
                pv.ProductVariantId,
                pv.ProductId,
                pv.SKU,
                pv.Price,
                pv.PurchasePrice,
                ISNULL(ib.QuantityReserved, 0) AS QuantityCommited,
                ISNULL(ib.QuantityIncoming, 0) AS QuantityIncoming,
                ISNULL(ib.QuantityAvailable, 0) AS QuantityAvailable,
                ISNULL(pv.LowQuantityLimit, 0) AS LowQuantityLimit,
                0 AS IsLowQuantity,
                ISNULL(ib.QuantityOnOrder, 0) AS QuantityOnOrder,
                ib.ProductStationId,
                (SELECT STRING_AGG(po.OptionValue, ' / ') 
                 FROM dbo.ProductVariantOption pvo 
                 INNER JOIN dbo.ProductOptions po ON po.ProductOptionsId = pvo.ProductOptionsId 
                 WHERE pvo.ProductVariantId = pv.ProductVariantId) AS VarientOption,
                pv.Active,
                0 AS QuantityDamage,
                pv.ProductVariantStatusId,
                pv.ImageGalleryId,
                pv.Barcode,
                pv.Weight,
                pv.Length,
                pv.Width,
                pv.Height
            FROM dbo.ProductVariant pv
            LEFT JOIN dbo.InventoryBalance ib ON pv.ProductVariantId = ib.ProductVariantId
            LEFT JOIN dbo.ProductStation ps ON ps.ProductStationId = ib.ProductStationId AND ps.Active = 1
            WHERE pv.ProductId = @ProductId
        ";
        var result = await connection.QueryAsync<dynamic>(query, new { ProductId = productId.Value });
        return result.ToList();
      }
  }

  public async Task<List<ProductStock>?> GetProductStockByProductIdAsync(ProductId productId)
  {
    var query = _context.ProductStocks    // your starting point - table in the "from" statement
   .Join(_context.ProductStations, // the source table of the inner join
      ps => ps.ProductStationId,        // Select the primary key (the first part of the "on" clause in an sql "join" statement)
      ps2 => ps2.ProductStationId,   // Select the foreign key (the second part of the "on" clause)
      (productStock, productStation) => new { ProductStock = productStock, ProductStation = productStation }) // selection
   .Where(pStopnAndStation => pStopnAndStation.ProductStock.ProductId == productId && pStopnAndStation.ProductStation.Active == true).Select(x => x.ProductStock);    // where statement


    return await query.ToListAsync();
  }

  public async Task<dynamic> UpdateProductAsync(Product product, List<ProductOption>? productOptions)
  {
    _context.Products.Update(product);
    _context.ProductOptions.UpdateRange(productOptions!);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> UpdateProductAsync(Product product)
  {
    _context.Products.Update(product);
    return await _context.SaveChangesAsync() > 0;
  }


  public async Task<Product?> GetProductStockOptionByIdAsync(ProductId productId)
  {
    return await _context.Products.FirstOrDefaultAsync(x => x.ProductId! == productId);
  }

  //public async Task<List<dynamic>> DashbaordGetAllProducts()
  //{
  //  var query = "SELECT * FROM Product";
  //  using (var connection = _dapperAppDbContext.CreateConnection())
  //  {
  //    var data = await connection.QueryAsync(query);
  //    return data.ToList();
  //  }
  //}
  public async Task<dynamic> GetAllProducts(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeId = "", bool? addOptions = false, int? productStationId = 0)
  {
    var orderTackingPageUrl = _configuration.GetValue<string>("OrderTackingPageUrl");
    if (length == -1)
    {
      start = 0;
      length = 1000000;
    }
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      //Currency need to join table after that
      var query = @$"SELECT 
                            ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                            COUNT(*) OVER () AS TotalCount,
                            CAST(p.ProductId AS NVARCHAR(40)) AS ProductId,
                            p.ProductName,
                            p.FeatureImage,
                            spAgg.StoreName,
                            spAgg.StoreIds,
                            spAgg.StoreImages,
                            p.SKU,
                            p.Price,
                            p.PurchasePrice,
                            p.ProductCategoryId,
                            p.ProductStatusId,
                            p.[Description],
                            p.HaveOptions,
                            p.TrackInventory,
                            p.Active,
                            ISNULL(sq.VarientCount, 0) AS VarientCount,
                            ISNULL(sq.QuantityAvailable, 0) AS QuantityAvailable,
                            p.[Weight],
                            p.UpdatedOn,
                            p.CreatedOn,
                            c.Code AS Currency,
                            ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                            scc.SaleChannelConfigId,
                            pc.CategoryName AS ProductCategoryName,
                            ISNULL(lpss.StatusName, '') AS StatusName
                        FROM dbo.Product AS p
                        INNER JOIN dbo.ProductCategory AS pc ON pc.ProductCategoryId = p.ProductCategoryId
                        INNER JOIN dbo.Currency AS c ON c.CurrencyId = p.CurrencyId
                        LEFT JOIN dbo.LookupProductStockStatus AS lpss ON lpss.ProductStockStatusId = p.ProductStatusId
                        LEFT JOIN dbo.SaleChannelConfig AS scc ON scc.SaleChannelConfigId = p.SaleChannelConfigId
                        LEFT JOIN (
                            SELECT 
                                ps.ProductId,
                                COUNT(DISTINCT ps.SKU) AS VarientCount,
                                SUM(ps.QuantityAvailable) AS QuantityAvailable
                            FROM dbo.ProductStock AS ps
                            INNER JOIN dbo.ProductStation AS ps2 ON ps2.ProductStationId = ps.ProductStationId
                            WHERE ps2.Active = 1
                            AND (@ProductStationId IS NULL OR @ProductStationId = 0 OR ps.ProductStationId = @ProductStationId)
                            GROUP BY ps.ProductId
                        ) AS sq ON sq.ProductId = p.ProductId
                        CROSS APPLY (
                                    SELECT 
                                        STRING_AGG(CAST(s.StoreId AS NVARCHAR), ', ') AS StoreIds,
                                        STRING_AGG(
                                            CASE 
                                                WHEN s.IsDefault = 1 THEN s.StoreName + ' (Default)'
                                                ELSE s.StoreName
                                            END, ', '
                                        ) AS StoreName,
                                        STRING_AGG(CAST(ISNULL(s.StoreImage, '') AS NVARCHAR(MAX)), ', ') AS StoreImages
                                    FROM dbo.StoreProduct AS sp
                                    INNER JOIN dbo.Stores AS s ON s.StoreId = sp.StoreId
                                    WHERE sp.ProductId = p.ProductId
                                      AND sp.Active = 1
                                      AND ( @storeId IS NULL OR @storeId = '' 
                                      OR (sp.StoreId IN (SELECT value FROM STRING_SPLIT(@storeId,',')))
                                    )
                                ) AS spAgg ";
      string whereStart = "WHERE (1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);
      dynamicParams.Add("@storeId", storeId ?? "");
      dynamicParams.Add("@ProductStationId", productStationId ?? 0);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@Search", search);
        whereStart += @"
                    AND 
                    (((p.ProductName IN (SELECT TRIM(value) FROM STRING_SPLIT(@Search, ','))
                                OR p.ProductName LIKE '%' + @Search + '%')
                            AND p.ClientId IN (SELECT TRIM(value) FROM STRING_SPLIT(@ClientId, ',')))
                        OR
                        ((p.SKU IN (SELECT TRIM(value) FROM STRING_SPLIT(@Search, ','))
                                OR p.SKU LIKE '%' + @Search + '%')
                            AND p.ClientId IN (SELECT TRIM(value) FROM STRING_SPLIT(@ClientId, ','))))";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("p.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("p.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      if (!string.IsNullOrEmpty(storeId) && length > 0)
      {
        query = query + @" INNER JOIN dbo.StoreProduct AS sp  ON p.ProductId = sp.ProductId AND sp.Active =1
                           INNER JOIN dbo.Stores AS s ON sp.StoreId = s.StoreId ";

        whereStart += "And ( (sp.StoreId in (select value from STRING_SPLIT(@storeId,',')))) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "p.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        foreach (var item in dataList)
        {
          if (!string.IsNullOrEmpty(item?.ProductId))
          {
            var items = await GetProductStocksDetailByProductIdAsync(item?.ProductId, clientId, productStationId);
            item!.ProductStocks = items!;
            if (addOptions.GetValueOrDefault())
            {
              if (item.HaveOptions)
              {
                var options = await GetProductOptionsbYProductIdAsync(item?.ProductId, clientId);
                item!.ProductOptions = options!;
              }
              item!.ProductMedias = await GetProductMediaById(item?.ProductId, clientId);
            }
          }
        }
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }
  public async Task<List<dynamic>?> GetProductStocksDetailByProductIdAsync(string productId, string clientId, int? productStationId = 0)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      //Currency need to join table after that
      var query = @"SELECT ps.SKU,
                           ps.ProductStockId,
                           ps.VarientOption,
                           ps.LowQuantityLimit,
                           ps.QuantityCommited,
                           ps.QuantityAvailable,
                           ps.QuantityOnOrder,
                           ps.QuantityInComing,
                           ps.QuantityDamage,
                           ps.Price,
                           ps.IsLowQuantity,
	                         ps.Active,
                           ps2.ProductStationId,
                           ps2.StationCode,
                           ps2.Name AS StationName,
                           ps2.CreatedOn
                    FROM dbo.ProductStock AS ps
                        INNER JOIN dbo.Product AS p
                            ON p.ProductId = ps.ProductId
		                    INNER JOIN dbo.ProductStation AS ps2 ON ps2.ProductStationId = ps.ProductStationId ";
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
      if (productStationId.GetValueOrDefault() > 0)
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
  public async Task<dynamic?> GetAllStoreWithTokenByProductId(string productId, string clientId, int storeId = 0)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var orderTackingPageUrl = _configuration.GetValue<string>("OrderTackingPageUrl");

      var dynamicParams = new DynamicParameters();
      //Currency need to join table after that
      var query = @$"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                           COUNT(*) OVER () AS TotalCount,
                           s.StoreId,
                           s.StoreName,
                           CASE
                               WHEN plt.Token IS NULL THEN
                                   ''
                               ELSE
                                   CONCAT('{orderTackingPageUrl}/', plt.Token)
                           END AS OrderLink,
                           sp.ProductId
                    FROM dbo.StoreProduct AS sp
                        INNER JOIN dbo.Stores AS s
                            ON s.StoreId = sp.StoreId
                        LEFT JOIN dbo.Product AS p
                            ON p.ClientId = s.ClientId
                        LEFT JOIN dbo.ProductLinkToken AS plt
                            ON plt.ProductId = sp.ProductId AND plt.StoreId = sp.StoreId ";
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
        whereStart += "And (sp.ProductId = @ProductId) ";
      }
      string groupBy = @$"GROUP BY CASE
                       WHEN plt.Token IS NULL THEN
                           ''
                       ELSE
                           CONCAT('{orderTackingPageUrl}/', plt.Token)
                   END,
                   s.StoreId,
                   s.StoreName,
                   sp.ProductId ";
      string where = whereStart + whereEnd;

      string queryData = query + where + groupBy;

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
  public async Task<List<dynamic>?> GetProductMediaById(string productId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      //Currency need to join table after that
      var query = @"SELECT pm.ProductMediaId,
                           ig.ImageGalleryId,
                           ig.ImageUrl
                    FROM dbo.Product AS p
                        INNER JOIN dbo.Client AS c
                            ON c.ClientId = p.ClientId
                        INNER JOIN dbo.ProductMedia AS pm
                            ON pm.ProductId = p.ProductId
                        INNER JOIN dbo.ImageGallery AS ig
                            ON pm.ImageGalleryId = ig.ImageGalleryId ";
      string whereStart = "WHERE (1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (c.ClientId = @ClientId) ";
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
  public async Task<ProductStock> GetProductStockByIdAsync(long? productStockId)
  {
    var data = await _context.ProductStocks.FirstOrDefaultAsync(x => x.ProductStockId! == productStockId!)!;
    return data!;
  }
  public async Task<ProductStock> UpdateProductStockAsync(ProductStock productStock)
  {
    _context.ProductStocks.Update(productStock);
    await _context.SaveChangesAsync();
    return productStock;
  }

  public async Task<ProductStock> CreateProductStockAsync(ProductStock productStock)
  {
    _context.ProductStocks.Add(productStock);
    await _context.SaveChangesAsync();
    return productStock;
  }
  public async Task<dynamic> CreateQuickAddProductOption(List<ProductOption> productOptions, List<ProductStock> productStocks)
  {
    _context.ProductStocks.AddRange(productStocks);
    _context.ProductOptions.AddRange(productOptions);
    return await _context.SaveChangesAsync() > 0;
  }


  
  
  public async Task<List<ProductVariant>> GetProductVariantsByProductIdAsync(ProductId productId)
  {
      return await _context.ProductVariants.Where(x => x.ProductId == productId).ToListAsync();
  }
  public async Task<dynamic> UpdateProductVariantAsync(ProductVariant productVariant)
  {
      _context.ProductVariants.Update(productVariant);
      return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> CreateProductVariantAsync(ProductVariant productVariant)
  {
      await _context.ProductVariants.AddAsync(productVariant);
      return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<InventoryBalance>> GetInventoryBalancesByProductIdAsync(ProductId productId)
  {
      var variantIds = await _context.ProductVariants.Where(x => x.ProductId == productId).Select(x => x.ProductVariantId).ToListAsync();
      return await _context.InventoryBalances.Where(x => variantIds.Contains(x.ProductVariantId)).ToListAsync();
  }
  public async Task<dynamic> UpdateInventoryBalanceAsync(InventoryBalance inventoryBalance)
  {
      _context.InventoryBalances.Update(inventoryBalance);
      return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> CreateInventoryBalanceAsync(InventoryBalance inventoryBalance)
  {
      await _context.InventoryBalances.AddAsync(inventoryBalance);
      return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> CreateProductVariantsAsync(List<ProductVariant> productVariants)
  {
      await _context.ProductVariants.AddRangeAsync(productVariants);
      return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> CreateInventoryBalancesAsync(List<InventoryBalance> inventoryBalances)
  {
      await _context.InventoryBalances.AddRangeAsync(inventoryBalances);
      return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> CreateInventoryTransactionsAsync(List<InventoryTransaction> inventoryTransactions)
  {
      await _context.InventoryTransactions.AddRangeAsync(inventoryTransactions);
      return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> CreateProductVariantOptionsAsync(List<ProductVariantOption> productVariantOptions)
  {
      await _context.ProductVariantOptions.AddRangeAsync(productVariantOptions);
      return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> CreateProductStockHistory(ProductStockHistory stockHistory)
  {
    _context.ProductStockHistories.Add(stockHistory);
    await _context.SaveChangesAsync();
    return stockHistory;
  }

  public async Task<dynamic> GetAllLowQuantityProductStock(string? storeId, int? productStationId, bool? isActive, int? availableQty, string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string query = @$"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY ib.CreatedOn)) AS RowNum,
                                             COUNT(*) OVER () AS TotalCount,
                                             ib.InventoryBalanceId AS ProductStockId,
                                             st.Name AS StationName,
                                             st.StationCode,
                                             p.ProductName,
                                             p.TrackInventory,
                                             p.SKU AS ProductSku,
                                             pv.SKU,
                                             COALESCE(
                                                (SELECT STRING_AGG(po.OptionValue, ' / ') 
                                                 FROM dbo.ProductVariantOption pvo 
                                                 INNER JOIN dbo.ProductOptions po ON po.ProductOptionsId = pvo.ProductOptionsId 
                                                 WHERE pvo.ProductVariantId = pv.ProductVariantId), 
                                                pv.VariantOptionText
                                             ) AS VarientOption,
                                             pv.LowQuantityLimit,
                                             ib.QuantityAvailable,
                                             pv.Price,
                                             ib.CreatedOn,
                                             ib.UpdatedOn,
                                             p.ProductId,
                                             p.FeatureImage,
                                             pv.Active,
                                             spAgg.StoreName
                                      FROM dbo.ProductVariant AS pv
                                          INNER JOIN dbo.InventoryBalance AS ib ON pv.ProductVariantId = ib.ProductVariantId
                                          INNER JOIN dbo.Product AS p
                                              ON pv.ProductId = p.ProductId
                                          CROSS APPLY (
                                                  SELECT 
                                                      STRING_AGG(CAST(s.StoreId AS NVARCHAR), ', ') AS StoreId,
                                                      STRING_AGG(
                                                          CASE 
                                                              WHEN s.IsDefault = 1 THEN s.StoreName + ' (Default)'
                                                              ELSE s.StoreName
                                                          END, ', '
                                                      ) AS StoreName
                                                  FROM dbo.StoreProduct AS sp
                                                  INNER JOIN dbo.Stores AS s ON s.StoreId = sp.StoreId
                                                  WHERE sp.ProductId = p.ProductId
                                                    AND sp.Active = 1
                                                   AND ( @storeId IS NULL OR @storeId = '' 
                                                         OR (sp.StoreId IN (SELECT value FROM STRING_SPLIT(@storeId,',')))
                                                        )
                                              ) AS spAgg
                                          INNER JOIN dbo.ProductStation st
                                              ON st.ProductStationId = ib.ProductStationId ";

      string whereStart = "WHERE ( 1=1 AND ISNULL(pv.LowQuantityLimit, 0) > 0 AND ISNULL(ib.QuantityAvailable, 0) <= pv.LowQuantityLimit ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);
      dynamicParams.Add("@storeId", storeId ?? "");

      if (!string.IsNullOrEmpty(storeId) && length > 0)
      {
        query = query + @" INNER JOIN dbo.StoreProduct AS sp  ON p.ProductId = sp.ProductId AND sp.Active =1
                           INNER JOIN dbo.Stores AS s ON sp.StoreId = s.StoreId ";

        whereStart += "And ( (sp.StoreId in (select value from STRING_SPLIT(@storeId,',')))) ";
      }

      if (productStationId != null && productStationId > 0)
      {
        dynamicParams.Add("@ProductStationId", productStationId);
        whereStart += " AND ib.ProductStationId = @ProductStationId";
      }
      if (isActive != null)
      {
        dynamicParams.Add("@Active", isActive);
        whereStart += " AND pv.Active = @Active";
      }
      if (availableQty != null)
      {
        whereStart += $" AND ib.QuantityAvailable  >= {availableQty} ";
      }
      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@Search", search);
        whereStart += @"
        AND (
            (pv.SKU IN (SELECT value FROM STRING_SPLIT(@Search, ',')) OR 
             p.SKU  IN (SELECT value FROM STRING_SPLIT(@Search, ',')))
            AND p.ClientId = @ClientId
        ) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " AND p.ClientId = @ClientId";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@CreatedOn", createdFrom);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("ib.CreatedOn", regionMinuts)} AS DATE) >= CAST(@CreatedOn AS DATE) )";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@CreatedTo", createdTo);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("ib.CreatedOn", regionMinuts)} AS DATE) <= CAST(@CreatedTo AS DATE))";
      }
      string where = whereStart + whereEnd;
      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "ib.InventoryBalanceId");

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

  public async Task<dynamic> GetAllProductInventoryAsync(string? storeId, int? productStationId, bool? isActive, bool? isAvailable, int? availableQty, string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      string query = @$"SELECT  ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY ib.CreatedOn)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               COALESCE(ib.InventoryBalanceId, pv.ProductVariantId * -1) AS ProductStockId,
                               p.ProductName,
                               p.TrackInventory,
                               st.Name AS StationName,
                               st.StationCode,
                               p.SKU AS ProductSku,
                               pv.SKU,
                               COALESCE(
                                  (SELECT STRING_AGG(po.OptionValue, ' / ') 
                                   FROM dbo.ProductVariantOption pvo 
                                   INNER JOIN dbo.ProductOptions po ON po.ProductOptionsId = pvo.ProductOptionsId 
                                   WHERE pvo.ProductVariantId = pv.ProductVariantId), 
                                  pv.VariantOptionText
                               ) AS VarientOption,
                               ib.QuantityIncoming AS QuantityInComing,
                               ib.QuantityCommitted AS QuantityCommited,
                               ib.QuantityDamaged AS QuantityDamage,
                               ib.QuantityAvailable,
                               ib.QuantityOnHand AS QuantityOnHand,
                               pv.LowQuantityLimit,
                               ib.QuantityOnOrder,
                               pv.Price,
                               ib.CreatedOn,
                               ib.UpdatedOn,
	                             p.ProductId,
                               p.FeatureImage,
                               pv.Active,
                               ISNULL((
                                   SELECT SUM(oi.Quantity)
                                   FROM dbo.OrderItem AS oi
                                       INNER JOIN dbo.[Order] AS s
                                           ON oi.OrderId = s.OrderId
                                   WHERE oi.ProductStockId = ib.InventoryBalanceId
                                         AND s.CarrierTrackingStatusId NOT IN (( SELECT value FROM STRING_SPLIT(CAST(ISNULL(( SELECT sgcs.DashboardStatusValue FROM dbo.ShipmentGridClientSetting AS sgcs WHERE sgcs.ShipmentGridColumnId =(SELECT sgc.ShipmentGridColumnId FROM dbo.ShipmentGridColumn AS sgc WHERE sgc.ColumnName = 'COMPLETED' AND sgc.ClientId = '{clientId}')), '') AS NVARCHAR(MAX)), ',')))
                                         AND s.FullFillmentStatusId = {(int)EnumFullfillmentStatus.Fulfilled}
                               ),0) AS TotalInTransitCount,
                               CASE
                                   WHEN pv.ProductVariantStatusId = {(int)EnumProductStockStatus.Disabled} THEN
                                       'Disabled'
                                   ELSE
                                       'Active'
                               END AS ProductStockStatus,
                                spAgg.StoreName
                            FROM dbo.ProductVariant AS pv
                                LEFT JOIN dbo.InventoryBalance AS ib ON pv.ProductVariantId = ib.ProductVariantId
                                INNER JOIN dbo.Product AS p
                                    ON pv.ProductId = p.ProductId
                                CROSS APPLY (
                                    SELECT 
                                        STRING_AGG(CAST(s.StoreId AS NVARCHAR), ', ') AS StoreId,
                                        STRING_AGG(
                                            CASE 
                                                WHEN s.IsDefault = 1 THEN s.StoreName + ' (Default)'
                                                ELSE s.StoreName
                                            END, ', '
                                        ) AS StoreName
                                    FROM dbo.StoreProduct AS sp
                                    INNER JOIN dbo.Stores AS s ON s.StoreId = sp.StoreId
                                    WHERE sp.ProductId = p.ProductId
                                      AND sp.Active = 1
                                   AND ( @storeId IS NULL OR @storeId = '' 
                                      OR (sp.StoreId IN (SELECT value FROM STRING_SPLIT(@storeId,',')))
                                    )
                                ) AS spAgg
                                LEFT JOIN dbo.ProductStation st
                                    ON st.ProductStationId = ib.ProductStationId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);
      dynamicParams.Add("@storeId", storeId ?? "");

      if (!string.IsNullOrEmpty(storeId) && length > 0)
      {
        query = query + @" INNER JOIN dbo.StoreProduct AS sp  ON p.ProductId = sp.ProductId AND sp.Active =1
                           INNER JOIN dbo.Stores AS s ON sp.StoreId = s.StoreId ";

        whereStart += "And ( (sp.StoreId in (select value from STRING_SPLIT(@storeId,',')))) ";
      }
      if (productStationId != null && productStationId > 0)
      {
        dynamicParams.Add("@ProductStationId", productStationId);
        whereStart += " AND ib.ProductStationId = @ProductStationId";
      }
      if (isActive != null)
      {
        dynamicParams.Add("@Active", isActive);
        whereStart += " AND pv.Active = @Active";
      }
      if (isAvailable.GetValueOrDefault())
      {
        whereStart += " AND ( ib.QuantityAvailable > 0) ";
      }
      else
      {
        whereStart += " AND ( ib.QuantityAvailable <= 0) "; 
      }
      if (availableQty.GetValueOrDefault() > 0)
      {
        whereStart += $" AND ib.QuantityAvailable  >= {availableQty} ";
      }
      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@Search", search);
        whereStart += @"
        AND (
            (pv.SKU IN (SELECT value FROM STRING_SPLIT(@Search, ',')) OR 
             p.SKU  IN (SELECT value FROM STRING_SPLIT(@Search, ',')))
            AND p.ClientId = @ClientId
        ) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " AND p.ClientId = @ClientId";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@CreatedOn", createdFrom);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("ib.CreatedOn", regionMinuts)} AS DATE) >= CAST(@CreatedOn AS DATE) )";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@CreatedTo", createdTo);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("ib.CreatedOn", regionMinuts)} AS DATE) <= CAST(@CreatedTo AS DATE))";
      }
      string where = whereStart + whereEnd;
      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "ib.InventoryBalanceId");


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
  public async Task<dynamic> GetAllProductInventorySummaryAsync(string? storeId, int? productStationId, bool? isActive, bool? isAvailable, int? availableQty, string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {
    var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      var groupBy = " GROUP BY p.ProductId,p.ProductName";
      string query = @"SELECT 
                    p.ProductName, 
                    COUNT(*) AS TotalSku,
                    SUM(ISNULL(ib.QuantityCommitted,0)) AS TotalQuantityCommited,
                    SUM(ISNULL(ib.QuantityIncoming,0)) AS TotalQuantityInComing, 
                    SUM(ISNULL(ib.QuantityAvailable,0)) AS TotalQuantityAvailable,
                    SUM(ISNULL(ib.QuantityOnOrder,0)) AS TotalQuantityOnOrder,
                    SUM(ISNULL(pv.Price,0)) AS TotalPrice
                     FROM dbo.Product AS p
                    LEFT JOIN dbo.ProductVariant AS pv ON pv.ProductId = p.ProductId
                    LEFT JOIN dbo.InventoryBalance AS ib ON ib.ProductVariantId = pv.ProductVariantId   ";

      string whereStart = "WHERE ( 1=1 AND p.Active = 1 AND pv.Active = 1 ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(storeId) && length > 0)
      {
        query = query + @" INNER JOIN dbo.StoreProduct AS sp  ON p.ProductId = sp.ProductId AND sp.Active =1
                           INNER JOIN dbo.Stores AS s ON sp.StoreId = s.StoreId ";

        dynamicParams.Add("@storeId", storeId);
        whereStart += "And ( (sp.StoreId in (select value from STRING_SPLIT(@storeId,',')))) ";
      }
      if (productStationId != null && productStationId > 0)
      {
        dynamicParams.Add("@ProductStationId", productStationId);
        whereStart += " AND ib.ProductStationId = @ProductStationId";
      }
      if (isActive != null)
      {
        dynamicParams.Add("@Active", isActive);
        whereStart += " AND pv.Active = @Active";
      }
      if (isAvailable.GetValueOrDefault())
      {
        whereStart += " AND (ISNULL(p.TrackInventory, 0) = 0 OR ib.QuantityAvailable > 0) ";
      }
      if (availableQty != null)
      {
        whereStart += $" AND ib.QuantityAvailable  >= {availableQty} ";
      }
      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @" And ( ( pv.SKU in (select value from STRING_SPLIT(@Search,',')))) 
                          OR ( ( p.SKU in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " AND p.ClientId = @ClientId";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@CreatedOn", createdFrom);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("ib.CreatedOn", regionMinuts)} AS DATE) >= CAST(@CreatedOn AS DATE) )";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@CreatedTo", createdTo);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("ib.CreatedOn", regionMinuts)} AS DATE) <= CAST(@CreatedTo AS DATE))";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where + " " + groupBy;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }

  public async Task<dynamic> GetProductStockHistoryByStockIdAsync(long ProductStockId, int ReasonId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY psh.CreatedOn)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               psh.InventoryTransactionId AS ProductStockHistorytId,
                               psh.ProductVariantId AS ProductStockId,
                               psh.TransactionTypeId AS ReasonId,
                               lar.Name AS Reason,
                               ps.SKU,
                               psh.PreviousQuantity,
                               psh.NewQuantity,
                               psh.CreatedOn,
                               psh.Comment,
 	                             spAgg.StoreName
                        FROM dbo.InventoryTransaction AS psh
                            LEFT JOIN dbo.InventoryTransactionTypeLookup AS lar
                                ON lar.InventoryTransactionTypeId = psh.TransactionTypeId
                            LEFT JOIN dbo.ProductVariant AS ps
                                ON psh.ProductVariantId = ps.ProductVariantId
                            LEFT JOIN dbo.Product AS p
                                ON p.ProductId = ps.ProductId
                            CROSS APPLY (
                                    SELECT 
                                        STRING_AGG(CAST(s.StoreId AS NVARCHAR), ', ') AS StoreId,
                                        STRING_AGG(
                                            CASE 
                                                WHEN s.IsDefault = 1 THEN s.StoreName + ' (Default)'
                                                ELSE s.StoreName
                                            END, ', '
                                        ) AS StoreName
                                    FROM dbo.StoreProduct AS sp
                                    INNER JOIN dbo.Stores AS s ON s.StoreId = sp.StoreId
                                    WHERE sp.ProductId = p.ProductId
                                      AND sp.Active = 1
                                      AND ( @storeId IS NULL OR @storeId = '' 
                                      OR (sp.StoreId IN (SELECT value FROM STRING_SPLIT(@storeId,',')))
                                    )
                                ) AS spAgg ";


      string whereStart = "WHERE ( 1=1 AND p.Active = 1 AND ps.Active = 1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);
      dynamicParams.Add("@storeId", "");


      if (ProductStockId > 0)
      {
        var balance = await _context.InventoryBalances.FindAsync(ProductStockId);
        if (balance != null)
        {
          dynamicParams.Add("@ProductVariantId", balance.ProductVariantId);
          dynamicParams.Add("@ProductStationId", balance.ProductStationId);
          whereStart += "And (psh.ProductVariantId = @ProductVariantId AND psh.ProductStationId = @ProductStationId) ";
        }
        else
        {
          dynamicParams.Add("@ProductVariantId", ProductStockId);
          whereStart += "And (psh.ProductVariantId = @ProductVariantId) ";
        }
      }
      if (ReasonId > 0)
      {
        dynamicParams.Add("@ReasonId", ReasonId);
        whereStart += "And (psh.TransactionTypeId = @ReasonId) ";
      }
      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( ps.SKU in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("psh.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("psh.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "psh.ProductStockHistorytId");

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
  public async Task<dynamic> GetProductStockCountAvailability(string productId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string v1, int sortCol, string sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      var groupBy = " GROUP BY pv.SKU";
      string query = @"SELECT
                      pv.SKU, 
                      COUNT(pv.SKU) AS ProductCount,
                      SUM(ib.QuantityAvailable) AS TotalAvailableQuantity 
                      FROM dbo.InventoryBalance AS ib
                      INNER JOIN dbo.ProductVariant AS pv ON pv.ProductVariantId = ib.ProductVariantId";
      if (!string.IsNullOrEmpty(productId))
      {
        query += $" WHERE pv.ProductId = '{productId}'";
        dynamicParams.Add("ProductId", productId);
      }
      //if (createdFrom != null)
      //{
      //  query += $"And ( CAST(ps.CreatedOn AS DATE) = CAST(ps.CreatedOn AS DATE))";
      //  dynamicParams.Add("CreatedOn", createdFrom);
      //}
      if (!string.IsNullOrEmpty(groupBy))
      {
        query += groupBy;
      }
      var data = await connection.QueryAsync(query, dynamicParams);
      return data.ToList();
    }
  }

  public async Task<bool> CheckUniqueProductStockSKU(string sku, string clientId)
  {
    bool isExist = false;
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT pv.SKU,* FROM dbo.Product AS p
                       INNER JOIN dbo.ProductVariant AS pv ON pv.ProductId = p.ProductId
                      WHERE 1=1 ";

      if (!string.IsNullOrEmpty(sku))
      {
        dynamicParams.Add("@Sku", sku);
        query += "And (pv.SKU = @Sku) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        query += "And (p.ClientId = @ClientId) ";
      }

      var data = await connection.QueryAsync(query, dynamicParams);
      if (data.Count() > 0)
      {
        isExist = true;
      }
      return isExist;
    }
  }
  public async Task<dynamic?> GetProductStockPriceByProductStockId(long? productStockId)
  {
    var balance = await _context.InventoryBalances.FindAsync(productStockId);
    if (balance != null)
    {
      return await _context.ProductVariants.FirstOrDefaultAsync(x => x.ProductVariantId == balance.ProductVariantId);
    }
    return null;
  }

  public async Task<dynamic?> GetProductStocksForSelection(string clientId, int storeId, int stationId = 0, string? search = null, long? productVariantId = null, long? inventoryBalanceId = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("StoreId", storeId > 0 ? storeId : (int?)null);

      string query = @"SELECT CAST(p.ProductId AS NVARCHAR(40)) AS ProductId,
                              p.ProductName,
                              p.TrackInventory,
                              spAgg.StoreName,
                              spAgg.StoreId,
                              ISNULL(ig.ImageUrl,p.FeatureImage) AS FeatureImage,
                              pv.SKU,
                              CASE
                                  WHEN p.HaveOptions = 1 THEN
                                      pv.SKU + ' | ' + COALESCE(
                                         (SELECT STRING_AGG(po.OptionValue, ' / ') 
                                          FROM dbo.ProductVariantOption pvo 
                                          INNER JOIN dbo.ProductOptions po ON po.ProductOptionsId = pvo.ProductOptionsId 
                                          WHERE pvo.ProductVariantId = pv.ProductVariantId), 
                                         pv.VariantOptionText
                                      )
                                  ELSE
                                      pv.SKU
                              END AS SKUOption,
                              p.HaveOptions,
                              COALESCE(
                                 (SELECT STRING_AGG(po.OptionValue, ' / ') 
                                  FROM dbo.ProductVariantOption pvo 
                                  INNER JOIN dbo.ProductOptions po ON po.ProductOptionsId = pvo.ProductOptionsId 
                                  WHERE pvo.ProductVariantId = pv.ProductVariantId), 
                                 pv.VariantOptionText
                              ) AS VarientOption,
                              pv.ProductVariantId AS ProductVariantId,
                              ib.InventoryBalanceId AS InventoryBalanceId,
                              pv.Price,
                              ISNULL(ib.QuantityAvailable, 0) AS QuantityAvailable,
                              ISNULL(ib.QuantityCommitted, 0) AS QuantityCommited,
                              ib.ProductStationId,
                              'AED' AS Currency,
                              ISNULL(p.Weight, 0) AS Weight,
                              ps2.[Name] AS Station
                        FROM dbo.ProductVariant AS pv
                            LEFT JOIN dbo.InventoryBalance AS ib
                                ON pv.ProductVariantId = ib.ProductVariantId
                            LEFT JOIN dbo.ImageGallery AS ig
                                ON ig.ImageGalleryId = pv.ImageGalleryId
                            INNER JOIN dbo.Product AS p
                                ON pv.ProductId = p.ProductId
                            LEFT JOIN dbo.ProductStation AS ps2
                                ON ps2.ProductStationId = ib.ProductStationId 
                            CROSS APPLY (
                                  SELECT 
                                      STRING_AGG(CAST(s.StoreId AS NVARCHAR), ', ') AS StoreId,
                                      STRING_AGG(
                                          CASE 
                                              WHEN s.IsDefault = 1 THEN s.StoreName + ' (Default)'
                                              ELSE s.StoreName
                                          END, ', '
                                      ) AS StoreName
                                  FROM dbo.StoreProduct AS sp
                                  INNER JOIN dbo.Stores AS s ON s.StoreId = sp.StoreId
                                  WHERE sp.ProductId = p.ProductId
                                    AND sp.Active = 1
                                    AND (@StoreId IS NULL OR s.StoreId = @StoreId)
                              ) AS spAgg ";

      string whereStart = "WHERE (1=1  AND p.Active = 1 AND pv.Active = 1  ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( p.SKU in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      if (inventoryBalanceId.GetValueOrDefault() > 0)
      {
        dynamicParams.Add("@inventoryBalanceId", inventoryBalanceId);
        whereStart += " AND ib.InventoryBalanceId = @inventoryBalanceId ";
      }
      else if (productVariantId.GetValueOrDefault() > 0)
      {
        dynamicParams.Add("@productVariantId", productVariantId);
        whereStart += " AND pv.ProductVariantId = @productVariantId ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " AND p.ClientId = @ClientId ";
      }
      if (storeId > 0)
      {
        query = query + @" INNER JOIN dbo.StoreProduct AS sp  ON p.ProductId = sp.ProductId AND sp.Active =1
                           INNER JOIN dbo.Stores AS s ON sp.StoreId = s.StoreId ";
        whereStart += " AND sp.StoreId = @StoreId ";  // 👈 add filter
      }
      if (stationId > 0)
      {
        whereStart += " AND ib.ProductStationId = @ProductStationId ";
        dynamicParams.Add("ProductStationId", stationId);
      }
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }
  public async Task<ProductStock?> GetProductStockBySKUAsync(string sku)
  {
    var result = await _context.ProductStocks.FirstOrDefaultAsync(p => p.Sku!.Trim().ToLower() == sku.Trim().ToLower());
    return result;
  }
  public async Task<List<dynamic>> GetVariantsWithoutStationAsync(string clientId)
  {
      using (var con = _dapperAppDbContext.CreateConnectionByClient(clientId))
      {
          var query = @"
            SELECT 
                pv.ProductVariantId, 
                pv.SKU, 
                pv.VariantOptionText, 
                p.ProductName 
            FROM ProductVariant pv
            JOIN Product p ON pv.ProductId = p.ProductId
            LEFT JOIN InventoryBalance ib ON pv.ProductVariantId = ib.ProductVariantId
            WHERE ib.InventoryBalanceId IS NULL 
              AND pv.ClientId = @ClientId 
              AND pv.Active = 1
              AND p.Active = 1 ";

          var result = await con.QueryAsync<dynamic>(query, new { ClientId = clientId });
          return result.ToList();
      }
  }


  public async Task<List<LookupProductStockStatus>> GetLookupProductStockStatusForSelectionQuery()
  {
    return await _context.LookupProductStockStatuses.ToListAsync();
  }
  public async Task<ProductOption> CheckUniqueOptionNameByOptionIdNameAndProductId(string? optionId, ProductId productId, string? optionValue)
  {
    int? opId = int.TryParse(optionId, out int parsedValue) ? parsedValue : null;


    var target = await _context.ProductOptions.FirstOrDefaultAsync(x => x.OptionId == opId && x.ProductId == productId && x.OptionValue!.Trim() == optionValue);
    return target!;
  }

  public async Task<bool> CheckAutogeneratedUniqueProductSKU(string? clientId, string generatedSku)
  {
    bool isExist = false;
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT p.SKU FROM dbo.Product AS p
                     WHERE 1=1 ";

      if (!string.IsNullOrEmpty(generatedSku))
      {
        dynamicParams.Add("@Sku", generatedSku);
        query += "And (p.SKU = @Sku) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        query += "And (p.ClientId = @ClientId) ";
      }

      var data = await connection.QueryAsync(query, dynamicParams);
      if (data.Count() > 0)
      {
        isExist = true;
      }
      return isExist;
    }

  }

  public async Task<ProductVariant?> GetProductVariantBySKUAsync(string sku)
  {
    return await _context.ProductVariants.FirstOrDefaultAsync(x => x.SKU != null && x.SKU.Trim().ToLower() == sku.Trim().ToLower());
  }

  public async Task<ProductVariant?> GetProductVariantByIdAsync(long productVariantId)
  {
    return await _context.ProductVariants.FirstOrDefaultAsync(x => x.ProductVariantId == productVariantId);
  }

  public async Task<Core.ProductAggregate.InventoryBalance?> GetInventoryBalanceByVariantIdAsync(long variantId)
  {
    return await _context.InventoryBalances.FirstOrDefaultAsync(x => x.ProductVariantId == variantId);
  }

  public async Task<Core.ProductAggregate.InventoryBalance?> GetInventoryBalanceByIdAsync(long inventoryBalanceId)
  {
    return await _context.InventoryBalances.FirstOrDefaultAsync(x => x.InventoryBalanceId == inventoryBalanceId);
  }
  public async Task<Core.ProductAggregate.InventoryBalance?> GetInventoryBalanceByVariantAndStationAsync(long variantId, int stationId)
  {
    return await _context.InventoryBalances.FirstOrDefaultAsync(x => x.ProductVariantId == variantId && x.ProductStationId == stationId);
  }

  public async Task<List<dynamic>?> GetAllProductStocksForSaleChannelInventorySync(string? clientId, string? productSKUs)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT  pv.SKU,
                               ISNULL(ib.QuantityAvailable, 0) AS QuantityAvailable,
                               CAST(p.ProductId AS NVARCHAR(40)) AS ProductId,
                               pv.ProductVariantId,
                               pv.SaleChannelVariantId
                         FROM dbo.ProductVariant AS pv
                             INNER JOIN dbo.Product AS p
                                 ON p.ProductId = pv.ProductId 
                             LEFT JOIN dbo.InventoryBalance AS ib
                                 ON ib.ProductVariantId = pv.ProductVariantId ";

      string whereStart = "WHERE ( 1=1 AND p.Active = 1 AND pv.Active = 1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(productSKUs))
      {
        dynamicParams.Add("@ProductSKUs", productSKUs);
        whereStart += @" And ( ( pv.SKU in (select value from STRING_SPLIT(@ProductSKUs,',')))) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.AsList();
    }
  }

  public async Task<string?> GetProductNameByProductIdAsync(ProductId productId)
  {
    return await _context.Products
        .Where(x => x.ProductId == productId)
        .Select(x => x.ProductName)
        .FirstOrDefaultAsync();
  }
  #region payment link token 
  public async Task<ProductLinkToken?> GetProductLinkTokenByProductId(ProductId productId, int? storeId, ClientId clientId)
  {
    var data = await _context.ProductLinkTokens.FirstOrDefaultAsync(x => x.ProductId == productId && x.StoreId == storeId && x.ClientId == clientId);
    return data;
  }
  public async Task<List<ProductLinkToken>?> GetAllProductLinkTokenByProductId(ProductId productId, ClientId clientId)
  {
    var data = await _context.ProductLinkTokens.Where(x => x.ProductId == productId && x.ClientId == clientId).ToListAsync();
    return data;
  }

  public async Task<bool> CreateProductLinkToken(ProductLinkToken productLink)
  {
    await _context.ProductLinkTokens.AddAsync(productLink);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> UpdateProductLinkToken(ProductLinkToken productLink)
  {
    _context.ProductLinkTokens.Update(productLink);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<string> GenerateUniqueShortTokenForProductAsync(ClientId clientId, int length = 6)
  {
    string token;
    bool exists;
    int databaseId = 0;
    var oCatalogue = await _catalogueRepository.GetCatalogByClientId(clientId.Value!.ToString());
    if (oCatalogue != null)
    {
      databaseId = oCatalogue.DatabaseId.GetValueOrDefault();
    }
    do
    {
      token = $"{databaseId}{ProductLinkToken.GenerateShortToken(length)}";
      exists = await _context.ProductLinkTokens.AnyAsync(x => x.Token == token && x.ClientId == clientId);

    } while (exists);

    return token;
  }
  #endregion
  #region image gallery
  public async Task<bool> CreateProductMedia(ProductMedia model)
  {
    await _context.ProductMedias.AddAsync(model);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<List<dynamic>?> GetAllProductMedia(string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT pm.ProductMediaId,
                               ig.ImageUrl,
	                           ig.MediaTypeId,
	                           pm.IsFeatured
                        FROM dbo.ImageGallery AS ig
                            INNER JOIN dbo.ProductMedia AS pm
                                ON pm.ImageGalleryId = ig.ImageGalleryId
                            INNER JOIN dbo.Product AS p 
                                ON P.ProductId = pm.ProductId
                            INNER JOIN dbo.Client AS c
                                ON c.ClientId = ig.ClientId ";

      string whereStart = "WHERE ( 1=1 AND p.Active =1 AND ig.Active = 1 AND c.Active AND pm.Active ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (ig.ClientId = @ClientId) ";
      }
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.AsList();
    }
  }

  public async Task<bool> CreateImageGallery(ImageGallery model)
  {
    await _context.ImageGalleries.AddAsync(model);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> DeleteProductMediaByID(ProductMedia productMedia)
  {
    _context.ProductMedias.Remove(productMedia);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<ImageGallery>> GetAllImageGalleries(ClientId clientId)
  {
    return await _context.ImageGalleries.Where(x => x.ClientId == clientId).ToListAsync();
  }
  public async Task<ProductMedia> GetProductMediaById(long productMediaId)
  {
    var data = await _context.ProductMedias.FirstOrDefaultAsync(x => x.ProductMediaId == productMediaId)!;
    return data!;
  }
  public async Task<List<ProductMedia>> GetProductMediaByProductId(ProductId roductId)
  {
    var data = await _context.ProductMedias.Where(x => x.ProductId == roductId).ToListAsync();
    return data!;
  }
  public async Task<bool> UpdateProductMedia(ProductMedia productMedia)
  {
    _context.ProductMedias.Update(productMedia);
    return await _context.SaveChangesAsync() > 0;
  }

  #endregion
}
