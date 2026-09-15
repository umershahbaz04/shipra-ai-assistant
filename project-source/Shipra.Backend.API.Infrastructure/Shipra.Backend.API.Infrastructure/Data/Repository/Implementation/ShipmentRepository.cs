using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ShipmentRepository : IShipmentRepository
{
  private readonly IOrderRepository _orderRepository;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;
  private readonly IShipmentStatusCommonRepository _shipmentStatusCommonRepository;

  public ShipmentRepository(IOrderRepository orderRepository, DapperAppDbContext dapperAppDbContext, AppDbContext context, IShipmentStatusCommonRepository shipmentStatusCommonRepository)
  {
    _orderRepository = orderRepository;
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
    _shipmentStatusCommonRepository = shipmentStatusCommonRepository;
  }
  public async Task<dynamic> GetAllShipments(DateTime? createdFrom, DateTime? createdTo, DateTime? orderFromDate, DateTime? orderToDate, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, string? carrierTrackingStatusIds, string? saleChannelConfigIds, string? salePersonIds, string? countryIds = null, Dictionary<string, AddressFilterModel>? addressFilter = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();
      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               o.OrderId,
                               o.OrderNo,
                               o.RefNo,
                               o.OrderDate,
                               o.CreatedOn,
                               o.Amount,
                               o.CarrierId,
                               o.CarrierLastUpdateDateTime,
                               ISNULL(c.IsClientCarrier, 0) AS IsClientCarrier,
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus,
                               o.CarrierTrackingStatusId, 
                               ISNULL(c.Name, '') AS CarrierName,
                               ISNULL(c.CarrierImage, '') AS CarrierImage,
                               o.Description,
                               o.Remarks,
                               o.FulFilledDate,
                               ISNULL(ffs.FullFillmentStatus, 'Unfulfilled') AS FullFillmentStatus,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, 'Unpaid') AS PaymentStatus,
                               ISNULL(pm.PMName, '') AS PaymentMethod,
                               o.Weight,
                               o.ItemValue,
                               ISNULL(ps.Name, '') AS ProductStationName,
                               o.Discount,
                               o.TotalTax,
                               ISNULL(odt.TypeName, 'Forward ') AS DeliveryTypeName,
                               ISNULL(s.StoreName, '') AS StoreName,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ot.OrderTypeName,
                               oa.OrderAddressId,
                               oa.CustomerName,
                               oa.CustomerFullAddress,
                               oa.Mobile1,
                               ISNULL(oa.Email, '') AS CustomerEmail,
                               con.Name AS CountryName,
                               ISNULL(o.StripeInvoiceHostURL, '') AS StripeInvoiceHostURL,
                               ISNULL(o.StripeInvoicePDFURL, '') AS StripeInvoicePDFURL
                        FROM dbo.[Order] AS o
                            LEFT JOIN dbo.OrderAddress AS oa
                                ON o.OrderAddressId = oa.OrderAddressId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            INNER JOIN dbo.Country AS con
                                ON oa.CountryId = con.CountryId
                            INNER JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            INNER JOIN dbo.PaymentStatusLookup AS psl
                                ON o.PaymentStatusId = psl.PaymentStatusId
                            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                            INNER JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId 
                            INNER JOIN dbo.PaymentMethodLookup AS pm
                                ON o.PaymentMethodId = pm.PaymentMethodId
                            LEFT JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId
                            INNER JOIN dbo.Client AS c2
                                ON o.ClientId = c2.ClientId
                            LEFT JOIN dbo.OrderDeliveryType AS odt
                                    ON odt.OrderDeliveryTypeId = o.OrderDeliveryTypeId ";

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
      //whereStart += @"And c2.CarrierId <> o.CarrierId ";

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
      if (!string.IsNullOrEmpty(carrierTrackingStatusIds) && carrierTrackingStatusIds != "0")
      {
        dynamicParams.Add("@carrierTrackingStatusIds", carrierTrackingStatusIds);
        whereStart += "And ( ( o.CarrierTrackingStatusId in (select value from STRING_SPLIT(@carrierTrackingStatusIds,',')))) ";
      }
      if (fullFillmentStatusId > 0)
      {
        dynamicParams.Add("@fullFillmentStatusId", fullFillmentStatusId);
        whereStart += "And (o.FullFillmentStatusId = @fullFillmentStatusId) ";
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
        salePersonIds = await _orderRepository.GetAllSaleChannelConfigIdsByEmployeeIDs(clientId, salePersonIds);
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
  public async Task<dynamic> GetAllShipmentsByDriverReceivableId(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? driverReceivableId, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
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
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                               ISNULL(c.Name, '') AS CarrierName,
                               ISNULL(c2.ClientCompanyName, c2.ClientName) AS ClientName,
                               ISNULL(cts.TrackingStatus, '') AS TrackingStatus,
                               o.Description,
                               o.Remarks,
                               CASE
                                   WHEN o.OrderTypeId = 1 THEN
                                       ISNULL(ffs.FullFillmentStatus, 'Unfulfilled')
                                   ELSE
                                       ISNULL(ffs.FullFillmentStatus, '-')
                               END AS FullFillmentStatus,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pm.PMName, '') AS PaymentMethod,
                               o.Weight,
                               ISNULL(ps.Name, '') AS StationName,
                               o.Discount,
                               o.VAT,
                               ISNULL(o.PaymentMethodId, '-') AS PaymentMethodId,
                               ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                               scc.SaleChannelConfigId,
                               CASE
                                   WHEN s.IsDefault = 1 THEN
                                       s.StoreName
                                   ELSE
                                       s.StoreName
                               END AS StoreName,
	                           sa.FullAddress AS StoreAddress,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ot.OrderTypeName,
                               oa.CustomerName,
                               oa.CustomerFullAddress,
                               oa.Mobile1,
                               con.Name AS CountryName,
                               dr.DriverReceivableNo
                        FROM dbo.DriverReceivable AS dr
                            INNER JOIN dbo.DeliveryNote AS dn
                                ON dn.DeliveryNoteId = dr.DeliveryNoteId
                            INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                            INNER JOIN dbo.[Order] AS o
                                ON o.OrderId = dnd.OrderId
                            LEFT JOIN dbo.OrderAddress AS oa
                                ON oa.OrderAddressId = o.OrderAddressId
                            INNER JOIN dbo.Stores AS s
                                ON s.StoreId = o.StoreId
                            LEFT JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = s.StoreId
                            INNER JOIN dbo.Country AS con
                                ON oa.CountryId = con.CountryId
                            INNER JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                            INNER JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                            INNER JOIN dbo.PaymentMethodLookup AS pm
                                ON o.PaymentMethodId = pm.PaymentMethodId
                            INNER JOIN dbo.Client AS c2
                                ON c2.ClientId = o.ClientId
                            INNER JOIN dbo.PaymentStatusLookup AS psl
                                ON o.PaymentStatusId = psl.PaymentStatusId
                            LEFT JOIN dbo.SaleChannelConfig AS scc
                                ON scc.SaleChannelConfigId = o.SaleChannelConfigId
                            INNER JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId ";

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

      //whereStart += @"And c2.CarrierId <> o.CarrierId "; 
      if (!string.IsNullOrEmpty(driverReceivableId))
      {
        dynamicParams.Add("@driverReceivableId", driverReceivableId);
        whereStart += "And ( dr.DriverReceivableId = @driverReceivableId ) ";
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
  public async Task<dynamic> GetAllShipmentsDetailByDriverReceivableId(string? driverReceivableId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
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
                               ISNULL(c2.ClientCompanyName, c2.ClientName) AS ClientName,
                               ISNULL(cts.TrackingStatus, '') AS TrackingStatus,
                               o.Description,
                               o.Remarks,
                               CASE
                                   WHEN o.OrderTypeId = 1 THEN
                                       ISNULL(ffs.FullFillmentStatus, 'Unfulfilled')
                                   ELSE
                                       ISNULL(ffs.FullFillmentStatus, '-')
                               END AS FullFillmentStatus,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pm.PMName, '') AS PaymentMethod,
                               o.Weight,
                               ISNULL(ps.Name, '') AS StationName,
                               o.Discount,
                               o.VAT,
                               ISNULL(o.PaymentMethodId, '-') AS PaymentMethodId,
                               ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                               scc.SaleChannelConfigId,
                               CASE
                                   WHEN s.IsDefault = 1 THEN
                                       s.StoreName
                                   ELSE
                                       s.StoreName
                               END AS StoreName,
                               sa.FullAddress AS StoreAddress,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ot.OrderTypeName,
                               oa.CustomerName,
                               oa.CustomerFullAddress,
                               oa.Mobile1,
                               con.Name AS CountryName,
                               dr.DriverReceivableNo
                        FROM dbo.DriverReceivable AS dr
                            INNER JOIN dbo.DeliveryNote AS dn
                                ON dn.DeliveryNoteId = dr.DeliveryNoteId
                            INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                            INNER JOIN dbo.[Order] AS o
                                ON o.OrderId = dnd.OrderId
                            INNER JOIN dbo.OrderAddress AS oa
                                ON oa.OrderAddressId = o.OrderAddressId
                            INNER JOIN dbo.Stores AS s
                                ON s.StoreId = o.StoreId
                            LEFT JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = s.StoreId
                            INNER JOIN dbo.Country AS con
                                ON oa.CountryId = con.CountryId
                            INNER JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                            INNER JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                            INNER JOIN dbo.PaymentMethodLookup AS pm
                                ON o.PaymentMethodId = pm.PaymentMethodId
                            INNER JOIN dbo.Client AS c2
                                ON c2.ClientId = o.ClientId
                            INNER JOIN dbo.PaymentStatusLookup AS psl
                                ON o.PaymentStatusId = psl.PaymentStatusId
                            LEFT JOIN dbo.SaleChannelConfig AS scc
                                ON scc.SaleChannelConfigId = o.SaleChannelConfigId
                            INNER JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId; ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      //whereStart += @"And c2.CarrierId <> o.CarrierId "; 
      if (!string.IsNullOrEmpty(driverReceivableId))
      {
        dynamicParams.Add("@driverReceivableId", driverReceivableId);
        whereStart += "And ( dr.DriverReceivableId = @driverReceivableId ) ";
      }

      string where = whereStart + whereEnd;
      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");

      string queryData = query + where + " ORDER BY " + keyValuePairs[0] + " " + "DESC";

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
  public async Task<dynamic> GetAllShipmentsByReturnReportId(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? carrierRRId, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
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
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                               ISNULL(c.Name, '') AS CarrierName,
                               ISNULL(c2.ClientCompanyName, c2.ClientName) AS ClientName,
                               ISNULL(cts.TrackingStatus, '') AS TrackingStatus,
                               o.Description,
                               o.Remarks,
                               CASE
                                   WHEN o.OrderTypeId = 1 THEN
                                       ISNULL(ffs.FullFillmentStatus, 'Unfulfilled')
                                   ELSE
                                       ISNULL(ffs.FullFillmentStatus, '-')
                               END AS FullFillmentStatus,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pm.PMName, '') AS PaymentMethod,
                               o.Weight,
                               ISNULL(ps.Name, '') AS StationName,
                               o.Discount,
                               o.VAT,
                               ISNULL(o.PaymentMethodId, '-') AS PaymentMethodId,
                               ISNULL(scc.SaleChannelName, '') AS SaleChannelName,
                               scc.SaleChannelConfigId,
                               CASE
                                   WHEN s.IsDefault = 1 THEN
                                       s.StoreName
                                   ELSE
                                       s.StoreName
                               END AS StoreName,
                               sa.FullAddress AS StoreAddress,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ot.OrderTypeName,
                               oa.CustomerName,
                               oa.CustomerFullAddress,
                               oa.Mobile1,
                               con.Name AS CountryName,
                               crr.ReturnReportNo
                        FROM dbo.[Order] AS o
                            INNER JOIN dbo.OrderAddress AS oa
                                ON oa.OrderAddressId = o.OrderAddressId
                            INNER JOIN dbo.CarrierReturnReport AS crr
                                ON crr.CarrierRRId = o.CarrierRRId
                            INNER JOIN dbo.Stores AS s
                                ON s.StoreId = o.StoreId
                            LEFT JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = s.StoreId
                            INNER JOIN dbo.Country AS con
                                ON oa.CountryId = con.CountryId
                            LEFT JOIN dbo.City AS ct
                                ON oa.CityId = ct.CityId
                            INNER JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                            INNER JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                            INNER JOIN dbo.PaymentMethodLookup AS pm
                                ON o.PaymentMethodId = pm.PaymentMethodId
                            INNER JOIN dbo.Client AS c2
                                ON c2.ClientId = o.ClientId
                            INNER JOIN dbo.PaymentStatusLookup AS psl
                                ON o.PaymentStatusId = psl.PaymentStatusId
                            LEFT JOIN dbo.SaleChannelConfig AS scc
                                ON scc.SaleChannelConfigId = o.SaleChannelConfigId
                            INNER JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId ";

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
      //whereStart += @"And c2.CarrierId <> o.CarrierId "; 
      if (!string.IsNullOrEmpty(carrierRRId))
      {
        dynamicParams.Add("@carrierRRId", carrierRRId);
        whereStart += "And ( o.CarrierRRId = @carrierRRId ) ";
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
  public async Task<List<OrderPODFile>> GetOrderPodFilesByOrderId(OrderId orderId)
  {
    return await _context.OrderPODFiles.Where(x => x.OrderId == orderId).ToListAsync();
  }
  public async Task<ActiveCarrierPickupLocation?> GetActiveCarrierPickupLocationById(int? activecarrierpickuplocationid)
  {
    return await _context.ActiveCarrierPickupLocations.FirstOrDefaultAsync(x => x.ActiveCarrierPickupLocationId == activecarrierpickuplocationid);
  }

  public async Task<dynamic> GetAllShipmentTabsCount2134(DateTime? createdFrom, DateTime? createdTo, DateTime? orderFromDate, DateTime? orderToDate, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, string? carrierTrackingStatusIds, string? saleChannelConfigIds, string? salePersonIds, string? countryIds = null, Dictionary<string, AddressFilterModel>? addressFilter = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();

      //var settingDashboard = await _context.DefaultShipmentDashboards.ToListAsync();
      //ClientId cId = new ClientId(new Guid(clientId!))!;
      List<ShipmentDashboardResponseModel> oShipmentGridClientSettings = await _shipmentStatusCommonRepository.GetAllShipmentGridClientSettings(clientId);

      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < oShipmentGridClientSettings.Count; i++)
      {
        string comma = "";
        if (i < oShipmentGridClientSettings.Count - 1)
        {
          comma = ", ";
        }
        var item = oShipmentGridClientSettings[i];
        if (item.DashboardStatusName?.ToLower() == "all")
        {
          sb.Append($"{"TotalShipment "} = COUNT (*){comma} ");
        }
        else
        {
          if (string.IsNullOrEmpty(item.DashboardStatusValue))
          {
            item.DashboardStatusValue = "0";
          }
          sb.Append($"[{item.DashboardStatusNameForKey}] = COUNT ( CASE WHEN o.CarrierTrackingStatusId IN ({item.DashboardStatusValue}) THEN 0 END ){comma} ");
        }
      }
      string query = @$"SELECT  
                       {sb.ToString()} ,
                       ISNULL(o.CarrierId, '') AS CarrierId,
                       ISNULL(c.Name, '') AS CarrierName,
                       ISNULL(c.CarrierImage, '') AS CarrierImage
                        FROM dbo.[Order] AS o
                        INNER JOIN dbo.Stores AS s
                            ON o.StoreId = s.StoreId
                        INNER JOIN dbo.OrderAddress AS oa
                            ON o.OrderAddressId = oa.OrderAddressId
                        INNER JOIN dbo.Country AS con
                            ON oa.CountryId = con.CountryId 
                        INNER JOIN dbo.Carrier AS c
                            ON o.CarrierId = c.CarrierId
                        INNER JOIN dbo.PaymentStatusLookup AS psl
                            ON o.PaymentStatusId = psl.PaymentStatusId
                        LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                            ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                        INNER JOIN dbo.OrderTypeLookup AS ot
                            ON o.OrderTypeId = ot.OrderTypeId 
                        INNER JOIN dbo.PaymentMethodLookup AS pm
                            ON o.PaymentMethodId = pm.PaymentMethodId
                        LEFT JOIN dbo.ProductStation AS ps
                            ON o.StationId = ps.ProductStationId 
                        INNER JOIN dbo.Client AS c2
                                ON c2.ClientId = o.ClientId ";


      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";
      #region filters

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += @"And ( ( o.OrderNo in (select value from STRING_SPLIT(@Search,',')))) 
                        OR ( ( o.RefNo in (select value from STRING_SPLIT(@Search,',')))) 
                        OR ( ( o.CarrierTrackingNo in (select value from STRING_SPLIT(@Search,',')))) ";
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

      whereStart += @"And (o.CarrierId IS NOT NULL) ";

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
      if (!string.IsNullOrEmpty(carrierTrackingStatusIds) && carrierTrackingStatusIds != "0")
      {
        dynamicParams.Add("@carrierTrackingStatusIds", carrierTrackingStatusIds);
        whereStart += "And ( ( o.CarrierTrackingStatusId in (select value from STRING_SPLIT(@carrierTrackingStatusIds,',')))) ";
      }
      if (fullFillmentStatusId > 0)
      {
        dynamicParams.Add("@fullFillmentStatusId", fullFillmentStatusId);
        whereStart += "And (o.FullFillmentStatusId = @fullFillmentStatusId) ";
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
        dynamicParams.Add("@salePersonIds", salePersonIds);
        whereStart += "And ( ( o.CreatedBy in (select value from STRING_SPLIT(@salePersonIds,',')))) ";
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

      #endregion
      string where = whereStart + whereEnd;

      string queryData = query + where + " ORDER BY(SELECT NULL)  " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";



      var data = await connection.QueryAsync(queryData, dynamicParams);

      var result = new { list = data.ToList() };

      return result;
    }
  }

  public async Task<dynamic> GetAllShipmentTabsCount(DateTime? createdFrom, DateTime? createdTo, DateTime? orderFromDate, DateTime? orderToDate, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, string? carrierTrackingStatusIds, string? saleChannelConfigIds, string? salePersonIds, string? countryIds = null, Dictionary<string, AddressFilterModel>? addressFilter = null)
  {
    using var connection = _dapperAppDbContext.CreateConnectionByClient(clientId);

    var regionMinutes = await CommonUtility.GetClientRegionMinutes(clientId, _context);
    var dynamicParams = new DynamicParameters();

    List<ShipmentDashboardResponseModel> dashboardSettings =
        await _shipmentStatusCommonRepository.GetAllShipmentGridClientSettings(clientId);

    #region SELECT BUILDER
    StringBuilder sb = new StringBuilder();

    for (int i = 0; i < dashboardSettings.Count; i++)
    {
      var item = dashboardSettings[i];
      string comma = i < dashboardSettings.Count - 1 ? "," : "";

      if (item.DashboardStatusName?.Equals("all", StringComparison.OrdinalIgnoreCase) == true)
      {
        sb.Append($"TotalShipment = COUNT(*){comma} ");
      }
      else
      {
        item.DashboardStatusValue ??= "0";
        sb.Append($@"
                [{item.DashboardStatusNameForKey}] =
                SUM(CASE 
                        WHEN o.CarrierTrackingStatusId IN ({item.DashboardStatusValue}) 
                        THEN 1 ELSE 0 
                    END){comma} ");
      }
    }
    #endregion

    #region BASE QUERY (reusable)
    string baseQuery = $@"
    FROM dbo.[Order] o
    INNER JOIN dbo.Stores s ON o.StoreId = s.StoreId
    INNER JOIN dbo.OrderAddress oa ON o.OrderAddressId = oa.OrderAddressId
    INNER JOIN dbo.Country con ON oa.CountryId = con.CountryId
    INNER JOIN dbo.Carrier c ON o.CarrierId = c.CarrierId
    INNER JOIN dbo.PaymentStatusLookup psl ON o.PaymentStatusId = psl.PaymentStatusId
    LEFT JOIN dbo.FullFillmentStatusLookup ffs ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
    INNER JOIN dbo.OrderTypeLookup ot ON o.OrderTypeId = ot.OrderTypeId
    INNER JOIN dbo.PaymentMethodLookup pm ON o.PaymentMethodId = pm.PaymentMethodId
    LEFT JOIN dbo.ProductStation ps ON o.StationId = ps.ProductStationId
    INNER JOIN dbo.Client cl ON cl.ClientId = o.ClientId
    ";
    #endregion

    #region WHERE BUILDER
    var where = new StringBuilder(" WHERE 1=1 ");

    dynamicParams.Add("@displayStart", start);
    dynamicParams.Add("@displayLength", length);

    if (!string.IsNullOrWhiteSpace(search))
    {
      dynamicParams.Add("@Search", search.Trim());
      where.Append(@"
            AND (
                o.OrderNo IN (SELECT value FROM STRING_SPLIT(@Search, ','))
                OR o.RefNo IN (SELECT value FROM STRING_SPLIT(@Search, ','))
                OR o.CarrierTrackingNo IN (SELECT value FROM STRING_SPLIT(@Search, ','))
            ) ");
    }

    where.Append(" AND o.ClientId = @ClientId ");
    dynamicParams.Add("@ClientId", clientId);

    #region DATE FILTERS
    if (createdFrom.HasValue)
    {
      dynamicParams.Add("@createdFrom", createdFrom);
      where.Append($" AND CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinutes)} AS DATE) >= CAST(@createdFrom AS DATE)");
    }

    if (createdTo.HasValue)
    {
      dynamicParams.Add("@createdTo", createdTo);
      where.Append($" AND CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinutes)} AS DATE) <= CAST(@createdTo AS DATE)");
    }

    if (orderFromDate.HasValue)
    {
      dynamicParams.Add("@orderFromDate", orderFromDate);
      where.Append($" AND CAST({CommonUtility.GetFormatedDateStr("o.OrderDate", regionMinutes)} AS DATE) >= CAST(@orderFromDate AS DATE)");
    }

    if (orderToDate.HasValue)
    {
      dynamicParams.Add("@orderToDate", orderToDate);
      where.Append($" AND CAST({CommonUtility.GetFormatedDateStr("o.OrderDate", regionMinutes)} AS DATE) <= CAST(@orderToDate AS DATE)");
    }
    #endregion

    where.Append(" AND o.CarrierId IS NOT NULL ");

    void AddInFilter(string? value, string column, string param)
    {
      if (!string.IsNullOrWhiteSpace(value) && value != "0")
      {
        dynamicParams.Add(param, value);
        where.Append($" AND {column} IN (SELECT value FROM STRING_SPLIT({param}, ','))");
      }
    }

    AddInFilter(storeIds, "o.StoreId", "@storeIds");
    AddInFilter(carrierIds, "o.CarrierId", "@carrierIds");
    AddInFilter(carrierTrackingStatusIds, "o.CarrierTrackingStatusId", "@carrierTrackingStatusIds");
    AddInFilter(stationIds, "o.StationId", "@stationIds");
    AddInFilter(saleChannelConfigIds, "o.SaleChannelConfigId", "@saleChannelConfigIds");
    AddInFilter(salePersonIds, "o.CreatedBy", "@salePersonIds");
    AddInFilter(countryIds, "oa.CountryId", "@countryIds");

    if (orderTypeId > 0) { dynamicParams.Add("@orderTypeId", orderTypeId); where.Append(" AND o.OrderTypeId = @orderTypeId"); }
    if (fullFillmentStatusId > 0) { dynamicParams.Add("@fullFillmentStatusId", fullFillmentStatusId); where.Append(" AND o.FullFillmentStatusId = @fullFillmentStatusId"); }
    if (paymentStatusId > 0) { dynamicParams.Add("@paymentStatusId", paymentStatusId); where.Append(" AND o.PaymentStatusId = @paymentStatusId"); }
    if (paymentMethodId > 0) { dynamicParams.Add("@paymentMethodId", paymentMethodId); where.Append(" AND o.PaymentMethodId = @paymentMethodId"); }
    #endregion

    #region FINAL QUERIES
    // 1️⃣ Flat List (as-is)
    string flatQuery = $@"
    SELECT
        {sb}
    {baseQuery}
    {where}
    ORDER BY (SELECT NULL)
    OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY;
";

    // 2️⃣ Grouped List (with pagination)
    string groupedQuery = $@"
    SELECT
        {sb},
        ISNULL(o.CarrierId,'') AS CarrierId,
        ISNULL(c.Name,'') AS CarrierName,
        ISNULL(c.CarrierImage,'') AS CarrierImage
    {baseQuery}
    {where}
    GROUP BY o.CarrierId, c.Name, c.CarrierImage
    ORDER BY c.Name
    OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY;
";
    #endregion

    #region EXECUTE BOTH
    var flatData = await connection.QueryAsync(flatQuery, dynamicParams);
    var groupedData = await connection.QueryAsync(groupedQuery, dynamicParams);
    #endregion

    #region RETURN BOTH
    return new
    {
      list = flatData.ToList(),
      carrierGoupedList = groupedData.ToList()
    };
    #endregion
  }

  public async Task<List<DefaultShipmentDashboard>?> GetDefaultShipmentDashboard()
  {
    return await _context.DefaultShipmentDashboards.ToListAsync();
  }
  public async Task<dynamic> GetOrderAddressInforForMapByOrderId(OrderId orderId)
  {
    var defLatitude = (decimal)25.2048;
    var defLongitude = (decimal)55.2708;

    var data = await _context.Orders
        .Join(_context.OrderAddresses, o => o.OrderAddressId, oa => oa.OrderAddressId, (o, oa) => new { o, oa })
        .Join(_context.Stores, ppc => ppc.o.StoreId, s => s.StoreId, (ppc, s) => new { ppc, s })
        .Join(_context.StoreAddresses, ppc_s => ppc_s.s.StoreId, sa => sa.StoreId, (ppc_s, sa) => new { ppc_s.ppc, ppc_s.s, sa })
        .Where(m => m.ppc.o.OrderId == orderId)
        .Select(m => new
        {
          OrigionLatitude = m.sa.Latitude ?? defLatitude,
          OrigionLongitude = m.sa.Longitude ?? defLongitude,

          DestinationLatitude = m.ppc.oa.Latitude,
          DestinationLongitude = m.ppc.oa.Longitude,

        }).FirstOrDefaultAsync();

    return data!;
  }
  public async Task<dynamic?> GetOrderInfoForPopupByOrderNo(string orderNo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT o.OrderId,
                               o.OrderNo,
                               o.RefNo,
                               o.OrderDate,
                               o.CreatedOn,
                               o.Amount,
                               o.CarrierTrackingStatusId,
                               o.CarrierId,
                               o.ActiveCarrierPickupLocationId,
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                               ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus,
                               ISNULL(c.Name, '') AS CarrierName,
                               ISNULL(c.CarrierImage, '') AS CarrierImage,
                               o.Description,
                               o.Remarks,
                               ISNULL(ffs.FullFillmentStatus, '') AS FullFillmentStatus,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pm.PMName, '') AS PaymentMethod,
                               o.PaymentMethodId,
                               o.Weight,
                               o.ItemValue,
                               ISNULL(ps.Name, '') AS ProductStationName,
                               o.Discount,
                               o.VAT,
                               ISNULL(s.StoreName, '') AS StoreName,
                               ISNULL(s.StoreImage, '') AS StoreImage,
                               ISNULL(s.CustomerServiceNo, '') AS CustomerServiceNo,
                               ISNULL(s.StoreCompany, '') AS StoreCompany,
                               ISNULL(s.Email, '') AS StoreEmail,
                               ISNULL(sa.FullAddress, '') AS StoreAddress,
                               ot.OrderTypeName,
                               oa.OrderAddressId,
                               oa.CustomerName,
                               o.OrderTypeId,
                               oa.Mobile1,
                               oa.OrderAddressId,
                               ISNULL(oa.Email, '') AS CustomerEmail,
                               ISNULL(oa.Mobile2, '') AS Mobile2,
                               ISNULL(oa.CustomerFullAddress, '') AS CustomerAddress,
                               cl.ClientName,
                                oa.Mobile1 as Mobile, 
                               cl.ClientCode,
                               cl.ClientCompanyName,
                               cl.Email,
                               cl.ClientId,
                               ISNULL(o.StripeInvoiceHostURL, '') AS StripeInvoiceHostURL,
                               ISNULL(o.StripeInvoicePDFURL, '') AS StripeInvoicePDFURL,
                               ISNULL(odt.TypeName, 'Forward') AS DeliveryTypeName,
                              CAST(
                              CASE
                                  WHEN o.CarrierId > 1000 THEN 1
                                  ELSE 0
                              END AS BIT
                          ) AS IsInhouseCarrier
                        FROM dbo.[Order] AS o
                            LEFT JOIN dbo.OrderDeliveryType AS odt
                                ON odt.OrderDeliveryTypeId = o.OrderDeliveryTypeId
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
                                ON o.ClientId = cl.ClientId  ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }

      if (!string.IsNullOrEmpty(orderNo))
      {
        dynamicParams.Add("@OrderNo", orderNo);
        whereStart += "And ( ( o.OrderNo = @OrderNo)) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);
      return data.FirstOrDefault();
    }
  }
  #region shipment dashboard setting

  public async Task<List<ShipmentGridColumn>> GetAllShipmentGridColumn(ClientId clientId)
  {
    var list = await _context.ShipmentGridColumns.Where(x => x.ClientId == clientId).ToListAsync();
    return list;
  }
  public async Task<List<ShipmentGridClientSetting>> GetAllShipmentGridClientSetting(ClientId clientId)
  {
    var list = await _context.ShipmentGridClientSettings.Where(x => x.ClientId == clientId).ToListAsync();
    return list;
  }
  public async Task<ShipmentGridClientSetting> GetShipmentGridClientSettingById(ShipmentGridClientSettingId? shipmentGridClientSettingId, ClientId clientId)
  {
    var data = await _context.ShipmentGridClientSettings.FirstOrDefaultAsync(x => x.ShipmentGridClientSettingId == shipmentGridClientSettingId && x.ClientId == clientId);
    return data!;
  }
  public async Task<ShipmentGridClientSetting> GetShipmentGridClientSettingByShipmentGridColumnId(int shipmentGridColumnId, ClientId clientId)
  {
    var data = await _context.ShipmentGridClientSettings.FirstOrDefaultAsync(x => x.ShipmentGridColumnId == shipmentGridColumnId && x.ClientId == clientId);
    return data!;
  }
  public async Task<bool> DeleteShipmentGridClientSetting(ShipmentGridClientSetting model)
  {
    _context.ShipmentGridClientSettings.Remove(model);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> GetAllShipmentGridClientSettingForDashboard(string? clientId)
  {
    List<ShipmentDashboardResponseModel> oShipmentGridClientSettings = await _shipmentStatusCommonRepository.GetAllShipmentGridClientSettings(clientId);
    return oShipmentGridClientSettings;
  }

  public async Task<bool> CreateShipmentGridColumn(ShipmentGridColumn oShipmentGridColumn)
  {
    await _context.AddAsync(oShipmentGridColumn);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> DeleteShipmentGridColumn(ShipmentGridColumn oShipmentGridColumn)
  {
    _context.Remove(oShipmentGridColumn);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> UpdateShipmentGridColumn(ShipmentGridColumn oShipmentGridColumn)
  {
    _context.Update(oShipmentGridColumn);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> CreateShipmentGridClientSetting(ShipmentGridClientSetting oShipmentGridClientSetting)
  {
    await _context.AddAsync(oShipmentGridClientSetting);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<bool> UpdateShipmentGridClientSetting(ShipmentGridClientSetting item)
  {
    _context.Update(item);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<ShipmentGridColumn> GetShipmentGridColumnById(int shipmentGridColumnId, ClientId clientId)
  {
    var target = await _context.ShipmentGridColumns.FirstOrDefaultAsync(x => x.ShipmentGridColumnId == shipmentGridColumnId && x.ClientId == clientId);
    return target!;
  }
  public async Task<ShipmentGridColumn> GetShipmentGridColumnByName(string? columnName, ClientId clientId)
  {
    var target = await _context.ShipmentGridColumns.FirstOrDefaultAsync(x => x.ColumnName!.Trim().ToLower() == columnName!.Trim().ToLower() && x.ClientId == clientId);
    return target!;
  }
  #endregion
}
