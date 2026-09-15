using System.Dynamic;
using System.Security.Cryptography;
using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderAggregate.Dto;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.Infrastructure.Services.Interface;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class OrderRepository : IOrderRepository
{
  private readonly IDbContextService _dbContextService;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public OrderRepository(DapperAppDbContext dapperAppDbContext, IDbContextService dbContextService, AppDbContext context)
  {
    _dbContextService = dbContextService;
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<Order> CreateOrder(Order order)
  {
    await _context.Orders.AddAsync(order);
    await _context.SaveChangesAsync();
    return order;

  }
  public async Task<dynamic> UpdateOrder(Order order)
  {
    _context.Orders.Update(order);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> DeleteOrder(Order order)
  {
    _context.Remove(order);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<List<Order>?> GetOrdersByReturnReportId(CarrierRRId carrierRRId, ClientId clientId)
  {
    return await _context.Orders.Where(x => x.CarrierRRId! == carrierRRId && x.ClientId == clientId).ToListAsync();
  }
  public async Task<Order?> GetOrderById(OrderId orderId, ClientId clientId)
  {
    return await _context.Orders.FirstOrDefaultAsync(x => x.OrderId! == orderId && x.ClientId == clientId);
  }

  public async Task<bool> ArchiveOrderAsync(OrderArchive archive)
  {
    await _context.OrderArchives.AddAsync(archive);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> GetAllArchveOrders(string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@displayStart", start);
      dynamicParams.Add("@displayLength", length);

      string query = @"
            SELECT  
           IsNull(oa.ArchiveNo,'') as ArchiveNo,
           CAST(oa.CreatedOn AS DATE) AS ArchiveDate,
           Count(*) As total  
           FROM dbo.OrderArchive AS oa 
        ";

      string whereStart = " WHERE (1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( oa.ArchiveNo like '%@search%' )) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += " AND (CAST(oa.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += " AND (CAST(oa.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (oa.ClientId = @ClientId) ";
      }
      var groupBy = @" GROUP BY oa.ArchiveNo,CAST(oa.CreatedOn AS DATE) ORDER BY ArchiveDate DESC ";

      string where = whereStart + whereEnd;

      string queryData = query + where + groupBy + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var result = await connection.QueryAsync<dynamic>(queryData, dynamicParams);

      return result.ToList();
    }
  }

  public async Task<List<OrderArchive>> GetArchiveOrdersByArchiveNo(ClientId? clientId, string? ArchiveNo)
  {
    return await _context.OrderArchives
    .Where(x => x.ArchiveNo == ArchiveNo && x.ClientId == clientId)
    .ToListAsync();
  }

  public async Task<bool> DeleteArchiveOrder(OrderArchive archive)
  {
    _context.OrderArchives.Remove(archive);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> ExcelExportOrdersArchive(string clientId, DateTime? date = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = $@"
            SELECT 
                ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                COUNT(*) OVER () AS TotalCount,
                o.OrderId,
                o.OrderNo,
                ISNULL(o.RefNo, '') AS RefNo,
                ISNULL(o.ItemsCount, 0) AS NumberOfPieces,
                o.OrderDate,
                o.CreatedOn,
                ISNULL(c.IsClientCarrier, 0) AS IsClientCarrier,
                o.Amount,
                o.FulFilledDate,
                ISNULL(odt.TypeName, 'Forward ') AS DeliveryTypeName,
                ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                o.CarrierLastUpdateDateTime,
                o.CarrierId,
                ISNULL(c.Name, '') AS CarrierName,
                ISNULL(c.CarrierImage, '') AS CarrierImage,
                ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus,
                o.Description,
                o.Remarks,
                CASE
                    WHEN o.OrderTypeId = 1 THEN ISNULL(ffs.FullFillmentStatus, 'Unfulfilled')
                    ELSE ISNULL(ffs.FullFillmentStatus, '-')
                END AS FullFillmentStatus,
                o.FullFillmentStatusId,
                o.ItemsCount,
                ISNULL(psl.StatusName, '') AS PaymentStatus,
                ISNULL(pm.PMName, '') AS PaymentMethod,
                o.Weight,
                o.ItemValue,
                ISNULL(ps.Name, '') AS ProductStationName,
                o.Discount,
                o.TotalTax,
                ISNULL(o.PaymentMethodId, '-') AS PaymentMethodId,
                ISNULL(o.StripeInvoiceHostURL, '-') AS StripeInvoiceHostURL,
                ISNULL(o.StripeInvoicePDFURL, '-') AS StripeInvoicePDFURL,
                ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                scc.SaleChannelConfigId,
                s.StoreName,
                CASE WHEN o.CarrierId IS NULL THEN 'Unassigned' ELSE c.Name END AS Carrier,
                s.StoreImage,
                o.StoreId,
                s.CustomerServiceNo,
                ot.OrderTypeName,
                oa.CustomerName,
                oa.CustomerFullAddress,
                oa.Mobile1,
                o.ClientId,
                o.OrderLabels,
                ISNULL(pl.PaymentLinkUrl, '') AS PaymentLinkUrl,
                CASE WHEN mf.EntityId IS NULL THEN 0 ELSE 1 END AS IsMetaFieldExist
            FROM dbo.OrderArchive AS dj
            CROSS APPLY OPENJSON(dj.OrderJson, '$.Order') WITH
            (
                OrderId UNIQUEIDENTIFIER '$.OrderId.Value',
                ClientId UNIQUEIDENTIFIER '$.ClientId.Value',
                StoreId INT '$.StoreId',
                SaleChannelConfigId INT '$.SaleChannelConfigId',
                OrderTypeId INT '$.OrderTypeId',
                OrderNo NVARCHAR(50) '$.OrderNo',
                OrderDate DATETIME2 '$.OrderDate',
                OrderAddressId INT '$.OrderAddressId',
                Amount DECIMAL(18,2) '$.Amount',
                CarrierId INT '$.CarrierId',
                CarrierTrackingNo NVARCHAR(100) '$.CarrierTrackingNo',
                CarrierTrackingStatus NVARCHAR(200) '$.CarrierTrackingStatus',
                CarrierLastUpdateDateTime DATETIME2 '$.CarrierLastUpdateDateTime',
                FullFillmentStatusId INT '$.FullFillmentStatusId',
                ItemsCount INT '$.ItemsCount',
                DeliveryCharges DECIMAL(18,2) '$.DeliveryCharges',
                PaymentStatusId INT '$.PaymentStatusId',
                PaymentMethodId INT '$.PaymentMethodId',
                StationId INT '$.StationId',
                Discount DECIMAL(18,2) '$.Discount',
                Vat DECIMAL(18,2) '$.Vat',
                TotalTax DECIMAL(18,2) '$.TotalTax',
                CShippingCharges DECIMAL(18,2) '$.CShippingCharges',
                Weight DECIMAL(18,3) '$.Weight',
                ItemValue DECIMAL(18,2) '$.ItemValue',
                Description NVARCHAR(500) '$.Description',
                Remarks NVARCHAR(500) '$.Remarks',
                RefNo NVARCHAR(100) '$.RefNo',
                OrderLabels NVARCHAR(MAX) '$.OrderLabels',
                CreatedOn DATETIME2 '$.CreatedOn',
                FulFilledDate DATETIME2 '$.FulFilledDate',
                OrderDeliveryTypeId INT '$.OrderDeliveryTypeId',
                StripeInvoiceHostURL NVARCHAR(500) '$.StripeInvoiceHostURL',
                StripeInvoicePDFURL NVARCHAR(500) '$.StripeInvoicePDFURL'
            ) AS o
            LEFT JOIN dbo.OrderAddress AS oa ON o.OrderAddressId = oa.OrderAddressId
            INNER JOIN dbo.Stores AS s ON o.StoreId = s.StoreId
            LEFT JOIN dbo.SaleChannelConfig AS scc ON scc.SaleChannelConfigId = o.SaleChannelConfigId
            LEFT JOIN dbo.Carrier AS c ON o.CarrierId = c.CarrierId
            LEFT JOIN dbo.PaymentStatusLookup AS psl ON o.PaymentStatusId = psl.PaymentStatusId
            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
            INNER JOIN dbo.OrderTypeLookup AS ot ON o.OrderTypeId = ot.OrderTypeId
            INNER JOIN dbo.PaymentMethodLookup AS pm ON o.PaymentMethodId = pm.PaymentMethodId
            LEFT JOIN dbo.ProductStation AS ps ON o.StationId = ps.ProductStationId
            INNER JOIN dbo.Client AS cl ON cl.ClientId = o.ClientId
            LEFT JOIN dbo.OrderDeliveryType AS odt ON odt.OrderDeliveryTypeId = o.OrderDeliveryTypeId
            LEFT JOIN dbo.PaymentLink AS pl ON pl.OrderId = o.OrderId
            LEFT JOIN dbo.MetaField AS mf ON o.OrderId = mf.EntityId
        ";

      // -------------------- WHERE CLAUSE FIX ------------------------
      string where = " WHERE 1 = 1 ";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        where += " AND o.ClientId = @ClientId ";
      }

      if (date != null)
      {
        dynamicParams.Add("@date", date);
        where += $" AND CAST({CommonUtility.GetFormatedDateStr("dj.CreatedOn", regionMinuts)} AS DATE) = CAST(@date AS DATE) ";
      }

      string finalQuery = query + where;

      var data = await connection.QueryAsync(finalQuery, dynamicParams);

      dynamic result = new ExpandoObject();
      var list = data.ToList();

      result.TotalCount = list.Count > 0 ? list.First().TotalCount : 0;
      result.list = list;

      return result;
    }
  }


  public async Task<dynamic> DashboardGetTotalNoofOrdersPlaced(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

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
      string where = whereStart + whereEnd;
      string queryForCount = TotalCount + where;

      var count = await connection.ExecuteScalarAsync<int>(queryForCount, dynamicParams);


      dynamic result = new ExpandoObject();
      result.TotalCount = count;
      return result;
    }
  }

  public async Task<OrderAddress> CreateOrderAddress(OrderAddress orderAddress)
  {
    await _context.OrderAddresses.AddAsync(orderAddress);
    await _context.SaveChangesAsync();
    return orderAddress;
  }
  #region store
  public async Task<Store?> GetStoreByid(int storeid)
  {
    return await _context.Stores.FirstOrDefaultAsync(x => x.StoreId! == storeid);
  }
  #endregion

  public async Task<int> GetOrderCountByClientId(ClientId clientId)
  {
    var orderCount = await _context.Orders.CountAsync(x => x.ClientId == clientId);
    return orderCount;
  }
  public async Task<List<ClientCarrierTrackingStatus>> GetAllCarrierTrackingStatusesByClientId(ClientId clientId)
  {
    var list = await _context.ClientCarrierTrackingStatuses.Where(x => x.ClientId == clientId).ToListAsync();
    return list;
  }
  public async Task<List<Order>> GetAllOrdersByPaymentSettlementId(CarrierPaymentSettlementId? carrierPaymentSettlementId)
  {
    var oList = await _context.Orders.Where(x => x.CarrierPaymentSettlementId == carrierPaymentSettlementId!).ToListAsync();
    return oList;
  }

  public async Task<OrderAddress> GetOrderAddressById(long orderAddressId)
  {
    var data = await _context.OrderAddresses.FirstOrDefaultAsync(x => x.OrderAddressId == orderAddressId);
    return data!;
  }
  public async Task<OrderAddress> UpdateOrderAddress(OrderAddress orderAddress)
  {
    _context.OrderAddresses.Update(orderAddress);
    await _context.SaveChangesAsync();
    return orderAddress;
  }
  public async Task<OrderItem> CreateOrderItem(OrderItem orderItem)
  {
    await _context.OrderItems.AddAsync(orderItem);
    await _context.SaveChangesAsync();
    return orderItem;
  }
  public async Task<List<OrderItem>> GetOrderItemsByOrderId(OrderId? orderId)
  {
    return await _context.OrderItems.Where(x => x.OrderId == orderId).ToListAsync()!;
  }

  public async Task<OrderItem> UpdateOrderItem(OrderItem? orderItem)
  {
    _context.OrderItems.Update(orderItem!);
    await _context.SaveChangesAsync();
    return orderItem!;
  }

  public async Task<OrderTrackingHistory> CreateOrderTrackingHistory(OrderTrackingHistory orderHistory)
  {
    await _context.OrderTrackingHistories.AddAsync(orderHistory!);
    await _context.SaveChangesAsync();
    return orderHistory!;
  }
  public async Task<OrderNote> CreateOrderNote(OrderNote orderNote)
  {
    await _context.OrderNotes.AddAsync(orderNote!);
    await _context.SaveChangesAsync();
    return orderNote!;
  }
  public async Task<OrderNote> GetOrderNoteById(OrderNoteId orderNoteId)
  {
    var data = await _context.OrderNotes.FirstOrDefaultAsync(x => x.OrderNoteId! == orderNoteId);
    return data!;
  }
  public async Task<List<Order>> GetOrdersWithOrderNos(string? orderNos, ClientId clientId)
  {
    var list = await _context.Orders.Where(x => orderNos!.Contains(x.OrderNo!) && x.OrderNo != "" && x.ClientId == clientId).ToListAsync();
    return list;
  }
  public async Task<List<Order>> GetAllOrdersByRefNos(string? refNos, ClientId? clientId)
  {
    List<Order> orders = new();
    if (!string.IsNullOrEmpty(refNos))
    {
      foreach (var refNo in refNos!.Split(","))
      {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.CarrierTrackingNo == refNo && x.CarrierTrackingNo != "" && x.ClientId == clientId);
        if (order != null)
        {
          orders.Add(order!);
        }
      }
    }
    return orders;
  }

  public async Task<dynamic> BatchUpdateOrders(List<Order> orders)
  {
    _context.Orders.UpdateRange(orders!);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> UpdateOrderPaymentStatus(List<Order> orders)
  {
    _context.UpdateRange(orders!);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<Order?> GetOrderByOrderNo(string orderNo, ClientId clientId)
  {
    var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderNo == orderNo && x.ClientId == clientId);
    if (order is null)
    {
      order = await _context.Orders.FirstOrDefaultAsync(x => x.CarrierTrackingNo == orderNo || x.RefNo == orderNo && x.ClientId == clientId);
    }
    return order!;
  }

  public async Task<Order?> GetOrderByStripeInvoiceId(string stripeInvoiceId)
  {
    return await _context.Orders.FirstOrDefaultAsync(x => x.StripeInvoiceId == stripeInvoiceId);
  }
  public async Task<string?> GetAllSaleChannelConfigIdsByEmployeeIDs(string? clientId, string employeeIds)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@" SELECT e.SaleChannelConfigId
                          FROM dbo.Employee AS e
                              INNER JOIN dbo.Client AS c
                                  ON c.ClientId = e.ClientId ";
      string whereStart = "WHERE ( 1=1 AND ( e.SaleChannelConfigId IS NOT NULL ) ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (e.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(employeeIds))
      {
        dynamicParams.Add("@employeeIds", employeeIds);
        whereStart += "And ( ( e.EmployeeId in (select value from STRING_SPLIT(@employeeIds,',')))) ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync<int>(queryData, dynamicParams);
      var ids = string.Join(",", data.ToList());
      return ids;
    }

  }
  public async Task<StoreWitAddresshModel> GetStoreWithAddress(int? storeId, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@" SELECT s.StoreName,
                         s.CustomerServiceNo,
                         s.Email,
                         sa.FullAddress 
                  FROM dbo.Stores AS s
                      INNER JOIN dbo.StoreAddress AS sa
                          ON sa.StoreId = s.StoreId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (s.ClientId = @ClientId) ";
      }
      if (storeId.GetValueOrDefault() > 0)
      {
        dynamicParams.Add("@storeId", storeId);
        whereStart += "And ( s.StoreId = @storeId ) ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync<StoreWitAddresshModel>(queryData, dynamicParams);
      var first = data.FirstOrDefault()!;
      return first;
    }

  }

  public async Task<dynamic> GetAllOrders(
    DateTime? createdFrom, DateTime? createdTo,
    DateTime? orderFromDate, DateTime? orderToDate,
    int start, int length,
    string search,
    int sortCol, string sortDir,
    string clientId, string? storeIds,
    int? orderTypeId, string? carrierIds,
    int? fullFillmentStatusId, int? paymentStatusId,
    int? paymentMethodId, string? stationIds,
    bool readyForAssignment, int? carrierAssign = 0,
    string? saleChannelConfigIds = "", string? salePersonIds = "",
    string? countryIds = null, string? carrierTrackingStatusIds = null,
    Dictionary<string, AddressFilterModel>? addressFilter = null,
    string? orderLabels = null,
    bool? isWithoutStation = false)
  {
    using var connection = _dapperAppDbContext.CreateConnectionByClient(clientId);
    var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
    var dynamicParams = new DynamicParameters();

    dynamicParams.Add("displayStart", start);
    dynamicParams.Add("displayLength", length);

    //---------------------------------------
    // Top-level JOIN variable
    //---------------------------------------
    string joins = @"
        INNER JOIN dbo.OrderAddress AS oa ON o.OrderAddressId = oa.OrderAddressId
        INNER JOIN dbo.Stores AS s ON o.StoreId = s.StoreId
        LEFT JOIN dbo.SaleChannelConfig AS scc ON scc.SaleChannelConfigId = o.SaleChannelConfigId
        LEFT JOIN dbo.Employee AS e ON e.SaleChannelConfigId = o.SaleChannelConfigId
        LEFT JOIN dbo.Carrier AS c ON o.CarrierId = c.CarrierId
        INNER JOIN dbo.PaymentStatusLookup AS psl ON o.PaymentStatusId = psl.PaymentStatusId
        INNER JOIN dbo.FullFillmentStatusLookup AS ffs ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
        INNER JOIN dbo.OrderTypeLookup AS ot ON o.OrderTypeId = ot.OrderTypeId
        INNER JOIN dbo.PaymentMethodLookup AS pm ON o.PaymentMethodId = pm.PaymentMethodId
        LEFT JOIN dbo.ProductStation AS ps ON o.StationId = ps.ProductStationId
        INNER JOIN dbo.Client AS cl ON cl.ClientId = o.ClientId
        LEFT JOIN dbo.OrderDeliveryType AS odt ON odt.OrderDeliveryTypeId = o.OrderDeliveryTypeId
        LEFT JOIN dbo.PaymentLink AS pl ON pl.OrderId = o.OrderId
        LEFT JOIN dbo.MetaField AS mf ON o.OrderId = mf.EntityId
    ";

    //---------------------------------------
    // Base SELECT query
    //---------------------------------------
    string query = $@"SELECT 
        ROW_NUMBER() OVER (ORDER BY o.CreatedOn {(sortDir ?? "DESC")}) AS RowNum,
        COUNT(1) OVER() AS TotalCount,
        o.OrderId,
        o.OrderNo,
        ISNULL(o.RefNo, '') AS RefNo,
        ISNULL(o.ItemsCount, 0) AS NumberOfPieces,
        o.OrderDate,
        o.CreatedOn,
        ISNULL(c.IsClientCarrier, 0) AS IsClientCarrier,
        o.Amount,
        o.FulFilledDate,
        ISNULL(odt.TypeName, 'Forward ') AS DeliveryTypeName,
        ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
        ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
        o.CarrierLastUpdateDateTime,
        o.CarrierId,
        ISNULL(c.Name, '') AS CarrierName,
        ISNULL(c.CarrierImage, '') AS CarrierImage,
        ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus, 
        o.Description,
        o.Remarks,
        CASE
            WHEN o.OrderTypeId = 1 THEN ISNULL(ffs.FullFillmentStatus, 'Unfulfilled')
            ELSE ISNULL(ffs.FullFillmentStatus, '-')
        END AS FullFillmentStatus,
        o.FullFillmentStatusId,
        o.ItemsCount,
        ISNULL(psl.StatusName, '') AS PaymentStatus,
        ISNULL(pm.PMName, '') AS PaymentMethod,
        o.Weight,
        o.ItemValue,
        ISNULL(ps.Name, '') AS ProductStationName,
        o.Discount,
        o.TotalTax,
        ISNULL(o.PaymentMethodId, '-') AS PaymentMethodId,
        ISNULL(o.StripeInvoiceHostURL, '-') AS StripeInvoiceHostURL,
        ISNULL(o.StripeInvoicePDFURL, '-') AS StripeInvoicePDFURL,
        ISNULL(e.EmployeeName, '') AS SaleChannelName,
        scc.SaleChannelConfigId,
        CASE WHEN s.IsDefault = 1 THEN s.StoreName ELSE s.StoreName END AS StoreName,
        CASE WHEN o.CarrierId IS NULL THEN 'Unassigned' ELSE c.Name END AS Carrier,
        s.StoreImage,
        o.StoreId,
        s.CustomerServiceNo,
        ot.OrderTypeName,
        oa.CustomerName,
        oa.CustomerFullAddress,
        oa.Mobile1,
        o.ClientId,
        o.OrderLabels,
        ISNULL(pl.PaymentLinkUrl, '') AS PaymentLinkUrl,
        CASE WHEN mf.EntityId IS NULL THEN 0 ELSE 1 END AS IsMetaFieldExist
        FROM dbo.[Order] AS o
        {joins}";

    //---------------------------------------
    // Filters (all your existing filters go here)
    //---------------------------------------
    string whereStart = "WHERE (1=1 ";

    if (!string.IsNullOrEmpty(clientId))
    {
      dynamicParams.Add("@ClientId", clientId);
      whereStart += "AND (o.ClientId = @ClientId) ";
    }

    if (!string.IsNullOrEmpty(search))
    {
      dynamicParams.Add("@Search", search);
      whereStart += @" AND (
             (o.OrderNo IN (SELECT value FROM STRING_SPLIT(@Search, ',')) AND o.ClientId = @ClientId)
          OR (o.RefNo IN (SELECT value FROM STRING_SPLIT(@Search, ',')) AND o.ClientId = @ClientId)
          OR (o.CarrierTrackingNo IN (SELECT value FROM STRING_SPLIT(@Search, ',')) AND o.ClientId = @ClientId)
          OR (oa.Mobile1 IN (SELECT value FROM STRING_SPLIT(@Search, ',')) AND o.ClientId = @ClientId)
        ) ";
    }

    if (createdFrom != null)
    {
      dynamicParams.Add("@createdFrom", createdFrom);
      whereStart += $"AND (CAST(o.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
    }
    if (createdTo != null)
    {
      dynamicParams.Add("@createdTo", createdTo);
      whereStart += $"AND (CAST(o.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
    }
    if (orderFromDate != null)
    {
      dynamicParams.Add("@orderFromDate", orderFromDate);
      whereStart += $"AND (CAST({CommonUtility.GetFormatedDateStr("o.OrderDate", regionMinuts)} AS DATE) >= CAST(@orderFromDate AS DATE)) ";
    }
    if (orderToDate != null)
    {
      dynamicParams.Add("@orderToDate", orderToDate);
      whereStart += $"AND (CAST({CommonUtility.GetFormatedDateStr("o.OrderDate", regionMinuts)} AS DATE) <= CAST(@orderToDate AS DATE)) ";
    }

    if (!string.IsNullOrEmpty(storeIds) && storeIds != "0")
    {
      dynamicParams.Add("@storeIds", storeIds);
      whereStart += "AND (o.StoreId IN (SELECT value FROM STRING_SPLIT(@storeIds, ','))) ";
    }

    if (orderTypeId > 0)
    {
      dynamicParams.Add("@orderTypeId", orderTypeId);
      whereStart += "AND (o.OrderTypeId = @orderTypeId) ";
    }

    if (!string.IsNullOrEmpty(carrierIds) && carrierIds != "0")
    {
      dynamicParams.Add("@carrierId", carrierIds);
      whereStart += "AND (o.CarrierId IN (SELECT value FROM STRING_SPLIT(@carrierId, ','))) ";
    }

    if (fullFillmentStatusId > 0)
    {
      dynamicParams.Add("@fullFillmentStatusId", fullFillmentStatusId);
      whereStart += $"AND (o.FullFillmentStatusId = @fullFillmentStatusId AND o.OrderTypeId = {(int)EnumOrderType.FullFilable}) ";
    }

    if (paymentStatusId > 0)
    {
      dynamicParams.Add("@paymentStatusId", paymentStatusId);
      whereStart += "AND (o.PaymentStatusId = @paymentStatusId) ";
    }

    if (paymentMethodId > 0)
    {
      dynamicParams.Add("@paymentMethodId", paymentMethodId);
      whereStart += "AND (o.PaymentMethodId = @paymentMethodId) ";
    }

    if (!string.IsNullOrEmpty(stationIds) && stationIds != "0")
    {
      dynamicParams.Add("@stationIds", stationIds);
      whereStart += "AND (o.StationId IN (SELECT value FROM STRING_SPLIT(@stationIds, ','))) ";
    }

    if (!string.IsNullOrEmpty(saleChannelConfigIds))
    {
      dynamicParams.Add("@saleChannelConfigIds", saleChannelConfigIds);
      whereStart += "AND (o.SaleChannelConfigId IN (SELECT value FROM STRING_SPLIT(@saleChannelConfigIds, ','))) ";
    }

    if (!string.IsNullOrEmpty(salePersonIds))
    {
      salePersonIds = await GetAllSaleChannelConfigIdsByEmployeeIDs(clientId, salePersonIds);
      if (!string.IsNullOrEmpty(salePersonIds))
      {
        dynamicParams.Add("@salePersonIds", salePersonIds);
        whereStart += "AND (o.SaleChannelConfigId IN (SELECT value FROM STRING_SPLIT(@salePersonIds, ','))) ";
      }
    }

    if (!string.IsNullOrEmpty(countryIds))
    {
      dynamicParams.Add("@countryIds", countryIds);
      whereStart += "AND (oa.CountryId IN (SELECT value FROM STRING_SPLIT(@countryIds, ','))) ";
    }

    if (addressFilter != null && addressFilter.Count > 0)
    {
      foreach (var filter in addressFilter)
      {
        string tableColumn = CivilEntityHelper.GetTableEntityPropertyFromKey(filter.Key, false);
        var filterValues = filter.Value?.Id;
        bool include = filter.Value?.Include ?? true;
        if (!string.IsNullOrEmpty(filterValues))
        {
          string paramName = $"@{filter.Key}Ids";
          dynamicParams.Add(paramName, filterValues);
          if (include)
            whereStart += $"AND (oa.{tableColumn} IN (SELECT value FROM STRING_SPLIT({paramName}, ','))) ";
          else
            whereStart += $"AND (oa.{tableColumn} NOT IN (SELECT value FROM STRING_SPLIT({paramName}, ','))) ";
        }
      }
    }

    if (!string.IsNullOrEmpty(carrierTrackingStatusIds))
    {
      dynamicParams.Add("@carrierTrackingStatusIds", carrierTrackingStatusIds);
      whereStart += "AND (o.CarrierTrackingStatusId IN (SELECT value FROM STRING_SPLIT(@carrierTrackingStatusIds, ','))) ";
    }

    if (!string.IsNullOrEmpty(orderLabels) && orderLabels != "0")
    {
      dynamicParams.Add("@orderLabels", orderLabels);
      whereStart += @"AND EXISTS (
                            SELECT 1 
                            FROM dbo.ClientOrderLabel col
                            WHERE col.OrderId = o.OrderId
                              AND col.LabelName IN (SELECT value FROM STRING_SPLIT(@orderLabels, ','))) ";
    }

    if (isWithoutStation == true)
    {
      whereStart += "AND (o.StationId IS NULL OR o.StationId = 0) ";
    }

    if (carrierAssign != 0)
    {
      if (carrierAssign == (int)EnumCarrierAssign.Assigned)
        whereStart += "AND (o.CarrierId IS NOT NULL) ";
      else
        whereStart += "AND (o.CarrierId IS NULL) ";
    }

    whereStart += ")";

    string finalQuery = $@"
        SELECT * 
        FROM ({query} {whereStart}) AS T
        WHERE RowNum > @displayStart AND RowNum <= (@displayStart + @displayLength)
        ORDER BY RowNum";

    var data = await connection.QueryAsync(finalQuery, dynamicParams);

    int totalCount = 0;
    var dataList = data.ToList();
    if (dataList.Count > 0)
      totalCount = dataList.First().TotalCount;

    dynamic result = new ExpandoObject();
    result.TotalCount = totalCount;
    result.list = dataList;
    return result;
  }

  public async Task<dynamic> GetAllOrders_Back(DateTime? createdFrom, DateTime? createdTo, DateTime? orderFromDate, DateTime? orderToDate, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, bool readyForAssignment, int? carrierAssign = 0, string? saleChannelConfigIds = "", string? salePersonIds = "", string? countryIds = null, string? carrierTrackingStatusIds = null, Dictionary<string, AddressFilterModel>? addressFilter = null, string? orderLabels = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                                 COUNT(*) OVER () AS TotalCount,
                                 o.OrderId,
                                 o.OrderNo,
                                 ISNULL(o.RefNo, '') AS RefNo,
                                 ISNULL(o.ItemsCount, 0) AS NumberOfPieces,
                                 o.OrderDate,
                                 o.CreatedOn,
                                 ISNULL(c.IsClientCarrier, 0) AS IsClientCarrier,
                                 o.Amount,
                                 o.FulFilledDate,
                                 ISNULL(odt.TypeName, 'Forward ') AS DeliveryTypeName,
                                 ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                                 ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                                 o.CarrierLastUpdateDateTime,
                                 o.CarrierId,
                                 ISNULL(c.Name, '') AS CarrierName,
                                 ISNULL(c.CarrierImage, '') AS CarrierImage,
                                 ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus, 
                                 o.Description,
                                 o.Remarks,
                                 CASE
                                     WHEN o.OrderTypeId = 1 THEN
                                         ISNULL(ffs.FullFillmentStatus, 'Unfulfilled')
                                     ELSE
                                         ISNULL(ffs.FullFillmentStatus, '-')
                                 END AS FullFillmentStatus,
                                 o.FullFillmentStatusId,
                                 o.ItemsCount,
                                 ISNULL(psl.StatusName, '') AS PaymentStatus,
                                 ISNULL(pm.PMName, '') AS PaymentMethod,
                                 o.Weight,
                                 o.ItemValue,
                                 ISNULL(ps.Name, '') AS ProductStationName,
                                 o.Discount,
                                 o.TotalTax,
                                 ISNULL(o.PaymentMethodId, '-') AS PaymentMethodId,
                                 ISNULL(o.StripeInvoiceHostURL, '-') AS StripeInvoiceHostURL,
                                 ISNULL(o.StripeInvoicePDFURL, '-') AS StripeInvoicePDFURL,
                                 ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                                 scc.SaleChannelConfigId,
                                 CASE
                                     WHEN s.IsDefault = 1 THEN
                                         s.StoreName
                                     ELSE
                                         s.StoreName
                                 END AS StoreName,
                                 CASE
                                     WHEN o.CarrierId IS NULL THEN
                                         'Unassigned'
                                     ELSE
                                         c.Name
                                 END AS Carrier,
                                 s.StoreImage,
                                 o.StoreId,
                                 s.CustomerServiceNo,
                                 ot.OrderTypeName,
                                 oa.CustomerName,
                                 oa.CustomerFullAddress,
                                 oa.Mobile1,
                                 o.ClientId,
                                 o.OrderLabels,
                                 ISNULL(pl.PaymentLinkUrl, '') AS PaymentLinkUrl,
                                 CASE WHEN mf.EntityId IS NULL THEN 0 ELSE 1 END AS IsMetaFieldExist
                          FROM dbo.[Order] AS o
                              LEFT JOIN dbo.OrderAddress AS oa
                                  ON o.OrderAddressId = oa.OrderAddressId
                              INNER JOIN dbo.Stores AS s
                                  ON o.StoreId = s.StoreId
                              LEFT JOIN dbo.SaleChannelConfig AS scc
                                  ON scc.SaleChannelConfigId = o.SaleChannelConfigId
                              LEFT JOIN dbo.Carrier AS c
                                  ON o.CarrierId = c.CarrierId
                              LEFT JOIN dbo.PaymentStatusLookup AS psl
                                  ON o.PaymentStatusId = psl.PaymentStatusId
                              LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                  ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                              INNER JOIN dbo.OrderTypeLookup AS ot
                                  ON o.OrderTypeId = ot.OrderTypeId 
                              INNER JOIN dbo.PaymentMethodLookup AS pm
                                  ON o.PaymentMethodId = pm.PaymentMethodId
                              LEFT JOIN dbo.ProductStation AS ps
                                  ON o.StationId = ps.ProductStationId 
                              INNER JOIN dbo.Client AS cl
                                  ON cl.ClientId = o.ClientId 
                              LEFT JOIN dbo.OrderDeliveryType AS odt
                                    ON odt.OrderDeliveryTypeId = o.OrderDeliveryTypeId 
                              LEFT JOIN dbo.PaymentLink AS pl
                                      ON pl.OrderId = o.OrderId 
                              LEFT JOIN dbo.MetaField AS mf
                                  ON o.OrderId = mf.EntityId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@Search", search);
        whereStart += @" AND (
                 (o.OrderNo IN (SELECT value FROM STRING_SPLIT(@Search, ',')) AND o.ClientId = @ClientId)
              OR (o.RefNo IN (SELECT value FROM STRING_SPLIT(@Search, ',')) AND o.ClientId = @ClientId)
              OR (o.CarrierTrackingNo IN (SELECT value FROM STRING_SPLIT(@Search, ',')) AND o.ClientId = @ClientId)
            ) ";

      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      #region create-date
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
      #region order-date 
      if (orderFromDate != null)
      {
        dynamicParams.Add("@orderFromDate", orderFromDate);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.OrderDate", regionMinuts)} AS DATE)  >= CAST(@orderFromDate AS DATE)) ";
      }
      if (orderToDate != null)
      {
        dynamicParams.Add("@orderToDate", orderToDate);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.OrderDate", regionMinuts)} AS DATE)  <= CAST(@orderToDate AS DATE)) ";
      }

      #endregion 
      /////////order fileters////
      if (readyForAssignment)
      {
        /////carrier 
        //whereStart += $@"And ((
        //  o.OrderTypeId = {(int)EnumOrderType.Regular}
        //  AND o.CarrierId IS NULL
        //  AND o.FullFillmentStatusId IS NULL
        //  )
        //  OR
        //  (
        //      o.OrderTypeId = {(int)EnumOrderType.FullFilable}
        //      AND o.CarrierId IS NULL
        //      AND o.FullFillmentStatusId = {(int)EnumFullfillmentStatus.Fulfilled}
        //  )) ";
      }
      if (carrierAssign != 0)
      {
        /////carrier 

        if (carrierAssign == (int)EnumCarrierAssign.Assigned)
        {
          whereStart += @"And (o.CarrierId IS NOT NULL) ";
        }
        else
        {
          //carrier not assigned
          whereStart += @"And (o.CarrierId IS NULL) ";
        }
      }
      if (!string.IsNullOrEmpty(orderLabels) && orderLabels != "0")
      {
        dynamicParams.Add("@orderLabels", orderLabels);

        whereStart += @"
                        AND EXISTS (
                            SELECT 1 
                            FROM dbo.ClientOrderLabel col
                            WHERE col.OrderId = o.OrderId
                              AND col.LabelName IN (SELECT value FROM STRING_SPLIT(@orderLabels, ','))
                        )";
      }
      if (!string.IsNullOrEmpty(storeIds) && storeIds != "0")
      {
        dynamicParams.Add("@storeIds", storeIds);
        whereStart += "And ( ( o.StoreId in (select value from STRING_SPLIT(@storeIds,',')))) ";
      }
      if (orderTypeId > 0)
      {
        dynamicParams.Add("@orderTypeId", orderTypeId);
        whereStart += "And (o.OrderTypeId = @orderTypeId) ";
      }
      if (!string.IsNullOrEmpty(carrierIds) && carrierIds != "0")
      {
        dynamicParams.Add("@carrierId", carrierIds);
        whereStart += "And ( ( o.CarrierId in (select value from STRING_SPLIT(@carrierId,',')))) ";
      }
      if (fullFillmentStatusId > 0)
      {
        dynamicParams.Add("@fullFillmentStatusId", fullFillmentStatusId);
        whereStart += $"And (o.FullFillmentStatusId = @fullFillmentStatusId) And (o.OrderTypeId = {(int)EnumOrderType.FullFilable})  ";
      }
      if (paymentStatusId > 0)
      {
        dynamicParams.Add("@paymentStatusId", paymentStatusId);
        whereStart += "And (o.PaymentStatusId = @paymentStatusId) ";
      }
      if (paymentMethodId > 0)
      {
        dynamicParams.Add("@paymentMethodId", paymentMethodId);
        whereStart += "And (o.PaymentMethodId = @paymentMethodId) ";
      }
      if (!string.IsNullOrEmpty(stationIds) && stationIds != "0")
      {
        dynamicParams.Add("@stationIds", stationIds);
        whereStart += "And ( ( o.StationId in (select value from STRING_SPLIT(@stationIds,',')))) ";
      }
      if (!string.IsNullOrEmpty(saleChannelConfigIds))
      {
        dynamicParams.Add("@saleChannelConfigIds", saleChannelConfigIds);
        whereStart += "And ( ( o.SaleChannelConfigId in (select value from STRING_SPLIT(@saleChannelConfigIds,',')))) ";
      }
      if (!string.IsNullOrEmpty(salePersonIds))
      {
        salePersonIds = await GetAllSaleChannelConfigIdsByEmployeeIDs(clientId, salePersonIds);
        //on behalf of config
        if (!string.IsNullOrEmpty(salePersonIds))
        {
          dynamicParams.Add("@salePersonIds", salePersonIds);
          whereStart += "And ( ( o.SaleChannelConfigId in (select value from STRING_SPLIT(@salePersonIds,',')))) ";
        }
      }
      #region dynamic address filter
      if (!string.IsNullOrEmpty(countryIds))
      {
        dynamicParams.Add("@countryIds", countryIds);
        whereStart += "And ( ( oa.CountryId in (select value from STRING_SPLIT(@countryIds,',')))) ";
      }
      if (addressFilter != null && addressFilter.Count > 0)
      {
        //CivilEntityHelper
        foreach (var filter in addressFilter)
        {
          string tableColumn = CivilEntityHelper.GetTableEntityPropertyFromKey(filter.Key, false); // Get the mapped table column name
          var filterValues = filter.Value?.Id;
          bool include = filter.Value?.Include ?? true; // Default to include if null

          if (!string.IsNullOrEmpty(filterValues))
          {
            string paramName = $"@{filter.Key}Ids";
            dynamicParams.Add(paramName, filterValues);

            if (include)
            {
              whereStart += $" AND (oa.{tableColumn} IN (SELECT value FROM STRING_SPLIT({paramName}, ',')))";
            }
            else
            {
              whereStart += $" AND (oa.{tableColumn} NOT IN (SELECT value FROM STRING_SPLIT({paramName}, ',')))";
            }
          }
        }
      }
      #endregion

      if (!string.IsNullOrEmpty(carrierTrackingStatusIds))
      {
        dynamicParams.Add("@carrierTrackingStatusIds", carrierTrackingStatusIds);
        whereStart += "And ( ( o.CarrierTrackingStatusId in (select value from STRING_SPLIT(@carrierTrackingStatusIds,',')))) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");
      keyValuePairs.Add(1, "o.CreatedOn");


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
  public async Task<dynamic> GetAllOrderStatusReport(DateTime? filterDate, string clientId, int carrierId = 0)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinutes = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();

      dynamicParams.Add("@ClientId", clientId);
      dynamicParams.Add("@FilterDate", filterDate);
      dynamicParams.Add("@RegionMinutes", regionMinutes);
      dynamicParams.Add("@CarrierId", carrierId);

      string query = @"
                        WITH LatestHistory AS (
                          SELECT 
                              oth.OrderId,
                              oth.CarrierTrackingStatusId,
                              oth.CreatedOn,
                              ROW_NUMBER() OVER (PARTITION BY oth.OrderId ORDER BY oth.CreatedOn DESC) AS rn
                          FROM dbo.OrderTrackingHistory oth
                          INNER JOIN dbo.[Order] o ON o.OrderId = oth.OrderId
                          WHERE o.ClientId = @ClientId
                            AND CAST(DATEADD(MINUTE, @RegionMinutes, oth.CreatedOn) AS DATE) = @FilterDate
                      )

                      -- Only return orders that have tracking on the filter date
                      SELECT 
                          o.OrderNo,
                          ISNULL(o.RefNo, '') AS RefNo,
                          oa.CustomerName,
                          o.ClientId,
                          o.OrderDate,
                          CASE
                              WHEN oa.Mobile2 IS NOT NULL AND LTRIM(RTRIM(oa.Mobile2)) <> '' 
                              THEN oa.Mobile1 + ', ' + oa.Mobile2
                              ELSE oa.Mobile1
                          END AS CustomerMobiles,
                          o.CarrierTrackingStatus AS CurrentStatus,
                          o.CarrierLastUpdateDateTime AS CurrentStatusUpdatedDateTime,
                          cs1.TrackingStatus AS LastStatus,
                          lh.CreatedOn AS LastStatusUpdatedDateTime
                      FROM dbo.[Order] o
                      INNER JOIN dbo.OrderAddress oa ON oa.OrderAddressId = o.OrderAddressId
                      INNER JOIN LatestHistory lh ON lh.OrderId = o.OrderId AND lh.rn = 1
                      INNER JOIN dbo.ClientCarrierTrackingStatus cs1 
                             ON cs1.CarrierTrackingStatusId = lh.CarrierTrackingStatusId
                            AND cs1.ClientId = o.ClientId
                      WHERE o.ClientId = @ClientId
                      AND (@CarrierId IS NULL OR @CarrierId = 0 OR o.CarrierId = @CarrierId) 
                       GROUP BY ISNULL(o.RefNo, ''),
                                     CASE
                                     WHEN oa.Mobile2 IS NOT NULL
                                     AND LTRIM(RTRIM(oa.Mobile2)) <> '' THEN
                                     oa.Mobile1 + ', ' + oa.Mobile2
                                     ELSE
                                     oa.Mobile1
                                     END,
                                     o.OrderNo,
                                     oa.CustomerName,
                                     o.ClientId,
                                     o.OrderDate,
                                     o.CarrierTrackingStatus,
                                     o.CarrierLastUpdateDateTime,
                                     cs1.TrackingStatus,
                                     lh.CreatedOn
                      ORDER BY o.OrderNo DESC ";
      var data = await connection.QueryAsync(query, dynamicParams);

      dynamic result = new ExpandoObject();
      var dataList = data.ToList();
      result.TotalCount = dataList.Count;
      result.List = dataList;

      return result;
    }
  }


  public async Task<dynamic> GetAllOrdersForGeneratePaymentLink(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@" SELECT 
                         o.OrderAddressId,
                         o.OrderId,
                         o.OrderNo,
                         oa.Email
                  FROM dbo.[Order] AS o
                      LEFT JOIN dbo.OrderAddress AS oa
                          ON o.OrderAddressId = oa.OrderAddressId
                      INNER JOIN dbo.Client AS cl
                          ON cl.ClientId = o.ClientId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", $"%{search}%");

        whereStart += @$"
        AND (
            (o.OrderNo LIKE @search)
            OR (o.RefNo LIKE @search)
        )";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += "And (CAST(o.OrderDate AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += "And (CAST(o.OrderDate AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[0] + " " + " DESC " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      return data.ToList();
    }

  }
  public async Task<List<AllOrderPaymentLinkResponseModel>> GetAllOrderPaymentLinks(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, int? paymentLinkStatusId = 0, bool? isForPayoutRequest = false, string trackingPageUrl = "", DateTime? schedualDateFrom = null, DateTime? schedualDateTo = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT 
                            ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                            COUNT(*) OVER () AS TotalCount,
                            CAST(pl.PaymentLinkId AS VARCHAR(36)) AS PaymentLinkId,
                            COALESCE(CAST(pl.ServiceUUId AS VARCHAR(36)), '') AS ServiceUUId,
                            pl.PaymentLinkUrl,
                            o.OrderNo,
                            pl.Amount,
                            oa.CustomerName,
                            oa.Email,
                            plsl.StatusName,
                            pl.CreatedOn,
                            pl.PaymentReleaseDate,
                            pl.ScheduledPayoutDate,
                            pl.PaidOn,
                            '{trackingPageUrl}/' + '{clientId}' + '_' + o.OrderNo AS TrackingUrl ,
                            CASE
                               WHEN pl.PaymentLinkStatusId = 1 THEN
                                  1
                            ELSE
                                  0
                            END AS AllowRefreshPaymentStatus
                        FROM dbo.PaymentLink AS pl
                        INNER JOIN dbo.PaymentLinkStatusLookup AS plsl
                            ON plsl.PaymentLinkStatusId = pl.PaymentLinkStatusId
                        INNER JOIN dbo.[Order] AS o
                            ON o.OrderId = pl.OrderId
                        INNER JOIN dbo.OrderAddress AS oa
                            ON oa.OrderAddressId = o.OrderAddressId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @"And ( ( o.OrderNo in (select value from STRING_SPLIT(@Search,',')))) 
                        OR ( ( o.RefNo in (select value from STRING_SPLIT(@Search,','))))  ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      // get onlu links with unpaid links and not payout created
      if (isForPayoutRequest.GetValueOrDefault())
      {
        whereStart += "And (pl.PayoutId IS NULL) ";

        #region schedual date
        if (schedualDateFrom != null)
        {
          dynamicParams.Add("@schedualDateFrom", schedualDateFrom);
          whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("pl.ScheduledPayoutDate", regionMinuts)} AS DATE) >= CAST(@schedualDateFrom AS DATE)) ";
        }
        if (schedualDateTo != null)
        {
          dynamicParams.Add("@schedualDateTo", schedualDateTo);
          whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("pl.ScheduledPayoutDate", regionMinuts)} AS DATE) <= CAST(@schedualDateTo AS DATE)) ";
        }

        #endregion
      }
      else
      {
        #region create date
        if (createdFrom != null)
        {
          dynamicParams.Add("@createdFrom", createdFrom);
          whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("pl.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
        }
        if (createdTo != null)
        {
          dynamicParams.Add("@createdTo", createdTo);
          whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("pl.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
        }

        #endregion
      }



      if (paymentLinkStatusId > 0)
      {
        dynamicParams.Add("@paymentStatusId", paymentLinkStatusId);
        whereStart += "And (pl.PaymentLinkStatusId = @paymentStatusId) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "pl.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync<AllOrderPaymentLinkResponseModel>(queryData, dynamicParams);


      return data.ToList();
    }

  }

  public async Task<dynamic> GetAllOrdersForSalePerson(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, bool readyForAssignment, bool assigned, string salePersonId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               o.OrderId,
                               o.OrderNo,
                               o.OrderDate,
                               o.CreatedOn,
                               o.Amount,
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                               ISNULL(c.Name, '') AS CarrierName,
                               ISNULL(cts.TrackingStatus, '') AS TrackingStatus,
                               o.Description,
                               o.Remarks,
                               CASE
                                   WHEN o.OrderTypeId = {(int)EnumOrderType.FullFilable} THEN
                                       ISNULL(ffs.FullFillmentStatus, 'Unfulfilled')
                                   ELSE
                                       ISNULL(ffs.FullFillmentStatus, '-')
                               END AS FullFillmentStatus,
                               o.FullFillmentStatusId,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pm.PMName, '') AS PaymentMethod,
                               o.Weight,
                               o.ItemValue,
                               ISNULL(ps.Name, '') AS ProductStationName,
                               o.Discount,
                               o.VAT,
                               ISNULL(o.PaymentMethodId, '-') AS PaymentMethodId,
                               ISNULL(o.StripeInvoiceHostURL, '-') AS StripeInvoiceHostURL,
                               ISNULL(o.StripeInvoicePDFURL, '-') AS StripeInvoicePDFURL,
                               ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                               scc.SaleChannelConfigId,
                               CASE
                                   WHEN s.IsDefault = 1 THEN
                                       s.StoreName + ' (Default)'
                                   ELSE
                                       s.StoreName
                               END AS StoreName,
                               s.StoreAddress,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ot.OrderTypeName,
                               oa.CustomerName,
                               oa.CustomerFullAddress,
                               oa.Mobile1,
                               con.Name AS CountryName,
                               r.Name AS RegionName,
                               ct.Name AS CityName
                        FROM dbo.[Order] AS o
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            LEFT JOIN dbo.SaleChannelConfig AS scc
                                ON scc.SaleChannelConfigId = o.SaleChannelConfigId
                            INNER JOIN dbo.OrderAddress AS oa
                                ON o.OrderAddressId = oa.OrderAddressId
                            LEFT JOIN dbo.Country AS con
                                ON oa.CountryId = con.CountryId
                            LEFT JOIN dbo.Region AS r
                                ON oa.RegionId = r.RegionId
                            LEFT JOIN dbo.City AS ct
                                ON oa.CityId = ct.CityId
                            LEFT JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            LEFT JOIN dbo.PaymentStatusLookup AS psl
                                ON o.PaymentStatusId = psl.PaymentStatusId
                            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                            LEFT JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                            LEFT JOIN dbo.PaymentMethodLookup AS pm
                                ON o.PaymentMethodId = pm.PaymentMethodId
                            LEFT JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId  ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( o.OrderNo in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(salePersonId))
      {
        dynamicParams.Add("@SalePersonId", salePersonId);
        whereStart += "And (o.CreatedBy = @SalePersonId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += "And (CAST(o.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += "And (CAST(o.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      /////////order fileters////
      if (readyForAssignment)
      {
        /////carrier 
        whereStart += $@"And ((
          o.OrderTypeId = {(int)EnumOrderType.Regular}
          AND o.CarrierId IS NULL
          AND o.FullFillmentStatusId IS NULL
          )
          OR
          (
              o.OrderTypeId = {(int)EnumOrderType.FullFilable}
              AND o.CarrierId IS NULL
              AND o.FullFillmentStatusId = {(int)EnumFullfillmentStatus.Fulfilled}
          )) ";
      }
      if (assigned)
      {
        /////carrier 
        whereStart += @"And (o.CarrierId IS NOT NULL) ";
      }
      if (!string.IsNullOrEmpty(storeIds) && storeIds != "0")
      {
        dynamicParams.Add("@storeIds", storeIds);
        whereStart += "And ( ( o.StoreId in (select value from STRING_SPLIT(@storeIds,',')))) ";
      }
      if (orderTypeId > 0)
      {
        dynamicParams.Add("@orderTypeId", orderTypeId);
        whereStart += "And (o.OrderTypeId = @orderTypeId) ";
      }
      if (!string.IsNullOrEmpty(carrierIds) && carrierIds != "0")
      {
        dynamicParams.Add("@carrierId", carrierIds);
        whereStart += "And ( ( o.CarrierId in (select value from STRING_SPLIT(@carrierId,',')))) ";
      }
      if (fullFillmentStatusId > 0)
      {
        dynamicParams.Add("@fullFillmentStatusId", fullFillmentStatusId);
        if (fullFillmentStatusId != 1)
          whereStart += "And (o.FullFillmentStatusId = @fullFillmentStatusId) ";
        else
          whereStart += "And (o.FullFillmentStatusId=@fullFillmentStatusId OR o.FullFillmentStatusId IS NULL) AND o.FullFillmentStatusId IS NULL ";
      }
      if (paymentStatusId > 0)
      {
        dynamicParams.Add("@paymentStatusId", paymentStatusId);
        whereStart += "And (o.PaymentStatusId = @paymentStatusId) ";
      }
      if (paymentMethodId > 0)
      {
        dynamicParams.Add("@paymentMethodId", paymentMethodId);
        whereStart += "And (o.PaymentMethodId = @paymentMethodId) ";
      }
      if (!string.IsNullOrEmpty(stationIds) && stationIds != "0")
      {
        dynamicParams.Add("@stationIds", stationIds);
        whereStart += "And ( ( o.StationId in (select value from STRING_SPLIT(@stationIds,',')))) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");


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
  public async Task<dynamic> GetAllOrdersByTenantId(string? clientId, DateTime? createdFrom, DateTime? createdTo)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();

      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      string query = $@"SELECT 
                      ROW_NUMBER() OVER (ORDER BY (SELECT 1)) As RowNum,
                      COUNT(*) OVER () AS TotalCount, 
                      o.OrderId,
                      o.OrderNo,
                      o.OrderDate,
                      o.CreatedOn,
                      o.Amount,
                      ISNULL(o.CarrierTrackingNo,'') AS CarrierTrackingNo,
                      ISNULL(o.CarrierTrackingStatus,'') AS CarrierTrackingStatus,
                      ISNULL(cts.TrackingStatus,'') AS TrackingStatus,
                      ISNULL(c.Name,'') AS CarrierName,
                      o.Description,
                      o.Remarks,
                      ISNULL(ffs.FullFillmentStatus,'') AS FullFillmentStatus,
                      o.ItemsCount,
                      ISNULL(psl.StatusName,'') AS PaymentStatus,
                      ISNULL(pm.PMName,'') AS PaymentMethod,
                      o.Weight,
                      o.ItemValue,
                      ISNULL(ps.Name,'') AS ProductStationName,
                      o.Discount,
                      o.VAT,
                      ISNULL(s.StoreName,'') AS StoreName,
                      s.StoreAddress,
                      s.StoreImage,
                      s.CustomerServiceNo,
                      ot.OrderTypeName,
                      oa.CustomerName,
                      oa.CustomerFullAddress,
                      oa.Mobile1,
                      con.Name AS CountryName,
                      r.Name AS RegionName,
                      oa.CityName
                      FROM dbo.[Order] AS o
                      LEFT JOIN dbo.Stores AS s ON o.StoreId = s.StoreId
                      LEFT JOIN dbo.OrderAddress AS oa ON o.OrderAddressId = oa.OrderAddressId
                      LEFT JOIN dbo.Country AS con ON oa.CountryId = con.CountryId
                      LEFT JOIN dbo.Region AS r ON oa.RegionId = r.RegionId 
                      LEFT JOIN dbo.Carrier AS c ON o.CarrierId = c.CarrierId
                      LEFT JOIN dbo.PaymentStatusLookup AS psl ON o.PaymentStatusId = psl.PaymentStatusId
                      LEFT JOIN dbo.FullFillmentStatusLookup AS ffs ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                      LEFT JOIN dbo.OrderTypeLookup AS ot ON o.OrderTypeId = ot.OrderTypeId
                      INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                      LEFT JOIN dbo.PaymentMethodLookup AS pm ON o.PaymentMethodId = pm.PaymentMethodId
                      LEFT JOIN dbo.ProductStation AS ps ON o.StationId = ps.ProductStationId ";

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
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;


      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[0];

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
  public async Task<dynamic> GetOrderInfoByOrderNo(string orderNo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT o.OrderId,
                               o.OrderNo,
                               o.RefNo,
                               o.OrderDate,
                               o.CreatedOn,
                               ISNULL(o.Amount,0) AS Amount,
                               o.CarrierTrackingStatusId,
                               o.CarrierId,
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                               ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus,
                               ISNULL(c.Name, '') AS CarrierName,
                               o.Description,
                               o.Remarks,
                               ISNULL(ffs.FullFillmentStatus, '') AS FullFillmentStatus,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pm.PMName, '') AS PaymentMethod,
                               o.PaymentMethodId,
                               o.Weight,
                               ISNULL(o.OrderDeliveryTypeId, 1) AS OrderDeliveryTypeId,
                               ISNULL(odt.TypeName, 'Forward ') AS DeliveryTypeName,
                               o.ItemValue,
                               ISNULL(ps.Name, '') AS ProductStationName,
                               ISNULL(o.Discount,0) AS Discount,
                               o.VAT,
                               ISNULL(s.StoreName, '') AS StoreName, 
                               ISNULL(s.StoreImage, '') AS StoreImage, 
                               ISNULL(s.URLs, '') AS URLs, 
                               ISNULL(s.CustomerServiceNo, '') AS CustomerServiceNo,
                               ISNULL(s.StoreCompany, '') AS StoreCompany,
                               ISNULL(s.Email, '') AS StoreEmail,
                               ISNULL(sa.FullAddress, '') AS StoreAddress,
                               s.StoreId,
                               ISNULL(c2.Name, '') AS StoreCountry,
                               ot.OrderTypeName,
                               o.OrderAddressId,
                               oa.CustomerName,
                               o.OrderTypeId, 
                               oa.Mobile1, 
                               ISNULL(oa.Email, '') AS CustomerEmail,
                               ISNULL(oa.Mobile2, '') AS Mobile2, 
                               ISNULL(oa.CustomerFullAddress, '') AS CustomerAddress,
                               ISNULL(con.Name, '') AS ConsigneCountryName,
                               cl.ClientName, 
                               cl.ClientCode,
                               ISNULL(o.TotalTax,0) AS TotalTax,
                               cl.ClientCompanyName,
                               cl.Email,
                               cl.ClientId,
                               ISNULL(o.StripeInvoiceHostURL, '') AS StripeInvoiceHostURL,
                               ISNULL(o.StripeInvoicePDFURL, '') AS StripeInvoicePDFURL
                        FROM dbo.[Order] AS o
                            LEFT JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            LEFT JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = o.StoreId
                            INNER JOIN dbo.OrderAddress AS oa
                                ON o.OrderAddressId = oa.OrderAddressId
                            INNER JOIN dbo.Country AS con
                                ON oa.CountryId = con.CountryId
                            LEFT JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            LEFT JOIN dbo.PaymentStatusLookup AS psl
                                ON o.PaymentStatusId = psl.PaymentStatusId
                            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                            INNER JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId 
                            INNER JOIN dbo.PaymentMethodLookup AS pm
                                ON o.PaymentMethodId = pm.PaymentMethodId
                            LEFT JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId
                            LEFT JOIN dbo.Country AS c2
                                ON sa.CountryId = c2.CountryId
                            INNER JOIN dbo.Client AS cl
                                ON o.ClientId = cl.ClientId 
                            LEFT JOIN dbo.OrderDeliveryType AS odt
                                    ON odt.OrderDeliveryTypeId = o.OrderDeliveryTypeId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }

      if (!string.IsNullOrEmpty(orderNo))
      {
        dynamicParams.Add("@search", orderNo);
        whereStart += "And ( ( o.OrderNo in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }

  }
  public async Task<dynamic> GetOrderItemsInfoByOrderId(string orderId, string clientId)
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
                             oi.Price AS Price,
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
  public async Task<bool> DeleteOrderById(OrderNote orderNote)
  {
    _context.Remove(orderNote);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic?> GetOrderNoteByOrderNo(string orderNo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT 
                        orn.OrderId,
                        orn.NoteDescription,
                        orn.CreatedBy,
                        orn.CreatedOn,
                        orn.UpdatedBy,
                        orn.UpdatedOn,
                        orn.Active,
                        o.ClientId
                        FROM dbo.[OrderNote] as orn
                        LEFT JOIN dbo.[Order] as o ON o.OrderId = orn.OrderId ";
      string whereStart = " WHERE ( ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += " o.ClientId = @ClientId ";
      }
      if (!string.IsNullOrEmpty(orderNo))
      {
        dynamicParams.Add("@orderNo", orderNo);
        whereStart += "And o.OrderNo = @orderNo ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }

  public async Task<dynamic?> GetOrderTrackingHistoryByOrderNo(string orderNo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT cts.TrackingStatus,
                               ot.TrackingStatusComments,
                               ot.Latitude,
                               ot.Longitude,
                               ot.Location,
                               s.StoreName,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               CASE
                                   WHEN ot.OrderHistoryTypeId = {(int)EnumOrderHistoryType.ThirdParty} THEN
                                       ohtl.OrderHistoryTypeValue + ' - ' + c.Name
                                   ELSE
                                       ohtl.OrderHistoryTypeValue + ' - ' + e.EmployeeName
                               END AS CreatedByName,
                               ot.CreatedOn,
                               ot.OrderHistoryTypeId
                        FROM dbo.[OrderTrackingHistory] AS ot
                            INNER JOIN dbo.[Order] AS o
                                ON ot.OrderId = o.OrderId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                            INNER JOIN dbo.Employee AS e
                                ON e.EmployeeId = ot.CreatedBy
                            INNER JOIN dbo.OrderHistoryTypeLookup AS ohtl
                                ON ohtl.OrderHistoryTypeId = ot.OrderHistoryTypeId
                            LEFT JOIN dbo.Carrier AS c
                                ON c.CarrierId = o.CarrierId ";
      string whereStart = "WHERE ( ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "o.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(orderNo))
      {
        dynamicParams.Add("@orderNo", orderNo);
        whereStart += "And o.OrderNo = @orderNo ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }
  }

  public async Task<List<Order>> GetOrdersByOrderNos(string orderNos, ClientId clientId)
  {
    return await _context.Orders.Where(x => (orderNos.Contains(x.OrderNo!)) && x.ClientId == clientId).ToListAsync();
  }
  public async Task<List<Order>> GetOrdersByOrderIds(string orderIds, ClientId clientId)
  {
    List<OrderId> oIds = new List<OrderId>();
    foreach (var item in orderIds.Split(','))
    {
      var id = new OrderId(new Guid(item));
      oIds.Add(id);
    }
    return await _context.Orders.Where(x => oIds.Contains(x.OrderId!) && x.ClientId == clientId).ToListAsync();
  }

  public async Task<dynamic> GetAllCODPendings(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, string? carrierIds, int? orderTypeId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               o.OrderId,
                               o.OrderNo,
                               o.OrderDate,
                               o.CreatedOn,
                               o.Amount,
                               o.CarrierId,
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus, 
                               ISNULL(s.StoreName, '') AS StoreName,
                               sa.FullAddress AS StoreAddress,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pml.PMName, '') AS PaymentMethod,
                               ISNULL(c.Name, '') AS CarrierName,
                               ot.OrderTypeName,
                               ISNULL(c2.ClientCompanyName, c2.ClientName) AS ClientName
                        FROM dbo.[Order] AS o 
                            INNER JOIN dbo.PaymentStatusLookup AS psl
                                ON psl.PaymentStatusId = o.PaymentStatusId
                            INNER JOIN dbo.PaymentMethodLookup AS pml
                                ON pml.PaymentMethodId = o.PaymentMethodId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            LEFT JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = s.StoreId
                            INNER JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId
                            INNER JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            INNER JOIN dbo.Client AS c2
                                ON c2.ClientId = o.ClientId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {

        dynamicParams.Add("@search", search);
        whereStart += @"AND(
              TRIM(o.OrderNo)IN
              (
                  SELECT TRIM(value)FROM STRING_SPLIT(@Search, ',')
              )
              OR TRIM(o.RefNo)IN
                 (
                     SELECT TRIM(value)FROM STRING_SPLIT(@Search, ',')
                 )
              OR TRIM(o.CarrierTrackingNo)IN
                 (
                     SELECT TRIM(value)FROM STRING_SPLIT(@Search, ',')
                 )
          ) ";

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
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      /////////order fileters//// 
      if (!string.IsNullOrEmpty(storeIds) && storeIds != "0")
      {
        dynamicParams.Add("@storeIds", storeIds);
        whereStart += "And ( ( o.StoreId in (select value from STRING_SPLIT(@storeIds,',')))) ";
      }
      if (orderTypeId > 0)
      {
        dynamicParams.Add("@orderTypeId", orderTypeId);
        whereStart += "And (o.OrderTypeId = @orderTypeId) ";
      }
      if (!string.IsNullOrEmpty(carrierIds) && carrierIds != "0")
      {
        dynamicParams.Add("@carrierId", carrierIds);
        whereStart += "And ( ( o.CarrierId in (select value from STRING_SPLIT(@carrierId,',')))) ";
      }

      #region cod related filter
      whereStart += $"And (o.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered}) ";
      whereStart += $"And (o.CarrierPaymentSettlementId IS NULL) ";
      //whereStart += $"And (o.PaymentStatusId <> {(int)EnumPaymentStatus.Paid}) ";
      whereStart += $"AND o.CarrierId <> c2.DefaultCarrierId ";
      #endregion

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");


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
  public async Task<dynamic> UpdateCustomerEmail(OrderAddress orderAddress)
  {
    _context.OrderAddresses.Update(orderAddress);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel> CheckMobileNoDuplicate(string mobileNo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var duplicateDaysStr = await CommonUtility.GetClientGenericSettingValue(clientId, "others", "duplicateDays", _context);
      int.TryParse(duplicateDaysStr, out int duplicateDays);

      if (string.IsNullOrEmpty(mobileNo) || mobileNo.Length < 8)
      {
        return new Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel { IsDuplicate = false, OrderNo = "", DaysAgo = 0 };
      }
      if (duplicateDays <= 0)
      {
        duplicateDays = 3; // Default fallback to 3 days for check validation
      }

      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@mobileNo", mobileNo);
      dynamicParams.Add("@duplicateDays", duplicateDays);

      string query = $@"
        SELECT TOP 1 
            o.OrderNo, 
            DATEDIFF(day, o.CreatedOn, GETUTCDATE()) as DaysAgo
        FROM dbo.[Order] o
        INNER JOIN dbo.[OrderAddress] oa ON o.OrderAddressId = oa.OrderAddressId
        WHERE o.ClientId = '{clientId}'
          AND CAST(o.CreatedOn AS DATE) >= DATEADD(day, -@duplicateDays, CAST(GETUTCDATE() AS DATE))
          AND (
            (LEN(oa.Mobile1) >= 8 AND RIGHT(oa.Mobile1, 8) = RIGHT(@mobileNo, 8)) OR
            (LEN(oa.Mobile2) >= 8 AND RIGHT(oa.Mobile2, 8) = RIGHT(@mobileNo, 8))
          )
        ORDER BY o.CreatedOn DESC";

      var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query, dynamicParams);
      if (result != null)
      {
        return new Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel { IsDuplicate = true, OrderNo = (string)result.OrderNo, DaysAgo = (int)result.DaysAgo, MobileNo = mobileNo };
      }

      return new Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel { IsDuplicate = false, OrderNo = "", DaysAgo = 0, MobileNo = mobileNo };
    }
  }

  public async Task<List<Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel>> CheckMobileNosDuplicateBulk(List<string> mobileNos, string clientId)
  {
    var responseList = new List<Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel>();
    if (mobileNos == null || !mobileNos.Any()) return responseList;

    var sanitizedMobiles = mobileNos
      .Where(m => !string.IsNullOrEmpty(m))
      .Select(m => new string(m.Where(char.IsDigit).ToArray()))
      .Where(m => m.Length >= 8)
      .Distinct()
      .ToList();

    if (!sanitizedMobiles.Any()) return responseList;

    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var duplicateDaysStr = await CommonUtility.GetClientGenericSettingValue(clientId, "others", "duplicateDays", _context);
      int.TryParse(duplicateDaysStr, out int duplicateDays);
      if (duplicateDays <= 0) duplicateDays = 3;

      // Extract last 8 digits of input numbers to match the database index logic
      var suffixMap = sanitizedMobiles.ToDictionary(m => m.Substring(m.Length - 8), m => m);
      var suffixes = suffixMap.Keys.ToList();

      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@duplicateDays", duplicateDays);
      dynamicParams.Add("@suffixes", suffixes);

      string query = $@"
        SELECT 
            o.OrderNo, 
            DATEDIFF(day, o.CreatedOn, GETUTCDATE()) as DaysAgo,
            oa.Mobile1,
            oa.Mobile2
        FROM dbo.[Order] o
        INNER JOIN dbo.[OrderAddress] oa ON o.OrderAddressId = oa.OrderAddressId
        WHERE o.ClientId = '{clientId}'
          AND CAST(o.CreatedOn AS DATE) >= DATEADD(day, -@duplicateDays, CAST(GETUTCDATE() AS DATE))
          AND (
            (LEN(REPLACE(REPLACE(REPLACE(oa.Mobile1, ' ', ''), '-', ''), '+', '')) >= 8 AND RIGHT(REPLACE(REPLACE(REPLACE(oa.Mobile1, ' ', ''), '-', ''), '+', ''), 8) IN @suffixes) OR
            (LEN(REPLACE(REPLACE(REPLACE(oa.Mobile2, ' ', ''), '-', ''), '+', '')) >= 8 AND RIGHT(REPLACE(REPLACE(REPLACE(oa.Mobile2, ' ', ''), '-', ''), '+', ''), 8) IN @suffixes)
          )";

      var queryResult = await connection.QueryAsync<dynamic>(query, dynamicParams);
      var queryList = queryResult.ToList();

      foreach (var mobile in sanitizedMobiles)
      {
        var suffix = mobile.Substring(mobile.Length - 8);
        var match = queryList.FirstOrDefault(q => 
          ((string)q.Mobile1 != null && new string(((string)q.Mobile1).Where(char.IsDigit).ToArray()).EndsWith(suffix)) ||
          ((string)q.Mobile2 != null && new string(((string)q.Mobile2).Where(char.IsDigit).ToArray()).EndsWith(suffix))
        );

        if (match != null)
        {
          responseList.Add(new Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel 
          { 
            IsDuplicate = true, 
            OrderNo = (string)match.OrderNo, 
            DaysAgo = (int)match.DaysAgo,
            MobileNo = mobile
          });
        }
        else
        {
          responseList.Add(new Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel 
          { 
            IsDuplicate = false, 
            OrderNo = "", 
            DaysAgo = 0,
            MobileNo = mobile
          });
        }
      }
    }
    return responseList;
  }
  public async Task<dynamic> GetOrderDetailByReturnReportFile(string orderIds, string trackingNos, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT COUNT(*) OVER () AS TotalCount,
                               o.OrderId,
                               o.OrderNo,
                               o.OrderDate,
                               o.CreatedOn,
                               o.Amount,
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                               c.CarrierId,
                               ISNULL(c.Name, '') AS CarrierName,
                               ISNULL(cts.TrackingStatus, '') AS TrackingStatus,
                               o.Description,
                               o.Remarks,
                               ISNULL(ffs.FullFillmentStatus, 'Unfulfilled') AS FullFillmentStatus,
                               o.FullFillmentStatusId,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pm.PMName, '') AS PaymentMethod,
                               o.Weight,
                               o.ItemValue,
                               ISNULL(ps.Name, '') AS ProductStationName,
                               o.Discount,
                               o.VAT,
                               ISNULL(o.PaymentMethodId, '-') AS PaymentMethodId,
                               ISNULL(o.StripeInvoiceHostURL, '-') AS StripeInvoiceHostURL,
                               ISNULL(o.StripeInvoicePDFURL, '-') AS StripeInvoicePDFURL,
                               ISNULL(s.StoreName, '') AS StoreName,
                               sa.FullAddress AS StoreAddress,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ot.OrderTypeName,
                               oa.CustomerName,
                               oa.CustomerFullAddress,
                               oa.Mobile1,
                               con.Name AS CountryName
                        FROM dbo.[Order] AS o
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            LEFT JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = s.StoreId
                            INNER JOIN dbo.OrderAddress AS oa
                                ON o.OrderAddressId = oa.OrderAddressId
                            INNER JOIN dbo.Country AS con
                                ON oa.CountryId = con.CountryId
                            INNER JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            LEFT JOIN dbo.PaymentStatusLookup AS psl
                                ON o.PaymentStatusId = psl.PaymentStatusId
                            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                            LEFT JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                             AND cts.ClientId = '{clientId}'
                            LEFT JOIN dbo.PaymentMethodLookup AS pm
                                ON o.PaymentMethodId = pm.PaymentMethodId
                            INNER JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(orderIds) && orderIds != "0")
      {
        dynamicParams.Add("@orderIds", orderIds);
        whereStart += "And ( ( o.OrderNo in (select value from STRING_SPLIT(@orderIds,',')))) ";
      }
      if (!string.IsNullOrEmpty(orderIds) && orderIds != "0")
      {
        dynamicParams.Add("@trackingNos", trackingNos);
        whereStart += "And ( ( o.CarrierTrackingNo in (select value from STRING_SPLIT(@trackingNos,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);

      return data.ToList();
    }
  }
  //same method also used inside job repository please also check that one
  #region new algo
  public async Task<ClientGenericSetting> GetGenericSettingByClientIdAsync(ClientId clientId)
  {
    var data = await _context.ClientGenericSettings.FirstOrDefaultAsync(x => x.ClientId == clientId);
    return data!;
  }
  public async Task<Employee> GetEmployee(EmployeeId employeeId, ClientId clientId, int saleChannelConfigId)
  {
    Employee? employee = null;
    if (saleChannelConfigId > 0)
    {
      var oSaleConfig = await _context.SaleChannelConfigs.FirstOrDefaultAsync(x => x.SaleChannelConfigId == saleChannelConfigId);
      if (oSaleConfig is not null && oSaleConfig.SaleChannelLookupId == (int)EnumSaleChannelLookup.SalePerson)
      {
        employee = await _context.Employees.FirstOrDefaultAsync(x => x.SaleChannelConfigId == saleChannelConfigId! && x.ClientId == clientId!);
      }
    }
    else
    {
      employee = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId! && x.ClientId == clientId!);
    }
    return employee!;
  }
  public async Task<string> GetClientNextOrderNo(ClientId clientId, EmployeeId employeeId, int roleId = 0, int? saleChannelConfigId = 0)
  {
    int orderCount;
    int? clientIdentifier = await _context.Clients.Where(x => x.ClientId == clientId && x.Active == true).Select(x => x.ClientIdentifier).FirstOrDefaultAsync();

    if (clientIdentifier is null)
      throw new Exception("Client not found or inactive.");

    var orders = await _context.Orders.Where(x => x.ClientId == clientId).ToListAsync();


    if (roleId == (int)EnumUserRole.SalePerson || saleChannelConfigId > 0)
    {
      var setting = await GetGenericSettingByClientIdAsync(clientId);
      if (setting is not null && !string.IsNullOrEmpty(setting.SettingConfig!))
      {
        string orderSerial = UtilityHelper.GetClientSettingValueWithByKey(setting.SettingConfig!, "order", "orderNo");
        // Compare the enum name with the string value
        bool isMatch = EnumOrderSerial.SalePersonSerial.ToString() == orderSerial;
        if (!string.IsNullOrEmpty(orderSerial) && isMatch)
        {
          var employee = await GetEmployee(employeeId, clientId, saleChannelConfigId.GetValueOrDefault());
          if (employee != null && saleChannelConfigId > 0)
          {
            employeeId = employee.EmployeeId!;
          }
          if (employee is not null)
          {
            orderCount = orders.Count(x => x.CreatedBy == employeeId) + 1;
            return await GenerateUniqueSalePersonOrderNoAsync(employee!.EmployeeName!, clientIdentifier.Value, orderCount, clientId);
          }
        }
      }
    }

    orderCount = orders.Count + 1;
    return await GenerateUniqueOrderNoAsync(clientIdentifier.Value, clientId, orderCount);
  }
  #region generate uniquer order no
  private async Task<string> GenerateUniqueOrderNoAsync(int clientIdentifier, ClientId clientId, int orderCount)
  {
    string newOrderNo;

    do
    {
      newOrderNo = AppDefaultNextOrderNumber(clientIdentifier, orderCount++);
    }
    while (await _context.Orders.AnyAsync(x => x.ClientId == clientId && x.OrderNo == newOrderNo));

    return newOrderNo;
  }
  public async Task<string> GenerateUniqueSalePersonOrderNoAsync(string salePerson, int clientIdentifier, int initialOrderCount, ClientId clientId)
  {
    int orderCount = initialOrderCount;
    string orderNo;

    do
    {
      orderNo = GenerateSalePersonOrderNo(salePerson, clientIdentifier, orderCount++);
    }
    while (await _context.Orders.AnyAsync(x => x.ClientId == clientId && x.OrderNo == orderNo));

    return orderNo;
  }

  #endregion
  #endregion
  #region tracking no
  //public async Task<string> GetClientNextOrderNo(ClientId? clientId, string? prefix = "")
  //{
  //  int orderCount = 0;
  //  int? clientIdentifier = 0;

  //  var client = await _context.Clients
  //      .Where(x => x.ClientId == clientId && x.Active == true)
  //      .FirstOrDefaultAsync();

  //  if (client is not null)
  //  {
  //    clientIdentifier = client.ClientIdentifier;
  //    orderCount = await _context.Orders.CountAsync(x => x.ClientId == clientId);
  //    orderCount++;
  //  }

  //  // Get new order no of client
  //  string clientNextOrderNo = AppDefaultNextOrderNumber(clientIdentifier, orderCount); // ST1001
  //  if (!string.IsNullOrEmpty(prefix))
  //  {
  //    // prefix identifier 
  //    string newNumberWithPrefixForSalePerson = prefix! + clientIdentifier;
  //    clientNextOrderNo = AppDefaultNextOrderNumber(newNumberWithPrefixForSalePerson, orderCount);
  //  }
  //  // Duplicate order number check, if exists then create a new one using recursion
  //  if (!string.IsNullOrEmpty(clientNextOrderNo))
  //  {
  //    var orderExist = await _context.Orders
  //        .FirstOrDefaultAsync(x => x.ClientId == clientId && x.OrderNo!.ToLower() == clientNextOrderNo.ToLower());

  //    if (orderExist is not null)
  //    {
  //      // If the order number already exists, generate a new unique order number recursively.
  //      clientNextOrderNo = await GenerateUniqueOrderNoAsync(clientIdentifier.GetValueOrDefault(), clientId!, orderCount);
  //    }
  //  }

  //  return clientNextOrderNo;
  //}

  private static string AppDefaultNextOrderNumber(dynamic? clientIdentifier, int? clientOrderCount)
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
  public static string GenerateSalePersonOrderNo(string salePerson, int clientIdentifier, int orderCount)
  {
    // Get first 3 characters of SalePerson name (uppercase)

    string salePersonCode = UtilityHelper.GenerateEmployeeCode(salePerson!, true, 3);

    // Ensure ClientIdentifier is 3-digit (pad if needed)
    string clientCode = clientIdentifier.ToString().PadLeft(3, '0');

    // Format orderCount: if ≤ 999, pad to 3 digits; else, keep as is
    string orderCode = orderCount <= 999 ? orderCount.ToString("D3") : orderCount.ToString();

    return $"{salePersonCode}{clientCode}{orderCode}";
  }

  #endregion

  public async Task<OrderNote?> GetOrderNoteByOrderId(OrderId orderId)
  {
    return await _context.OrderNotes.FirstOrDefaultAsync(x => x.OrderId == orderId);
  }

  public async Task<dynamic> GetOrderForDriverById(string? orderId, string? driverId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT 
                          ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY o.CreatedOn)) AS RowNum,
                          COUNT(*) OVER () AS TotalOrderCount,
                          o.OrderId,
                          o.OrderNo,
                          o.OrderAddressId,
                          o.StoreId,
                          o.ItemsCount AS NoOfPieces,
                          o.Weight,
                          o.Amount,
						              oa.ReferenceNo,
                          CASE
                              WHEN pml.PaymentMethodId =1 THEN 'PP'
                              ELSE 'Cash On Delivery'
                          END AS PaymentMethod,
                          CASE
                           WHEN NULLIF(LTRIM(RTRIM(o.Description)), '') IS NOT NULL THEN
                               o.Description
                           ELSE
                               ISNULL(oa.Description, '')
                       END AS Description,
                       ISNULL(o.Remarks, '') AS Remarks
                      FROM dbo.[Order] AS o
					            INNER JOIN OrderItem AS oa
							             ON oa.OrderId=o.OrderId
                      INNER JOIN PaymentMethodLookup AS pml 
                           ON pml.PaymentMethodId = o.PaymentMethodId
                      INNER JOIN DeliveryNoteDetail AS dnd 
                           ON dnd.OrderId = o.OrderId
                      INNER JOIN DeliveryNote AS dn 
                           ON dn.DeliveryNoteId = dnd.DeliveryNoteId  ";
      string whereStart = "WHERE (1=1 ";
      string whereEnd = ")";

      #region orderId
      if (!string.IsNullOrEmpty(orderId))
      {
        dynamicParams.Add("@orderId", orderId);
        whereStart += "AND (o.orderId = @orderId) ";
      }
      #endregion

      #region driverId
      if (!string.IsNullOrEmpty(driverId))
      {
        dynamicParams.Add("@driverId", driverId);
        whereStart += "AND (dn.driverId = @driverId) ";
      }
      #endregion

      string where = whereStart + whereEnd;
      var data = await connection.QueryAsync(query + where, dynamicParams);
      return data.FirstOrDefault()!;
    }
  }

  public async Task<OrderItem> GetOrderItemByIdAndOrderItemId(OrderId orderId, OrderItemId orderItemId)
  {
    var data = await _context.OrderItems.FirstOrDefaultAsync(x => x.OrderId == orderId && x.OrderItemId == orderItemId);
    return data!;
  }

  public async Task<OrderItem> DeleteOrderItem(OrderItem oOrderItem)
  {
    _context.Remove(oOrderItem);
    await _context.SaveChangesAsync();
    return oOrderItem;
  }
  public async Task<OrderPODFile> CreateOrderPODFiles(OrderPODFile orderPODFile)
  {
    await _context.OrderPODFiles.AddAsync(orderPODFile);
    await _context.SaveChangesAsync();
    return orderPODFile;
  }

  public async Task<dynamic?> GetOrderTrackingHistoryByOrderNoForView(string orderNo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT cts.TrackingStatus,
                               ot.TrackingStatusComments, 
                               s.StoreName,
                               s.StoreImage,
                               s.CustomerServiceNo ,
                               ot.CreatedOn 
                        FROM dbo.[OrderTrackingHistory] AS ot
                            INNER JOIN dbo.[Order] AS o
                                ON ot.OrderId = o.OrderId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON ot.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}' 
                            INNER JOIN dbo.OrderHistoryTypeLookup AS ohtl
                                ON ohtl.OrderHistoryTypeId = ot.OrderHistoryTypeId  ";
      string whereStart = "WHERE ( ";
      string whereEnd = ")";
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "(o.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(orderNo))
      {
        dynamicParams.Add("@orderNo", orderNo);
        whereStart += "And (o.OrderNo = @orderNo) ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where + " ORDER BY ot.CreatedOn desc ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.ToList();
    }

  }
  public Task<OrderNote> UpdateOrderNote(OrderNote orderNote)
  {
    throw new NotImplementedException();
  }
  #region carrier return report
  public async Task<dynamic> GetAllCarrierPendingForReturnShipments(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               o.OrderNo,
                               o.OrderId,
                               ISNULL(o.CarrierTrackingNo, '') AS TrackingNo,
                               cts.TrackingStatus AS CarrierTrackingStatus,
                               cts.CarrierTrackingStatusId,
                               PML.Code AS PaymentMethodStatus,
                               o.PaymentMethodId,
                               o.Amount,
                               oa.CustomerName AS Customer,
                               oa.Mobile1 AS Phone,
                               o.Remarks,
                               c.CarrierImage,
                               c.Name AS CarrierName
                        FROM dbo.[Order] AS o
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                            INNER JOIN dbo.OrderAddress AS oa
                                ON oa.OrderAddressId = o.OrderAddressId
                            INNER JOIN dbo.PaymentMethodLookup AS PML
                                ON PML.PaymentMethodId = o.PaymentMethodId
                            INNER JOIN dbo.Carrier AS c
                                ON c.CarrierId = o.CarrierId
                            INNER JOIN dbo.Client AS c2
                                ON c2.ClientId = o.ClientId   ";


      string whereStart = "WHERE ( 1=1 AND (o.CarrierId <> c2.DefaultCarrierId) AND (c.IsClientCarrier <> 1) "; //remove default carrier 
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      //pending for return filter
      dynamicParams.Add("@carrierTrackingStatusId", (int)EnumCarrierTrackingStatus.PendingForReturn);
      whereStart += $"And ((o.CarrierTrackingStatusId) = {(int)EnumCarrierTrackingStatus.PendingForReturn} ) ";

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( o.OrderNo in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dn.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dn.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");


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
  public async Task<bool> CreateOrderDeleted(OrderDeleted orderDeleted)
  {
    await _context.OrderDeleteds.AddAsync(orderDeleted);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<long> GetOrderCount(ClientId clientId)
  {
    return await _context.Orders.CountAsync(x => x.ClientId == clientId);
  }
  #endregion
  #region ordertax

  public async Task<bool> CreateOrderTax(OrderTax orderTax)
  {
    await _context.OrderTaxes.AddAsync(orderTax);
    return await _context.SaveChangesAsync() > 0;
  }


  public async Task<OrderTax?> GetOrderTaxById(OrderTaxId orderTaxId)
  {
    return await _context.OrderTaxes.FirstOrDefaultAsync(x => x.OrderTaxId == orderTaxId);
  }

  public async Task<List<OrderTax>?> GetAllOrderTaxByOrderId(OrderId orderId)
  {
    return await _context.OrderTaxes.Where(x => x.OrderId == orderId).ToListAsync();

  }

  public async Task<bool> UpdateOrderTax(OrderTax orderTax)
  {
    _context.OrderTaxes.Update(orderTax);
    return await _context.SaveChangesAsync() > 0;
  }
  #endregion
  #region order file
  public async Task<List<OrderUplaodSampleFile>> GetOrderUplaodSampleFile()
  {
    return await _context.OrderUplaodSampleFiles.ToListAsync();
  }

  #endregion
  #region order label
  public async Task<List<ClientOrderLabel>> GetAllClientOrderLabelByOrderId(ClientId clientId, OrderId orderId)
  {
    var data = await _context.ClientOrderLabels.Where(x => x.ClientId == clientId && x.OrderId == orderId).ToListAsync();
    return data!;
  }
  public async Task<ClientOrderLabel> GetClientOrderLabelById(ClientOrderLabelId? ClientOrderLabelId, ClientId clientId)
  {
    var data = await _context.ClientOrderLabels.FirstOrDefaultAsync(x => x.ClientOrderLabelId == ClientOrderLabelId && x.ClientId == clientId);
    return data!;

  }

  public async Task<bool> CreateClientOrderLabel(ClientOrderLabel clientOrderLabel)
  {
    await _context.ClientOrderLabels.AddAsync(clientOrderLabel);
    return await _context.SaveChangesAsync() > 0;

  }

  public async Task<bool> DeleteClientOrderLabel(ClientOrderLabel clientOrderLabel)
  {
    _context.ClientOrderLabels.Remove(clientOrderLabel!);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<ClientOrderLabel>> GetAllClientOrderLabelForSelection(ClientId clientId)
  {
    var data = await _context.ClientOrderLabels.Where(x => x.ClientId == clientId).ToListAsync();
    return data!;
  }
  #endregion
  #region order label lookup 
  public async Task<ClientOrderLabelLookup> GetClientOrderLabelLookupId(int? clientOrderLabelLookupId, ClientId clientId)
  {
    var data = await _context.ClientOrderLabelLookups.FirstOrDefaultAsync(x => x.ClientOrderLabelLookupId == clientOrderLabelLookupId && x.ClientId == clientId);
    return data!;

  }

  public async Task<bool> CreateClientOrderLabelLookup(ClientOrderLabelLookup clientOrderLabel)
  {
    await _context.ClientOrderLabelLookups.AddAsync(clientOrderLabel);
    return await _context.SaveChangesAsync() > 0;

  }

  public async Task<bool> UpdateClientOrderLabelLookup(ClientOrderLabelLookup clientOrderLabel)
  {
    _context.ClientOrderLabelLookups.Update(clientOrderLabel!);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> DeleteClientOrderLabelLookup(ClientOrderLabelLookup clientOrderLabel)
  {
    _context.ClientOrderLabelLookups.Remove(clientOrderLabel!);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<ClientOrderLabelLookup>> GetAllClientOrderLabelLookupForSelection(ClientId clientId)
  {
    var data = await _context.ClientOrderLabelLookups.Where(x => x.ClientId == clientId).ToListAsync();
    return data!;
  }
  public async Task<dynamic> GetAllClientOrderLabelLookup(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT TOP (1) 1 ORDER BY cll.CreatedOn)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               cll.ClientOrderLabelLookupId,
                               cll.LabelName,
                               cll.ColorCode,
                               cll.Active,
                               cll.CreatedOn
                        FROM ClientOrderLabelLookup  AS cll ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( cll.LabelName in (select value from STRING_SPLIT(@Search,',')))) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (cll.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("cll.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("cll.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }


      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "cll.CreatedOn");


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

  public async Task<List<OrderTrackingHistory>?> GetOrderTrackingHistoryByOrderId(OrderId orderId)
  {
    var data = await _context.OrderTrackingHistories.Where(x => x.OrderId == orderId).ToListAsync();
    return data;
  }
  public async Task<bool> DeleteOrderTrackingHistory(List<OrderTrackingHistory> orderTrackingHistories)
  {
    _context.OrderTrackingHistories.RemoveRange(orderTrackingHistories);
    return await _context.SaveChangesAsync() > 0;
  }

  #endregion
  #region draft order
  public async Task<bool> CreateOrderDraft(OrderDraft orderDraft)
  {
    await _context.OrderDrafts.AddAsync(orderDraft);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> UpdateOrderDraft(OrderDraft orderDraft)
  {
    _context.OrderDrafts.Update(orderDraft);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<OrderDraft> GetDraftOrderById(long orderDraftId, ClientId clientId)
  {
    var data = await _context.OrderDrafts.FirstOrDefaultAsync(x => x.OrderDraftId == orderDraftId && x.ClientId == clientId);
    return data!;
  }

  public async Task<bool> DeleteDraftOrder(OrderDraft orderDraft)
  {
    _context.OrderDrafts.RemoveRange(orderDraft);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<OrderDraft>> GetAllDraftOrders(ClientId? clientId)
  {
    var data = await _context.OrderDrafts.Where(x => x.ClientId == clientId).ToListAsync();
    return data;
  }
  public async Task<int> GetAllDraftOrdersCount(ClientId? clientId)
  {
    return await _context.OrderDrafts.Where(x => x.ClientId == clientId).CountAsync();
  }
  public async Task<ClientConfigSetting?> GetClientConfigSetting(ClientId clientId)
  {
    return await _context.ClientConfigSettings.FirstOrDefaultAsync(x => x.ClientId == clientId);
  }

  public async Task<string?> GetMapApiKey()
  {
    using (var connection = _dapperAppDbContext.CreateConnection())
    {
      var sql = @"
            SELECT mk.MapKey1
            FROM dbo.MapKey mk
            INNER JOIN dbo.MapKeysInUse mkiu 
                ON mk.MapkeyId = mkiu.MapkeyId
        ";

      var result = await connection.QueryFirstOrDefaultAsync<string>(sql);
      return result!;
    }
  }

  public async Task<List<AdvanceSearchOrderDto>> AdvanceSearchOrders(string? searchType, string? searchQuery, int start, int length, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@ClientId", clientId);
      dynamicParams.Add("@DisplayStart", start);
      dynamicParams.Add("@DisplayLength", length);

      string where = " WHERE o.ClientId = @ClientId ";

      if (!string.IsNullOrWhiteSpace(searchQuery))
      {
        dynamicParams.Add("@SearchQuery", $"%{searchQuery.Trim()}%");
        string type = (searchType ?? "All").Trim().ToLower();

        if (type == "orderno")
        {
          where += " AND o.OrderNo LIKE @SearchQuery ";
        }
        else if (type == "name")
        {
          where += " AND oa.CustomerName LIKE @SearchQuery ";
        }
        else if (type == "mobile")
        {
          where += " AND (oa.Mobile1 LIKE @SearchQuery OR oa.Mobile2 LIKE @SearchQuery) ";
        }
        else if (type == "refno")
        {
          where += " AND o.RefNo LIKE @SearchQuery ";
        }
        else if (type == "trackingno")
        {
          where += " AND o.CarrierTrackingNo LIKE @SearchQuery ";
        }
        else // "all"
        {
          where += @" AND (
            o.OrderNo LIKE @SearchQuery OR 
            oa.CustomerName LIKE @SearchQuery OR 
            oa.Mobile1 LIKE @SearchQuery OR 
            oa.Mobile2 LIKE @SearchQuery OR 
            o.RefNo LIKE @SearchQuery OR 
            o.CarrierTrackingNo LIKE @SearchQuery
          ) ";
        }
      }

      string query = $@"
        SELECT
            ROW_NUMBER() OVER(ORDER BY o.CreatedOn DESC) AS RowNum,
            COUNT(*) OVER() AS TotalCount,
            o.OrderId,
            o.OrderNo,
            oa.CustomerName,
            oa.Mobile1,
            o.RefNo,
            o.CarrierTrackingNo,
            o.OrderDate,
            o.Amount,
            o.Description,
            o.Remarks
        FROM dbo.[Order] o
        LEFT JOIN dbo.OrderAddress oa ON o.OrderAddressId = oa.OrderAddressId
        {where}
        ORDER BY o.CreatedOn DESC
        OFFSET @DisplayStart ROWS FETCH NEXT @DisplayLength ROWS ONLY;
      ";

      var list = await connection.QueryAsync<AdvanceSearchOrderDto>(query, dynamicParams);
      return list.ToList();
    }
  }

  #endregion
}
