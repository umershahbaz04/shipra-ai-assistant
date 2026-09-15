using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Amazon.Util.Internal.PlatformServices;
using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.DTOs.Common.Response;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetStripeWebhook;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Repository;
using Stripe.Tax;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class CarrierRepository : ICarrierRepository
{
  private readonly IConfigRepository _configRepository;
  private readonly ICarrierSharedRepository _carrierSharedRepository;
  private readonly IMapper _mapper;
  private readonly ICountryRepository _countryRepository;
  private readonly ShipraMasterDbContext _shipraMasterDbContext;
  private readonly DapperAppDbContext _dapperAppDbContext;
  private readonly AppDbContext _context;
  private readonly IConfiguration _configuration;


  public CarrierRepository(IConfigRepository configRepository, ICarrierSharedRepository carrierSharedRepository, IMapper mapper, ICountryRepository countryRepository, ShipraMasterDbContext shipraMasterDbContext, DapperAppDbContext dapperAppDbContext, AppDbContext context, IConfiguration configuration)
  {
    _configRepository = configRepository;
    _carrierSharedRepository = carrierSharedRepository;
    _mapper = mapper;
    _countryRepository = countryRepository;
    _shipraMasterDbContext = shipraMasterDbContext;
    _dapperAppDbContext = dapperAppDbContext;
    _context = context;
    _configuration = configuration;
  }

  #region Active Carrier
  public async Task<dynamic> CreateActiveCarrier(ActiveCarrier model)
  {
    await _context.ActiveCarriers.AddAsync(model);
    await _context.SaveChangesAsync();
    return model;
  }
  public async Task<dynamic> UpdateActiveCarrier(ActiveCarrier model)
  {
    _context.ActiveCarriers.Update(model);
    await _context.SaveChangesAsync();
    return model;
  }
  public async Task<bool> DeleteActiveCarrier(ActiveCarrier model)
  {
    bool isDeleted = false;
    _context.Remove(model);
    if (await _context.SaveChangesAsync() > 0)
    {
      isDeleted = true;
    }
    return isDeleted;
  }
  public async Task<ActiveCarrier?> GetActiveCarrierByCarrierId(int carrierId, ClientId? clientId)
  {
    return await _context.ActiveCarriers.Where(x => x.CarrierId == carrierId && x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync()!;
  }
  public async Task<ActiveCarrier?> GetActiveCarrierByActiveCarrierId(int activeCarrierId, ClientId? clientId)
  {
    return await _context.ActiveCarriers.Where(x => x.ActiveCarrierId == activeCarrierId && x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync()!;
  }
  public async Task<List<ShipraContractClientCarrier>?> GetAllShipraContractClientCarriersByClientId(ClientId? clientId)
  {
    return await _context.ShipraContractClientCarriers.Where(x => x.ClientId == clientId).ToListAsync()!;
  }
  public async Task<ActiveCarrier?> GetActiveCarrierByCarrierIdAndUserName(int carrierId, string? userName, ClientId? clientId)
  {
    return await _context.ActiveCarriers.Where(x => x.CarrierId == carrierId && x.ClientId == clientId && x.UserName == userName && x.Active == true).FirstOrDefaultAsync()!;
  }
  public async Task<ActiveCarrier?> GetActiveCarrierById(int activeCarrierId, ClientId? clientId)
  {
    return await _context.ActiveCarriers.Where(x => x.ActiveCarrierId == activeCarrierId && x.ClientId == clientId && x.Active == true).FirstOrDefaultAsync()!;
  }
  public async Task<dynamic> GetAllActiveCarriers(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, int countryId, int deliveryServiceId, ClientId? clientId = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!.Value!.ToString()))
    {
      var dynamicParams = new DynamicParameters();
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId!.Value.ToString(), _context);

      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                             COUNT(*) OVER () AS TotalCount,
                             ac.ActiveCarrierId,
                             ac.CarrierId,
                             CONVERT(NVARCHAR(MAX), ac.SettingConfig) AS SettingConfig,
                             CONCAT(   c.Name,
                                       CASE
                                           WHEN c3.Name IS NOT NULL THEN
                                               CONCAT(' (', c3.Name, ')')
                                           ELSE
                                               ''
                                       END
                                   ) AS Name,
                             ISNULL(c.CarrierImage, '-') AS CarrierImage,
                             ISNULL(c.CarrierWebsite, '-') AS CarrierWebsite,
                             c.IsClientCarrier,
                             ac.IsActiveCarrier,
                             ISNULL(ac.CarrierAlias, '') AS CarrierAlias,
                             ISNULL(ac.UserName, '') AS UserName,
                             ac.CreatedOn,
                             ac.Active,
                             ISNULL(c.DisplayOrder, 0) AS DisplayOrder,
                             ISNULL(c.GuideUrl, '') AS GuideUrl
                      FROM dbo.ActiveCarrier AS ac
                          INNER JOIN dbo.Carrier AS c
                              ON c.CarrierId = ac.CarrierId
                          LEFT JOIN dbo.CarrierDeliveryService AS cds
                              ON cds.CarrierId = c.CarrierId
                          INNER JOIN dbo.Client AS c2
                              ON c2.ClientId = ac.ClientId
                          LEFT JOIN dbo.CarrierLocation AS cl
                              ON cl.CarrierLocationId = ac.CarrierLocationId
                          LEFT JOIN dbo.Country AS c3
                              ON c3.CountryId = cl.CountryId ";

      string whereStart = "WHERE ac.IsActiveCarrier = 1 AND ( 1=1 ";
      string whereEnd = ") ";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        whereStart += @$"And ( (c.Name  LIKE '%{search}%' ) 
                        OR ( ( ac.UserName LIKE '%{search}%' )) 
                        OR ( ( ac.CarrierAlias LIKE '%{search}%' )) )";
      }
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("ac.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("ac.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      if (clientId is not null)
      {
        dynamicParams.Add("@ClientId", clientId.Value.ToString());
        whereStart += "And (ac.ClientId = @ClientId) ";
      }
      if (countryId > 0)
      {
        dynamicParams.Add("@countryId", countryId);
        whereStart += $"And ( cl.CountryId = @countryId ) ";
      }
      if (deliveryServiceId > 0)
      {
        dynamicParams.Add("@deliveryServiceId", deliveryServiceId);
        whereStart += $"And ( cds.DeliveryServiceId  = @deliveryServiceId ) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "ac.CreatedOn");
      string groupBy = @" GROUP BY CONVERT(NVARCHAR(MAX), ac.SettingConfig),
                                       CONCAT(   c.Name,
                                       CASE
                                       WHEN c3.Name IS NOT NULL THEN
                                       CONCAT(' (', c3.Name, ')')
                                       ELSE
                                       ''
                                       END
                                       ),
                                       ISNULL(c.CarrierImage, '-'),
                                       ISNULL(c.CarrierWebsite, '-'),
                                       ISNULL(ac.CarrierAlias, ''),
                                       ISNULL(ac.UserName, ''),
                                       ISNULL(c.DisplayOrder, 0),
                                       ISNULL(c.GuideUrl, ''),
                                       ac.ActiveCarrierId,
                                       ac.CarrierId,
                                       c.IsClientCarrier,
                                       ac.IsActiveCarrier,
                                       ac.CreatedOn,
                                       ac.Active ";

      string queryData = query + where + groupBy + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";
      var data = await connection.QueryAsync(queryData, dynamicParams);

      foreach (var item in data.ToList())
      {
        item.CarrierFeature = await GetAllFeatureCarriers(item.CarrierId);
      }
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

  public async Task<List<ActiveCarriersForSelectionResponseModel>> GetAllActiveCarriersForSelection(string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"
            SELECT ac.ActiveCarrierId,
                   ac.CarrierId,
                   ac.CarrierContractTypeId,
                   c.Config,
                   0 AS ShipraContractCarrierId,
                   ISNULL(c.IsDispatchExCompany,0) AS IsDispatchExCompany,
                   COALESCE(CONCAT(c.Name, ' - ', ac.CarrierAlias), c.Name) AS Name,
                   ISNULL(ac.CarrierAlias, '') AS CarrierAlias,
                   ISNULL(c.IsRateCheck, 0) AS IsRateCheck,
                   ISNULL(c.ValidateAddress, 0) AS ValidateAddress
            FROM dbo.ActiveCarrier AS ac
                INNER JOIN dbo.Carrier AS c
                    ON c.CarrierId = ac.CarrierId
                INNER JOIN dbo.Client AS c2
                    ON c2.ClientId = ac.ClientId ";

      string whereStart = "WHERE ac.Active = 1 AND (1=1 AND (c.CarrierId < 10000) ";
      string whereEnd = ") ";

      dynamicParams.Add("@ClientId", clientId);
      whereStart += "AND ac.ClientId = @ClientId "; // Ensuring the correct ClientId filter

      string where = whereStart + whereEnd;
      string queryData = query + where;

      var data = await connection.QueryAsync<ActiveCarriersForSelectionResponseModel>(queryData, dynamicParams); // Ensure it's a List<dynamic>
      var dataList = data.ToList();
      var clId = new ClientId(new Guid(clientId));
      var allowShipraCarriers = await GetAllFilterdShipraContractClientCarrierByShipraClientCarrier(clId);

      if (allowShipraCarriers.Any())
      {
        dataList.AddRange(allowShipraCarriers);
      }

      return dataList;
    }
  }
  public async Task<List<ActiveCarrierPickupLocationForSelectionResponseModel>> GetAllActiveCarrierPickupLocationByActiveCarrierIdForSelection(int? activeCarrierId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      string query = @"
                          SELECT acpl.ActiveCarrierPickupLocationId,
                           acpl.ActiveCarrierId,
                           acpl.CarrierId,
                           acpl.CountryId,
                           acpl.StreetAddress,
                           acpl.LocationName,
                           c2.Name AS CountryName,
                           CASE
                               WHEN acpl.LocationCode IS NOT NULL
                                    AND LTRIM(RTRIM(acpl.LocationCode)) <> '' THEN
                                   acpl.LocationCode + ' | ' + acpl.FullAddress
                               ELSE
                                   acpl.FullAddress
                           END AS FullAddress,
                           cc.CarrierImage,
                           cc.Name AS CarrierName
                    FROM dbo.ActiveCarrierPickupLocation AS acpl
                        INNER JOIN dbo.Client AS c
                            ON c.ClientId = acpl.ClientId
                        INNER JOIN dbo.Carrier AS cc
                            ON cc.CarrierId = acpl.CarrierId
                        INNER JOIN dbo.Country AS c2
                            ON c2.CountryId = acpl.CountryId 
		               ";

      string whereStart = "WHERE acpl.Active = 1 AND (1=1 ";
      string whereEnd = ") ";

      dynamicParams.Add("@ClientId", clientId);
      whereStart += "AND acpl.ClientId = @ClientId "; // Ensuring the correct ClientId filter

      if (activeCarrierId.GetValueOrDefault() > 0)
      {
        dynamicParams.Add("@activeCarrierId", activeCarrierId);
        whereStart += "AND acpl.ActiveCarrierId = @activeCarrierId "; // Ensuring the correct ClientId filter
      }
      string where = whereStart + whereEnd;
      string queryData = query + where;

      var data = await connection.QueryAsync<ActiveCarrierPickupLocationForSelectionResponseModel>(queryData, dynamicParams); // Ensure it's a List<dynamic>
      var dataList = data.ToList();
      return dataList;
    }
  }
  public async Task<ActiveCarrierPickupLocation?> GetActiveCarrierLocationbyId(int ActivePickupLocationId, ClientId? clientId)
  {
    return await _context.ActiveCarrierPickupLocations.FirstOrDefaultAsync(x => x.ActiveCarrierPickupLocationId! == ActivePickupLocationId && x.ClientId == clientId);
  }

  public async Task<List<ActiveCarriersForSelectionResponseModel>> GetAllFilterdShipraContractClientCarrierByShipraClientCarrier(ClientId clientId)
  {
    var allShipraContractClientCarrier = await GetAllShipraContractClientCarrier(clientId);
    var shipraContractCarriers = await GetShipraContractCarriers();
    var carriers = await _shipraMasterDbContext.Carriers.ToListAsync(); // Assuming you have a method to get carriers

    var result = allShipraContractClientCarrier
       .Join(shipraContractCarriers!, accc => accc.ShipraContractCarrierId, scc => scc.ShipraContractCarrierId, (accc, scc) => new { accc, scc })
       .Join(carriers, x => x.scc.CarrierId, c => c.CarrierId, (x, c) => new ActiveCarriersForSelectionResponseModel
       {
         ShipraContractCarrierId = x.scc.ShipraContractCarrierId,
         Config = c.Config,
         ActiveCarrierId = 0,
         CarrierId = x.scc.CarrierId,
         IsDispatchExCompany = c.IsDispatchExCompany,
         CarrierContractTypeId = x.scc.CarrierContractTypeId,
         Name = string.IsNullOrEmpty(x.scc.CarrierAlias) ? c.Name : $"{c.Name} - Shipra",
         CarrierAlias = x.scc.CarrierAlias ?? "",
         IsRateCheck = c.IsRateCheck ?? false, // Default to false if null
         ValidateAddress = c.ValidateAddress ?? false // Default to false if null
       })
       .ToList();

    return result;
  }
  public async Task<CarrierLocation> GetCarrierLocationByCarrier(Carrier carrier)
  {
    var carrierID = carrier != null ? carrier.CarrierId : 0;
    var countryId = carrier != null ? carrier.CountryId : 0;
    var data = await _shipraMasterDbContext.CarrierLocations.FirstOrDefaultAsync(x => x.CarrierId == carrierID && x.CountryId == countryId && x.Active == true);
    return data!;
  }
  public async Task<List<CarrierLocationWithCountryResponseModel>> GetAllCarrierLocationsByCarrier(Carrier carrier)
  {
    var carrierID = carrier != null ? carrier.CarrierId : 0;
    if (carrierID == 0)
    {
      return new List<CarrierLocationWithCountryResponseModel>();
    }
    var data = await _shipraMasterDbContext.CarrierLocations
        .Where(cl => cl.CarrierId == carrierID && cl.Active == true)
        .Join(_shipraMasterDbContext.Countries,
            cl => cl.CountryId,
            c => c.CountryId,
            (cl, c) => new CarrierLocationWithCountryResponseModel
            {
              CarrierLocationId = cl.CarrierLocationId,
              CountryId = cl.CountryId,
              CarrierId = cl.CarrierId,
              Active = cl.Active,
              AddressingScheme = cl.AddressingScheme,
              CountryName = c.Name
            })
        .ToListAsync();


    return data;
  }
  public async Task<List<CivilEntityExtended>> GetCivilEntityExtended(int carrierId)
  {
    var data = await _shipraMasterDbContext.CivilEntityExtendeds.Where(x => x.CarrierId == carrierId).ToListAsync();
    return data!;
  }
  public async Task<List<ShipraContractCarrier>?> GetShipraContractCarriers()
  {
    return await _shipraMasterDbContext.ShipraContractCarriers.Where(x => x.Active == true).ToListAsync();
  }
  public async Task<List<ShipraContractClientCarrier>> GetAllShipraContractClientCarrier(ClientId clientId)
  {
    var data = await _context.ShipraContractClientCarriers.Where(x => x.ClientId == clientId).ToListAsync();
    return data;
  }
  public async Task<ShipraContractClientCarrier?> GetShipraContractClientCarrier(ClientId clientId, int shipraContractCarrierId)
  {
    var data = await _context.ShipraContractClientCarriers.FirstOrDefaultAsync(x => x.ClientId == clientId && x.ShipraContractCarrierId == shipraContractCarrierId);
    return data;
  }

  public async Task<List<dynamic>> DashbaordGetAllActiveCarriers(string clientId)
  {
    var query = $"SELECT * FROM dbo.[ActiveCarrier] where ClientId='{clientId}' ";
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var data = await connection.QueryAsync(query);
      return data.ToList();
    }
  }
  public async Task<ActiveCarrier> GetActiveCarrierByAlias(string? carrierAlias, ClientId? clientId)
  {
    var data = await _context.ActiveCarriers.Where(x => x.CarrierAlias!.Trim().ToLower() == carrierAlias!.Trim().ToLower() && x.ClientId == clientId).FirstOrDefaultAsync();

    return data!;
  }
  public async Task<ActiveCarrier> GetActiveCarrierByCarrierAlias(int? carrierId, string? carrierAlias, ClientId? clientId)
  {
    var data = await _context.ActiveCarriers.Where(x => x.CarrierId == carrierId && x.CarrierAlias!.Trim().ToLower() == carrierAlias!.Trim().ToLower() && x.ClientId == clientId).FirstOrDefaultAsync();

    return data!;
  }
  public async Task<List<ActiveCarrier>> GetActiveCarrieriersByCarrierId(int? carrierId, ClientId? clientId)
  {
    var data = await _context.ActiveCarriers.Where(x => x.CarrierId == carrierId && x.ClientId == clientId).ToListAsync();

    return data!;
  }
  #endregion

  #region Carrier
  public async Task<dynamic> CreateCarrier(Carrier model)
  {
    await _context.Carriers.AddAsync(model);
    await _context.SaveChangesAsync();
    return model;
  }
  public async Task<dynamic> UpdateCarrier(Carrier model)
  {
    _context.Carriers.Update(model);
    await _context.SaveChangesAsync();
    return model;
  }
  public async Task<bool> DeleteCarrier(Carrier model)
  {
    bool isDeleted = false;
    _context.Remove(model);
    if (await _context.SaveChangesAsync() > 0)
    {
      isDeleted = true;
    }
    return isDeleted;
  }
  public async Task<Carrier?> GetCarrierFromMasterDbById(int id)
  {
    return await _shipraMasterDbContext.Carriers.FirstOrDefaultAsync(x => x.CarrierId == id && x.Active == true);
  }


  public async Task<Carrier?> GetCarrierById(int id)
  {
    return await _context.Carriers.FirstOrDefaultAsync(x => x.CarrierId == id && x.Active == true);
  }
  public async Task<dynamic> GetAllCarriers(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId))
    {
      var dynamicParams = new DynamicParameters();
      var regionMinuts = await CommonUtility.GetClientRegionMinutes(clientId, _context);

      string query = @"SELECT 
                      ROW_NUMBER() OVER (ORDER BY (SELECT 1)) As RowNum,
                      COUNT(*) OVER () AS TotalCount, 
                      c.CarrierId,
                      c.Name,
                      c.CarrierImage,
                      c.CarrierWebsite,
                      c.IsClientCarrier,
                      CAST(0 AS bit) AS IsActiveCarrier,
                      c.Config,
                      c.InputRequiredConfig,
	                    c2.Name AS CountryName,
	                    ISNULL(c.GuideUrl,'') as GuideUrl
                      FROM dbo.Carrier AS c 
                      LEFT JOIN dbo.Country AS c2 ON c2.CountryId = c.CountryId   ";
      string whereStart = "WHERE c.IsClientCarrier <> 1 AND c.Active = 1 AND ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        dynamicParams.Add("@search", search);
        whereStart += "And ( ( c.Name in (select value from STRING_SPLIT(@search,',')))) ";
      }
      //if (!string.IsNullOrEmpty(clientId))
      //{
      //  dynamicParams.Add("@ClientId", clientId);
      //  whereStart += @$" AND ac.ClientId != '{clientId}'  AND ac.ActiveCarrierId IS NULL ";
      //}

      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("c.CreatedOn", regionMinuts)} AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += $" And (CAST({CommonUtility.GetFormatedDateStr("c.CreatedOn", regionMinuts)} AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "c.DisplayOrder");
      sortDir = "ASC";
      string queryData = query + where + " ORDER BY " + keyValuePairs[sortCol] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      foreach (var item in data.ToList())
      {
        item.CarrierFeature = await GetAllFeatureCarriers(item.CarrierId);
      }
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
  public async Task<dynamic> GetAllActiveCarrierForCreateOrderSelectionFilterByCountry(string? clientId, int? countryId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClient(clientId!))
    {
      var dynamicParams = new DynamicParameters();

      string query = @" SELECT ac.CarrierId,
                               c.Name,
                               cl.AddressingScheme,
                               ISNULL(c.IsRateCheck, 0) AS IsRateCheck,
                               ISNULL(c.ValidateAddress, 0) AS ValidateAddress
                        FROM dbo.ActiveCarrier AS ac
                            INNER JOIN dbo.Carrier AS c
                                ON c.CarrierId = ac.CarrierId
                            LEFT JOIN dbo.CarrierLocation AS cl
                                ON ac.CarrierId = cl.CarrierId ";
      string whereStart = $"WHERE c.IsClientCarrier <> 1 AND c.Active = 1 AND ac.Active = 1 AND ( 1=1 ";
      string whereEnd = ")";



      if (!string.IsNullOrEmpty(clientId))
      {
        dynamicParams.Add("@ClientId", clientId);
        whereStart += @$" AND ac.ClientId = '{clientId}' ";
      }
      if (countryId > 0)
      {
        dynamicParams.Add("@countryId", countryId);
        whereStart += @$" AND cl.CountryId = @countryId ";
      }

      string groupBy = @" GROUP BY ISNULL(c.IsRateCheck, 0),
                 ISNULL(c.ValidateAddress, 0),
                 ac.CarrierId,
                 c.Name,
                 cl.AddressingScheme ";
      string where = whereStart + whereEnd + groupBy;

      string queryData = query + where + " ORDER BY ac.CarrierId DESC ";

      var data = await connection.QueryAsync(queryData, dynamicParams);

      return data.ToList();
    }
  }
  public async Task<List<dynamic>> DashbaordGetAllCarriers(DateTime? createdFrom, DateTime? createdTo)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var dynamicParams = new DynamicParameters();

      var query = @"SELECT * FROM Carrier as c ";
      //default carrier
      string whereStart = " WHERE (c.Active = 1 AND c.IsDefault = 0 OR c.IsDefault IS NULL AND c.IsClientCarrier <> 1 AND (1=1)";
      string whereEnd = ")";
      if (createdFrom != null)
      {
        dynamicParams.Add("@createdFrom", createdFrom);
        whereStart += "And (CAST(c.CreatedOn AS DATE) >= CAST(@createdFrom AS DATE)) ";
      }
      if (createdTo != null)
      {
        dynamicParams.Add("@createdTo", createdTo);
        whereStart += "And (CAST(c.CreatedOn AS DATE) <= CAST(@createdTo AS DATE)) ";
      }
      var where = whereStart + whereEnd;
      var data = await connection.QueryAsync(query + where);
      return data.ToList();
    }
  }
  #endregion

  #region Webhook //we will use webhookurlhash from master table
  public async Task<bool> CreateWebhookUrlHash(WebhookUrlHash webhookUrlHash)
  {
    await _shipraMasterDbContext.WebhookUrlHashes.AddAsync(webhookUrlHash);
    return await _shipraMasterDbContext.SaveChangesAsync() > 0;
  }

  public async Task<WebhookUrlHash> CheckWebhookUrlHashByClientAndCarrierId(int carrierId, Guid clientId)
  {

    var tarfet = await _shipraMasterDbContext.WebhookUrlHashes.FirstOrDefaultAsync(x => x.CarrierId == carrierId && x.ClientId == clientId);
    return tarfet!;

  }
  public async Task<WebhookUrlHash> CheckWebhookUrlHashByContractIdAndCarrierId(int carrierId, int carrierContractTypeId)
  {

    var tarfet = await _shipraMasterDbContext.WebhookUrlHashes.FirstOrDefaultAsync(x => x.CarrierId == carrierId && x.CarrierContractTypeId == carrierContractTypeId);
    return tarfet!;

  }
  #endregion
  //public async Task<List<Carrier>> GetAllClientCarrier()
  //{
  //  var allCarriers = await _context.Carriers.Where(x => x.IsClientCarrier == true).ToListAsync();
  //  return allCarriers!;
  //} 
  public async Task<dynamic?> GetAllFeatureCarriers(int carrierId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT cf.CarrierFeatureId,
                               cf.Feature,
                               c.CarrierId
                        FROM dbo.CarrierFeature AS cf
                        INNER JOIN dbo.Carrier AS c
        ON c.CarrierId = cf.CarrierId ";
      string whereStart = $"WHERE cf.carrierId = {carrierId} AND ( 1=1 ";
      string whereEnd = ")";
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync(queryData);
      return data.ToList();
    }
  }
  #region carrier with location and service 
  public async Task<dynamic> GetAllCarrierWithServiceAndLocation(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, int countryId, int deliveryServiceId, string clientId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT 
                      ROW_NUMBER() OVER (ORDER BY (SELECT 1)) As RowNum,
                      COUNT(*) OVER () AS TotalCount, 
                      c.CarrierId,
                       c.Name,
                       c.CarrierImage,
                       c.CarrierWebsite,
                       ISNULL(c.BackgroundColor,'#65a3a673') AS BackgroundColor,
					             ISNULL(c.BorderColor,'#65a3a673') AS BorderColor,
                       ISNULL(c.GuideUrl,'') as GuideUrl
                FROM dbo.Carrier AS c
                    LEFT JOIN dbo.CarrierDeliveryService AS cds
                        ON cds.CarrierId = c.CarrierId
                    LEFT JOIN dbo.CarrierLocation AS cl
                        ON cl.CarrierId = c.CarrierId   ";
      string whereStart = @"WHERE c.IsClientCarrier <> 1
                            AND c.Active = 1
                            AND cds.Active = 1
                            AND cl.Active = 1
                            AND ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        whereStart += $"And ( c.Name LIKE '%{search}%' ) ";
      }
      if (countryId > 0)
      {
        dynamicParams.Add("@countryId", countryId);
        whereStart += $"And ( cl.CountryId = @countryId ) ";
      }
      if (deliveryServiceId > 0)
      {
        dynamicParams.Add("@deliveryServiceId", deliveryServiceId);
        whereStart += $"And ( cds.DeliveryServiceId  = @deliveryServiceId ) ";
      }
      var groupBy = @" GROUP BY c.CarrierId,
                               c.Name,
                               c.CarrierImage,
                               c.CarrierWebsite,
                                c.BackgroundColor,
                                c.GuideUrl,
                                c.BorderColor ";
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "c.Name");
      sortDir = "ASC";
      string queryData = query + where + groupBy + " ORDER BY " + keyValuePairs[0] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

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
  public async Task<dynamic> GetCarrierWithServiceAndLocationByCarrierId(int carrierId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT c.CarrierId,
                               c.Name,
                               c.CarrierImage,
	                             c.CarrierWebsite 
                        FROM dbo.Carrier AS c
                            LEFT JOIN dbo.CarrierDeliveryService AS cds
                                ON cds.CarrierId = c.CarrierId
                            LEFT JOIN dbo.CarrierLocation AS cl
                                ON cl.CarrierId = c.CarrierId ";
      string whereStart = @"WHERE c.IsClientCarrier <> 1 AND ( 1=1 ";
      string whereEnd = ")";

      if (carrierId > 0)
      {
        dynamicParams.Add("@carrierId", carrierId);
        whereStart += "And ( c.CarrierId = @carrierId ) ";
      }

      var groupBy = @" GROUP BY c.CarrierId,
                       c.Name,
                       c.CarrierImage,
                       c.CarrierWebsite ";
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();

      string queryData = query + where + groupBy;

      var data = await connection.QueryAsync(queryData, dynamicParams);

      var dataList = data.ToList();
      var obj = dataList!.FirstOrDefault()!;
      if (obj != null)
      {
        obj.LocationNames = await GetCarrierLocationsByCarrierId(carrierId);
        obj.CarrierServices = await GetCarrierServiceByCarrierId(carrierId);
        obj.CarrierFeature = await GetAllFeatureCarriers(carrierId);
      }
      return obj!;
    }
  }
  public async Task<dynamic> GetCarrierServiceByCarrierId(int carrierId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT cds.CarrierDeliveryServiceId,
                               ds.ServiceName,
                               ds.ServiceLogo
                        FROM dbo.CarrierDeliveryService AS cds
                            INNER JOIN dbo.DeliveryService AS ds
                                ON ds.DeliveryServiceId = cds.DeliveryServiceId   ";
      string whereStart = @"WHERE ( 1=1 ";
      string whereEnd = ")";

      if (carrierId > 0)
      {
        dynamicParams.Add("@carrierId", carrierId);
        whereStart += "And ( cds.CarrierId = @carrierId ) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);

      var dataList = data.ToList();
      return dataList!;
    }
  }
  public async Task<dynamic> GetCarrierLocationsByCarrierId(int carrierId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT STRING_AGG(c.Name, ',') AS LocationName
                        FROM dbo.CarrierLocation AS cl
                        INNER JOIN dbo.Country AS c ON c.CountryId = cl.CountryId ";
      string whereStart = @"WHERE  ( 1=1 AND cl.Active = 1 ";
      string whereEnd = ")";

      if (carrierId > 0)
      {
        dynamicParams.Add("@carrierId", carrierId);
        whereStart += "And ( cl.CarrierId = @carrierId ) ";
      }

      string where = whereStart + whereEnd;

      string queryData = query + where;

      var data = await connection.QueryAsync(queryData, dynamicParams);

      var dataList = data.ToList();
      var obj = dataList!.FirstOrDefault()!;
      return obj == null ? "" : obj!.LocationName;
    }
  }

  public async Task<List<DeliveryService>> GetAllDeliveryService()
  {
    return await _shipraMasterDbContext.DeliveryServices.ToListAsync()!;
  }
  public async Task<List<CarrierDeliveryService>> GetAllCarrierDeliveryServices(int carrierId)
  {
    return await _shipraMasterDbContext.CarrierDeliveryServices.Where(x => x.CarrierId == carrierId).ToListAsync()!;
  }
  public async Task<List<CarrierLocation>> GetAllCarrierLocations(int carrierId)
  {
    return await _shipraMasterDbContext.CarrierLocations.Where(x => x.CarrierId == carrierId).ToListAsync()!;
  }

  public async Task<ShipraContractCarrier?> GetShipraContractCarrierByCarrierId(int carrierId)
  {
    return await _shipraMasterDbContext.ShipraContractCarriers.FirstOrDefaultAsync(x => x.CarrierId == carrierId && x.Active == true);
  }
  public async Task<ShipraContractCarrier?> GetShipraContractCarrierByContractId(int shipraContractCarrierId)
  {
    return await _shipraMasterDbContext.ShipraContractCarriers.FirstOrDefaultAsync(x => x.ShipraContractCarrierId == shipraContractCarrierId && x.Active == true);
  }

  #endregion
  #region contract carrier with location and service 
  public async Task<dynamic> GetAllShipraContractCarrierWithServiceAndLocation(int start, int length, string search, int sortCol, string sortDir, int countryId, int deliveryServiceId, int deliveryTypeId, bool? isForAdmin, ClientId? clientId = null)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var filtedContractCarriersForClient = string.Empty;
      if (clientId != null)
      {
        var allowShipraCarriers = await GetAllFilterdShipraContractClientCarrierByShipraClientCarrier(clientId);
        filtedContractCarriersForClient = string.Join(",", allowShipraCarriers.Select(x => x.ShipraContractCarrierId));
      }

      var dynamicParams = new DynamicParameters();
      var whereStr = isForAdmin.GetValueOrDefault() ? " " : @" AND scc.Active = 1 ";
      string query = @"SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS RowNum,
                               COUNT(*) OVER () AS TotalCount,
                               scc.ShipraContractCarrierId,
                               scc.FlatRate,
                               scc.CarrierId,
                               c.Name AS CarrierName,
                               c.CarrierImage,
                               c.CarrierWebsite,
                               STRING_AGG(ds.ServiceName, ', ') AS ServiceNames,
                               ISNULL(c.GuideUrl, '') AS GuideUrl,
                               cs.DropOffMethod,
                               cs.DeliveryTime,
                               cs.DeliveryMethod,
                               cs.Config AS SlaConfig,
                               ISNULL(scc.Active,0) AS Active
                        FROM dbo.ShipraContractCarrier AS scc
                            INNER JOIN dbo.Carrier AS c
                                ON c.CarrierId = scc.CarrierId
                            LEFT JOIN dbo.CarrierSLA AS cs
                                ON cs.CarrierId = c.CarrierId
                            INNER JOIN dbo.CarrierDeliveryService AS cds
                                ON cds.CarrierId = c.CarrierId
                            INNER JOIN dbo.DeliveryService AS ds
                                ON ds.DeliveryServiceId = cds.DeliveryServiceId
                            INNER JOIN dbo.CarrierLocation AS cl
                                ON cl.CarrierId = c.CarrierId
                            INNER JOIN dbo.Country AS c2
                                ON c2.CountryId = cl.CountryId ";
      string whereStart = $@"WHERE c.IsClientCarrier <> 1
                           AND c.Active = 1
                           {whereStr}
                           AND cds.Active = 1
                           AND cl.Active = 1 
                           AND ( 1=1 ";
      string whereEnd = ")";

      dynamicParams.Add("displayStart", start);
      dynamicParams.Add("displayLength", length);

      if (!string.IsNullOrEmpty(search))
      {
        whereStart += $"And ( c.Name LIKE '%{search}%' ) ";
      }
      if (countryId > 0)
      {
        dynamicParams.Add("@countryId", countryId);
        whereStart += $"And ( cl.CountryId = @countryId ) ";
      }
      if (deliveryServiceId > 0)
      {
        dynamicParams.Add("@deliveryServiceId", deliveryServiceId);
        whereStart += $"And ( cds.DeliveryServiceId  = @deliveryServiceId ) ";
      }
      if (clientId != null)
      {
        dynamicParams.Add("@shipraContractCarrierId", filtedContractCarriersForClient);
        whereStart += @"And ( ( scc.ShipraContractCarrierId in (select value from STRING_SPLIT(@shipraContractCarrierId,',')))) ";
      }
      var groupBy = @" GROUP BY ISNULL(c.GuideUrl, ''),
                                                scc.ShipraContractCarrierId,
                                                scc.FlatRate,
                                                scc.CarrierId,
                                                c.Name,
                                                c.CarrierImage,
                                                c.CarrierWebsite,
                                                cs.DropOffMethod,
                                                cs.DeliveryTime,
                                                cs.DeliveryMethod,
                                                cs.Config,
                                                scc.Active ";
      string where = whereStart + whereEnd;

      Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();
      keyValuePairs.Add(0, "scc.ShipraContractCarrierId");
      sortDir = "ASC";
      string queryData = query + where + groupBy + " ORDER BY " + keyValuePairs[0] + " " + sortDir + " OFFSET @displayStart ROWS FETCH NEXT @displayLength ROWS ONLY; ";

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
  public async Task<dynamic> GetShipraContractByShipraContractCarrierId(int shipraContractCarrierId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT c.Name AS CarrierName,
                             c.CarrierImage,
                             c.CarrierWebsite,
                             scc.FlatRate,
                             scc.ShipraContractCarrierId,
                             scc.CarrierContractTypeId,
                             scc.CarrierId,
                             STRING_AGG(ds.ServiceName, ', ') AS ServiceNames,
	                         cs.DropOffMethod,
                             cs.DeliveryTime,
                             cs.DeliveryMethod,
                             cs.Config AS SlaConfig
                      FROM dbo.ShipraContractCarrier AS scc
                          INNER JOIN dbo.Carrier AS c
                              ON c.CarrierId = scc.CarrierId
	                      LEFT JOIN dbo.CarrierSLA AS cs
                              ON cs.CarrierId = c.CarrierId
                          INNER JOIN dbo.CarrierDeliveryService AS cds
                              ON cds.CarrierId = c.CarrierId
                          INNER JOIN dbo.DeliveryService AS ds
                              ON ds.DeliveryServiceId = cds.DeliveryServiceId  ";
      string whereStart = @"WHERE c.IsClientCarrier <> 1
                            AND c.Active = 1
                            AND scc.Active = 1
                            AND cds.Active = 1 
                            AND ( 1=1 ";
      string whereEnd = ")";

      if (shipraContractCarrierId > 0)
      {
        dynamicParams.Add("@shipraContractCarrierId", shipraContractCarrierId);
        whereStart += $"And ( scc.ShipraContractCarrierId = @shipraContractCarrierId ) ";
      }
      var groupBy = @" GROUP BY c.Name,
                       c.CarrierImage,
                       c.CarrierWebsite,
                       scc.FlatRate,
                       scc.ShipraContractCarrierId,
                       scc.CarrierContractTypeId,
                       scc.CarrierId,
                       cs.DropOffMethod,
                       cs.DeliveryTime,
                       cs.DeliveryMethod,
                       cs.Config ";
      string where = whereStart + whereEnd;

      string queryData = query + where + groupBy;

      var data = await connection.QueryAsync(queryData, dynamicParams);

      var dataList = data.ToList();
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        return firstRecord!;
      }
      return null!;
    }
  }

  public async Task<ShipraContractCarrier> GetShipraContractCarrierByAlias(string? carrierAlias, int? carrierId)
  {
    var data = await _shipraMasterDbContext.ShipraContractCarriers.Where(x => x.CarrierId == carrierId).FirstOrDefaultAsync()!;
    return data!;
  }

  public async Task<bool> CreateShipraContractCarrier(ShipraContractCarrier shipraContractCarrier)
  {
    await _shipraMasterDbContext.ShipraContractCarriers.AddAsync(shipraContractCarrier);
    return await _shipraMasterDbContext.SaveChangesAsync() > 0;
  }

  public async Task<ShipraContractCarrier> GetShipraCarrierContractByCarrierId(int? shipraContractCarrierId)
  {
    var data = await _shipraMasterDbContext.ShipraContractCarriers.FirstOrDefaultAsync(x => x.ShipraContractCarrierId == shipraContractCarrierId);
    return data!;
  }
  public async Task<bool> UpdateShipraCarrierContractByCarrierId(ShipraContractCarrier shipraContractCarrier)
  {
    _shipraMasterDbContext.ShipraContractCarriers.Update(shipraContractCarrier);
    return await _shipraMasterDbContext.SaveChangesAsync() > 0;
  }

  #endregion
  #region contract carrier
  public async Task<bool> DeleteShipraContractClientCarrierAsync(ShipraContractClientCarrier carrier)
  {
    _context.ShipraContractClientCarriers.Remove(carrier);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<ShipraContractClientCarrier> GetShipraContractClientCarrierById(int shipraContractClientCarrierId)
  {
    var data = await _context.ShipraContractClientCarriers.FirstOrDefaultAsync(x => x.ShipraContractClientCarrierId == shipraContractClientCarrierId);
    return data!;
  }
  public async Task<bool> AddShipraContractClientCarrierAsync(ShipraContractClientCarrier carrier)
  {
    await _context.ShipraContractClientCarriers.AddAsync(carrier);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<bool> CreateActiveCarrierPickupLocation(ActiveCarrierPickupLocation acp)
  {
    await _context.ActiveCarrierPickupLocations.AddAsync(acp);
    return await _context.SaveChangesAsync() > 0;
  }
  #endregion

  public async Task<bool> deletepickLocation(ActiveCarrierPickupLocation model)
  {
    bool isDeleted = false;
    _context.Remove(model);
    if (await _context.SaveChangesAsync() > 0)
    {
      isDeleted = true;
    }
    return isDeleted;
  }
  public async Task<dynamic> UpdatepickLocation(ActiveCarrierPickupLocation model)
  {
    _context.ActiveCarrierPickupLocations.Update(model);
    await _context.SaveChangesAsync();
    return model;
  }
  public async Task<dynamic> GetAllClientRateAsync(PriceCalculatorFilter filter)
  {
    if (string.IsNullOrEmpty(filter.ClientId))
      throw new ArgumentNullException(nameof(filter.ClientId));

    var clIdGuid = new ClientId(new Guid(filter.ClientId));

    #region if order no exist
    if (!string.IsNullOrEmpty(filter.Search))
    {
      filter.From = 0;
      filter.To = 0;
      var order = await _context.Orders.FirstOrDefaultAsync(x => x.ClientId == clIdGuid && x.OrderNo == filter.Search);

      OrderAddress? orderAddress = null;
      StoreAddress? oStoreAddress = null;

      // Convert to enum
      var type = (EnumCivilEntityType)filter.OrigionTypeId.GetValueOrDefault();
      // Convert to lowercase key
      string key = type.ToString().ToLower();

      if (order != null)
      {
        if (order.OrderAddressId.HasValue)
        {
          oStoreAddress = await _context.StoreAddresses.FirstOrDefaultAsync(x => x.StoreId == order.StoreId);
          if (oStoreAddress is not null)
          {
            var oAddress = CivilEntityHelper.GetEnumValueFromAddressEntityMap(oStoreAddress);
            if (oAddress is not null)
            {
              filter.From = oAddress.LastOrDefault().Value;
            }
          }
        }

        orderAddress = await _context.OrderAddresses.FirstOrDefaultAsync(x => x.OrderAddressId == order.OrderAddressId);
        if (orderAddress is not null)
        { // we are assuming address inside order is our db address not from third part tables 
          var oAddressAddress = CivilEntityHelper.GetEnumValueFromAddressEntityMap(orderAddress);
          if (oAddressAddress is not null)
          {
            filter.To = oAddressAddress.LastOrDefault().Value;
          }
        }
      }
      filter.Weight = order!.Weight.GetValueOrDefault(1);
      filter.Amount = order.Amount.GetValueOrDefault();
    }
    #endregion

    using (var connection = _dapperAppDbContext.CreateConnection())
    {
      var dynamicParams = new DynamicParameters();
      //dynamicParams.Add("displayStart", start);
      //dynamicParams.Add("displayLength", length);
      dynamicParams.Add("ClientId", filter.ClientId); // filter by client 
      dynamicParams.Add("From", filter.From);
      dynamicParams.Add("To", filter.To);

      string query = $@"
                      SELECT  
                      cr3.ClientRateId,
                      cr3.carrierRateId,
                      c2.Name AS CarrierName,
                      c2.CarrierId,
                      c2.CarrierImage,
                      c2.IsRateCheck,
                      c2.settingconfig,
                      ds.ServiceName,
                      ISNULL(cr3.Weight, 1) AS ClientWeight,
                      ISNULL(cr3.Rate,0) AS ClientRate,
                      cr3.AdditionalRate AS ClientAdditionalRate,
                      cr.Code,
                      cr.OriginTypeId,
                      ul.UOMName,
                      ISNULL(cr.Unit,0) as Unit,
                      cr.CalculationMethodId,
                      otl.OriginTypeName,
                      rtl.RateTypeName,
                      rtl.RateTypeId,
                      ul.UOMName,
                      cr.UomId,
                      cr.Weight,
                      ISNULL(cs.DeliveryTime, '') AS DeliveryTime,
                      ISNULL(cs.DropOffMethod, '') AS DropOffMethod,
                      ISNULL(cs.DeliveryMethod, '') AS DeliveryMethod,
                      cr.CalculationTypeId,
                      cr.[From],
                      cr.[To],

                      -- UOMFrom
                      CASE 
                          WHEN cr.CalculationMethodId = {(int)EnumCalculationMethod.Flat} THEN ISNULL(cr.Weight,0)
                          ELSE ISNULL(crs2.UomFrom,0) 
                      END AS UomFrom,

                      -- UOMTo
                      CASE 
                          WHEN cr.CalculationMethodId = {(int)EnumCalculationMethod.Flat}  THEN ISNULL(cr.Weight,0)
                          ELSE ISNULL(crs2.UomTo,0) 
                      END AS UomTo,

                      -- Rate
                      CASE 
                          WHEN cr.CalculationMethodId = {(int)EnumCalculationMethod.Flat}  THEN ISNULL(cr.Rate,0)
                          ELSE ISNULL(crs.Rate,0) 
                      END AS Rate
                                FROM dbo.CarrierAssignedContracts AS cc
                                     LEFT JOIN dbo.Contract AS c
                                         ON c.ContractId = cc.ContractId
                                     LEFT JOIN dbo.ContractCarrierRates AS ccr
                                         ON ccr.ContractId = cc.ContractCarrierId
                                     INNER JOIN dbo.ClientRate AS cr3
                                         ON cr3.ContractCarrierRateId = ccr.ContractCarrierRateId
                                     LEFT JOIN dbo.ClientRateSlab AS crs
                                         ON crs.ClientRateId = cr3.ClientRateId
                                     LEFT JOIN dbo.CarrierRateSlab AS crs2
                                         ON crs2.CarrierRateSlabId = crs.CarrierRateSlabId
                                     INNER JOIN dbo.CarrierRate AS cr
                                         ON ccr.CarrierRateId = cr.CarrierRateId
                                     LEFT JOIN dbo.Carrier AS c2
                                         ON cc.CarrierId = c2.CarrierId
                                     LEFT JOIN dbo.OriginTypeLookup AS otl
                                         ON otl.OriginTypeId = cr.OriginTypeId
                                     LEFT JOIN dbo.DeliveryService AS ds
                                         ON cr.ServiceTypeId = ds.DeliveryServiceId
                                     LEFT JOIN dbo.RateTypeLookup AS rtl
                                         ON rtl.RateTypeId = cr.RateTypeId
                                     LEFT JOIN dbo.UOMLookup AS ul
                                         ON ul.UOMId = cr.UomId
                                     LEFT JOIN dbo.CarrierSLA AS cs
                                         ON cs.CarrierId = c2.CarrierId
                                     LEFT JOIN dbo.CalculationMethod AS cm
                                         ON cr.CalculationMethodId = cm.CalculationMethodIdId
                      WHERE cr3.ClientId = @ClientId
                      AND (
                             (cr.OriginTypeId = {(int)EnumCivilEntityType.Country} AND cr.[From] = @From AND cr.[To] = @To) -- Country
                          OR (cr.OriginTypeId = {(int)EnumCivilEntityType.City} AND cr.[From] = @From AND cr.[To] = @To) -- City
                          OR (cr.OriginTypeId = {(int)EnumCivilEntityType.Area} AND cr.[From] = @From AND cr.[To] = @To) -- Area
                          OR (cr.OriginTypeId = {(int)EnumCivilEntityType.Province} AND cr.[From] = @From AND cr.[To] = @To) -- Province
                          OR (cr.OriginTypeId = {(int)EnumCivilEntityType.State} AND cr.[From] = @From AND cr.[To] = @To) -- State
                          OR (cr.OriginTypeId = {(int)EnumCivilEntityType.PinCode} AND cr.[From] = @From AND cr.[To] = @To) -- PinCode
                      ) ";


      var data = await connection.QueryAsync<ClientCarrierRateResponseModel>(query, dynamicParams);
      var dataList = data.Where(x => x.RateTypeId != (int)EnumRateType.API).ToList();
      var ApidataList = data.Where(x => x.RateTypeId == (int)EnumRateType.API).ToList();
      #region filter weight
      int? ChargeWeight = (int)EnumUOMType.ChargeWeight;
      var returnrows = new List<ClientCarrierRateResponseModel>();
      if (ChargeWeight != null)
      {
        // Group by carrier → so we only return one row per carrier
        #region charge weight
        var ChargeWeightROws = dataList.Where(x => x.UomId == ChargeWeight).ToList();

        var carrierGroups = ChargeWeightROws.GroupBy(r => r.CarrierRateId);
        foreach (var carrierRates in carrierGroups)
        {
          var carrierList = carrierRates.ToList();

          // Try to find matching slab first
          var matchedSlab = carrierList
              .FirstOrDefault(r => r.UomFrom.HasValue && r.UomTo.HasValue &&
                                   filter.Weight >= r.UomFrom.Value && filter.Weight <= r.UomTo.Value);

          if (matchedSlab != null)
          {
            // ✅ Slab matched
            matchedSlab.CalculatedRate = matchedSlab.Rate;
            returnrows.Add(matchedSlab);
          }
          else
          {
            // ✅ No slab → apply additional logic
            var baseRow = carrierList.OrderByDescending(x => x.UomTo).FirstOrDefault();
            if (baseRow is not null)
            {
              if (carrierList.Count > 1) // may be we need any kind of type for tax and anything else
              {
                if (baseRow != null)
                {
                  var RemaingWeight = filter.Weight - baseRow.UomTo;
                  var RemaingFrom = baseRow.UomTo - filter.Weight;
                  decimal actualRate = baseRow.Rate ?? 0;

                  if (baseRow.Unit.HasValue && baseRow.Unit > 0 && baseRow.ClientAdditionalRate.HasValue && baseRow.ClientAdditionalRate > 0)
                  {
                    decimal units = RateCalculatorHelper.RoundToNearestHalf(RemaingWeight.GetValueOrDefault() / baseRow.Unit.Value);
                    actualRate += units * baseRow.ClientAdditionalRate.Value;
                  }
                  //else if (RemaingFrom > 0)
                  //{
                  //  var weightvalue = (RemaingFrom / baseRow.Unit.GetValueOrDefault()) * baseRow.Unit.GetValueOrDefault();
                  //  var TotalAdditionalValue = weightvalue * baseRow.ClientAdditionalRate;
                  //  actualRate += TotalAdditionalValue.GetValueOrDefault();
                  //}
                  baseRow.CalculatedRate = actualRate;
                  returnrows.Add(baseRow);
                }
              }
              else
              {
                baseRow!.CalculatedRate = baseRow.ClientRate ?? 0; //in case if not slab but flat rate
                returnrows.Add(baseRow);
              }
            }
          }
        }

        #endregion
        #region charge invoice
        var InvoiceROws = dataList.Where(x => x.UomId == (int)EnumUOMType.Invoice).ToList();

        carrierGroups = InvoiceROws.GroupBy(r => r.CarrierRateId);
        foreach (var carrierRates in carrierGroups)
        {
          var carrierList = carrierRates.ToList();

          // Try to find matching slab first
          var matchedSlab = carrierList
              .FirstOrDefault(r => r.UomFrom.HasValue && r.UomTo.HasValue &&
                                   filter.Weight >= r.UomFrom.Value && filter.Weight <= r.UomTo.Value);

          if (matchedSlab != null)
          {
            // ✅ Slab matched
            matchedSlab.CalculatedRate = matchedSlab.Rate;
            returnrows.Add(matchedSlab);
          }
          else
          {
            var baseRow = carrierList.OrderByDescending(x => x.UomTo ?? x.UomFrom).FirstOrDefault();

            if (baseRow != null)
            {
              if (baseRow.CalculationTypeId == (int)EnumCalculationType.Percentage)
              {
                var rate = (filter.Amount * baseRow.Rate / 100).GetValueOrDefault();
                baseRow.CalculatedRate = rate;
              }
              else
              {
                var rate = baseRow.Rate.GetValueOrDefault();
                baseRow.CalculatedRate = rate;
              }
              returnrows.Add(baseRow);
            }
          }
        }

        #endregion
        #region charge Tariff
        var BasedOnTariff = dataList.Where(x => x.UomId == (int)EnumUOMType.BasedOnTariff).ToList();

        carrierGroups = BasedOnTariff.GroupBy(r => r.CarrierRateId);
        foreach (var carrierRates in carrierGroups)
        {
          var carrierList = carrierRates.ToList();

          // Try to find matching slab first
          var matchedSlab = carrierList
              .FirstOrDefault(r => r.UomFrom.HasValue && r.UomTo.HasValue &&
                                   filter.Weight >= r.UomFrom.Value && filter.Weight <= r.UomTo.Value);

          if (matchedSlab != null)
          {
            // ✅ Slab matched
            matchedSlab.CalculatedRate = matchedSlab.Rate;
            returnrows.Add(matchedSlab);
          }
          else
          {
            var baseRow = carrierList.OrderByDescending(x => x.UomTo ?? x.UomFrom).FirstOrDefault();

            if (baseRow != null)
            {
              if (baseRow.CalculationTypeId == (int)EnumCalculationType.Percentage)
              {
                var rate = (filter.Weight * baseRow.Rate / 100).GetValueOrDefault();
                baseRow.CalculatedRate = rate;
              }
              else
              {
                var rate = baseRow.Rate.GetValueOrDefault();
                baseRow.CalculatedRate = rate;
              }
              returnrows.Add(baseRow);
            }
          }
        }

        #endregion

        #region Api
        carrierGroups = ApidataList.GroupBy(r => r.CarrierRateId);
        foreach (var carrierapi in carrierGroups)
        {
          var carriergroup = carrierapi.ToList();
          foreach (var item in carriergroup)
          {
            if (item.IsRateCheck)
            {
              #region Address
              var FromAddress = await _countryRepository.GetEntityAddressByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.From.GetValueOrDefault());
              var ToAddress = await _countryRepository.GetEntityAddressByOriginTypeId(item.OriginTypeId.GetValueOrDefault(), item.To.GetValueOrDefault());
              #endregion
              #region Setting Config
              var settingsArray = JArray.Parse(item.SettingConfig!);

              List<string> productTypeIntl = UtilityHelper.GetSettingConfigByKey(settingsArray, "producttypeintl");
              List<string> productTypeDom = UtilityHelper.GetSettingConfigByKey(settingsArray, "producttypedom");
              string paymentType = UtilityHelper.GetSelectedConfigByKey(settingsArray, "paymenttypedt") ?? "";
              string productGroupIntl = UtilityHelper.GetSelectedConfigByKey(settingsArray, "productgroupintl") ?? "";
              string productGroupDom = UtilityHelper.GetSelectedConfigByKey(settingsArray, "productgroupdom") ?? "";
              #endregion
              ////Api Call For Third Party Rate////
              if (item.RateTypeId == (int)EnumRateType.API)
              {
                var requestBody = new
                {
                  carrierId = item.CarrierId,
                  activeCarrierId = 0,
                  clientId = filter.ClientId,
                  orderNos = "",
                  orderList = new[]
            {
    new {
        ActiveCarrierPickupLocationId = 0,
        OrderNo = "",
        Others = new Dictionary<string, string>{ },
        ServiceType = item.ServiceName,
        DestinationAddress = new {
            Line1 = "",
            City =ToAddress?.City?.Name,
            StateOrProvinceCode = "",
            PostCode = "",
            CountryCode = ToAddress?.Country?.ISOA2Code,
            Longitude = 0,
            Latitude = 0,
            BuildingNumber = "",
            BuildingName = "",
            Floor = "",
            Apartment = "",
            POBox = "",
            Description = ""
        },
         PickupAddress  = new {
            Line1 = "",
            City =FromAddress?.City?.Name,
            StateOrProvinceCode = "",
            PostCode = "",
            CountryCode =FromAddress?.Country?.ISOA2Code,
            Longitude = 0,
            Latitude = 0,
            BuildingNumber = "",
            BuildingName = "",
            Floor = "",
            Apartment = "",
            POBox = "",
            Description = ""
        },
        MultiRateDetails = new {
            //Dimensions = new { Length = 10, Width = 5, Height = 2, Unit = "CM" },
            ActualWeight = new { Unit = "KG", Value = 1/* item.Weight*/ },
            //ChargeableWeight = new { Unit = "KG", Value = 2.5 },
            DescriptionOfGoods = "",
            GoodsOriginCountry = "",
            NumberOfPieces = 0,
            ProductGroup = FromAddress!.Country?.ISOA2Code ==  ToAddress!.Country?.ISOA2Code ?productGroupDom : productGroupIntl,
            ProductTypes =FromAddress!.Country?.ISOA2Code == ToAddress!.Country?.ISOA2Code ? productTypeDom :productTypeIntl,
            PaymentType = paymentType,
            PaymentOptions = "",
            CustomsValueAmount = 0.0,
            CashOnDeliveryAmount = 0.0,
            InsuranceAmount = 0.0,
            CashAdditionalAmount = 0.0,
            CashAdditionalAmountDescription = "",
            CollectAmount = 0.0,
            Services = "",
            Items = "",
            DeliveryInstructions = "",
            AdditionalProperties = "",
            ContainsDangerousGoods = false
        }
    }
},
                  IsOther = true,
                  carrierContractTypeId = 2
                };
                #region BaseUrl
                var enId = _configuration.GetValue<int>("EnvironmentTypeId");
                var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, enId);
                if (mcconfig is null)
                {
                  throw new EntityNotFoundException("Mcconfig", "Integration Value");
                }
                #endregion
                var json = JsonConvert.SerializeObject(requestBody);
                var res = await _carrierSharedRepository.PostAsync(json, "/api/Order/GetCalcuatedRate", mcconfig.Value!, filter.ClientId);
                var deserialized = JsonConvert.DeserializeObject<Root>(res);
                #region Return
                if (deserialized?.Data?.RateResult != null && deserialized.Data.RateResult.Any())
                {
                  var apiRateRows = deserialized.Data.RateResult.Select((r, index) => new ClientCarrierRateResponseModel
                  {
                    ClientRateId = returnrows.Count + index + 1,
                    CarrierName = item.CarrierName,
                    ServiceName = r.ProductType,
                    From = item.From,
                    To = item.To,
                    OriginTypeId = item.OriginTypeId,
                    CarrierId = item.CarrierId,
                    CarrierImage = item.CarrierImage,
                    UOMName = item.UOMName,
                    RateTypeName = item.RateTypeName,
                    CalculatedRate = r.Amount,
                    Rate = r.Amount
                  }).ToList();
                  returnrows.AddRange(apiRateRows);
                }
              }
            }
          }
          #endregion
        }
      }
      #endregion
      returnrows = returnrows
                  .GroupBy(x => new
                  {
                    x.CarrierName,
                    x.ServiceName,
                    x.From,
                    x.To,
                    x.OriginTypeId,
                    x.CarrierId,
                    x.CarrierImage,
                    x.UOMName,
                    x.RateTypeName
                  })
                  .Select((g, index) => new ClientCarrierRateResponseModel
                  {//
                    ClientRateId = index + 1, // <-- index starts at 0, so add 1
                    CarrierName = g.Key.CarrierName,
                    ServiceName = g.Key.ServiceName,
                    From = g.Key.From,
                    To = g.Key.To,
                    OriginTypeId = g.Key.OriginTypeId,
                    CarrierImage = g.Key.CarrierImage,
                    CarrierId = g.Key.CarrierId,
                    CalculatedRate = g.Sum(x => x.CalculatedRate),
                    UOMName = g.Key.UOMName,
                    RateTypeName = g.Key.RateTypeName

                  }).ToList();

      foreach (var rateModel in returnrows)
      {
        rateModel.FromName = await _countryRepository.GetEntityNamesByOriginTypeId(
            rateModel.OriginTypeId.GetValueOrDefault(),
            rateModel.From.GetValueOrDefault()
        );

        rateModel.ToName = await _countryRepository.GetEntityNamesByOriginTypeId(
            rateModel.OriginTypeId.GetValueOrDefault(),
            rateModel.To.GetValueOrDefault()
        );
      }

      returnrows.ForEach(x => x.Rate = x.CalculatedRate);
      var result = new ExpandoObject() as dynamic;
      result.TotalCount = returnrows.Count;
      result.list = returnrows.ToList();

      return result;
    }
  }
  public async Task<List<ClientRateSlabResponseModel>?> GetAllClientRateSlabByClientRateId(int clientRateId)
  {
    using (var connection = _dapperAppDbContext.CreateConnectionByClientOrMasterDb(true))
    {
      var dynamicParams = new DynamicParameters();

      string query = @"SELECT  crs.ClientRateSlabId,
                               crs2.UomFrom,
                               crs2.UomTo,
                               crs.Rate
                        FROM dbo.ClientRateSlab AS crs
                            INNER JOIN dbo.CarrierRateSlab AS crs2
                                ON crs2.CarrierRateSlabId = crs.CarrierRateSlabId ";
      string whereStart = $"WHERE crs.ClientRateId = {clientRateId} AND ( 1=1 ";
      string whereEnd = ")";
      string where = whereStart + whereEnd;
      string queryData = query + where;
      var data = await connection.QueryAsync<ClientRateSlabResponseModel>(queryData);
      return data.ToList();
    }
  }
}
#endregion
#region Rtae Model Res
public class AdditionalCharge
{
}
public class RateResult
{
  public string? ProductGroup { get; set; }
  public string? ProductType { get; set; }
  public string? AccountEntity { get; set; }
  public string? AccountNumber { get; set; }
  public decimal? Amount { get; set; }
  public string? Currency { get; set; }
  public double AmountBeforeTax { get; set; }
  public double TaxAmount { get; set; }
  public double TaxRate { get; set; }
  public bool Skip { get; set; }
  public List<AdditionalCharge> AdditionalCharges { get; set; } = new();
}
public class Data
{
  public string? OrderNo { get; set; }
  public List<RateResult> RateResult { get; set; } = new();
}
public class Root
{
  public bool IsSuccess { get; set; }
  public int StatusCode { get; set; }
  public string? Message { get; set; }
  public Data? Data { get; set; }
  public Dictionary<string, object>? Errors { get; set; }
  public object? ConfigErrors { get; set; }
  public DateTime CreatedTime { get; set; }
}
#endregion
