using System.Dynamic;
using Dapper;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class InventoryRepository : IInventoryRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;
  public InventoryRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  public async Task<dynamic> GetAllInventorySales(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? productStationIds, string? productSKUs, string? trackingStatusId, bool? IsFulfilled, bool? IsInTransit, string? regionIds, string? saleChannelConfigIds, string storeIds, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = @"SELECT COUNT(*) OVER () AS TotalCount,
                             o.OrderId,
                             ISNULL(c.ClientCompanyName, c.ClientName) AS ClientName,
                             ps.Name AS StationName,
                             spAgg.StoreName,
                             p.SKU,
                             p.ProductName,
                             o.OrderNo,
                             o.OrderDate,
                             ffsl.FullFillmentStatus,
                             SUM(oi.Discount) AS Discount,
                             SUM(oi.Quantity) AS Quantity,
                             SUM(oi.Price) AS Price
                        FROM dbo.OrderItem AS oi
                        INNER JOIN dbo.[Order] AS o
                            ON o.OrderId = oi.OrderId
                        INNER JOIN dbo.OrderAddress AS oa
                            ON oa.OrderAddressId = o.OrderAddressId
                        INNER JOIN dbo.InventoryBalance AS ib
                            ON ib.InventoryBalanceId = oi.ProductStockId
                        INNER JOIN dbo.ProductVariant AS ps2
                            ON ps2.ProductVariantId = ib.ProductVariantId
                        INNER JOIN dbo.Product AS p
                            ON p.ProductId = ps2.ProductId
                        INNER JOIN dbo.Client AS c
                            ON c.ClientId = o.ClientId
                        INNER JOIN dbo.ProductStation AS ps
                            ON o.StationId = ps.ProductStationId
                        INNER JOIN dbo.FullFillmentStatusLookup AS ffsl
                            ON ffsl.FullFillmentStatusId = o.FullFillmentStatusId
                         CROSS APPLY (
                                    SELECT 
                                        STRING_AGG(CAST(s.StoreId AS NVARCHAR), ', ') AS StoreIds,
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
                                ) AS spAgg  ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( o.OrderNo in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.OrderDate", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.OrderDate", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      //if (!string.IsNullOrEmpty(storeIds))
      //{
      //  dynamicParams.Add("@storeIds", storeIds);
      //  whereStart += "And ((o.StoreId in (select value from STRING_SPLIT(@storeIds,',')) )) ";
      //}
      if (!string.IsNullOrEmpty(storeIds) && length > 0)
      {
        query = query + @" INNER JOIN dbo.StoreProduct AS sp  ON p.ProductId = sp.ProductId AND sp.Active =1
                           INNER JOIN dbo.Stores AS s ON sp.StoreId = s.StoreId ";

        dynamicParams.Add("@storeId", storeIds);
        whereStart += "And ( (sp.StoreId in (select value from STRING_SPLIT(@storeId,',')))) ";
      }
      if (!string.IsNullOrEmpty(trackingStatusId))
      {
        dynamicParams.Add("@trackingStatusId", trackingStatusId);
        whereStart += "And ((o.CarrierTrackingStatusId in (select value from STRING_SPLIT(@trackingStatusId,',')) )) ";
      }
      if (!string.IsNullOrEmpty(regionIds))
      {
        dynamicParams.Add("@regionIds", regionIds);
        whereStart += "And ((oa.RegionId in (select value from STRING_SPLIT(@regionIds,',')) )) ";
      }
      if (!string.IsNullOrEmpty(saleChannelConfigIds))
      {
        dynamicParams.Add("@saleChannelConfigIds", saleChannelConfigIds);
        whereStart += "And ((o.SaleChannelConfigId in (select value from STRING_SPLIT(@saleChannelConfigIds,',')) )) ";
      }
      if (!string.IsNullOrEmpty(productStationIds))
      {
        dynamicParams.Add("@productStationIds", productStationIds);
        whereStart += "And ((o.StationId in (select value from STRING_SPLIT(@productStationIds,',')) )) ";
      }
      if (IsFulfilled == true)
      {
        whereStart += $"And (o.FullFillmentStatusId = {(int)EnumFullfillmentStatus.Fulfilled}) ";
      }
      if (IsInTransit == true)
      {
        whereStart += $"And (o.CarrierTrackingStatusId not in (( SELECT value FROM STRING_SPLIT(( SELECT sgcs.DashboardStatusValue FROM dbo.ShipmentGridClientSetting AS sgcs WHERE sgcs.ShipmentGridColumnId =(SELECT sgc.ShipmentGridColumnId FROM dbo.ShipmentGridColumn AS sgc WHERE sgc.ColumnName = 'COMPLETED' AND sgc.ClientId = '{clientId}')), ',')))) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      var groupby = @" GROUP BY o.OrderId,
                                 c.ClientName,
                                 c.ClientCompanyName,
                                 o.OrderNo,
                                 o.OrderDate,
                                 ps.Name,
                                 ffsl.FullFillmentStatus,
                                  p.SKU,
                                  p.ProductName,
                                  spAgg.StoreName,
                                 o.CreatedOn ";

      string where = whereStart + whereEnd + groupby;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.OrderDate");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";
      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        foreach (var item in dataList.ToList())
        {
          item.InventorySalesDetail = await GetOrderInventorySalesDetail(item, productSKUs, storeIds,clientId);
        }
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      //filtert data where item must be greater than 0
      dataList = dataList.Where(x => x.InventorySalesDetail.Count > 0).ToList();

      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }
  }

  private async Task<dynamic> GetOrderInventorySalesDetail(dynamic item, string productSKUs, string storeIds, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT o.OrderId,
                               o.OrderNo,
                               p.ProductName,
                               ps2.VariantOptionText AS VarientOption,
                               oi.Discount,
                               oi.Price,
                               oi.Quantity
                        FROM dbo.[Order] AS o
                            INNER JOIN dbo.OrderItem AS oi
                                ON o.OrderId = oi.OrderId
                            INNER JOIN dbo.Product AS p
                                ON p.ProductId = oi.ProductId
                            INNER JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId
                            INNER JOIN dbo.InventoryBalance AS ib
                                ON ib.InventoryBalanceId = oi.ProductStockId
                            INNER JOIN dbo.ProductVariant AS ps2
                                ON ps2.ProductVariantId = ib.ProductVariantId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(productSKUs))
      {
        dynamicParams.Add("@productSKUs", productSKUs);
        whereStart += "And ((ps2.SKU in (select value from STRING_SPLIT(@productSKUs,',')) )) ";
      }
      if (item != null)
      {
        dynamicParams.Add("@OrderId", item.OrderId);
        whereStart += "And (o.OrderId = @OrderId) ";
      }
      var groupby = @" GROUP BY o.OrderId,
                                 o.OrderNo,
                                 p.ProductName,
                                 ps2.VariantOptionText,
                                 oi.Discount,
                                 oi.Price,
                                 oi.Quantity ";

      string where = whereStart + whereEnd + groupby;

      string queryData = query + where;
      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data;

    }

  }

  public async Task<dynamic> GetInventorySalesSummary(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? productStationIds, string? productSKUs, string? trackingStatusId, bool? IsFulfilled, bool? IsInTransit, string? regionIds, string? saleChannelConfigIds, string storeIds, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

 
      string query = @"SELECT COUNT(*) OVER () AS TotalCount,
                               ISNULL(c.ClientCompanyName, c.ClientName) AS ClientName,
                               s.StoreName,
                               ps2.Name AS StationName,
                               ffsl.FullFillmentStatus,
                               p.ProductName,
                               ps.SKU,
                               ps.VariantOptionText AS VarientOption,
                               o.OrderNo,
                               oi.Quantity,
                               o.Discount,
                               o.Amount
                        FROM dbo.[Order] AS o
                            INNER JOIN dbo.OrderItem AS oi
                                ON oi.OrderId = o.OrderId
                            INNER JOIN dbo.ProductStation AS ps2
                                ON o.StationId = ps2.ProductStationId
                            INNER JOIN dbo.InventoryBalance AS ib ON ib.InventoryBalanceId = oi.ProductStockId
                            INNER JOIN dbo.ProductVariant AS ps ON ps.ProductVariantId = ib.ProductVariantId
                            INNER JOIN dbo.Product AS p
                                ON p.ProductId = oi.ProductId
                            INNER JOIN dbo.Client AS c
                                ON c.ClientId = o.ClientId
                            INNER JOIN dbo.Stores AS s
                                ON s.StoreId = p.StoreId
                            INNER JOIN dbo.FullFillmentStatusLookup AS ffsl
                                ON ffsl.FullFillmentStatusId = o.FullFillmentStatusId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( o.OrderNo in (select value from STRING_SPLIT(@Search,',')))) ";
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
      if (!string.IsNullOrEmpty(storeIds))
      {
        dynamicParams.Add("@storeIds", storeIds);
        whereStart += "And ((o.StoreId in (select value from STRING_SPLIT(@storeIds,',')) )) ";
      }
      if (!string.IsNullOrEmpty(regionIds))
      {
        dynamicParams.Add("@regionIds", regionIds);
        whereStart += "And ((oa.RegionId in (select value from STRING_SPLIT(@regionIds,',')) )) ";
      }
      if (!string.IsNullOrEmpty(saleChannelConfigIds))
      {
        dynamicParams.Add("@saleChannelConfigIds", saleChannelConfigIds);
        whereStart += "And ((o.SaleChannelConfigId in (select value from STRING_SPLIT(@saleChannelConfigIds,',')) )) ";
      }
      if (!string.IsNullOrEmpty(productStationIds))
      {
        dynamicParams.Add("@productStationIds", productStationIds);
        whereStart += "And ((o.StationId in (select value from STRING_SPLIT(@productStationIds,',')) )) ";
      }
      if (!string.IsNullOrEmpty(productSKUs) && productSKUs != "")
      {
        dynamicParams.Add("@productSKUs", productSKUs);
        whereStart += "And ( ( ps.SKU in (select value from STRING_SPLIT(@productSKUs,',')))) ";
      }
      if (!string.IsNullOrEmpty(trackingStatusId) && trackingStatusId != "0")
      {
        dynamicParams.Add("@trackingStatusId", trackingStatusId);
        whereStart += "And (o.CarrierTrackingStatusId in (select value from STRING_SPLIT(@trackingStatusId,',')))) ";
      }
      if (IsFulfilled == true)
      {
        whereStart += $"And (o.FullFillmentStatusId = {(int)EnumFullfillmentStatus.Fulfilled}) ";
      }
      if (IsInTransit == true)
      {
        whereStart += $"And (o.CarrierTrackingStatusId not in (( SELECT value FROM STRING_SPLIT(( SELECT sgcs.DashboardStatusValue FROM dbo.ShipmentGridClientSetting AS sgcs WHERE sgcs.ShipmentGridColumnId =(SELECT sgc.ShipmentGridColumnId FROM dbo.ShipmentGridColumn AS sgc WHERE sgc.ColumnName = 'COMPLETED' AND sgc.ClientId = '{clientId}')), ','))) ";
      }
      //if (!string.IsNullOrEmpty(Cities))
      //{
      //  dynamicParams.Add("@Cities", Cities);
      //  whereStart += "And ( ( o.SKU in (select value from STRING_SPLIT(@Cities,',')))) ";
      //}
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId)) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");


      string queryData = query + where + " ORDER BY p.SKU OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";
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
}
