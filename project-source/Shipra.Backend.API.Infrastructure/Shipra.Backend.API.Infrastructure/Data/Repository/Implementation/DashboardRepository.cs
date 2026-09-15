using System.Data;
using System.Dynamic;
using Dapper;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class DashboardRepository : IDashboardRepository
{
  private readonly ICommonLookupRepository _commonLookupRepository;
  private readonly AppDbContext _dbContext;
  private readonly DapperAppDbContext _dapperAppDbContext;
  public DashboardRepository(ICommonLookupRepository commonLookupRepository, AppDbContext dbContext, DapperAppDbContext dapperAppDbContext)
  {
    _commonLookupRepository = commonLookupRepository;
    _dbContext = dbContext;
    _dapperAppDbContext = dapperAppDbContext;

  }
  public async Task<dynamic> GetToBeShippedCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(o.OrderId) ";
      TotalCount += @"FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1 AND o.CarrierId IS NULL ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }


      #region ToBeShipped 
      whereStart += $"And (o.FullFillmentStatusId = {(int)EnumFullfillmentStatus.Fulfilled}) ";
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }
  public async Task<dynamic> GetToBePackedCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(o.OrderId) ";
      TotalCount += @"FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }


      #region unpacked 
      whereStart += $"And (o.FullFillmentStatusId = {(int)EnumFullfillmentStatus.Unfulfilled}) ";
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }

  public async Task<dynamic> GetAllItemCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string Total = "SELECT COUNT(*) ";
      Total += @"FROM dbo.[Product] AS p ";
      string whereStart = "WHERE ( 1=1 AND  p.Active = 1 ";
      string whereEnd = ")";


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
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("p.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = Total + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }
  }

  public async Task<dynamic> GetTopSellingItemsCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT TOP (5) p.ProductName,p.SKU,SUM(oi.Quantity) AS Quantity,SUM(oi.Price) AS Revenue FROM dbo.OrderItem AS oi
                          INNER JOIN dbo.Product AS p ON p.ProductId = oi.ProductId ";
      string whereStart = "WHERE ( 1=1 AND  p.Active = 1 ";
      string whereEnd = ")";


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
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("p.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where + " GROUP BY p.ProductName,p.SKU " + " ORDER BY p.SKU DESC ";

      var count = await connection.QueryAsync(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result = count;
      return result;

    }
  }

  public async Task<dynamic> GetDeliveryRatioCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      //var clientQuery = !string.IsNullOrEmpty(clientId) ? $"AND o2.ClientId = '{clientId}'" : "";
      //string condition = @$"(
      //                      SELECT COUNT(o2.OrderId) AS Total
      //                      FROM dbo.[Order] AS o2
      //                          WHERE o2.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered} 
      //                          AND (CAST(o2.CreatedOn AS DATE) >= CAST('{createdFrom}' AS DATE))
      //                          AND (CAST(o2.CreatedOn AS DATE) <= CAST('{createdTo}' AS DATE))
      //                          {clientQuery}
      //                      ) X
      //                      JOIN
      //                      (
      //                       SELECT COUNT(o2.OrderId) AS Total
      //                       FROM dbo.[Order] AS o2
      //                          WHERE (CAST(o2.CreatedOn AS DATE) >= CAST('{createdFrom}' AS DATE))
      //                          AND (CAST(o2.CreatedOn AS DATE) <= CAST('{createdTo}' AS DATE))
      //                          {clientQuery}
      //                      ) Y";

      //string query = @$"SELECT COALESCE((CAST(CAST(X.Total AS DECIMAL(18, 2)) / CAST(NULLIF(Y.Total,0) AS DECIMAL(18, 2)) AS DECIMAL(18, 4)) * 100),0) AS Result
      //                      FROM
      //                      {condition} ON 1 = 1; ";



      string TotalCount = $@"SELECT  
                             CASE
                                 WHEN COUNT(*) > 0 THEN
                                     CAST(CAST(COUNT(   CASE
                                                       WHEN o.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered} THEN
                                                           1
                                                   END
                                               ) AS decimal(10,2)) / COUNT(*) * 100 AS decimal(10,4))
                                 ELSE
                                     0
                             END
                          FROM dbo.[Order] AS o ";
      string whereStart = $"WHERE ( 1=1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<decimal>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }
  }

  public async Task<dynamic> GetTotalActiveCarrierCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT COUNT(ac.ActiveCarrierId) AS TotalCount FROM dbo.ActiveCarrier AS ac ";
      string whereStart = $"WHERE ( 1=1 AND  ac.Active = 1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (ac.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ac.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ac.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }
  }

  public async Task<dynamic> GetAllProductVariantsCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT COUNT(ps.ProductStockId) AS TotalCount  FROM dbo.ProductStock AS ps  
                          inner JOIN dbo.Product AS p ON p.ProductId = ps.ProductId ";
      string whereStart = $"WHERE ( 1=1 AND  ps.Active = 1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ps.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ps.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }

  }

  public async Task<dynamic> GetLowStockItemsCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT COUNT(ib.InventoryBalanceId) AS TotalCount  FROM dbo.InventoryBalance AS ib  
                          INNER JOIN dbo.ProductVariant AS pv ON pv.ProductVariantId = ib.ProductVariantId
                          INNER JOIN dbo.Product AS p ON p.ProductId = pv.ProductId ";
      string whereStart = $"WHERE ( 1=1 AND  pv.Active = 1 AND ISNULL(pv.LowQuantityLimit, 0) > 0 AND ISNULL(ib.QuantityAvailable, 0) <= pv.LowQuantityLimit ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ib.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ib.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }

  }
  public async Task<dynamic> GetTotalStoreCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT COUNT(s.StoreId) AS TotalCount FROM dbo.Stores AS s ";
      string whereStart = $"WHERE ( 1=1 AND  s.Active = 1 ";
      string whereEnd = ")";


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
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("s.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }

  }
  public async Task<dynamic> GetTotalCollected(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT SUM(o.Amount) AS TotalCollected FROM dbo.[Order] AS o ";
      string whereStart = $"WHERE ( 1=1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }

      #region cod related filter
      whereStart += $"And (o.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered}) ";
      whereStart += $"And (o.CarrierPaymentSettlementId IS NOT NULL) ";
      whereStart += $"And (o.PaymentStatusId = {(int)EnumPaymentStatus.Paid}) ";
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }

  }
  public async Task<dynamic> GetTotalUncollected(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT SUM(o.Amount) AS TotalUncollected FROM dbo.[Order] AS o ";
      string whereStart = $"WHERE ( 1=1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }

      #region cod related filter
      whereStart += $"And (o.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered}) ";
      whereStart += $"And (o.CarrierPaymentSettlementId IS NULL) ";
      whereStart += $"And (o.PaymentStatusId != {(int)EnumPaymentStatus.Paid}) ";
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }

  }
  public async Task<dynamic> GetTotalStockValue(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT SUM(ps.QuantityAvailable * ps.Price) AS TotalStockValue
                            FROM dbo.ProductStock AS ps INNER JOIN dbo.Product AS p ON p.ProductId = ps.ProductId ";
      string whereStart = $"WHERE ( 1=1 AND  ps.Active = 1 AND  p.Active = 1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (p.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ps.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("ps.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<decimal>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }

  }
  public async Task<dynamic> GetTotalPurchaseStockValue(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT SUM(p.QuantityAvailable * p.PurchasePrice) AS TotalStockValue
                            FROM  dbo.Product AS p  ";
      string whereStart = $"WHERE ( 1=1 AND  p.Active = 1 ";
      string whereEnd = ")";


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
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("p.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<decimal>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }

  }

  #region CarrierActivity
  public async Task<dynamic> GetTotalCompletedOrderCountWithCarrier(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(o.OrderId) As TotalCompletedOrders ";
      TotalCount += @"FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1  ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }


      #region completedorder

      whereStart += $"And (o.CarrierRRId IS NULL)  ";
      whereStart += $"And (o.CarrierPaymentSettlementId IS NOT NULL) ";

      whereStart += $"And (o.CarrierId IS NOT NULL)";
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }
  public async Task<dynamic> GetInProgressOrderCountWithCarrier(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(o.OrderId) As TotalInProgressOrders ";
      TotalCount += @"FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1  ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }


      #region inprogressorder

      whereStart += $"And (o.CarrierRRId IS NULL)  ";
      whereStart += $"And (o.CarrierPaymentSettlementId IS NULL) ";

      whereStart += $"And (o.CarrierId IS NOT NULL)";
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }
  #endregion

  #region orderactivity
  public async Task<dynamic> GetReturnedOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(o.OrderId) As TotalReturnedOrders ";
      TotalCount += @"FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1  ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }

      #endregion

      #region returnedreport

      whereStart += $"And (o.CarrierRRId IS NOT NULL)  ";

      whereStart += $"And (o.CarrierId IS NOT NULL) ";
      #endregion

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }

  public async Task<dynamic> GetInProgressOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(o.OrderId) AS InProgressOrders ";
      TotalCount += @"FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }


      #region inprogress 
      whereStart += $"And (o.CarrierTrackingStatusId != {(int)EnumCarrierTrackingStatus.ReturnToOrigin}) ";
      whereStart += $"And (o.CarrierTrackingStatusId != {(int)EnumCarrierTrackingStatus.Delivered}) ";
      #endregion
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }

  public async Task<dynamic> GetOrdersCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = "Select COUNT(o.OrderId) AS InProgressOrders ";
      TotalCount += @"FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }


      #region inprogress 
      whereStart += $"And (o.CarrierTrackingStatusId != {(int)EnumCarrierTrackingStatus.OrderPlaced}) ";

      #endregion
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }
  public async Task<dynamic> GetRegularOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT COUNT(o.OrderId) AS TotalCount FROM dbo.[Order] AS o ";
      string whereStart = $"WHERE ( 1=1 AND  o.OrderTypeId = {(int)EnumOrderType.Regular} ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }
  }
  public async Task<dynamic> GetFulfillableOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"SELECT COUNT(o.OrderId) AS TotalCount FROM dbo.[Order] AS o ";
      string whereStart = $"WHERE ( 1=1 AND  o.OrderTypeId = {(int)EnumOrderType.FullFilable} ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
        string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;

    }
  }

  public async Task<dynamic> GetDelieveredOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @$"Select COUNT(o.OrderId) As DelieveredOrders ";
      TotalCount += @"FROM dbo.[Order] AS o ";

      string whereStart = $"WHERE ( 1=1 AND (o.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered}) ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }


      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<long>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }
  public async Task<dynamic> GetCarrierActivityWithDetail(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();
      string inProgress = $"{(int)EnumCarrierTrackingStatus.Delivered},{(int)EnumCarrierTrackingStatus.ReturnToOrigin}";

      string query = @$"SELECT c.Name AS CarrierName,
       COUNT(o.OrderId) AS TotalOrder, 
       InProgress = COUNT(   CASE
                                 WHEN o.CarrierTrackingStatusId NOT IN ( {inProgress} ) THEN
                                     0
                             END
                         ),
       Delivered = COUNT(   CASE
                                WHEN o.CarrierTrackingStatusId IN ( {(int)EnumCarrierTrackingStatus.Delivered} ) THEN
                                    0
                            END
                        ),
       CODPending = COUNT(   CASE
                                 WHEN o.CarrierTrackingStatusId IN ( {(int)EnumCarrierTrackingStatus.Delivered} ) 
                                  AND o.CarrierPaymentSettlementId IS NULL
                                  AND o.CarrierRRId IS NULL
                                  AND o.PaymentStatusId != {(int)EnumPaymentStatus.Paid} THEN
                                     0
                             END
                         ),
       CODSettled = COUNT(   CASE
                                 WHEN o.CarrierTrackingStatusId IN ( {(int)EnumCarrierTrackingStatus.Delivered} ) 
                                  AND o.CarrierPaymentSettlementId IS NOT NULL
                                  AND o.PaymentStatusId = {(int)EnumPaymentStatus.Paid} THEN
                                     0
                             END
                         ),
       DeliveryRatio = (   CASE
                            WHEN COUNT(o.OrderId) > 0 THEN 
                                CAST(CAST(COUNT(CASE WHEN o.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered} THEN 1 END) AS FLOAT) / COUNT(o.OrderId) * 100 as decimal(10,4))
                              ELSE 0
                             END
                         )  ";
      query += @"FROM dbo.[Order] AS o
    INNER JOIN dbo.Carrier AS c
        ON c.CarrierId = o.CarrierId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }

      string groupBy = " GROUP BY c.Name ";
      string where = whereStart + whereEnd + groupBy;
      string queryForCount = query + where;

      var data = await connection.QueryAsync(queryForCount, dynamicParams);
      return data;
    }
  }
  public async Task<dynamic> GetCarrierStates(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var completedStatus = await _commonLookupRepository.GetCompletedShipmentGridSetting(clientId);
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);

      var dynamicParams = new DynamicParameters();
      // Calculate start and end dates 
      if (createdFrom == null)
      {
        createdFrom = DateTime.UtcNow.AddDays(-30);
      }
      if (createdTo == null)
      {
        createdTo = DateTime.UtcNow;
      }

      createdFrom = createdFrom.Value.AddMinutes(regionMinuts);
      createdTo = createdTo.Value.AddMinutes(regionMinuts);

      // Calculate the day difference
      int dayDifference = (createdTo - createdFrom)?.Days ?? 0;
      if (dayDifference > 30)
      {
        dayDifference = 30;
      }
      DateTime? endDate = createdTo.Value.Date;
      DateTime? startDate = endDate?.AddDays(-dayDifference).Date;

      string query = @$"WITH Dates
                          AS (SELECT @StartDate AS [Date]
                              UNION ALL
                              SELECT DATEADD(DAY, 1, [Date])
                              FROM Dates
                              WHERE DATEADD(DAY, 1, [Date]) <= @EndDate)
                              SELECT d.[Date],
                                     o.CarrierId,
                                     c.Name AS CarrierName,
                                     COUNT(o.OrderId) AS TotalOrder,
                                     InProgress = SUM(   CASE
                                                             WHEN o.CarrierTrackingStatusId NOT IN ( {completedStatus} )
                                                                  AND o.TrackingLock <> 1 THEN
                                                                 1
                                                             ELSE
                                                                 0
                                                         END
                                                     ),
                                     Delivered = SUM(   CASE
                                                            WHEN o.CarrierTrackingStatusId IN ( {completedStatus} )
                                                                 AND o.TrackingLock = 1 THEN
                                                                1
                                                            ELSE
                                                                0
                                                        END
                                                    ),
                                     CODPending = SUM(   CASE
                                                             WHEN o.CarrierPaymentSettlementId IS NULL
                                                                  AND o.CarrierRRId IS NULL
                                                                  AND o.PaymentStatusId <> {(int)EnumPaymentStatus.Paid} THEN
                                                                 1
                                                             ELSE
                                                                 0
                                                         END
                                                     ),
                                     CODSettled = SUM(   CASE
                                                             WHEN o.CarrierPaymentSettlementId IS NOT NULL
                                                                  AND o.PaymentStatusId = {(int)EnumPaymentStatus.Paid} THEN
                                                                 1
                                                             ELSE
                                                                 0
                                                         END
                                                     ),
                                     DeliveryRatio = (CASE
                                                          WHEN COUNT(o.OrderId) > 0 THEN
                                                              CAST((CAST(((SUM(   CASE
                                                                                      WHEN o.CarrierTrackingStatusId NOT IN ( {completedStatus} )
                                                                                           AND o.TrackingLock <> 1 THEN
                                                                                          1
                                                                                      ELSE
                                                                                          0
                                                                                  END
                                                                              )
                                                                          ) + (SUM(   CASE
                                                                                          WHEN o.CarrierTrackingStatusId IN ( {completedStatus} )
                                                                                               AND o.TrackingLock = 1 THEN
                                                                                              1
                                                                                          ELSE
                                                                                              0
                                                                                      END
                                                                                  )
                                                                              )
                                                                         ) AS DECIMAL(10, 2)) / COUNT(o.OrderId) * 100
                                                                   ) AS DECIMAL(10, 2))
                                                          ELSE
                                                              0
                                                      END
                                                     ) ";
      query += @" FROM dbo.[Order] AS o
                        INNER JOIN dbo.Carrier AS c
                            ON c.CarrierId = o.CarrierId
                        INNER JOIN Dates AS d
                            ON d.[Date] >= CAST(o.CreatedOn AS DATE)
                               AND d.[Date] <= CAST(o.CreatedOn AS DATE) ";
      string whereStart = @$"WHERE ( 1=1 AND o.ClientId = '{clientId}' ";
      string whereEnd = ")";

      string groupBy = " GROUP BY d.[Date], c.Name,o.CarrierId ";
      string where = whereStart + whereEnd + groupBy + " ORDER BY d.[Date] ";
      string queryForCount = query + where;

      var data = await connection.QueryAsync(queryForCount, new { StartDate = startDate, EndDate = endDate });

      return data;
    }
  }
  public async Task<dynamic> GetCarrierStatesStackedChart(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var completedStatus = await _commonLookupRepository.GetCompletedShipmentGridSetting(clientId);
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);

      var dynamicParams = new DynamicParameters();
      // Calculate start and end dates 
      if (createdFrom == null)
      {
        createdFrom = DateTime.UtcNow.AddDays(-30);
      }
      if (createdTo == null)
      {
        createdTo = DateTime.UtcNow;
      }

      createdFrom = createdFrom.Value.AddMinutes(regionMinuts);
      createdTo = createdTo.Value.AddMinutes(regionMinuts);


      string dateFilter = @"(
                              (@createdFromDate IS NULL AND @createdToDate IS NULL)
                              OR
                              (@createdFromDate IS NULL AND CAST(o.CreatedOn AS DATE) <= CAST(@createdToDate AS DATE))
                              OR
                              (@createdToDate IS NULL AND CAST(o.CreatedOn AS DATE) >= CAST(@createdFromDate AS DATE))
                              OR
                              (@createdFromDate IS NOT NULL AND @createdToDate IS NOT NULL 
                                  AND CAST(o.CreatedOn AS DATE) BETWEEN CAST(@createdFromDate AS DATE) AND CAST(@createdToDate AS DATE))
                          )  ";


      // Build the dynamic SQL query
      var colsQuery = @$"
            SELECT STRING_AGG(QUOTENAME(REPLACE(Statuses.TrackingStatus, ' ', '')), ',') AS Cols
            FROM (
                SELECT DISTINCT ccts.TrackingStatus
                FROM dbo.[Order] AS o
                INNER JOIN dbo.ClientCarrierTrackingStatus AS ccts 
                    ON ccts.CarrierTrackingStatusId = o.CarrierTrackingStatusId
                    AND ccts.ClientId =  @ClientId 
                WHERE o.ClientId = @ClientId  
                AND {dateFilter}
                GROUP BY ccts.TrackingStatus
                HAVING COUNT(*) > 0
            ) AS Statuses ";

      var parameters = new
      {
        ClientId = clientId, // replace with your actual client ID
        createdFromDate = createdFrom, // replace with your actual start date or null
        createdToDate = createdTo // replace with your actual end date or null
      };
      var cols = (await connection.QueryFirstOrDefaultAsync<string>(colsQuery, parameters))?.Trim() ?? "";

      var query = $@"
            SELECT carrierId,Name as carrierName,COUNT(*) OVER (PARTITION BY Name) AS TotalCount, {cols}
            FROM
            (
                SELECT 
                    c.CarrierId,
                    c.Name,
                    REPLACE(ccts.TrackingStatus, ' ', '') AS TrackingStatus,
                    COUNT(*) OVER (PARTITION BY c.Name) AS TotalCount,
                    COUNT(*) AS TrackingStatusCount
                FROM 
                    dbo.[Order] AS o
                INNER JOIN 
                    dbo.Carrier AS c ON c.CarrierId = o.CarrierId
                INNER JOIN 
                    dbo.ClientCarrierTrackingStatus AS ccts ON ccts.CarrierTrackingStatusId = o.CarrierTrackingStatusId
                       AND ccts.ClientId = @ClientId  
                WHERE 
                    o.ClientId = @ClientId  
                AND {dateFilter}
                GROUP BY 
                    c.CarrierId,
                    c.Name,
                    ccts.TrackingStatus
                HAVING COUNT(*) > 0
            ) AS SourceTable
            PIVOT
            (
                MAX(TrackingStatusCount)
                FOR TrackingStatus IN ({cols})
            ) AS PivotTable;";

      // Execute the query
      var result = await connection.QueryAsync<dynamic>(query,parameters);
      return result;

    }
  }
  public async Task<dynamic> GetCarrierDashboardStats(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinutes = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);

      // Load client shipment grid status configurations
      string columnsQuery = $@"SELECT sgc.ShipmentGridColumnId,
                     sgc.ColumnName AS DashboardStatusName,
                     REPLACE(sgc.ColumnName, ' ', '') AS DashboardStatusNameForKey,
                     sgc.DisplayOrder,
                     sgc.IsDisplay,
                     sgc.IsCompleted,
                     ISNULL(sgc.Active, 0) AS Active,
                     ISNULL(sgc.IsDefaultStatusTab, 1) AS IsDefaultStatusTab,
                     CASE
                         WHEN sgc.IsFetchAllPendingStatus = 1 THEN
                         (
                             SELECT STRING_AGG(ccts.CarrierTrackingStatusId, ',') AS CommaSeparatedIds
                             FROM dbo.ClientCarrierTrackingStatus AS ccts
                             WHERE ccts.ClientId = @ClientId
                                   AND NOT EXISTS
                             (
                                 SELECT 1
                                 FROM STRING_SPLIT(
                                      (
                                          SELECT STRING_AGG(sgcs.DashboardStatusValue, ',') AS AllDashboardStatusValues
                                          FROM dbo.ShipmentGridClientSetting AS sgcs
                                              INNER JOIN dbo.ShipmentGridColumn AS sgc2
                                                  ON sgc2.ShipmentGridColumnId = sgcs.ShipmentGridColumnId
                                                     AND sgcs.ClientId = @ClientId
                                          WHERE sgc2.IsFetchAllPendingStatus IS NULL
                                                OR sgc2.IsFetchAllPendingStatus <> 1
                                                   AND sgc2.Active = 1
                                      ), ',')
                                 WHERE value = ccts.CarrierTrackingStatusId
                             )
                         )
                         ELSE
                             sgcs.DashboardStatusValue
                     END AS DashboardStatusValue
              FROM dbo.ShipmentGridColumn AS sgc
                  INNER JOIN dbo.ShipmentGridClientSetting AS sgcs
                      ON sgcs.ShipmentGridColumnId = sgc.ShipmentGridColumnId
              WHERE sgc.ClientId = @ClientId AND sgc.Active = 1
              ORDER BY sgc.DisplayOrder;";

      var dashboardSettings = (await connection.QueryAsync<dynamic>(columnsQuery, new { ClientId = clientId })).ToList();

      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      for (int i = 0; i < dashboardSettings.Count; i++)
      {
        var item = dashboardSettings[i];
        string comma = i < dashboardSettings.Count - 1 ? "," : "";

        string statusName = item.DashboardStatusName;
        string statusNameForKey = item.DashboardStatusNameForKey;
        string statusValue = item.DashboardStatusValue;

        if (statusName?.Equals("all", StringComparison.OrdinalIgnoreCase) == true)
        {
          sb.Append($"TotalShipment = COUNT(o.OrderId){comma} ");
        }
        else
        {
          statusValue ??= "0";
          if (string.IsNullOrWhiteSpace(statusValue)) statusValue = "0";
          sb.Append($@"
                  [{statusNameForKey}] =
                  ISNULL(SUM(CASE 
                          WHEN o.CarrierTrackingStatusId IN ({statusValue}) 
                          THEN 1 ELSE 0 
                      END), 0){comma} ");
        }
      }

      var dateFilter = "";
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@ClientId", clientId);
      dynamicParams.Add("@regionMinutes", regionMinutes);

      if (createdFrom.HasValue)
      {
        dynamicParams.Add("@createdFrom", createdFrom.Value);
        dateFilter += " AND CAST(DATEADD(minute, @regionMinutes, o.CreatedOn) AS DATE) >= CAST(@createdFrom AS DATE) ";
      }
      if (createdTo.HasValue)
      {
        dynamicParams.Add("@createdTo", createdTo.Value);
        dateFilter += " AND CAST(DATEADD(minute, @regionMinutes, o.CreatedOn) AS DATE) <= CAST(@createdTo AS DATE) ";
      }

      var baseQuery = $@" FROM dbo.[Order] AS o
                          INNER JOIN dbo.Carrier c
                              ON c.CarrierId = o.CarrierId
                                 AND o.ClientId = @ClientId
                          {dateFilter}
                          INNER JOIN dbo.OrderAddress oa
                              ON o.OrderAddressId = oa.OrderAddressId
                            ";

      string where = " WHERE c.Active = 1 ";

      string query = $@"
      SELECT
          {sb},
          ISNULL(c.CarrierId, 0) AS CarrierId,
          ISNULL(c.Name, '') AS CarrierName,
          ISNULL(c.CarrierImage, '') AS CarrierImage
      {baseQuery}
      {where}
      GROUP BY c.CarrierId, c.Name, c.CarrierImage
      ORDER BY c.Name;
      ";

      var result = await connection.QueryAsync<dynamic>(query, dynamicParams);
      return result;
    }
  }
  public async Task<dynamic> GetCarrierStatesStackedChart_backup(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var completedStatus = await _commonLookupRepository.GetCompletedShipmentGridSetting(clientId);
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);

      var dynamicParams = new DynamicParameters();
      // Calculate start and end dates 
      if (createdFrom == null)
      {
        createdFrom = DateTime.UtcNow.AddDays(-30);
      }
      if (createdTo == null)
      {
        createdTo = DateTime.UtcNow;
      }

      createdFrom = createdFrom.Value.AddMinutes(regionMinuts);
      createdTo = createdTo.Value.AddMinutes(regionMinuts);

      // Calculate the day difference
      int dayDifference = (createdTo - createdFrom)?.Days ?? 0;
      if (dayDifference > 30)
      {
        dayDifference = 30;
      }
      DateTime? endDate = createdTo.Value.Date;
      DateTime? startDate = endDate?.AddDays(-dayDifference).Date;

      string query = @$"SELECT 
                                     o.CarrierId,
                                     c.Name AS CarrierName,
                                     COUNT(o.OrderId) AS TotalOrder,
                                     InProgress = SUM(   CASE
                                                             WHEN o.CarrierTrackingStatusId NOT IN ( {completedStatus} )
                                                                  AND o.TrackingLock <> 1 THEN
                                                                 1
                                                             ELSE
                                                                 0
                                                         END
                                                     ),
                                     Delivered = SUM(   CASE
                                                            WHEN o.CarrierTrackingStatusId IN ( {completedStatus} )
                                                                 AND o.TrackingLock = 1 THEN
                                                                1
                                                            ELSE
                                                                0
                                                        END
                                                    ),
                                     CODPending = SUM(   CASE
                                                             WHEN o.CarrierPaymentSettlementId IS NULL
                                                                  AND o.CarrierRRId IS NULL
                                                                  AND o.PaymentStatusId <> {(int)EnumPaymentStatus.Paid} THEN
                                                                 1
                                                             ELSE
                                                                 0
                                                         END
                                                     ),
                                     CODSettled = SUM(   CASE
                                                             WHEN o.CarrierPaymentSettlementId IS NOT NULL
                                                                  AND o.PaymentStatusId = {(int)EnumPaymentStatus.Paid} THEN
                                                                 1
                                                             ELSE
                                                                 0
                                                         END
                                                     ),
                                     DeliveryRatio = (CASE
                                                          WHEN COUNT(o.OrderId) > 0 THEN
                                                              CAST((CAST(((SUM(   CASE
                                                                                      WHEN o.CarrierTrackingStatusId NOT IN ( {completedStatus} )
                                                                                           AND o.TrackingLock <> 1 THEN
                                                                                          1
                                                                                      ELSE
                                                                                          0
                                                                                  END
                                                                              )
                                                                          ) + (SUM(   CASE
                                                                                          WHEN o.CarrierTrackingStatusId IN ( {completedStatus} )
                                                                                               AND o.TrackingLock = 1 THEN
                                                                                              1
                                                                                          ELSE
                                                                                              0
                                                                                      END
                                                                                  )
                                                                              )
                                                                         ) AS DECIMAL(10, 2)) / COUNT(o.OrderId) * 100
                                                                   ) AS DECIMAL(10, 2))
                                                          ELSE
                                                              0
                                                      END
                                                     ) ";
      query += @" FROM dbo.[Order] AS o
                        INNER  JOIN dbo.Carrier AS c
                            ON c.CarrierId = o.CarrierId
                        INNER  JOIN dbo.Client AS c2 ON c2.ClientId = o.ClientId ";

      string whereStart = @$"WHERE ( 1=1 AND o.ClientId = '{clientId}' ";
      string whereEnd = ")";

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string groupBy = " GROUP BY c.Name,o.CarrierId ";
      string where = whereStart + whereEnd + groupBy;
      string queryForCount = query + where;

      var data = await connection.QueryAsync<CarrierStatsResponseModel>(queryForCount, dynamicParams);
      return data;
    }
  }

  public async Task<dynamic> GetAllOrderCountWithCarrier(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"
                        SELECT 
                            COUNT(o.OrderId) AS TotalOrders, 
                            SUM(o.Amount) AS TotalAmount
                        FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1  ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      whereStart += $"And (o.CarrierId IS NOT NULL)";

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      // Fetch the single row result
      var resultData = await connection.QueryFirstOrDefaultAsync<dynamic>(queryForCount, dynamicParams);

      dynamic result = new ExpandoObject();
      result.TotalOrders = resultData?.TotalOrders ?? 0;
      result.TotalAmount = resultData?.TotalAmount ?? 0;

      return result;
    }
  }

  public async Task<dynamic> GetAllOrderCountWithCarrierAndCODAmount(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();

      string TotalCount = @"
                        SELECT 
                            COUNT(o.OrderId) AS TotalOrders, 
                            SUM(o.Amount) AS TotalAmount
                        FROM dbo.[Order] AS o ";

      string whereStart = "WHERE ( 1=1  ";
      string whereEnd = ")";


      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      whereStart += "And (o.CarrierId IS NOT NULL) ";
      whereStart += "And (o.PaymentMethodId = @COD) ";
      dynamicParams.Add("@COD", (int)EnumPaymentMethod.COD);

      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var resultData = await connection.QueryFirstOrDefaultAsync<dynamic>(queryForCount, dynamicParams);

      dynamic result = new ExpandoObject();
      result.TotalOrders = resultData?.TotalOrders ?? 0;
      result.TotalAmount = resultData?.TotalAmount ?? 0;

      return result;
    }
  }

  public async Task<dynamic> GetSaleDashboardChannels(int storeId, string clientId, DateTime? createdFrom = null, DateTime? createdTo = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _dbContext);
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@StoreId", storeId);
      dynamicParams.Add("@ClientId", clientId);

      string dateFilter = "";
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        dateFilter += $" AND (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        dateFilter += $" AND (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }

      string query = $@"SELECT scc.SaleChannelConfigId AS Id,
                              ISNULL(emp.EmployeeName, scc.SaleChannelName) AS Name,
                              ISNULL(emp.EmployeeImage, scl.ImageUrl) AS ImageUrl,
                              (
                                  SELECT COUNT(*)
                                  FROM dbo.[Order] AS o
                                  WHERE o.StoreId = scc.StoreId
                                      AND o.SaleChannelConfigId = scc.SaleChannelConfigId
                                      {dateFilter}
                              ) AS OrderCount
                       FROM dbo.SaleChannelConfig AS scc
                           INNER JOIN dbo.SaleChannelLookup AS scl
                               ON scl.SaleChannelLookupId = scc.SaleChannelLookupId
                           INNER JOIN dbo.Stores AS s
                               ON s.StoreId = scc.StoreId
                           OUTER APPLY (
                               SELECT TOP 1 e.EmployeeName, e.EmployeeImage
                               FROM dbo.Employee AS e
                               WHERE e.SaleChannelConfigId = scc.SaleChannelConfigId
                                   AND e.Active = 1
                           ) AS emp
                       WHERE scc.StoreId = @StoreId
                           AND scc.Active = 1
                           AND s.ClientId = @ClientId";

      var data = await connection.QueryAsync(query, dynamicParams);
      return data.ToList();
    }
  }

  public async Task<dynamic> GetSaleDashboardProducts(int storeId, int? saleChannelConfigId, int start, int length, string search, string clientId, DateTime? createdFrom = null, DateTime? createdTo = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@StoreId", storeId);
      dynamicParams.Add("@ClientId", clientId);
      dynamicParams.Add("@SaleChannelConfigId", saleChannelConfigId);
      dynamicParams.Add("@Start", start);
      dynamicParams.Add("@Length", length);
      dynamicParams.Add("@Search", $"%{search}%");
      dynamicParams.Add("@CreatedFrom", createdFrom);
      dynamicParams.Add("@CreatedTo", createdTo);

      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedOn DESC) AS RowNum,
                              COUNT(*) OVER () AS TotalCount,
                              CAST(p.ProductId AS NVARCHAR(40)) AS ProductId,
                              p.ProductName,
                              p.FeatureImage,
                              p.SKU,
                              p.Price,
                              ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                              scc.SaleChannelConfigId,
                              ISNULL((SELECT TOP 1 e.EmployeeName FROM dbo.Employee AS e WHERE e.SaleChannelConfigId = scc.SaleChannelConfigId AND e.Active = 1), '') AS SalespersonName,
                              ISNULL(sq.QuantityAvailable, 0) AS QuantityAvailable
                       FROM dbo.Product AS p
                           INNER JOIN dbo.StoreProduct AS sp
                               ON sp.ProductId = p.ProductId
                           LEFT JOIN dbo.SaleChannelConfig AS scc
                               ON scc.SaleChannelConfigId = p.SaleChannelConfigId
                           LEFT JOIN (
                               SELECT pv.ProductId,
                                      SUM(ib.QuantityAvailable) AS QuantityAvailable
                               FROM dbo.InventoryBalance AS ib
                               INNER JOIN dbo.ProductVariant AS pv ON pv.ProductVariantId = ib.ProductVariantId
                               GROUP BY pv.ProductId
                           ) AS sq
                               ON sq.ProductId = p.ProductId
                       WHERE sp.StoreId = @StoreId
                           AND sp.Active = 1
                           AND p.Active = 1
                           AND p.ClientId = @ClientId
                           AND (@CreatedFrom IS NULL OR CAST(p.CreatedOn AS DATE) >= CAST(@CreatedFrom AS DATE))
                           AND (@CreatedTo IS NULL OR CAST(p.CreatedOn AS DATE) <= CAST(@CreatedTo AS DATE))
                           AND (@SaleChannelConfigId IS NULL OR @SaleChannelConfigId = 0 OR p.SaleChannelConfigId = @SaleChannelConfigId)
                           AND (@Search IS NULL OR @Search = '' OR p.ProductName LIKE @Search OR p.SKU LIKE @Search)
                       ORDER BY p.CreatedOn DESC
                       OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";

      var data = await connection.QueryAsync(query, dynamicParams);
      var dataList = data.ToList();
      int totalCount = 0;
      if (dataList.Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount ?? 0;
      }

      dynamic result = new ExpandoObject();
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }

  public async Task<dynamic> GetSaleDashboardStores(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@ClientId", clientId);

      string query = @"SELECT s.StoreId,
                              s.StoreName,
                              s.StoreImage,
                              (
                                  SELECT COUNT(*)
                                  FROM dbo.SaleChannelConfig AS scc
                                  WHERE scc.StoreId = s.StoreId
                                    AND scc.Active = 1
                              ) AS SaleChannelConfigCount
                       FROM dbo.Stores AS s
                       WHERE s.Active = 1
                         AND s.ClientId = @ClientId
                       ORDER BY s.StoreName ASC";

      var data = await connection.QueryAsync(query, dynamicParams);
      return data.ToList();
    }
  }

}
