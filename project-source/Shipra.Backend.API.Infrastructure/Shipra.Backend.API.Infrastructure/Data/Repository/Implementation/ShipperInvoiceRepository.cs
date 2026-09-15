using System.Data;
using System.Dynamic;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShipperInvoiceAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;

public sealed class ShipperInvoiceRepository : IShipperInvoiceRepository
{
  private readonly ICountryRepository _countryRepository;
  private readonly ShipraMasterDbContext _shipraMasterDbContext;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly ShipperInvoiceDbContext _db;

  public ShipperInvoiceRepository(ICountryRepository countryRepository, ShipraMasterDbContext shipraMasterDbContext, DapperAppDbContext dapperAppDbContext, ShipperInvoiceDbContext db)
  {
    _countryRepository = countryRepository;
    _shipraMasterDbContext = shipraMasterDbContext;
    _dapperAppDbContext = dapperAppDbContext;
    _db = db;
  }

  // ================= ShipperRate =================
  public async Task<ShipperRate?> CreateShipperRate(ShipperRate entity)
  {
    await _db.ShipperRates.AddAsync(entity);
    return await _db.SaveChangesAsync() > 0 ? entity : null;
  }

  public async Task<bool> UpdateShipperRate(ShipperRate entity)
  {
    _db.ShipperRates.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }

  public Task<ShipperRate?> GetShipperRateById(long shipperRateId, Guid clientId, int saleChannelConfigId)
      => _db.ShipperRates.AsNoTracking()
          .FirstOrDefaultAsync(x => x.ShipperRateId == shipperRateId && x.ClientId == clientId && x.SaleChannelConfigId == saleChannelConfigId);

  // ============ ContractShipperRate ==============
  // ================= ShipperRateSlab =================

  public Task<ShipperRateSlab?> GetShipperRateSlab(long shipperRateId, int shipperRateSlabId)
  {
    return _db.ShipperRateSlabs.FirstOrDefaultAsync(x => x.ShipperRateId == shipperRateId && x.ShipperRateSlabId == shipperRateSlabId);
  }

  public async Task<ShipperRateSlab?> CreateShipperRateSlab(ShipperRateSlab entity)
  {
    await _db.ShipperRateSlabs.AddAsync(entity);
    return await _db.SaveChangesAsync() > 0 ? entity : null;
  }

  public async Task<bool> UpdateShipperRateSlab(ShipperRateSlab entity)
  {
    _db.ShipperRateSlabs.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }
  public async Task<List<ShipperRateSlab>> GetAllShipperRateSlabById(long? shipperRateId)
  {
    var data = await _db.ShipperRateSlabs.Where(x => x.ShipperRateId == shipperRateId).ToListAsync();
    return data;
  }
  public async Task<int?> GetShipperInvoiceCount(int saleChannelConfigId, Guid clientId)
  {
    return await _db.ShipperInvoices.CountAsync(x => x.ClientId == clientId && x.SaleChannelConfigId == saleChannelConfigId);
  }
  public async Task<int> DeleteShipperRatesAsync(int saleChannelConfigId, Guid clientId)
  {
    var rateIdsQuery = _db.ShipperRates
        .Where(r => r.ClientId == clientId
                 && r.SaleChannelConfigId == saleChannelConfigId)
        .Select(r => r.ShipperRateId);

    await _db.ShipperRateSlabs
        .Where(s => s.ShipperRateId.HasValue &&
                    rateIdsQuery.Contains(s.ShipperRateId.Value))
        .ExecuteDeleteAsync();

    return await _db.ShipperRates
        .Where(r => r.ClientId == clientId
                 && r.SaleChannelConfigId == saleChannelConfigId)
        .ExecuteDeleteAsync();
  }


  public async Task<ContractShipperRate?> CreateContractShipperRate(ContractShipperRate entity)
  {
    await _db.ContractShipperRates.AddAsync(entity);
    return await _db.SaveChangesAsync() > 0 ? entity : null;
  }

  public async Task<bool> UpdateContractShipperRate(ContractShipperRate entity)
  {
    _db.ContractShipperRates.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }

  public Task<ContractShipperRate?> GetContractShipperRateById(long contractShipperRatesId)
      => _db.ContractShipperRates.AsNoTracking()
          .FirstOrDefaultAsync(x => x.ContractShipperRatesId == contractShipperRatesId);

  public async Task<dynamic> GetAllClientRateAsync(int? start, int length, string? search, string? clientId)
  {
    using (var connection = _dapperAppDbContext.CreateShipperInvoiceConnection())
    {
      var dynamicParams = new DynamicParameters();

      string query = $@" SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                                 COUNT(*) OVER () AS TotalCount,
                                 sr.ShipperRateId,
                                 sr.ServiceRateGroupId, 
	                               sr.SaleChannelConfigId,
                                 sr.AdditionalRate AS CarrierAdditionalRate,  
                                 sr.CreatedOn,
                                 srg.[From],
                                 srg.[To], 
                                 srg.Code,
                                 srg.OriginTypeId,
                                 '' AS OriginTypeName,
                                 '' AS ServiceName
	   
                          FROM dbo.ShipperRate AS sr
                              LEFT JOIN dbo.ServiceRateGroup AS srg 
                                  ON srg.ServiceRateGroupId = sr.ServiceRateGroupId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(clientId))
      {
        whereStart += " AND sr.ClientId = @clientId ";
        dynamicParams.Add("clientId", clientId);
      }

      if (!string.IsNullOrEmpty(search))
      {

      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "sr.ShipperRateId");


      string queryData = query + where + " ORDER BY " + keyValuePairs[0] + " " + " DESC " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);
      foreach (var item in data)
      {
        item.FromName = await _countryRepository.GetEntityNamesByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.From.GetValueOrDefault());
        item.ToName = await _countryRepository.GetEntityNamesByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.To.GetValueOrDefault());
      }

      dynamic result = new ExpandoObject();
      int? totalCount = 0;
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

  // ========= ServiceRateGroupSlab =================
  public async Task<ServiceRateGroupSlab?> CreateServiceRateGroupSlab(ServiceRateGroupSlab entity)
  {
    await _db.ServiceRateGroupSlabs.AddAsync(entity);
    return await _db.SaveChangesAsync() > 0 ? entity : null;
  }

  public async Task<bool> UpdateServiceRateGroupSlab(ServiceRateGroupSlab entity)
  {
    _db.ServiceRateGroupSlabs.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }

  public Task<ServiceRateGroupSlab?> GetServiceRateGroupSlabById(int serviceRateGroupSlabId)
      => _db.ServiceRateGroupSlabs.AsNoTracking()
          .FirstOrDefaultAsync(x => x.ServiceRateGroupSlabId == serviceRateGroupSlabId);
  public async Task<List<ServiceRateGroupSlab>> GetAllServiceRateGroupSlabByServiceRateGroupId(int serviceRateGroupId)
  {
    return await _db.ServiceRateGroupSlabs.AsNoTracking().Where(x => x.ServiceRateGroupId == serviceRateGroupId).ToListAsync();
  }
  // ============== ServiceRateGroup ===============
  public async Task<ServiceRateGroup?> CreateServiceRateGroup(ServiceRateGroup entity)
  {
    await _db.ServiceRateGroups.AddAsync(entity);
    return await _db.SaveChangesAsync() > 0 ? entity : null;
  }

  public async Task<bool> UpdateServiceRateGroup(ServiceRateGroup entity)
  {
    _db.ServiceRateGroups.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }

  public Task<ServiceRateGroup?> GetServiceRateGroupById(int serviceRateGroupId, Guid? clientId)
      => _db.ServiceRateGroups
          .FirstOrDefaultAsync(x => x.ServiceRateGroupId == serviceRateGroupId && x.ClientId == clientId);
  public Task<ServiceRateGroupSlab?> GetServiceRateGroupSlabByRange(int? serviceRateGroupId, decimal? weightFrom, decimal? weightTo)
  {
    weightFrom = weightFrom.GetValueOrDefault();
    weightTo = weightTo.GetValueOrDefault();
    return _db.ServiceRateGroupSlabs
        .FirstOrDefaultAsync(x =>
            x.ServiceRateGroupId == serviceRateGroupId &&
            x.WeightFrom == weightFrom &&
            x.WeightTo == weightTo);
  }
  public async Task<dynamic> GetAllServiceRateGroups(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int? sortCol, string? sortDir, string? clientId, Dictionary<string, string>? addressFrom, Dictionary<string, string>? addressTo)
  {
    var from = addressFrom != null && addressFrom.Count > 0 ? addressFrom.Values.LastOrDefault() : null;
    var to = addressTo != null && addressTo.Count > 0 ? addressTo.Values.LastOrDefault() : null;
    using (var connection = _dapperAppDbContext.CreateShipperInvoiceConnection())
    {
      var dynamicParams = new DynamicParameters();

      string query = $@" SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                         COUNT(*) OVER () AS TotalCount,
                         srg.ServiceRateGroupId,
                         srg.Code,
                         srg.[From],
                         srg.[To],
                         srg.AdditionalRate,
                         srg.OriginTypeId,
                         '' AS OriginTypeName,
                         '' AS ServiceName,
                         srg.CreatedOn,
                         (
                             SELECT COUNT(*)
                             FROM dbo.ServiceRateGroupSlab sgrs
                             WHERE sgrs.ServiceRateGroupId = srg.ServiceRateGroupId
                         ) AS SlabCount,
                         srg.CreatedOn
                  FROM dbo.ServiceRateGroup AS srg ";
      string whereStart = " WHERE (1 = 1 ";
      string whereEnd = ")";
      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@Search", $"%{search}%");
        whereStart += " AND (srg.Code LIKE @Search)";
      }

      // Add enum values
      dynamicParams.Add("@CountryType", (int)EnumCivilEntityType.Country);
      dynamicParams.Add("@CityType", (int)EnumCivilEntityType.City);
      dynamicParams.Add("@AreaType", (int)EnumCivilEntityType.Area);
      dynamicParams.Add("@ProvinceType", (int)EnumCivilEntityType.Province);
      dynamicParams.Add("@StateType", (int)EnumCivilEntityType.State);
      dynamicParams.Add("@PinCodeType", (int)EnumCivilEntityType.PinCode);
      // ✅ Simple conditional filters
      if (!string.IsNullOrEmpty(from))
      {
        whereStart += @"
                        AND (
                            (srg.OriginTypeId = @CountryType AND cr.[From] IN (SELECT value FROM STRING_SPLIT(@From, ',')))
                         OR (srg.OriginTypeId = @CityType AND cr.[From] IN (SELECT value FROM STRING_SPLIT(@From, ',')))
                         OR (srg.OriginTypeId = @AreaType AND cr.[From] IN (SELECT value FROM STRING_SPLIT(@From, ',')))
                         OR (srg.OriginTypeId = @ProvinceType AND cr.[From] IN (SELECT value FROM STRING_SPLIT(@From, ',')))
                         OR (srg.OriginTypeId = @StateType AND cr.[From] IN (SELECT value FROM STRING_SPLIT(@From, ',')))
                         OR (srg.OriginTypeId = @PinCodeType AND cr.[From] IN (SELECT value FROM STRING_SPLIT(@From, ',')))
                        )";
        dynamicParams.Add("@From", from);
      }

      if (!string.IsNullOrEmpty(to))
      {
        whereStart += @"
                    AND (
                        (srg.OriginTypeId = @CountryType AND cr.[To] IN (SELECT value FROM STRING_SPLIT(@To, ',')))
                     OR (srg.OriginTypeId = @CityType AND cr.[To] IN (SELECT value FROM STRING_SPLIT(@To, ',')))
                     OR (srg.OriginTypeId = @AreaType AND cr.[To] IN (SELECT value FROM STRING_SPLIT(@To, ',')))
                     OR (srg.OriginTypeId = @ProvinceType AND cr.[To] IN (SELECT value FROM STRING_SPLIT(@To, ',')))
                     OR (srg.OriginTypeId = @StateType AND cr.[To] IN (SELECT value FROM STRING_SPLIT(@To, ',')))
                     OR (srg.OriginTypeId = @PinCodeType AND cr.[To] IN (SELECT value FROM STRING_SPLIT(@To, ',')))
                    )";
        dynamicParams.Add("@To", to);
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        whereStart += " AND srg.ClientId = @clientId ";
        dynamicParams.Add("clientId", clientId);
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "srg.ServiceRateGroupId");

      string queryData = query + where +
        $" ORDER BY {keyValuePairs[0]} DESC OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync<ShipperRateResponseModel>(queryData, dynamicParams);
      List<DeliveryService> listDeliveryServices = new();
      if (data.Count() > 0)
      {
        listDeliveryServices = await _shipraMasterDbContext.DeliveryServices.ToListAsync();
      }
      foreach (var item in data)
      {
        item.FromName = await _countryRepository.GetEntityNamesByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.From.GetValueOrDefault());
        item.ToName = await _countryRepository.GetEntityNamesByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.To.GetValueOrDefault());

        item.ServiceName = listDeliveryServices?.FirstOrDefault(x => x.DeliveryServiceId == item.OriginTypeId)?.ServiceName ?? string.Empty;
        item.OriginTypeName = item.OriginTypeId.HasValue ? Enum.GetName(typeof(EnumCivilEntityType), item.OriginTypeId.Value) ?? string.Empty : string.Empty;

      }



      dynamic result = new ExpandoObject();
      int? totalCount = 0;
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
  public async Task<dynamic> GetAllCarrierRateWithContractDataAsync(int? start, int length, string? search, int? deliveryServiceId, string? clientId, int? saleChannelConfigId)
  {
    using (var connection = _dapperAppDbContext.CreateShipperInvoiceConnection())
    {
      var dynamicParams = new DynamicParameters();

      string query = $@" SELECT
                          ROW_NUMBER() OVER (ORDER BY srg.ServiceRateGroupId DESC) AS RowNum,
                          COUNT(*) OVER() AS TotalCount,

                          srg.ServiceRateGroupId,
                          srg.[From],
                          srg.[To],
                          srg.OriginTypeId,
                          srg.CalculationMethodId,
                          srg.Unit,
                          srg.AdditionalRate,
                          isNull(sr.AdditionalRate,srg.AdditionalRate) As shipperAddRate,
                          srg.Code,
                          srg.ServiceTypeId,
                          srg.ClientId,
                          srg.CreatedOn,

                          ISNULL(sr.ShipperRateId, 0) AS ShipperRateId,
                          ISNULL(sr.SaleChannelConfigId, 0) AS SaleChannelConfigId
                      FROM dbo.ServiceRateGroup AS srg
                      LEFT JOIN dbo.ShipperRate AS sr
                          ON sr.ServiceRateGroupId = srg.ServiceRateGroupId
                         AND @saleChannelConfigId IS NOT NULL
                         AND @saleChannelConfigId > 0
                         AND sr.SaleChannelConfigId = @saleChannelConfigId ";
      string whereStart = " WHERE (1 = 1 ";// AND  (sr.ServiceRateGroupId IS NOT NULL) 
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);
      dynamicParams.Add("@saleChannelConfigId", saleChannelConfigId);

      if (!string.IsNullOrEmpty(search))
      {
        // Add filtering logic here
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        whereStart += " AND srg.ClientId = @clientId ";
        dynamicParams.Add("clientId", clientId);
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "srg.ServiceRateGroupId");

      string queryData = query + where + $" ORDER BY {keyValuePairs[0]} DESC OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync<ServiceGroupRateResponseModel>(queryData, dynamicParams);
      foreach (var item in data)
      {
        item.FromName = await _countryRepository.GetEntityNamesByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.From.GetValueOrDefault());
        item.ToName = await _countryRepository.GetEntityNamesByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.To.GetValueOrDefault());

        if (item.CalculationMethodId.GetValueOrDefault() == (int)EnumCalculationMethod.Slab)
        {
          item.CarrierRateSlabs = await GetServiceRateGroupSlabBySGRId(item.ServiceRateGroupId, saleChannelConfigId);
        }

      }

      dynamic result = new ExpandoObject();
      int? totalCount = 0;
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
  public async Task<dynamic> GetServiceRateGroupSlabBySGRId(int serviceRateGroupId, int? saleChannelConfigId)
  {
    using (var connection = _dapperAppDbContext.CreateShipperInvoiceConnection())
    {
      var dynamicParams = new DynamicParameters();

      string query = $@" SELECT
                          srgs.ServiceRateGroupSlabId,
                          srgs.ServiceRateGroupId,
                          srgs.WeightFrom,
                          srgs.WeightTo,
                          srgs.Rate AS CarrierRate,

                          ISNULL(sr.ShipperRateId, 0) AS ShipperRateId,
                          ISNULL(srs.ShipperRateSlabId, 0) AS ShipperRateSlabId,
                          ISNULL(srs.Rate, srgs.Rate) AS ShipperRate
                      FROM dbo.ServiceRateGroupSlab AS srgs
                      LEFT JOIN dbo.ShipperRate AS sr
                          ON sr.ServiceRateGroupId = srgs.ServiceRateGroupId
                         AND @saleChannelConfigId IS NOT NULL
                         AND @saleChannelConfigId > 0
                         AND sr.SaleChannelConfigId = @saleChannelConfigId
                      LEFT JOIN dbo.ShipperRateSlab AS srs
                          ON srs.ServiceRateGroupSlabId = srgs.ServiceRateGroupSlabId
                         AND srs.ShipperRateId = sr.ShipperRateId
                      WHERE srgs.ServiceRateGroupId = @serviceRateGroupId
                      ORDER BY srgs.ServiceRateGroupSlabId ";

      dynamicParams.Add("@serviceRateGroupId", serviceRateGroupId);
      dynamicParams.Add("@saleChannelConfigId", saleChannelConfigId);

      var data = await connection.QueryAsync(query, dynamicParams);

      return data.ToList();
    }
  }
  // ============ ShipperInvoiceDetail ==============
  public async Task<ShipperInvoiceDetail?> CreateShipperInvoiceDetail(ShipperInvoiceDetail entity)
  {
    await _db.ShipperInvoiceDetails.AddAsync(entity);
    return await _db.SaveChangesAsync() > 0 ? entity : null;
  }

  public async Task<bool> UpdateShipperInvoiceDetail(ShipperInvoiceDetail entity)
  {
    _db.ShipperInvoiceDetails.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }

  public Task<ShipperInvoiceDetail?> GetShipperInvoiceDetailById(long detailId, Guid tenantId)
     => _db.ShipperInvoiceDetails.AsNoTracking()
         .FirstOrDefaultAsync(x => x.ShipperInvoiceDetailId == detailId && x.ClientId == tenantId);


  // ================= ShipperInvoice ==============
  public async Task<ShipperInvoice?> CreateShipperInvoice(ShipperInvoice entity)
  {
    await _db.ShipperInvoices.AddAsync(entity);
    return await _db.SaveChangesAsync() > 0 ? entity : null;
  }

  public async Task<bool> UpdateShipperInvoice(ShipperInvoice entity)
  {
    _db.ShipperInvoices.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }

  public Task<ShipperInvoice?> GetShipperInvoiceById(int shipperInvoiceId, Guid clientId)
     => _db.ShipperInvoices.AsNoTracking()
         .FirstOrDefaultAsync(x => x.ShipperInvoiceId == shipperInvoiceId && x.ClientId == clientId);

  // ---------------- CREATE ----------------
  public async Task<ShipperInvoiceAdjustment?> CreateShipperInvoiceAdjustment(ShipperInvoiceAdjustment entity)
  {
    await _db.ShipperInvoiceAdjustments.AddAsync(entity);

    var saved = await _db.SaveChangesAsync();
    return saved > 0 ? entity : null;
  }

  // ---------------- UPDATE ----------------
  public async Task<bool> UpdateShipperInvoiceAdjustment(ShipperInvoiceAdjustment entity)
  {
    _db.ShipperInvoiceAdjustments.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }

  // ---------------- GET BY ID + CLIENT + SALECHANNEL ----------------
  public Task<ShipperInvoiceAdjustment?> GetShipperInvoiceAdjustmentById(int shipperInvoiceAdjustmentId, Guid clientId)
  {
    return _db.ShipperInvoiceAdjustments
     .AsNoTracking()
     .FirstOrDefaultAsync(x =>
         x.ShipperInvoiceAdjustmentId == shipperInvoiceAdjustmentId &&
         x.ClientId == clientId);

  }
  public async Task<List<ShipperInvoiceAdjustment>?> GetShipperInvoiceAdjustmentBySCId(int saleChannelConfigId, Guid clientId)
  {
    var data = await _db.ShipperInvoiceAdjustments
        .AsNoTracking()
        .Where(x =>
            x.ClientId == clientId &&
            (saleChannelConfigId == 0 || x.SaleChannelConfigId == saleChannelConfigId))
        .ToListAsync();
    return data!;
  }

  public async Task<bool> DeleteShipperInvoiceAdjustment(ShipperInvoiceAdjustment entity)
  {
    _db.ShipperInvoiceAdjustments.Remove(entity);
    return await _db.SaveChangesAsync() > 0;
  }
  public async Task<List<ShipperInvoiceAdjustment>> GetAllDeliveryAdjustmentByIds(List<int>? selectedadjustmentIds, Guid? clientId)
  {
    var oDInvoiceAdjustment = await _db.ShipperInvoiceAdjustments.Where(x => x.ClientId == clientId && selectedadjustmentIds!.Contains(x.ShipperInvoiceAdjustmentId)).ToListAsync();
    return oDInvoiceAdjustment;

  }
  public async Task<List<ShipperInvoiceListRow>> GetAllShipperInvoice(DateTime? createdFrom, DateTime? createdTo, int? start, int? length, string? search, int? sortCol, string? sortDir, string? clientId, int? transactionTypeId = 0, int? invoiceStatusId = 0, int? saleChannelConfigId = 0)
  {
    using (var connection = _dapperAppDbContext.CreateShipperInvoiceConnection())
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT DISTINCT
                           ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                           COUNT(*) OVER () AS TotalCount,
                           si.InvoiceNo,
                           si.RefNo,
                           si.TotalOrder,
                           si.Amount,
                           ist.StatusName,
                           si.ShipperInvoiceId,
                           (
                               SELECT TOP 1
                                      sr.EmployeeName
                               FROM dbo.ShipperRate AS sr
                               WHERE sr.SaleChannelConfigId = si.SaleChannelConfigId
                               ORDER BY sr.ShipperRateId DESC
                           ) AS EmployeeName,
                           si.CreatedOn
                    FROM dbo.ShipperInvoice AS si
                        INNER JOIN dbo.ShipperOrder AS so
                            ON so.ShipperInvoiceId = si.ShipperInvoiceId
                        LEFT JOIN dbo.InvoiceStatus AS ist
                            ON ist.InvoiceStatusId = si.InvoiceStatusId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", $"%{search}%");

        whereStart += @$" AND ((si.InvoiceNo LIKE @search)
					                                       OR (si.RefNo LIKE @search)
					                                   ) ";
      }

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += "And (CAST(si.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += "And (CAST(si.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (si.ClientId = @ClientId) ";
      }
      if (transactionTypeId > 0)
      {
        dynamicParams.Add("@transactionTypeId", transactionTypeId);
        whereStart += "And (si.TransactionTypeId = @transactionTypeId) ";
      }
      if (invoiceStatusId.GetValueOrDefault() > 0)
      {
        dynamicParams.Add("@invoiceStatusId", invoiceStatusId);
        whereStart += "And (si.InvoiceStatusId = @invoiceStatusId) ";
      }
      if (saleChannelConfigId.GetValueOrDefault() > 0)
      {
        dynamicParams.Add("@saleChannelConfigId", saleChannelConfigId);
        whereStart += "And (si.SaleChannelConfigId = @saleChannelConfigId) ";
      }


      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "si.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[0] + " " + " DESC " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync<ShipperInvoiceListRow>(queryData, dynamicParams);



      return data.ToList();
    }

  }
  public async Task<List<ShipperInvoiceAdjustmentModel>> GetAllShipperInvoiceAdjustment(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? clientId, int transactionTypeId = 0, int? InvoiceCreateId = 0)
  {
    using (var connection = _dapperAppDbContext.CreateShipperInvoiceConnection())
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                                 COUNT(*) OVER () AS TotalCount, 
                                 sia.ShipperInvoiceAdjustmentId,
                                 sia.ShipperInvoiceId,
                                 sia.TransactionTypeId,
                                 sia.Amount,
                                 sia.Comment,
                                 CAST(sia.ClientId AS nvarchar(40)) AS ClientId,
                                 sia.SaleChannelConfigId,
                                 sia.CreatedOn,
                                 sia.UpdatedOn,
                                 tt.TransactionTypeId,
                                 tt.Name,
                                 tt.Description,
                                 si.InvoiceNo
                          FROM dbo.ShipperInvoiceAdjustment AS sia
                              INNER JOIN dbo.TransactionType AS tt
                                  ON tt.TransactionTypeId = sia.TransactionTypeId
                              LEFT JOIN dbo.ShipperInvoice AS si
                                  ON si.ShipperInvoiceId = sia.ShipperInvoiceId ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", $"%{search}%");

        whereStart += @$" AND ((si.InvoiceNo LIKE @search)
					                                       OR (si.RefNo LIKE @search)
					                                   ) ";
      }

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += "And (CAST(sia.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += "And (CAST(sia.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (sia.ClientId = @ClientId) ";
      }
      if (transactionTypeId > 0)
      {
        dynamicParams.Add("@transactionTypeId", transactionTypeId);
        whereStart += "And (sia.TransactionTypeId = @transactionTypeId) ";
      }
      if (InvoiceCreateId > 0) // 1 not created , 2 created
      {
        dynamicParams.Add("@deliveryInvoiceCreateId", InvoiceCreateId);

        if (InvoiceCreateId == (int)EnumInvoiceCreateId.NotCreated)
        {
          whereStart += "And (sia.ShipperInvoiceAdjustmentId IS NULL) ";
        }
        if (InvoiceCreateId == (int)EnumInvoiceCreateId.Created)
        {
          whereStart += "And (sia.ShipperInvoiceAdjustmentId IS NOT NULL) ";
        }
      }

      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "sia.CreatedOn");


      string queryData = query + where + " ORDER BY " + keyValuePairs[0] + " " + " DESC " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync<ShipperInvoiceAdjustmentModel>(queryData, dynamicParams);



      return data.ToList();
    }

  }
  public async Task<List<ShipperInvoiceAdjustment>?> GetShipperInvoiceAdjustmentByShipperInvoiceId(int? shipperInvoiceId, Guid? clientId)
  {
    return await _db.ShipperInvoiceAdjustments.Where(x => x.ShipperInvoiceId == shipperInvoiceId && x.ClientId == clientId).ToListAsync();
  }
  public async Task<bool> DeleteShiperOrder(ShipperOrder oShiperOrder)
  {
    _db.ShipperOrders.Remove(oShiperOrder);
    return await _db.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> GetAllShipperForGenerateInvocice(string? clientId)
  {
    if (string.IsNullOrWhiteSpace(clientId))
      return new List<dynamic>();

    using var connection = _dapperAppDbContext.CreateShipperInvoiceConnection();

    var parameters = new DynamicParameters();
    parameters.Add("@ClientId", clientId);
    parameters.Add("@InvoiceStatusId", (int)EnumInvoiceStatus.Draft);

    string sql = @"
        SELECT
            ISNULL(sr.EmployeeName, 'Unknown') AS EmployeeName,
            so.SaleChannelConfigId
        FROM dbo.ShipperOrder AS so
        LEFT JOIN dbo.ShipperRate AS sr
            ON sr.SaleChannelConfigId = so.SaleChannelConfigId
        WHERE so.ClientId = @ClientId
          AND so.InvoiceStatusId = @InvoiceStatusId
        GROUP BY
            sr.EmployeeName,
            so.SaleChannelConfigId;
    ";

    var data = await connection.QueryAsync(sql, parameters);
    return data.ToList();
  }



  public async Task<bool?> CreateShipperOrder(ShipperOrder entity)
  {
    await _db.ShipperOrders.AddAsync(entity);
    return await _db.SaveChangesAsync() > 0;
  }
  public async Task<ShipperOrder?> GetShipperOrderByOrderNo(string? orderNo, Guid clientId)
  {
    var data = await _db.ShipperOrders.FirstOrDefaultAsync(x => x.OrderNo == orderNo && x.ClientId == clientId);
    return data;
  }

  public async Task<List<ServiceRateGroup>> GetAllServiceRateGroup(Guid? clientId)
  {
    return await _db.ServiceRateGroups.Where(x => x.ClientId == clientId).ToListAsync();
  }

  public async Task<bool?> UpdateShipperOrder(ShipperOrder entity)
  {
    _db.ShipperOrders.Update(entity);
    return await _db.SaveChangesAsync() > 0;
  }
  public async Task<List<OrderResponseModel>> GetShipperOrderForInvoices(int? start, int? length, string? search, string? shipperInvoiceId, int? saleChannelConfigId, string? clientId, DateTime? createdFrom = null, DateTime? createdTo = null)
  {
    using (var connection = _dapperAppDbContext.CreateShipperInvoiceConnection())
    {
      var dynamicParams = new DynamicParameters();

      string query = $@"  SELECT so.ShipperOrderId,
                                  CAST(so.ClientId AS nvarchar(40)) AS ClientId,
                                  so.ShipperInvoiceId,
                                  so.InvoiceStatusId,
                                  so.SaleChannelConfigId,
                                  CAST(so.OrderId  AS nvarchar(40)) AS OrderId ,
                                  so.OrderNo,
                                  so.CustomerName,
                                  so.CustomerFullAddress,
                                  so.Email,
                                  so.Mobile1,
                                  so.Mobile2,
                                  so.StoreName,
                                  so.CustomerServiceNo,
                                  so.Amount,
                                  IsNull(so.Weight,0) as Weight,
                                  so.CreatedOn FROM dbo.ShipperOrder AS so ";
      string whereStart = "WHERE ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( so.OrderNo in (select value from STRING_SPLIT(@Search,',')))) ";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $"And (CAST(so.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST(so.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (so.ClientId = @ClientId) ";
      }
      if (!string.IsNullOrEmpty(shipperInvoiceId))
      {
        dynamicParams.Add("@shipperInvoiceId", shipperInvoiceId);
        whereStart += "And ( ( so.InvoiceStatusId in (select value from STRING_SPLIT(@shipperInvoiceId,',')))) ";
      }
      if (saleChannelConfigId.GetValueOrDefault() > 0)
      {
        dynamicParams.Add("@saleChannelConfigId", saleChannelConfigId);
        whereStart += "And ( so.SaleChannelConfigId = @saleChannelConfigId) ";
      }
      if (!string.IsNullOrEmpty(shipperInvoiceId) &&
          shipperInvoiceId.Trim() == ((int)EnumInvoiceStatus.Due).ToString())
      {
        whereStart += "AND (ShipperInvoiceId IS NULL) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where + " ORDER BY so.CreatedOn DESC " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY;";
      var data = await connection.QueryAsync<OrderResponseModel>(queryData, dynamicParams);
      var resDataList = data.ToList();

      #region get rate
      var oFilters = new PriceCalculatorFilter()
      {
        ClientId = clientId!.ToString(),
        Search = string.Join(",", data.Select(x => x.OrderNo)),
        SaleChannelConfigIds = string.Join(",", data.Select(x => x.SaleChannelConfigId)),
      };
      var oRateListWithOrderNos = await GetAllShipperRateAsync(oFilters);

      var rateByOrderNo = oRateListWithOrderNos
            .Where(x => !string.IsNullOrWhiteSpace(x.OrderNo))
            .ToDictionary(
              x => x.OrderNo!.Trim().ToLowerInvariant(),
              x => new
              {
                Rate = x.Rate ?? x.CalculatedRate ?? 0m,
                FromName = x.FromName ?? string.Empty,
                ToName = x.ToName ?? string.Empty
              }
            );
      #endregion
      foreach (var order in resDataList.ToList())
      {
        if (string.IsNullOrWhiteSpace(order.OrderNo)) continue;

        if (rateByOrderNo.TryGetValue(order.OrderNo.Trim().ToLowerInvariant(), out var rateInfo))
        {
          // ✅ set DeliveryCharges only if not already set 
          order.DeliveryCharges = rateInfo.Rate;

          // ✅ set FromName
          order.FromName = rateInfo.FromName;
          order.ToName = rateInfo.ToName;
        }
      }

      return resDataList.ToList();
    }
  }

  #region client rate
  public async Task<List<ShipperRateResponseModel>> GetAllShipperRateAsync(PriceCalculatorFilter filter)
  {
    //using (var connection = _dapperAppDbContext.CreateConnectionWithEncryptedConStr(catalogueDatabases))
    //{
    if (string.IsNullOrWhiteSpace(filter.ClientId))
      throw new ArgumentNullException(nameof(filter.ClientId));

    var clientGuid = new Guid(filter.ClientId);

    // ✅ Seeds: (OrderNo, From, To, Weight, Amount)
    // You already have this method; do NOT paste it here.
    int? saleChannelConfigId = filter.SaleChannelConfigIds?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(x => int.TryParse(x, out var v) ? v : (int?)null)
    .FirstOrDefault(v => v.HasValue);
    List<OrderRateSeed> seeds = await GetOrdersFromToSeedsAsync(clientGuid, filter.Search, saleChannelConfigId);

    if (seeds == null || seeds.Count == 0)
    {
      return new List<ShipperRateResponseModel>(); ;
    }

    var allRows = new List<ShipperRateResponseModel>();
    //create master db connection
    using var connection = _dapperAppDbContext.CreateShipperInvoiceConnection();


    foreach (var s in seeds)
    {
      try
      {
        // ✅ Skip invalid seed (prevents many exceptions)
        //if (string.IsNullOrWhiteSpace(s.OrderNo) || s.fro <= 0 || s.To <= 0)
        //  continue;
        // per-order filter (don’t mutate the incoming filter across orders)
        var localFilter = new PriceCalculatorFilter
        {
          ClientId = filter.ClientId,
          OrigionTypeId = s.OriginTypeId,

          // ✅ current order context 
          Weight = s.Weight,
          Amount = s.Amount,
          From = s.From,
          To = s.To,
          Search = s.OrderNo,
          SaleChannelConfigIds = filter.SaleChannelConfigIds
        };

        // same old logic, but run per order
        var rows = await ComputeRatesForSingleRouteAsync(connection, localFilter);
        // ✅ extra safety: rows can be null
        if (rows != null && rows.Count > 0)
          allRows.AddRange(rows);
      }
      catch (Exception ex)
      {
        _ = ex.Message;
        continue;
      }
    }

    // ✅ Keep your existing final grouping behavior
    var returnrows = allRows
      .GroupBy(x => new
      {
        x.OrderNo,
        x.ServiceName,
        x.From,
        x.To,
        x.OriginTypeId
      })
      .Select((g, index) => new ShipperRateResponseModel
      {
        ShipperRateId = index + 1,
        OrderNo = g.Key.OrderNo,
        ServiceName = g.Key.ServiceName,
        From = g.Key.From,
        To = g.Key.To,
        OriginTypeId = g.Key.OriginTypeId,
        CalculatedRate = g.Sum(x => x.CalculatedRate ?? 0m)
      })
      .ToList();

    // ✅ Names resolve same as before (if need from and to name then uncomment this code)
    foreach (var rateModel in returnrows)
    {
      //rateModel.FromName = await countryRepository.GetEntityNamesByOriginTypeId(
      //	rateModel.OriginTypeId.GetValueOrDefault(),
      //	rateModel.From.GetValueOrDefault());

      rateModel.ToName = await _countryRepository.GetEntityNamesByOriginTypeId(
        rateModel.OriginTypeId.GetValueOrDefault(),
        rateModel.To.GetValueOrDefault());
    }

    returnrows.ForEach(x => x.Rate = x.CalculatedRate ?? 0m);
    return returnrows;
  }

  // =========================================================
  // SINGLE ROUTE COMPUTE (YOUR EXISTING LOGIC KEPT SAME)
  // =========================================================
  private async Task<List<ShipperRateResponseModel>> ComputeRatesForSingleRouteAsync(IDbConnection connection, PriceCalculatorFilter filter)
  {


    var dynamicParams = new DynamicParameters();
    dynamicParams.Add("ClientId", filter.ClientId);
    dynamicParams.Add("From", filter.From);
    dynamicParams.Add("To", filter.To);
    dynamicParams.Add("saleChannelConfigId", filter.SaleChannelConfigIds);

    // ✅ Paste your existing query here (unchanged)
    string query = GetClientRatesQuery(filter.Search);

    var data = await connection.QueryAsync<ShipperRateResponseModel>(query, dynamicParams);
    //ClientCarrierRateResponseModel
    var returnrows = new List<ShipperRateResponseModel>();

    if (data.Count() > 0)
    {
      var carrierGroups = data.GroupBy(r => new { r.Code, r.ServiceTypeId });

      foreach (var carrierRates in carrierGroups)
      {
        var carrierList = carrierRates.ToList();
        var matchedSlab = carrierList
          .FirstOrDefault(r => r.WeightFrom.HasValue && r.WeightTo.HasValue &&
                     filter.Weight >= r.WeightFrom.Value && filter.Weight <= r.WeightTo.Value);

        if (matchedSlab != null)
        {
          matchedSlab.CalculatedRate = matchedSlab.Rate;
          returnrows.Add(matchedSlab);
        }
        else
        {
          var baseRow = carrierList.OrderByDescending(x => x.WeightTo).FirstOrDefault();
          if (baseRow is not null)
          {
            if (carrierList.Count() > 0)
            {
              var RemaingWeight = filter.Weight - baseRow.WeightTo;
              decimal actualRate = baseRow.Rate ?? 0;

              if (baseRow.Unit.HasValue && baseRow.Unit > 0 &&
                baseRow.ShipperAdditionalRate.HasValue && baseRow.ShipperAdditionalRate > 0)
              {
                decimal units = RateCalculatorHelper.RoundToNearestHalf(
                  RemaingWeight.GetValueOrDefault() / baseRow.Unit.Value);

                actualRate += units * baseRow.ShipperAdditionalRate.Value;
              }

              baseRow.CalculatedRate = actualRate;
              returnrows.Add(baseRow);
            }
            else
            {
              baseRow.CalculatedRate = baseRow.ClientRate ?? 0;
              returnrows.Add(baseRow);
            }
          }
        }
      }
      // -------- API (minimal fix for multi-order: use filter.Search + filter.Weight)
    }

    return returnrows;
  }

  // =========================================================
  // YOUR EXISTING QUERY (PASTE AS-IS)
  // =========================================================
  private static string GetClientRatesQuery(string? orderNo)
  {
    // Paste your exact SQL query string here WITHOUT changing it.
    // return @"SELECT ...";
    string query = $@" 
                      SELECT ISNULL(sr.ShipperRateId, 0) AS ShipperRateId,
                             ISNULL(sr.SaleChannelConfigId, 0) AS SaleChannelConfigId,
                             ISNULL(srg.ServiceRateGroupId, 0) AS ServiceRateGroupId,
                             '{orderNo}' AS OrderNo,
                             ISNULL(sr.AdditionalRate, srg.AdditionalRate) AS ShipperAdditionalRate,
                             srg.Code,
                             srg.ServiceTypeId,
                             srg.OriginTypeId,
                             ISNULL(srg.Unit, 0) AS Unit,
                             srg.[From],
                             srg.[To],
                             srgs.WeightFrom,
                             srgs.WeightTo,
                             ISNULL(srs.Rate, 0) AS Rate
                      FROM dbo.ServiceRateGroup AS srg
                          INNER JOIN dbo.ServiceRateGroupSlab AS srgs
                              ON srgs.ServiceRateGroupId = srg.ServiceRateGroupId
                          LEFT JOIN dbo.ShipperRate AS sr
                              ON sr.ServiceRateGroupId = srg.ServiceRateGroupId
                          LEFT JOIN dbo.ShipperRateSlab AS srs
                              ON srs.ServiceRateGroupSlabId = srgs.ServiceRateGroupSlabId
                            AND srs.ShipperRateId = sr.ShipperRateId
                      WHERE sr.ClientId = @ClientId And ( ( sr.SaleChannelConfigId in (select value from STRING_SPLIT(@saleChannelConfigId,','))))  
                      AND (
                             (srg.OriginTypeId = {(int)EnumCivilEntityType.Country} AND srg.[From] = @From AND srg.[To] = @To) -- Country
                          OR (srg.OriginTypeId = {(int)EnumCivilEntityType.City} AND srg.[From] = @From AND srg.[To] = @To) -- City
                          OR (srg.OriginTypeId = {(int)EnumCivilEntityType.Area} AND srg.[From] = @From AND srg.[To] = @To) -- Area
                          OR (srg.OriginTypeId = {(int)EnumCivilEntityType.Province} AND srg.[From] = @From AND srg.[To] = @To) -- Province
                          OR (srg.OriginTypeId = {(int)EnumCivilEntityType.State} AND srg.[From] = @From AND srg.[To] = @To) -- State
                          OR (srg.OriginTypeId = {(int)EnumCivilEntityType.PinCode} AND srg.[From] = @From AND srg.[To] = @To) -- PinCode
                      )  ";

    return query;
  }

  // =========================================================
  // You already have this method; don't paste it here
  // =========================================================  
  public async Task<List<OrderRateSeed>> GetOrdersFromToSeedsAsync(
    Guid clientId,
    string? search,
    int? saleChannelConfigId)
  {
    // -------------------------------------------------
    // 1) Parse order numbers safely
    // -------------------------------------------------
    var orderNos = (search ?? string.Empty)
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList();

    if (orderNos.Count == 0)
      return new List<OrderRateSeed>();

    // -------------------------------------------------
    // 2) Load ONLY relevant ServiceRateGroups (1 query)
    // -------------------------------------------------
    var oList = await (
        from sr in _db.ShipperRates
        join srg in _db.ServiceRateGroups
            on sr.ServiceRateGroupId equals srg.ServiceRateGroupId
        where sr.SaleChannelConfigId == saleChannelConfigId
              && sr.ClientId == clientId
        select srg
    ).ToListAsync();

    if (oList.Count == 0)
      return new List<OrderRateSeed>();

    var availableOriginTypes = oList
        .Select(x => x.OriginTypeId)
        .Where(x => x.HasValue)
        .Select(x => x!.Value)
        .Distinct()
        .ToHashSet();

    if (availableOriginTypes.Count == 0)
      return new List<OrderRateSeed>();

    // -------------------------------------------------
    // 3) Load Orders + Addresses (Dapper, unchanged)
    // -------------------------------------------------
    const string sql = @"
        SELECT  o.OrderNo,
                ISNULL(o.Weight, 1) AS Weight,
                ISNULL(o.Amount, 0) AS Amount,
                ISNULL(o.SaleChannelConfigId, 0) AS SaleChannelConfigId,

                sa.StoreAddressId,
                sa.CountryId,
                sa.CityId,
                sa.AreaId,
                sa.ProvinceId,
                sa.PinCodeId,
                sa.StateId,
                sa.Latitude,
                sa.Longitude,

                oa.OrderAddressId,
                oa.CountryId,
                oa.CityId,
                oa.AreaId,
                oa.ProvinceId,
                oa.PinCodeId,
                oa.StateId,
                oa.Latitude,
                oa.Longitude,
                oa.EntityAddressDataJson
        FROM dbo.[Order] o
        LEFT JOIN dbo.StoreAddress sa ON sa.StoreId = o.StoreId
        LEFT JOIN dbo.OrderAddress oa ON oa.OrderAddressId = o.OrderAddressId
        WHERE o.ClientId = @ClientId
          AND o.OrderNo IN @OrderNos;
    ";

    using var conn =
        _dapperAppDbContext.CreateConnectionByClientOrMasterDb(false, clientId.ToString());

    var seeds = (await conn.QueryAsync<
        OrderRateSeed,
        StoreAddressDto,
        OrderAddressDto,
        OrderRateSeed>(
        sql,
        (order, storeAddr, orderAddr) =>
        {
          order.StoreAddress = storeAddr ?? new StoreAddressDto();
          order.OrderAddress = orderAddr ?? new OrderAddressDto();
          return order;
        },
        new { ClientId = clientId, OrderNos = orderNos },
        splitOn: "StoreAddressId,OrderAddressId"
    )).ToList();

    if (seeds.Count == 0)
      return seeds;

    // -------------------------------------------------
    // 4) Resolve OriginTypeId + From + To (FIXED)
    // -------------------------------------------------
    foreach (var row in seeds)
    {
      var fromMap = CivilEntityHelper
          .GetEnumValueFromAddressEntityMap(row.StoreAddress);

      var toMap = CivilEntityHelper
          .GetEnumValueFromAddressEntityMap(row.OrderAddress);

      if (fromMap == null || toMap == null)
        continue;

      var resolved = CivilEntityHelper.ResolveFromToByAvailableServiceGroups(
          fromMap,
          toMap,
          availableOriginTypes);

      if (resolved == null)
        continue;

      // ✅ CORRECT assignments
      row.OriginTypeId = resolved.Value.originTypeId;
      row.From = resolved.Value.from;
      row.To = resolved.Value.to;
    }

    return seeds;
  }

  //public async Task<List<OrderRateSeed>> GetOrdersFromToSeedsAsync(Guid clientId, string? search, int? saleChannelConfigId)
  //{
  //  // 1) Parse comma-separated order numbers
  //  var orderNos = (search ?? "")
  //    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
  //    .Where(x => !string.IsNullOrWhiteSpace(x))
  //    .Distinct(StringComparer.OrdinalIgnoreCase)
  //    .ToList();

  //  if (orderNos.Count == 0)
  //    return new List<OrderRateSeed>();

  //  var shipperRates = await _db.ShipperRates.Where(x => x.SaleChannelConfigId == saleChannelConfigId).ToListAsync();
  //  var ids = shipperRates.Where(x => x.ClientId.ToString() == clientId.ToString()).Select(x => x.ServiceRateGroupId).ToList();
  //  var oList = await _db.ServiceRateGroups.Where(x => ids.Contains(x.ServiceRateGroupId)).ToListAsync();

  //  var availableOriginTypes = oList
  //  .Select(x => x.OriginTypeId.GetValueOrDefault())
  //  .Distinct()
  //  .ToHashSet();

  //  // 2) Single query: Orders + StoreAddress + OrderAddress
  //  // IMPORTANT: Adjust table names / PK/FK column names if yours differ.
  //  // We alias columns exactly to match your DTO property names.
  //  const string sql = @"
  //                              SELECT o.OrderNo,
  //                             ISNULL(o.Weight, 1) AS Weight,
  //                             ISNULL(o.Amount, 0) AS Amount,
  //                             ISNULL(o.SaleChannelConfigId, 0) AS SaleChannelConfigId,

  //                             -- StoreAddress (From) -> matches StoreAddress DTO
  //                             sa.StoreAddressId AS StoreAddressId,
  //                             sa.CountryId AS CountryId,
  //                             sa.CityId AS CityId,
  //                             sa.AreaId AS AreaId,
  //                             sa.ProvinceId AS ProvinceId,
  //                             sa.PinCodeId AS PinCodeId,
  //                             sa.StateId AS StateId,
  //                             sa.Latitude AS Latitude,
  //                             sa.Longitude AS Longitude,

  //                             -- OrderAddress (To) -> matches OrderAddress DTO
  //                             oa.OrderAddressId AS OrderAddressId,
  //                             oa.CountryId AS CountryId,
  //                             oa.CityId AS CityId,
  //                             oa.AreaId AS AreaId,
  //                             oa.ProvinceId AS ProvinceId,
  //                             oa.PinCodeId AS PinCodeId,
  //                             oa.StateId AS StateId,
  //                             oa.Latitude AS Latitude,
  //                             oa.Longitude AS Longitude,
  //                             oa.EntityAddressDataJson AS EntityAddressDataJson
  //                      FROM dbo.[Order] AS o
  //                          LEFT JOIN dbo.StoreAddress AS sa
  //                              ON sa.StoreId = o.StoreId
  //                          LEFT JOIN dbo.OrderAddress AS oa
  //                              ON oa.OrderAddressId = o.OrderAddressId
  //                              WHERE o.ClientId = @ClientId
  //                                AND o.OrderNo IN @OrderNos;
  //                              ";

  //  using var conn = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(false, clientId!.ToString());

  //  // 3) Dapper multi-mapping: OrderRateSeed + StoreAddress + OrderAddress
  //  // splitOn tells Dapper where the second object starts (at SplitHere),
  //  // and third object starts at OrderAddressId (first col of OrderAddress).
  //  var seeds = (await conn.QueryAsync<
  //      OrderRateSeed,
  //      StoreAddressDto,
  //      OrderAddressDto,
  //      OrderRateSeed>(
  //      sql,
  //      (order, storeAddr, orderAddr) =>
  //      {
  //        order.StoreAddress = storeAddr ?? new StoreAddressDto();
  //        order.OrderAddress = orderAddr ?? new OrderAddressDto();
  //        return order;
  //      },
  //      new { ClientId = clientId, OrderNos = orderNos },
  //      splitOn: "StoreAddressId,OrderAddressId"
  //  )).ToList();

  //  // 4) Resolve From/To using your existing reflection helper
  //  var result = new List<(string OrderNo, int From, int To, decimal Weight, decimal Amount)>();

  //  //foreach (var row in seeds)
  //  //{
  //  //  var fromMap = CivilEntityHelper.GetEnumValueFromAddressEntityMap(row.StoreAddress);
  //  //  var toMap = CivilEntityHelper.GetEnumValueFromAddressEntityMap(row.OrderAddress);

  //  //  // Your old code used .Last().Value.
  //  //  // If you rely on Enum order, this keeps same behavior.
  //  //  int from = 0, to = 0;

  //  //  if (fromMap != null && fromMap.Any() && fromMap.Last().Value.HasValue)
  //  //    from = (int)fromMap.Last().Value!.Value;

  //  //  if (toMap != null && toMap.Any() && toMap.Last().Value.HasValue)
  //  //    to = (int)toMap.Last().Value!.Value;

  //  //  result.Add((row.OrderNo, from, to, row.Weight, row.Amount));
  //  //}
  //  foreach (var row in seeds)
  //  {
  //    var fromMap = CivilEntityHelper.GetEnumValueFromAddressEntityMap(row.StoreAddress);
  //    var toMap = CivilEntityHelper.GetEnumValueFromAddressEntityMap(row.OrderAddress);

  //    if (fromMap == null || toMap == null)
  //      continue;

  //    var resolved = CivilEntityHelper.ResolveFromToByAvailableServiceGroups(
  //        fromMap,
  //        toMap,
  //        availableOriginTypes);

  //    if (resolved == null)
  //      continue;

  //    row.Weight = resolved.Value.originTypeId;
  //    row.From = resolved.Value.from;
  //    row.To = resolved.Value.to;
  //  }

  //  return seeds;
  //}

  #endregion

  public async Task<dynamic> GetAllOrders(int start, int length, string? search, string? orderNos, string? statusIds, string? clientId, DateTime? createdFrom, DateTime? createdTo)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();

      string query = $@" SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                         COUNT(*) OVER () AS TotalCount,
                         o.OrderId,
                         o.OrderNo,
                         ISNULL(o.RefNo, '') AS RefNo,
                         o.OrderDate,
                         o.CreatedOn,
                         o.Amount,
                         ISNULL(o.DeliveryCharges, 0) AS DeliveryCharges,
                         ISNULL(o.CarrierTrackingStatus, '') AS CarrierTrackingStatus,
                         o.Description,
                         o.Remarks,
                         o.ItemsCount,
                         ISNULL(psl.StatusName, '') AS PaymentStatus,
                         o.Weight,
                         o.ItemValue,
                         scc.SaleChannelConfigId,
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
                         o.ClientId,
                         e.EmployeeName
                  FROM dbo.[Order] AS o
                      LEFT JOIN dbo.OrderAddress AS oa
                          ON o.OrderAddressId = oa.OrderAddressId
                      INNER JOIN dbo.Stores AS s
                          ON o.StoreId = s.StoreId
                      LEFT JOIN dbo.SaleChannelConfig AS scc
                          ON scc.SaleChannelConfigId = o.SaleChannelConfigId
                      INNER JOIN dbo.Employee AS e
                          ON e.SaleChannelConfigId = scc.SaleChannelConfigId
                      LEFT JOIN dbo.PaymentStatusLookup AS psl
                          ON o.PaymentStatusId = psl.PaymentStatusId ";
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
        whereStart += $"And (CAST(o.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $"And (CAST(o.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += "And (o.ClientId = @ClientId) ";
      }
      string where = whereStart + whereEnd;

      string queryData = query + where + " ORDER BY " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync<OrderResponseModel>(queryData, dynamicParams);

      return data.ToList();
    }
  }
  public async Task<List<dynamic>> GetAllShipperInvoiceDetail(string? clientId, int? shipperInvoiceId)
  {
    if (string.IsNullOrWhiteSpace(clientId) || shipperInvoiceId.GetValueOrDefault() <= 0)
      return new List<dynamic>();

    using var connection = _dapperAppDbContext.CreateShipperInvoiceConnection();

    var parameters = new DynamicParameters();
    parameters.Add("@ClientId", clientId);
    parameters.Add("@ShipperInvoiceId", shipperInvoiceId);

    string sql = @"
        SELECT
            si.InvoiceNo,
            si.RefNo,
            si.Amount as InvoiceTotal,
            ist.StatusName,
            si.CreatedOn,
            si.SaleChannelConfigId,

            sid.ShipperInvoiceDetailId,
            sid.OrderId,
            sid.Rate,

            so.ShipperOrderId,
            so.OrderNo,
            so.CustomerName,
            so.CustomerFullAddress,
            so.Email,
            so.Mobile1,
            so.Mobile2,
            so.StoreName,
            so.Amount,
            IsNULL(so.Weight,0) as Weight
        FROM dbo.ShipperInvoice AS si
        INNER JOIN dbo.ShipperInvoiceDetail AS sid
            ON sid.ShipperInvoiceId = si.ShipperInvoiceId
        INNER JOIN dbo.ShipperOrder AS so
            ON so.OrderId = sid.OrderId
           AND so.ShipperInvoiceId = si.ShipperInvoiceId   -- ✅ PREVENT DUPLICATES
        LEFT JOIN dbo.InvoiceStatus AS ist
            ON ist.InvoiceStatusId = si.InvoiceStatusId
        WHERE si.ClientId = @ClientId
          AND si.ShipperInvoiceId = @ShipperInvoiceId
        ORDER BY sid.ShipperInvoiceDetailId DESC
    ";

    var data = await connection.QueryAsync(sql, parameters);
    return data.ToList();
  }

  public async Task<List<ShipperRateResponseModel>> GetShipperRateBySaleChannelConfig(int? saleChannelConfigId, string? clientId, int? start, int? length, DateTime? createFrom, DateTime? createTo)
  {
    using var connection = _dapperAppDbContext.CreateShipperInvoiceConnection();

    var dynamicParams = new DynamicParameters();
    dynamicParams.Add("@displayStart", start);
    dynamicParams.Add("@displayLength", length);


    string query = @"
        SELECT 
            sr.ShipperRateId,
            sr.ClientId,
            sr.SaleChannelConfigId,
            sr.ServiceRateGroupId,
            sr.AdditionalRate,
            sr.CreatedOn,
            sr.EmployeeName,
            srg.CalculationMethodId,
            srg.[From],
            srg.[To],
            srg.OriginTypeId,
            srg.Code,
            srg.ServiceTypeId
        FROM dbo.ShipperRate AS sr
        INNER JOIN dbo.ServiceRateGroup AS srg
            ON srg.ServiceRateGroupId = sr.ServiceRateGroupId
    ";
    string whereStart = " WHERE (1=1 ";
    string whereEnd = ")";
    if (!string.IsNullOrEmpty(clientId))
    {
      dynamicParams.Add("@ClientId", clientId);
      whereStart += "And (sr.ClientId = @ClientId) ";
    }
    if (saleChannelConfigId.GetValueOrDefault() > 0)
    {
      dynamicParams.Add("@SaleChannelConfigId", saleChannelConfigId);
      whereStart += "And (sr.SaleChannelConfigId = @SaleChannelConfigId) ";
    }
    string where = whereStart + whereEnd;

    string queryData = query + where + " ORDER BY sr.CreatedOn DESC " + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY;";

    var data = (await connection
        .QueryAsync<ShipperRateResponseModel>(queryData, dynamicParams))
        .ToList();
    List<DeliveryService> listDeliveryServices = new();
    if (data.Count() > 0)
    {
      listDeliveryServices = await _shipraMasterDbContext.DeliveryServices.ToListAsync();
    }
    foreach (var item in data)
    {
      item.FromName = await _countryRepository.GetEntityNamesByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.From.GetValueOrDefault());
      item.ToName = await _countryRepository.GetEntityNamesByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.To.GetValueOrDefault());
      item.ServiceName = listDeliveryServices?.FirstOrDefault(x => x.DeliveryServiceId == item.OriginTypeId)?.ServiceName ?? string.Empty;
      item.OriginTypeName = item.OriginTypeId.HasValue ? Enum.GetName(typeof(EnumCivilEntityType), item.OriginTypeId.Value) ?? string.Empty : string.Empty;
    }


    return data;
  }

  public async Task<ShipperOrder?> GetShiperOrderByOrderNo(string orderNo, Guid? clientId)
  {
    return await _db.ShipperOrders.FirstOrDefaultAsync(x => x.OrderNo == orderNo && x.ClientId == clientId);
  }
  public async Task<List<InvoiceStatus>> GetAllInvoiceStatus()
  {
    return await _db.InvoiceStatuses.ToListAsync();
  }

  public async Task<List<TransactionType>> GetAllTransactionType()
  {
    return await _db.TransactionTypes.ToListAsync();
  }
  public async Task<List<ServiceRateGroup>> GetAllServiceRateGroupForSelection(Guid? clientId)
  {
    return await _db.ServiceRateGroups.Where(x => x.ClientId == clientId).ToListAsync();
  }

}
