using Dapper;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.ActivityLogAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ReturnAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Infrastructure.Services.Interface;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ReturnRepository : IReturnRepository
{
  private readonly IDbContextService _dbContextService;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public ReturnRepository(IDbContextService dbContextService, DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dbContextService = dbContextService;
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  #region return reason
  public async Task<bool> CreateClientReturnReson(ClientReturnReason clientReturnReason)
  {
    await _context.ClientReturnReasons.AddAsync(clientReturnReason);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> GetAllClientReturnReason(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();
      var query = @"SELECT
                    ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY e.CreatedOn)) AS RowNum,
                    COUNT(*) OVER () AS TotalCount,
                    crr.ClientReturnReasonId,
                    crr.Reason,
                    crr.ReasonDetail,
                    e.EmployeeName AS CreatedBy,
                    crr.CreatedOn,
                    crr.Active,
                    crr.UpdatedOn
              FROM dbo.ClientReturnReason AS crr
                  LEFT JOIN dbo.Employee AS e
                      ON e.EmployeeId = crr.CreatedBy
	                    ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @" ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (crr.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("crr.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("crr.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "crr.CreatedOn");


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

  public async Task<ClientReturnReason> GetClientReturnResonByClientReasonId(int clientReturnReasonId, ClientId clientId)
  {
    var target = await _context.ClientReturnReasons.FirstOrDefaultAsync(x => x.ClientReturnReasonId == clientReturnReasonId && x.ClientId == clientId);
    return target!;
  }
  public async Task<bool> UpdateClientReturnReson(ClientReturnReason clientReturnReason)
  {
    _context.ClientReturnReasons.Update(clientReturnReason);
    return await _context.SaveChangesAsync() > 0;
  }

  #endregion
  public async Task<dynamic> GetAllOrderReturn(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId, int? returnReasonId = null, int? returnStatusId = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();
      var query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY r.ReturnId)) AS RowNum,
       COUNT(*) OVER () AS TotalCount,
       r.ReturnId,
       r.ReturnComment,
       r.RefundAmount,
       r.ReturnCharges,
       crr2.Reason,
       rtl.RefundTypeName,
       o.OrderNo,
       ISNULL(o2.OrderNo, '') AS ReturnOrderNo,
       o.Amount,
       CASE
           WHEN s.IsDefault = 1 THEN
               s.StoreName
           ELSE
               s.StoreName
       END AS StoreName,
       s.CustomerServiceNo,
       oa.CustomerName,
       oa.CustomerFullAddress,
       oa.Mobile1,
       psl.StatusName,
       CASE
           WHEN r.RTOOrderId IS NULL THEN
               'Forward'
           ELSE
               'Reverse'
       END AS DeliveryType,
       r.ReturnStatusId,
       rsl.ReturnStatus
FROM dbo.[Return] AS r
    INNER JOIN dbo.ClientReturnReason AS crr2
        ON crr2.ClientReturnReasonId = r.ClientReturnReasonId
    INNER JOIN dbo.ClientReturnReason AS crr
        ON crr.ClientReturnReasonId = r.ClientReturnReasonId
    LEFT JOIN dbo.RefundTypeLookup AS rtl
        ON rtl.RefundTypeId = r.RefundTypeId
    INNER JOIN dbo.[Order] AS o
        ON o.OrderId = r.OrderId
    LEFT JOIN dbo.[Order] AS o2
        ON r.RTOOrderId = o2.OrderId
    INNER JOIN dbo.OrderAddress AS oa
        ON o.OrderAddressId = oa.OrderAddressId
    INNER JOIN dbo.Stores AS s
        ON o.StoreId = s.StoreId
    INNER JOIN dbo.PaymentStatusLookup AS psl
        ON o.PaymentStatusId = psl.PaymentStatusId 
    INNER JOIN dbo.ReturnStatusLookup AS rsl
        ON r.ReturnStatusId = rsl.ReturnStatusLookupId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @" ";
      }
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
      if (returnReasonId != null && returnReasonId > 0)
      {
        dynamicParams.Add("@returnReasonId", returnReasonId);
        whereStart += $"And r.ClientReturnReasonId  = @returnReasonId ";
      }
      if (returnStatusId != null && returnStatusId > 0)
      {
        dynamicParams.Add("@returnStatusId", returnStatusId);
        whereStart += $"And r.ReturnStatusId  = @returnStatusId ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.OrderNo");


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


  public async Task<List<ClientReturnReason>> GetAllClientReturnReasonForSelection(ClientId clientId)
  {
    var cId = clientId.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(cId!))
    {
      return await _context.ClientReturnReasons.Where(x => x.ClientId == clientId).ToListAsync()!;
    }
  }
  public async Task<List<RefundTypeLookup>> GetAllRefundTypeLookupForSelection(ClientId clientId)
  {
    var cId = clientId.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(cId!))
    {
      return await _context.RefundTypeLookups.ToListAsync()!;
    }
  }
  public async Task<List<ReturnStatusLookup>> GetAllReturnStatusLookupForSelection(ClientId clientId)
  {
    var cId = clientId.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(cId!))
    {
      return await _context.ReturnStatusLookups.ToListAsync()!;
    }
  }

  public async Task<bool> CreateReturn(Return returnObj, string? clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      await _context.Returns.AddAsync(returnObj!);
      return await _context.SaveChangesAsync() > 0;
    }
  }

  public async Task<Order> GetOrderById(OrderId orderId, ClientId clientId)
  {
    var cId = clientId.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(cId!))
    {
      var target = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId! == orderId && x.ClientId == clientId)!;
      return target!;
    }
  }

  public async Task<PaymentLink?> GetPaymentLinkByOrderId(OrderId? orderId, ClientId clientId)
  {
    var cId = clientId.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(cId!))
    {
      var target = await _context.PaymentLinks.FirstOrDefaultAsync(x => x.OrderId! == orderId && x.ClientId == clientId)!;
      return target!;
    }
  }
  public async Task<Order> GetOrderByOrderNo(string orderNo, ClientId clientId)
  {
    var cId = clientId.Value.ToString();
    using (var _context = _dbContextService.GetAppDbContext(cId!))
    {
      var target = await _context.Orders.FirstOrDefaultAsync(x => x.OrderNo! == orderNo && x.ClientId == clientId)!;
      return target!;
    }
  }
  public async Task<bool> CreateReturnProduct(ReturnProduct returnProduct, string? clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      var target = await _context.ReturnProducts.AddAsync(returnProduct)!;
      return await _context.SaveChangesAsync() > 0!;
    }
  }

  public async Task<ReturnOrderWrapperResponseModel> GetReturnReportDataByOrderIdForTracking(string? orderNo, string? clientId)
  {

    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      ReturnOrderWrapperResponseModel? objReturnResponse = new();
      var oOrder = await _context.Orders.FirstOrDefaultAsync(x => x.OrderNo! == orderNo && x.ClientId == new ClientId(new Guid(clientId!)))!;
      if (oOrder is not null)
      {
        var objReturn = await _context.Returns.FirstOrDefaultAsync(x => x.OrderId == oOrder.OrderId);
        if (objReturn is not null)
        {
          string status = string.Empty;
          var oReturnStatusLookups = _context.ReturnStatusLookups.FirstOrDefault(x => x.ReturnStatusLookupId == objReturn.ReturnStatusId);
          if (oReturnStatusLookups is not null)
          {
            status = oReturnStatusLookups.ReturnStatus!;
          }
          objReturnResponse.ReturnId = objReturn.ReturnId!.Value.ToString();
          objReturnResponse.IsReturnExist = true;
        }
        objReturnResponse.IsAllowReturn = oOrder.CarrierTrackingStatusId != (int)EnumCarrierTrackingStatus.Delivered;//target.TrackingLock.GetValueOrDefault();
        objReturnResponse.OrderId = oOrder!.OrderId!.Value.ToString();
        objReturnResponse.OrderTypeId = oOrder!.OrderTypeId;
        if (!objReturnResponse.IsReturnExist.GetValueOrDefault() && oOrder.OrderTypeId == (int)EnumOrderType.FullFilable)
        {
          //objReturnResponse.OrderItems = await GetOrderItemByOrderId(clientId!, oOrder.OrderId!.Value.ToString()!);
        }
      }
      return objReturnResponse!;
    }
  }
  public async Task<dynamic?> GetOrderItemByOrderId(string clientId, string orderId)
  {
    clientId = clientId.Trim();
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"SELECT CAST(p.ProductId AS NVARCHAR(40)) AS ProductId,
                             p.FeatureImage,
                             p.ProductName,
                             pv.SKU,
                             CASE
                                 WHEN p.HaveOptions = 1 THEN
                                     pv.SKU + ' | ' + COALESCE((SELECT STRING_AGG(po.OptionValue, ' / ') FROM dbo.ProductVariantOption pvo INNER JOIN dbo.ProductOptions po ON po.ProductOptionsId = pvo.ProductOptionsId WHERE pvo.ProductVariantId = pv.ProductVariantId), pv.VariantOptionText)
                                 ELSE
                                     pv.SKU
                             END AS SKUOption,
                             p.HaveOptions,
                             COALESCE((SELECT STRING_AGG(po.OptionValue, ' / ') FROM dbo.ProductVariantOption pvo INNER JOIN dbo.ProductOptions po ON po.ProductOptionsId = pvo.ProductOptionsId WHERE pvo.ProductVariantId = pv.ProductVariantId), pv.VariantOptionText) AS VarientOption,
                             oi.ProductStockId,
                             oi.Price,
                             s.StoreName,
                             'AED' AS Currency
                      FROM dbo.OrderItem AS oi
                          INNER JOIN dbo.InventoryBalance AS ib
                              ON ib.InventoryBalanceId = oi.ProductStockId
                          INNER JOIN dbo.ProductVariant AS pv
                              ON pv.ProductVariantId = ib.ProductVariantId
                          INNER JOIN dbo.Product AS p
                              ON pv.ProductId = p.ProductId
                          INNER JOIN dbo.Stores AS s
                              ON s.StoreId = p.StoreId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";


      dynamicParams.Add("@ClientId", clientId);
      whereStart += "And (p.ClientId = @ClientId) ";

      dynamicParams.Add("@orderId", orderId);
      whereStart += "And (oi.OrderId = @orderId) ";

      string where = whereStart + whereEnd;
      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }

  public async Task<Return> GetReturnByReturnId(ReturnId? returnId)
  {
    var objReturn = await _context.Returns.FirstOrDefaultAsync(x => x.ReturnId == returnId);
    return objReturn!;
  }
  public async Task<bool> UpdateReturn(Return objReturn)
  {
    _context.Returns.Update(objReturn);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> CreateActivity(ReturnActivityLog returnActivityLog, string? clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      await _context.ReturnActivityLogs.AddAsync(returnActivityLog);
      return await _context.SaveChangesAsync() > 0;
    }
  }

  public async Task<bool> CreateReturnTrackingHistory(ReturnTrackingHistory model, string? clientId)
  {
    using (var _context = _dbContextService.GetAppDbContext(clientId!))
    {
      await _context.ReturnTrackingHistories.AddAsync(model);
      return await _context.SaveChangesAsync() > 0;
    }
  }

  public async Task<dynamic> GetReturnTrackingHistory(string? returnId, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT rth.ReturnStatusLookupId as TrackingStatusId,
                               rsl.ReturnStatus AS TrackingStatus,
                               '' AS TrackingStatusComments,
                               '' AS StoreName,
                               '' AS StoreImage,
                               '' AS CustomerServiceNo,
                               ISNULL(o.OrderNo, '') AS ReturnOrderNo,
                               rth.CreatedOn
                        FROM dbo.ReturnTrackingHistory AS rth
                            INNER JOIN dbo.[Return] AS r
                                ON r.ReturnId = rth.ReturnId
                            INNER JOIN dbo.ReturnStatusLookup AS rsl
                                ON rsl.ReturnStatusLookupId = rth.ReturnStatusLookupId 
                            LEFT JOIN dbo.[Order] AS o
                                    ON r.RTOOrderId = o.OrderId ";
      string whereStart = $"WHERE (  "; //( (rsl.ReturnStatusLookupId <> {(int)EnumReturnStatus.New})
      string whereEnd = ")";

      dynamicParams.Add("@ClientId", clientId);
      whereStart += " (r.ClientId = @ClientId) ";

      string where = whereStart + whereEnd;

      string queryData = query + where + " ORDER BY rth.CreatedOn desc ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }


  public async Task<dynamic?> GetStoreAndCustomerAddressByOrderId(string? orderId, string clientId)
  {
    dynamic result = new ExpandoObject();
    result.store = await GetStoreAddressByOrderId(orderId, clientId);
    result.customer = await GetCustomerAddressByOrderId(orderId, clientId);

    return result;
  }
  public async Task<dynamic> GetStoreAddressByOrderId(string? orderId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT s.StoreImage,
                               s.StoreName,
                               s.CustomerServiceNo,
                               sa.FullAddress,
	                           o.CreatedOn
                        FROM dbo.[Order] AS o
                            INNER JOIN dbo.Stores AS s
                                ON s.StoreId = o.StoreId
                                   AND o.ClientId = s.ClientId
                            INNER JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = o.StoreId ";
      string whereStart = "WHERE ( ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "(o.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(orderId))
      {
        dynamicParams.Add("@orderId", orderId);
        whereStart += "And (o.OrderId = @orderId) ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic? storeAddress = null;
      if (data != null)
      {
        storeAddress = data.FirstOrDefault();
      }
      return storeAddress!;
    }

  }
  public async Task<dynamic> GetCustomerAddressByOrderId(string? orderId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT oa.CustomerName,
                               oa.Mobile1 AS Mobile,
                               oa.CustomerFullAddress,
                               o.CreatedOn
                        FROM dbo.[Order] AS o
                            INNER JOIN dbo.OrderAddress AS oa
                                ON oa.OrderAddressId = o.OrderAddressId ";
      string whereStart = "WHERE ( ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "(o.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(orderId))
      {
        dynamicParams.Add("@orderId", orderId);
        whereStart += "And (o.OrderId = @orderId) ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic? customerAddress = null;
      if (data != null)
      {
        customerAddress = data.FirstOrDefault();
      }
      return customerAddress!;
    }

  }
  public async Task<dynamic> GetAllOrderItems(string orderId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT o.OrderId,
                             oi.OrderItemId,
                             o.OrderTypeId,
                             ISNULL(p.SKU, 'N/A') AS ProductSku,
                             ISNULL(p.ProductName, 'N/A') AS ProductName,
                             ISNULL(pv.SKU, 'N/A') AS ProductStockSku,
                             ISNULL(oi.Price,0) AS Price,
                             ISNULL(oi.Description, 'N/A') AS OrderItemDescription,
                             oi.Remarks AS OrderItemRemarks,
                             oi.Quantity AS OrderItemQuantity,
                             oi.ItemBarcode,
                             oi.Discount AS OrderItemDiscount
                      FROM dbo.[Order] AS o
                          INNER JOIN dbo.OrderItem AS oi
                              ON oi.OrderId = o.OrderId
                          LEFT JOIN dbo.Product AS p
                              ON p.ProductId = oi.ProductId
                          LEFT JOIN dbo.InventoryBalance AS ib
                              ON ib.InventoryBalanceId = oi.ProductStockId
                          LEFT JOIN dbo.ProductVariant AS pv
                              ON pv.ProductVariantId = ib.ProductVariantId  ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(orderId))
      {
        dynamicParams.Add("@orderId", orderId);
        whereStart += "And ( ( oi.OrderId = @orderId)) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }

  }
  public async Task<dynamic> GetOrderTaxInfo(string orderId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT ISNULL(o.CShippingCharges,0) AS CShippingCharges,
                               ISNULL(o.TotalTax,0) AS TotalTax,
                               ISNULL(o.Discount,0) AS Discount,
                               ISNULL(o.Amount,0) AS Amount
                        FROM dbo.[Order] AS o ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(orderId))
      {
        dynamicParams.Add("@orderId", orderId);
        whereStart += "And ( ( o.OrderId = @orderId)) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      dynamic? oTax = null;
      if (data != null)
      {
        oTax = data.FirstOrDefault();
      }
      return oTax!;
    }

  }


}
