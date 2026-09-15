using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ClientAggregate;

using Newtonsoft.Json;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class DeliveryTaskRepository : IDeliveryTaskRepository
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public DeliveryTaskRepository(DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }

  public async Task<DeliveryTask?> CheckDeliveryTaskExists(OrderId? orderId)
  {
    return await _context.DeliveryTasks.FirstOrDefaultAsync(x => x.OrderId! == orderId);
  }
  public async Task<DeliveryTask> CreateDeliveryTask(DeliveryTask deliverytask)
  {
    await _context.DeliveryTasks.AddAsync(deliverytask);
    await _context.SaveChangesAsync();
    return deliverytask;
  }
  public async Task<dynamic> GetAllDeliveryTask(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId, int? driverAssignedStatus, string? countryIds = null, string? carrierTrackingStatusIds = null, string? storeIds = null, string? deliveryTaskStatusIds = null, string? driverIds = null, bool? includeDriver = true, Dictionary<string, AddressFilterModel>? addressFilter = null, string? salePersonIds = null, string? orderLabels = null, int? duplicateStatus = null, int? deliveryNoteStatusId = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      var duplicateDaysStr = await CommonUtility.GetClientGenericSettingValue(clientId, "others", "duplicateDays", _context);
      int.TryParse(duplicateDaysStr, out int duplicateDays);

      var dynamicParams = new DynamicParameters();
      dynamicParams.Add("@duplicateDays", duplicateDays);

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               dt.DeliveryTaskId,
                               o.OrderId,
                               o.OrderNo,
                               o.RefNo,
                               ISNULL(o.OrderLabels, '') AS OrderLabels,
                               ISNULL(o.CarrierTrackingNo, '') AS TrackingNo,
                               cts.TrackingStatus,
                               dtsl.DeliveryTaskStatusId,
                               dtsl.DeliveryTaskStatus,
                               ISNULL(e.EmployeeName, '') AS DriverName,
                               ISNULL(e.MobileNo, '') AS DriverMobile,
                               oa.CustomerName AS Customer,
                               o.OrderAddressId,
                               oa.Mobile1,
                               oa.Mobile2,
                               c.[Name] AS CountryName,
                               c2.Code  AS CurrencyCode,
                               oa.CustomerFullAddress,
                               oa.Latitude,
                               oa.Longitude,
                               CASE
                                   WHEN oa.Latitude IS NULL
                                        OR oa.Longitude IS NULL THEN
                                       0
                                   ELSE
                                       1
                               END AS IsValidLatLng,
                               o.Remarks,
                               o.Description,
                               o.OrderDate,
                               o.Amount,
                               s.StoreName,
                               s.CustomerServiceNo,
                               ISNULL(e2.EmployeeName,'') AS SalePersonName,
                               ISNULL(dn.NoteNo, '') AS DeliveryNoteNo,
                               dn.DeliveryNoteId,
                               dn.DeliveryNoteStatusId,
                               ISNULL(dnsl.DeliveryNoteStatusName, '') AS DeliveryNoteStatus,
                               CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.MetaField mf WHERE mf.EntityId = CAST(o.OrderId AS VARCHAR(50)) AND mf.ClientId = '{clientId}' AND mf.SettingConfig IS NOT NULL AND DATALENGTH(mf.SettingConfig) > 0) THEN 1 ELSE 0 END AS BIT) AS HasAdditionalField,
                               CAST(CASE WHEN @duplicateDays > 0 AND EXISTS (
                                   SELECT 1 FROM dbo.[Order] o_d
                                   INNER JOIN dbo.[OrderAddress] oa_d ON o_d.OrderAddressId = oa_d.OrderAddressId
                                   WHERE o_d.ClientId = '{clientId}'
                                     AND o_d.OrderId <> o.OrderId
                                     AND CAST(o_d.CreatedOn AS DATE) >= DATEADD(day, -@duplicateDays, CAST(GETUTCDATE() AS DATE))
                                     AND (
                                       (LEN(oa_d.Mobile1) >= 8 AND LEN(oa.Mobile1) >= 8 AND RIGHT(oa_d.Mobile1, 8) = RIGHT(oa.Mobile1, 8)) OR
                                       (LEN(oa_d.Mobile2) >= 8 AND LEN(oa.Mobile2) >= 8 AND RIGHT(oa_d.Mobile2, 8) = RIGHT(oa.Mobile2, 8)) OR
                                       (LEN(oa_d.Mobile1) >= 8 AND LEN(oa.Mobile2) >= 8 AND RIGHT(oa_d.Mobile1, 8) = RIGHT(oa.Mobile2, 8)) OR
                                       (LEN(oa_d.Mobile2) >= 8 AND LEN(oa.Mobile1) >= 8 AND RIGHT(oa_d.Mobile2, 8) = RIGHT(oa.Mobile1, 8))
                                     )
                               ) THEN 1 ELSE 0 END AS BIT) AS IsDuplicateDays
                        FROM [dbo].[DeliveryTask] AS dt
                            INNER JOIN [dbo].[Order] AS o
                                ON o.OrderId = dt.OrderId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
		                        AND cts.ClientId = '{clientId}'
                            LEFT JOIN [dbo].[OrderAddress] AS oa
                                ON o.OrderAddressId = oa.OrderAddressId
                            INNER JOIN dbo.Country AS c
                                ON c.CountryId = oa.CountryId
                            LEFT JOIN dbo.Currency AS c2 ON c2.CountryId = c.CountryId
                            LEFT JOIN [dbo].[Driver] AS d
                                ON d.DriverId = dt.DriverId
                            LEFT JOIN dbo.Employee AS e
                                ON d.EmployeeId = e.EmployeeId
                            LEFT JOIN [dbo].[DeliveryTaskStatusLookup] AS dtsl
                                ON dt.DeliveryTaskStatusId = dtsl.DeliveryTaskStatusId
                            LEFT JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            LEFT JOIN dbo.SaleChannelConfig AS scc
                                ON o.SaleChannelConfigId = scc.SaleChannelConfigId
                            LEFT JOIN dbo.Employee AS e2
                                ON o.SaleChannelConfigId = e2.SaleChannelConfigId
                            LEFT JOIN dbo.DeliveryNoteDetail AS dnd
                                ON dnd.DeliveryNoteDetailId = dt.DeliveryNoteDetailId
                            LEFT JOIN dbo.DeliveryNote AS dn
                                ON dn.DeliveryNoteId = dnd.DeliveryNoteId
                            LEFT JOIN dbo.DeliveryNoteStatusLookup AS dnsl
                                ON dnsl.DeliveryNoteStatusId = dn.DeliveryNoteStatusId ";
      string whereStart = "WHERE ( dt.Active=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ((o.OrderNo in (select value from STRING_SPLIT(@search,','))) OR (dn.NoteNo in (select value from STRING_SPLIT(@search,',')))) ";
      } 
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@clientId", clientId);
        whereStart += "And (o.ClientId = @clientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dt.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("dt.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      #region Assign Status
      if (driverAssignedStatus > 0)
      {
        if (driverAssignedStatus == 1)
          whereStart += $"And (dt.DeliveryTaskStatusId = {(int)EnumDeliveryTaskStatusLookup.Unallocated} ) ";
        else
        {
          whereStart += $"And (dt.DeliveryTaskStatusId != {(int)EnumDeliveryTaskStatusLookup.Unallocated} ) ";
        }
      }
      #endregion
      #region MyRegion
      if (!string.IsNullOrEmpty(storeIds) && storeIds != "0")
      {
        dynamicParams.Add("@storeIds", storeIds);
        whereStart += "And ( ( o.StoreId in (select value from STRING_SPLIT(@storeIds,',')))) ";
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
      if (!string.IsNullOrEmpty(deliveryTaskStatusIds))
      {
        dynamicParams.Add("@deliveryTaskStatusIds", deliveryTaskStatusIds);
        whereStart += "And ( ( dt.DeliveryTaskStatusId in (select value from STRING_SPLIT(@deliveryTaskStatusIds,',')))) ";
      }
      if (!string.IsNullOrEmpty(driverIds))
      {
        dynamicParams.Add("@driverIds", driverIds);
        if (includeDriver.GetValueOrDefault())
        {
          whereStart += $"And ( ( dt.DriverId in (select value from STRING_SPLIT(@driverIds,',')))) AND d.ClientId = '{clientId}' ";
        }
        else
        { 
          whereStart += $"And ( ( dt.DriverId not in (select value from STRING_SPLIT(@driverIds,',')))) AND d.ClientId = '{clientId}' ";
        }
      }
      if (!string.IsNullOrEmpty(orderLabels) && orderLabels != "0")
      {
        dynamicParams.Add("@orderLabels", orderLabels);
        whereStart += @"And EXISTS (
                            SELECT 1 
                            FROM dbo.ClientOrderLabel col
                            WHERE col.OrderId = o.OrderId
                              AND col.LabelName IN (SELECT value FROM STRING_SPLIT(@orderLabels, ','))
                        ) ";
      }
      if (duplicateStatus.HasValue && duplicateStatus.Value > 0)
      {
        string subquery = @"(
                              SELECT 1 
                              FROM dbo.[Order] o_dup 
                              INNER JOIN dbo.[OrderAddress] oa_dup ON o_dup.OrderAddressId = oa_dup.OrderAddressId 
                              WHERE o_dup.ClientId = @clientId 
                                AND oa_dup.Mobile1 = oa.Mobile1 
                                AND o_dup.OrderId <> o.OrderId 
                                AND oa_dup.Mobile1 IS NOT NULL 
                                AND oa_dup.Mobile1 <> ''
                          ) ";

        if (duplicateStatus.Value == 1) // Duplicate
        {
          whereStart += $"And EXISTS {subquery}";
        }
        else if (duplicateStatus.Value == 2) // Not Duplicate
        {
          whereStart += $"And NOT EXISTS {subquery}";
        }
      }
      #endregion
      #region DeliveryNoteStatus Filter
      if (deliveryNoteStatusId.HasValue && deliveryNoteStatusId.Value == 2)
      {
        dynamicParams.Add("@DeliveryTaskStatusId", (int)EnumDeliveryTaskStatusLookup.Completed);
        whereStart += "And (dt.DeliveryTaskStatusId = @DeliveryTaskStatusId) ";
      }
      else
      {
        dynamicParams.Add("@DeliveryTaskStatusId", (int)EnumDeliveryTaskStatusLookup.Completed);
        whereStart += "And (dt.DeliveryTaskStatusId <> @DeliveryTaskStatusId) ";
      }
      #endregion
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "dt.CreatedOn");
      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + " DESC " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

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
  public async Task<dynamic> GetAllPendingForReturnShipments(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId)
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
                       e.EmployeeCode,
                       d.DriverCode,
                       e.EmployeeName AS DriverName,
                       e.MobileNo AS DriverMobile,
                       e.WorkEmail AS DriverEmail,
                       o.Remarks
                FROM dbo.DeliveryNote AS dn
                    INNER JOIN dbo.DeliveryNoteDetail AS dnd
                        ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                    INNER JOIN dbo.[Order] AS o
                        ON dnd.OrderId = o.OrderId
                   INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                                   AND cts.ClientId = '{clientId}'
                    INNER JOIN dbo.OrderAddress AS oa
                        ON oa.OrderAddressId = o.OrderAddressId
                    INNER JOIN dbo.PaymentMethodLookup AS PML
                        ON PML.PaymentMethodId = o.PaymentMethodId
                    INNER JOIN dbo.Driver AS d
                        ON d.DriverId = dn.DriverId
                    INNER JOIN dbo.Employee AS e
                        ON e.EmployeeId = d.EmployeeId   ";


      string whereStart = "WHERE ( 1=1 ";
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
      //IsInProcess
      whereStart += " And (dnd.IsInProcess = 1) ";

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

  public async Task<DeliveryTask?> GetDeliveryTaskById(DeliveryTaskId deliveryTaskId)
  {
    return await _context.DeliveryTasks.FirstOrDefaultAsync(x => x.DeliveryTaskId == deliveryTaskId);
  }

  public async Task<DeliveryTask?> GetDeliveryTaskByOrderId(OrderId? orderId)
  {
    return await _context.DeliveryTasks.FirstOrDefaultAsync(x => x.OrderId == orderId);
  }

  public async Task<List<DeliveryTask>?> GetUnAssignedDeliveryTaskListByOrderId(List<OrderId> orderIds)
  {
    var list = await _context.DeliveryTasks.Where(x => orderIds!.Contains(x.OrderId!) && x.DriverId == null).ToListAsync();
    return list;
  }

  public async Task<bool> DeleteDeliveryTaskById(DeliveryTask deliveryTask)
  {
    bool isDeleted = false;
    _context.Remove(deliveryTask);
    if (await _context.SaveChangesAsync() > 0)
    {
      isDeleted = true;
    }
    return isDeleted;
  }
  public async Task<DeliveryTask?> UpdateDeliveryTask(DeliveryTask deliveryTask)
  {
    _context.DeliveryTasks.Update(deliveryTask);
    await _context.SaveChangesAsync();
    return deliveryTask;
  }

  public async Task<dynamic> GetAllCODCollectionPendingsMyCarrier(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId)
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
                               ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                               ISNULL(cts.TrackingStatus, '') AS TrackingStatus,
                               ISNULL(s.StoreName, '') AS StoreName,
                               sa.FullAddress AS StoreAddress,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               ISNULL(pml.PMName, '') AS PaymentMethod,
                               ISNULL(c.Name, '') AS CarrierName,
                               ot.OrderTypeName
                        FROM dbo.DriverReceivable AS dr
                            INNER JOIN dbo.DeliveryNote AS dn
                                ON dn.DeliveryNoteId = dr.DeliveryNoteId
                            INNER JOIN dbo.DeliveryNoteDetail AS dnd
                                ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                            AND dnd.DeliveryNoteDetailStatusId = {(int)EnumDeliveryNoteDetailStatusLookup.Completed}
                            INNER JOIN dbo.[Order] AS o
                                ON o.OrderId = dnd.OrderId
                            INNER JOIN dbo.ClientCarrierTrackingStatus AS cts
                                ON o.CarrierTrackingStatusId = cts.CarrierTrackingStatusId
                            AND cts.ClientId = '{clientId}'
                            INNER JOIN dbo.PaymentStatusLookup AS psl
                                ON psl.PaymentStatusId = o.PaymentStatusId
                            INNER JOIN dbo.PaymentMethodLookup AS pml
                                ON pml.PaymentMethodId = o.PaymentMethodId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            INNER JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = o.StoreId
                            INNER JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId
                            INNER JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            INNER JOIN dbo.Client AS c2
                                ON c2.ClientId = o.ClientId ";

      string whereStart = $"WHERE ( 1=1 ";
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
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("o.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }

      /////////order filters//// 
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
      #region cod related filter
      whereStart += $"AND c2.DefaultCarrierId = o.CarrierId ";
      whereStart += $"AND dr.DriverPaidStatusId = {(int)EnumDriverPaidStatus.Paid} ";
      whereStart += $"AND (o.CarrierTrackingStatusId = {(int)EnumCarrierTrackingStatus.Delivered}) ";
      whereStart += $"AND (o.CarrierPaymentSettlementId IS NULL) ";
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

  public async Task<dynamic> GetAllCODCollectionsMyCarrier(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, string? carrierIds, int? orderTypeId)
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
                       ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                       ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                       ISNULL(cts.TrackingStatus, '') AS TrackingStatus,
                       ISNULL(c.Name, '') AS CarrierName,
                       o.Description,
                       o.Remarks,
                       ISNULL(ffs.FullFillmentStatus, '') AS FullFillmentStatus,
                       o.ItemsCount,
                       ISNULL(psl.StatusName, '') AS PaymentStatus,
                       ISNULL(pm.PMName, '') AS PaymentMethod,
                       o.Weight,
                       o.ItemValue,
                       ISNULL(ps.Name, '') AS ProductStationName,
                       o.Discount,
                       o.VAT,
                       ISNULL(s.StoreName, '') AS StoreName,
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
                FROM dbo.DeliveryNote AS dn
                    INNER JOIN dbo.DeliveryNoteDetail AS dnd 
                        ON dnd.DeliveryNoteId = dn.DeliveryNoteId
                    INNER JOIN dbo.[Order] AS o
                        ON dnd.OrderId = o.OrderId
                    INNER JOIN dbo.Stores AS s
                        ON o.StoreId = s.StoreId
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
      whereStart += $"And (o.CarrierPaymentSettlementId IS NOT NULL) ";
      whereStart += $"And (o.PaymentStatusId = {(int)EnumPaymentStatus.Paid}) ";
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
  public async Task<List<DeliveryTask>?> GetDeliveryTaskListByOrderId(List<OrderId> orderIds)
  {
    var list = await _context.DeliveryTasks.Where(x => orderIds!.Contains(x.OrderId!)).ToListAsync();
    return list;
  }
  public async Task<List<DeliveryTaskStatusLookup>> GetAllDeliveryTaskStatusForSelection()
  {
    var list = await _context.DeliveryTaskStatusLookups.ToListAsync();
    return list;
  }
}
