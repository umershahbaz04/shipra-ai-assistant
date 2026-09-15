using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class CarrierReturnReportRepository : ICarrierReturnReport
{
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;


  public CarrierReturnReportRepository(AppDbContext context, DapperAppDbContext dapperAppDbContext)
  {
    _context = context;
    _dapperAppDbContext = dapperAppDbContext;
  }
  public async Task<CarrierReturnReport> CreateCarrierReturnReport(CarrierReturnReport report)
  {
    await _context.CarrierReturnReports.AddAsync(report);
    await _context.SaveChangesAsync();
    return report;
  }

  public async Task<bool> DeleteCarrierReturnRerport(CarrierReturnReport report)
  {
    _context.CarrierReturnReports.Remove(report);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<dynamic> GetAllMyCarrierReturnReport(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, int? carrierId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();
      var query = $@"SELECT ROW_NUMBER() OVER (ORDER BY crr.CreatedOn) AS RowNum,
                             COUNT(*) OVER () AS TotalCount,
                             crr.CarrierRRId,
                             crr.CarrierId,
                             crr.ReturnReportNo,
                             crr.TotalOrders,
                             crr.CreatedOn
                      FROM dbo.CarrierReturnReport AS crr
                          LEFT JOIN
                          (
                              SELECT DISTINCT
                                     CarrierRRId,
			                         ClientId
                              FROM dbo.[Order]
                              WHERE ClientId = '{clientId}'
                          ) AS o
                              ON crr.CarrierRRId = o.CarrierRRId
                          INNER JOIN dbo.Client AS c
                              ON c.ClientId = o.ClientId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( crr.ReturnReportNo in (select value from STRING_SPLIT(@search,',')))) ";
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
      if (carrierId > 0)
      {
        dynamicParams.Add("@carrierId", carrierId);
        whereStart += "And (crr.carrierId = @carrierId) ";
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

  public async Task<CarrierReturnReport?> GetCarrierReturnReportById(CarrierRRId carrierRRId)
  {
    return await _context.CarrierReturnReports.FirstOrDefaultAsync(x => x.CarrierRrid == carrierRRId);
  }

  public async Task<CarrierReturnReport> GetCarrierReturnRerport(CarrierRRId carrierRrid)
  {
    var target = await _context.CarrierReturnReports.FirstOrDefaultAsync(x => x.CarrierRrid! == carrierRrid);
    return target!;
  }

  public async Task<dynamic> GetReturnRerports(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();
 
      string query = @"Select COUNT(*) OVER () AS TotalCount,
                               crr.CarrierRRId,
                               crr.ReturnReportNo,
                               crr.TotalOrders,
                               crr.CreatedOn,
                               c.Name AS CarrierName,
                               c.CarrierImage
                               FROM dbo.CarrierReturnReport AS crr
                                INNER JOIN dbo.[Order] AS o
                                    ON o.CarrierRRId = crr.CarrierRRId
                                INNER JOIN dbo.Carrier AS c
                                ON c.CarrierId = crr.CarrierId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( crr.ReturnReportNo in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
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

      var groupby = @" GROUP BY crr.CarrierRRId,
         crr.ReturnReportNo,
         crr.TotalOrders,
         crr.CreatedOn,
         c.Name,
         c.CarrierImage ";

      string where = whereStart + whereEnd + groupby;
   
      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "crr.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      dynamic result = new ExpandoObject();
      int totalCount = 0;
      var dataList = data.ToList();
      if (dataList.Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;
      return result;
    }

  }

  public async Task<dynamic> GetShipmentsByReturnReportId(string carrierRRId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();


      string query = @$"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               o.OrderId,
                               o.OrderNo,
                               FORMAT(o.OrderDate, 'dd-MM-yyyy') OrderDate,
                               FORMAT(o.CreatedOn, 'dd-MM-yyyy') CreatedOn,
                               o.Amount,
                               ISNULL(o.CarrierTrackingNo, '') AS CarrierTrackingNo,
                               ISNULL(c.Name, '') AS CarrierName,
                               ISNULL(o.CarrierTrackingStatus, '') AS TrackingStatus,
                               o.Description,
                               o.Remarks,
                               ISNULL(ffs.FullFillmentStatus, '') AS FullFillmentStatus,
                               o.ItemsCount,
                               ISNULL(psl.StatusName, '') AS PaymentStatus,
                               o.Weight,
                               o.ItemValue,
                               ISNULL(ps.Name, '') AS ProductStationName,
                               o.Discount,
                               o.VAT,
                               ISNULL(s.StoreName, '') AS StoreName,
                               sa.FullAddress,
                               s.StoreImage,
                               s.CustomerServiceNo,
                               ot.OrderTypeName,
                               oa.CustomerName,
                               oa.CustomerFullAddress,
                               oa.Mobile1,
                               ISNULL(cl.ClientCompanyName, cl.ClientName) AS ClientName
                        FROM dbo.[Order] AS o
                            LEFT JOIN dbo.OrderAddress AS oa
                                ON o.OrderAddressId = oa.OrderAddressId
                            INNER JOIN dbo.Stores AS s
                                ON o.StoreId = s.StoreId
                            INNER JOIN dbo.StoreAddress AS sa
                                ON sa.StoreId = s.StoreId
                            LEFT JOIN dbo.SaleChannelConfig AS scc
                                ON scc.SaleChannelConfigId = o.SaleChannelConfigId
                            LEFT JOIN dbo.Carrier AS c
                                ON o.CarrierId = c.CarrierId
                            LEFT JOIN dbo.PaymentStatusLookup AS psl
                                ON o.PaymentStatusId = psl.PaymentStatusId
                            LEFT JOIN dbo.FullFillmentStatusLookup AS ffs
                                ON o.FullFillmentStatusId = ffs.FullFillmentStatusId
                            LEFT JOIN dbo.OrderTypeLookup AS ot
                                ON o.OrderTypeId = ot.OrderTypeId
                            INNER JOIN dbo.PaymentMethodLookup AS pm
                                ON o.PaymentMethodId = pm.PaymentMethodId
                            LEFT JOIN dbo.ProductStation AS ps
                                ON o.StationId = ps.ProductStationId
                            INNER JOIN dbo.Client AS cl
                                ON cl.ClientId = o.ClientId
                            LEFT JOIN dbo.OrderDeliveryType AS odt
                                ON odt.OrderDeliveryTypeId = o.OrderDeliveryTypeId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      #region filter
      dynamicParams.Add("@carrierRRId", carrierRRId);
      whereStart += "And ( ( o.CarrierRRId  = @carrierRRId )) ";
      whereStart += $"And ( ( o.CarrierTrackingStatusId  = {(int)EnumCarrierTrackingStatus.Returned} )) ";
      #endregion
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "o.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[0] + " " + "Desc";

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
