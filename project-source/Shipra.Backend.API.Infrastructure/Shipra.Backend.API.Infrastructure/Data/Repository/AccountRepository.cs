using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;

namespace Shipra.Backend.API.Infrastructure.Data.Repository;
public class AccountRepository : IAccountRepository
{
  private readonly ICommonLookupRepository _commonLookupRepository;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;

  public AccountRepository(ICommonLookupRepository commonLookupRepository, DapperAppDbContext dapperAppDbContext, AppDbContext context)
  {
    _commonLookupRepository = commonLookupRepository;
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
  }
  public async Task<CarrierPaymentSettlement> CreateCarrierPaymentSettlement(CarrierPaymentSettlement model)
  {
    await _context.CarrierPaymentSettlements.AddAsync(model);
    await _context.SaveChangesAsync();
    return model;
  }

  public async Task<dynamic> GetAllCarrierPaymentSettlements(string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir)
  {

    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               cps.CarrierPaymentSettlementId,
                               cps.PaymentRef,
                               cps.Amount,
                               cps.AmountReceived,
                               cps.PaymentDate,
                               cps.CreatedOn,
                               c.Name AS CarrierName,
                               c.CarrierImage,
                               c.CarrierWebsite,
                               ISNULL(psl.StatusName, 'Unpaid') AS PaymentStatusName,
                               cps.PaymentStatusId
                        FROM dbo.CarrierPaymentSettlement AS cps
                            INNER JOIN dbo.[Order] AS o
                                ON o.CarrierPaymentSettlementId = cps.CarrierPaymentSettlementId
                            INNER JOIN dbo.Carrier AS c
                                ON c.CarrierId = cps.CarrierId
                            INNER JOIN dbo.PaymentStatusLookup AS psl
                                ON psl.PaymentStatusId = cps.PaymentStatusId ";

      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        //dynamicParams.Add("@search", search);
        //whereStart += "And ( ( cps.PaymentRef like '%@search%' )) ";
      }

      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("cps.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST({CommonUtility.GetFormatedDateStr("cps.CreatedOn", regionMinuts)} AS DATE)  <= CAST(@createdTo AS DATE)) ";
      }

      string where = whereStart + whereEnd;


      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "cps.CreatedOn");

      var groupBy = @" GROUP BY ISNULL(psl.StatusName, 'Unpaid'),
                                         cps.CarrierPaymentSettlementId,
                                         cps.PaymentRef,
                                         cps.Amount,
                                         cps.AmountReceived,
                                         cps.PaymentDate,
                                         cps.CreatedOn,
                                         c.Name,
                                         c.CarrierImage,
                                         c.CarrierWebsite,
                                         cps.PaymentStatusId";

      string queryData = query + where + groupBy + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

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

  public async Task<dynamic> GetShipmentsBySettlementId(string carrierPaymentSettlementId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();


      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
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
      dynamicParams.Add("@carrierPaymentSettlementId", carrierPaymentSettlementId);
      whereStart += "And ( ( o.CarrierPaymentSettlementId  = @carrierPaymentSettlementId )) ";
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

  public async Task<CarrierPaymentSettlement?> GetCarrierPaymentSettlementById(CarrierPaymentSettlementId carrierPaymentId)
  {
    return await _context.CarrierPaymentSettlements.FirstOrDefaultAsync(x => x.CarrierPaymentSettlementId == carrierPaymentId);
  }
  public async Task<bool> DeleteCarrierPaymentSettlement(CarrierPaymentSettlement oCarrierPaymentSettlement)
  {
    _context.CarrierPaymentSettlements.Remove(oCarrierPaymentSettlement);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<CarrierPaymentSettlement> UpdateCarrierPaymentSettlement(CarrierPaymentSettlement model)
  {
    _context.CarrierPaymentSettlements.Update(model);
    await _context.SaveChangesAsync();
    return model;
  }

  public async Task<CPSettlementPopFile> CreateCpsettlementPopFile(CPSettlementPopFile oCpsettlementPodFile)
  {
    await _context.CpsettlementPopFiles.AddAsync(oCpsettlementPodFile)!;
    await _context.SaveChangesAsync();
    return oCpsettlementPodFile;
  }

  public async Task<CPSettlementPopFile> GetCpsettlementPopFile(int cpsettlementPopFileId, CarrierPaymentSettlementId carrierPaymentSettlementId)
  {
    var data = await _context.CpsettlementPopFiles.FirstOrDefaultAsync(x => x.CarrierPaymentSettlementId == carrierPaymentSettlementId && x.CpsettlementPopFileId == cpsettlementPopFileId);
    return data!;
  }

  public async Task<bool> DeleteCpsettlementPopFile(CPSettlementPopFile oCpsettlementPodFile)
  {
    _context.CpsettlementPopFiles.Update(oCpsettlementPodFile!);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<CPSettlementPopFile>> GetAllCpsettlementPopFiles(CarrierPaymentSettlementId carrierPaymentSettlementId)
  {
    return await _context.CpsettlementPopFiles.Where(x => x.CarrierPaymentSettlementId == carrierPaymentSettlementId && x.Active == true).ToListAsync();
  }
  public async Task<ClientGenericSetting> GetGenericSettingByClientIdAsync(ClientId clientId)
  {
    var data = await _context.ClientGenericSettings.FirstOrDefaultAsync(x => x.ClientId == clientId);
    return data!;
  }
  public async Task<dynamic> GetAllCarrierWithCodPending(DateTime? createdFrom, DateTime? createdTo, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT c.CarrierId,
                       ISNULL(c.CarrierImage, '') AS CarrierImage,
                       c.Name,
                       SUM(o.Amount) AS TotalAmount
                FROM dbo.[Order] AS o
                    INNER JOIN dbo.Carrier AS c
                        ON c.CarrierId = o.CarrierId ";

      string whereStart = "WHERE ( 1=1 AND ( o.CarrierPaymentSettlementId IS NULL ) ";
      string whereEnd = ")";

      var setting = await GetGenericSettingByClientIdAsync(new ClientId(new Guid(clientId!)));
      string codPendingStatus = "";
      if (setting != null)
      {
         codPendingStatus = UtilityHelper.GetClientSettingValueWithByKey(setting.SettingConfig!, "account", "codPendingStatus"); 
      }


      #region status
      dynamicParams.Add("@codPendingStatus", codPendingStatus);
      whereStart += "And ( ( o.CarrierTrackingStatusId in (select value from STRING_SPLIT(@codPendingStatus,',')))) ";

      #endregion

      dynamicParams.Add("@ClientId", clientId);
      whereStart += "And (o.ClientId = @ClientId) ";

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
       
      var groupBy = @" GROUP BY c.CarrierId,
         c.Name,
         c.CarrierImage ";

      string queryData = query + where + groupBy + " " + " ORDER BY c.CarrierId DESC ";

      var data = await connection.QueryAsync(queryData, dynamicParams); 
      return data.ToList();
    }

  }

}
