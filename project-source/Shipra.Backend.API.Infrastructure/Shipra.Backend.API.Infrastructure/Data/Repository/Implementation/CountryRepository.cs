
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text;
using Dapper;
using Google.Protobuf.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.MapKeyAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.Models.GoogleMap;
using Shipra.Backend.API.Infrastructure.Helpers;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class CountryRepository : ICountryRepository
{
  private readonly ShipraMasterDbContext _context;
  private readonly AppDbContext _appDbContext;
  private readonly IMemoryCache _cache;
  private readonly IOptions<CacheSettings> _cacheSettings;
  private readonly MemoryCacheEntryOptions _cacheOptions;
  private readonly string _connectionString;

  public CountryRepository(ShipraMasterDbContext context, AppDbContext appDbContext, IConfiguration configuration, IMemoryCache cache, IOptions<CacheSettings> cacheSettings)
  {
    _context = context;
    _appDbContext = appDbContext;
    _cache = cache;
    _cacheSettings = cacheSettings;
    _connectionString = configuration.GetConnectionString("ShipraMasterConnection")!;

    _cacheOptions = new MemoryCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheSettings.Value.AbsoluteExpirationInMinutes),
      SlidingExpiration = TimeSpan.FromMinutes(cacheSettings.Value.SlidingExpirationInMinutes)
    };
  }
  public async Task<dynamic> CreateCountry(Country country, List<City> city)
  {
    _context.Countries.Add(country);
    _context.Cities.AddRange(city);
    return await _context.SaveChangesAsync() > 0;
  }

  public async Task<List<Area>?> GetAllAreaByCityId(long cityId)
  {
    var areas = await GetAllCacheAreas();
    var filteredAreas = areas.Where(x => x.CityId == cityId).ToList();

    if (filteredAreas.Count == 0)
    {
      // If areas is null or empty, attempt to clear cache and reload
      areas = await GetAllCacheAreas(clearCacheOnRequest: true);
      filteredAreas = areas.Where(x => x.CityId == cityId).ToList();
    }
    return filteredAreas;
  }


  public async Task<List<RegionTimeZone>> GetAllRegionTimeZone()
  {
    var regionTimeZones = await GetAllCacheRegionTimeZones();
    return regionTimeZones.ToList();
  }

  public async Task<List<CivilEntityType>> GetAllCivilEntityType()
  {
    return await _context.CivilEntityTypes.ToListAsync()!;
  }
  public async Task<RegionTimeZone?> GetRegionTimeZoneById(int? regionTimeZoneId)
  {
    var regionTimeZones = await GetAllCacheRegionTimeZones();
    var regionTimeZone = regionTimeZones.FirstOrDefault(x => x.RegionTimeZoneId == regionTimeZoneId);

    if (regionTimeZone == null)
    {
      // If regionTimeZone is null, attempt to clear cache and reload
      regionTimeZones = await GetAllCacheRegionTimeZones(clearCacheOnRequest: true);
      regionTimeZone = regionTimeZones.FirstOrDefault(x => x.RegionTimeZoneId == regionTimeZoneId);
    }
    return regionTimeZone;
  }
  public async Task<City?> GetCityById(long cityId)
  {
    var cities = await GetAllCacheCities();
    var city = cities.FirstOrDefault(x => x.CityId == cityId);

    if (city == null)
    {
      // If city is null, attempt to clear cache and reload
      cities = await GetAllCacheCities(clearCacheOnRequest: true);
      city = cities.FirstOrDefault(x => x.CityId == cityId);
    }

    return city;
  }
  public async Task<Area?> GetAreaById(int areaid)
  {
    var areas = await GetAllCacheAreas();
    var area = areas.FirstOrDefault(x => x.AreaId == areaid);

    if (area == null)
    {
      // If area is null, attempt to clear cache and reload
      areas = await GetAllCacheAreas(clearCacheOnRequest: true);
      area = areas.FirstOrDefault(x => x.AreaId == areaid);
    }

    return area;
  }

  public async Task<Country?> GetCountryById(int? countryId)
  {
    var countries = await GetAllCacheCountries();
    var country = countries.FirstOrDefault(x => x.CountryId == countryId);

    if (country == null)
    {
      // If country is null, attempt to clear cache and reload
      countries = await GetAllCacheCountries(clearCacheOnRequest: true);
      country = countries.FirstOrDefault(x => x.CountryId == countryId);
    }

    return country;
  }

  public async Task<dynamic> UpdateCountry(Country country, List<City>? city)
  {
    _context.Countries.Update(country);
    _context.Cities.UpdateRange(city!);
    return await _context.SaveChangesAsync() > 0;
  }
  public async Task<dynamic> GetAllAreaByNameForSelection(int? cityId, string? search = "")
  {
    if (cityId != null && cityId > 0)
    {
      using (var connection = new SqlConnection(_connectionString))
      {
        var dynamicParams = new DynamicParameters();

        string query = $@" SELECT c.CityId,
                               c.Code,
                               c.Name,
                               c.Latitude,
                               c.Longitude
                        FROM dbo.Area AS c  ";
        string whereStart = $"WHERE (  ( c.Name LIKE  '%{search}%' ) AND (c.CityId = {cityId}) ";
        string whereEnd = ")";

        string where = whereStart + whereEnd;

        string queryData = query + where;

        var data = await connection.QueryAsync(queryData, dynamicParams);

        return data.ToList();
      }
    }
    else
    {
      return new List<dynamic>();
    }

  }

  #region get address
  public async Task<string> GetFullAddress(string? streetAddress, int? countryId, int? cityId, int? areaId = 0, int? provinceId = 0, int? pinCodeId = 0, int? stateId = 0, string? entityAddressDataJson = null)
  {

    StringBuilder sb = new StringBuilder();

    var country = await GetCountryById(countryId!);

    if (country is not null)
    {
      var keyList = JsonConvert.DeserializeObject<CountryAddressSchema>(country!.AddressingScheme!);

      if (!string.IsNullOrEmpty(streetAddress))
      {
        sb.Append($"{streetAddress}");
      }

      if (keyList is not null)
      {
        //reverse all key for address creation
        keyList!.keys!.Reverse();

        // Loop through the reversed list
        foreach (string key in keyList!.keys)
        {
          // If-else statement to check the value of each string
          if (key == "city")
          {
            var city = await GetCityById(cityId.GetValueOrDefault());
            if (cityId != null && cityId > 0)
            {
              if (city != null)
              {
                sb.Append($" ,{city.Name}");
              }
            }
            if (cityId == -1)
            {
              var name = await GetCivilEntityExtendedNameByOrderJsonAndKey(entityAddressDataJson!, key);
              if (!string.IsNullOrEmpty(name))
              {
                sb.Append($" ,{name}");
              }
            }
          }
          else if (key == "area")
          {
            //check for area
            if (areaId != null && areaId > 0)
            {
              var area = await GetAreaById(areaId.GetValueOrDefault());
              if (area != null)
              {
                sb.Append($" ,{area.Name}");
              }
            }
            if (areaId == -1)
            {
              var name = await GetCivilEntityExtendedNameByOrderJsonAndKey(entityAddressDataJson!, key);
              if (!string.IsNullOrEmpty(name))
              {
                sb.Append($" ,{name}");
              }
            }
          }
          else if (key == "province")
          {
            //check for province 
            if (provinceId != null && provinceId > 0)
            {
              var provicne = await GetProvinceById(provinceId.GetValueOrDefault());
              if (provicne != null)
              {
                sb.Append($" ,{provicne.Name}");
              }
            }
            if (provinceId == -1)
            {
              var name = await GetCivilEntityExtendedNameByOrderJsonAndKey(entityAddressDataJson!, key);
              if (!string.IsNullOrEmpty(name))
              {
                sb.Append($" ,{name}");
              }
            }
          }
          else if (key == "pinCode")
          {
            //check for province 
            if (pinCodeId != null && pinCodeId > 0)
            {
              var pinCode = await GetPinCodeById(pinCodeId.GetValueOrDefault());
              if (pinCode != null)
              {
                sb.Append($" ,{pinCode.PinCodeValue}");
              }
            }
            if (pinCodeId == -1)
            {
              var name = await GetCivilEntityExtendedNameByOrderJsonAndKey(entityAddressDataJson!, key);
              if (!string.IsNullOrEmpty(name))
              {
                sb.Append($" ,{name}");
              }
            }
          }
          else if (key == "state")
          {
            //check for state 
            if (stateId != null && stateId > 0)
            {
              var state = await GetStateById(stateId.GetValueOrDefault());
              if (state != null)
              {
                sb.Append($" ,{state.Name}");
              }
            }
            if (stateId == -1)
            {
              var name = await GetCivilEntityExtendedNameByOrderJsonAndKey(entityAddressDataJson!, key);
              if (!string.IsNullOrEmpty(name))
              {
                sb.Append($" ,{name}");
              }
            }
          }
          else
          {
            Console.WriteLine("Unknown key.");
          }
        }

      }

      sb.Append($" ,{country.Name}");
    }

    return sb.ToString();
  }
  private async Task<string> GetCivilEntityExtendedNameByOrderJsonAndKey(string entityAddressDataJson, string? key)
  {
    string? name = string.Empty;
    if (!string.IsNullOrEmpty(entityAddressDataJson) && !string.IsNullOrEmpty(key))
    {
      var orderJsonEntitymodel = CivilEntityHelper.GetOrderJsonEntityValueByKey(entityAddressDataJson!, key!);
      if (orderJsonEntitymodel is not null)
      {
        // get value from entitytype extended
        if (orderJsonEntitymodel.Value == 0)
        {
          name = await GetCivilEntityExtendedNameByIdAndTypeId(orderJsonEntitymodel.CivilEntityExtendedId, orderJsonEntitymodel.CivilEntityTypeId);
        }
      }
    }
    return name!;
  }

  #endregion
  private async Task<State?> GetStateById(int stateId)
  {
    var states = await GetAllCacheStates();
    var state = states.FirstOrDefault(x => x.StateId == stateId);

    if (state == null)
    {
      // If state is null, attempt to clear cache and reload
      states = await GetAllCacheStates(clearCacheOnRequest: true);
      state = states.FirstOrDefault(x => x.StateId == stateId);
    }

    return state;
  }

  private async Task<PinCode?> GetPinCodeById(int pinCodeId)
  {
    var pinCodes = await GetAllCachePinCodes();
    var pinCode = pinCodes.FirstOrDefault(x => x.PinCodeId == pinCodeId);

    if (pinCode == null)
    {
      // If pinCode is null, attempt to clear cache and reload
      pinCodes = await GetAllCachePinCodes(clearCacheOnRequest: true);
      pinCode = pinCodes.FirstOrDefault(x => x.PinCodeId == pinCodeId);
    }

    return pinCode;
  }

  private async Task<Province?> GetProvinceById(int provinceId)
  {
    var provinces = await GetAllCacheProvinces();
    var province = provinces.FirstOrDefault(x => x.ProvinceId == provinceId);

    if (province == null)
    {
      // If province is null, attempt to clear cache and reload
      provinces = await GetAllCacheProvinces(clearCacheOnRequest: true);
      province = provinces.FirstOrDefault(x => x.ProvinceId == provinceId);
    }

    return province;
  }

  public async Task<Country?> GetCountryByCode(string code)
  {
    var countries = await GetAllCacheCountries();
    var country = countries.FirstOrDefault(x => x.Code?.Trim().ToLower() == code.Trim().ToLower());

    if (country == null)
    {
      // If country is null, attempt to clear cache and reload
      countries = await GetAllCacheCountries(clearCacheOnRequest: true);
      country = countries.FirstOrDefault(x => x.Code?.Trim().ToLower() == code.Trim().ToLower());
    }

    return country;
  }


  public async Task<City?> GetCityByCode(string code)
  {
    var cities = await GetAllCacheCities();
    var city = cities.FirstOrDefault(x => x.Code?.Trim().ToLower() == code.Trim().ToLower());

    if (city == null)
    {
      // If city is null, attempt to clear cache and reload
      cities = await GetAllCacheCities(clearCacheOnRequest: true);
      city = cities.FirstOrDefault(x => x.Code?.Trim().ToLower() == code.Trim().ToLower());
    }

    return city;
  }
  public async Task<Country?> GetCountryByName(string name)
  {
    var countries = await GetAllCacheCountries();
    var country = countries.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());

    if (country == null)
    {
      // If country is null, attempt to clear cache and reload
      countries = await GetAllCacheCountries(clearCacheOnRequest: true);
      country = countries.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());
    }

    return country;
  }
  public async Task<Country?> GetCountryByISOCode(string countryISOCode)
  {
    var countries = await GetAllCacheCountries();
    var country = countries.FirstOrDefault(x => x.Code?.Trim().ToLower() == countryISOCode.Trim().ToLower());

    if (country == null)
    {
      // If country is null, attempt to clear cache and reload
      countries = await GetAllCacheCountries(clearCacheOnRequest: true);
      country = countries.FirstOrDefault(x => x.Code?.Trim().ToLower() == countryISOCode.Trim().ToLower());
    }

    return country;
  }

  public async Task<Area?> GetAreaByName(string name)
  {
    var areas = await GetAllCacheAreas();
    var area = areas.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());

    if (area == null)
    {
      // If area is null, attempt to clear cache and reload
      areas = await GetAllCacheAreas(clearCacheOnRequest: true);
      area = areas.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());
    }

    return area;
  }
  public async Task<City?> GetCityByName(string name)
  {
    var cities = await GetAllCacheCities();
    var city = cities.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());

    if (city == null)
    {
      // If city is null, attempt to clear cache and reload
      cities = await GetAllCacheCities(clearCacheOnRequest: true);
      city = cities.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());
    }

    return city;
  }
  public async Task<Province?> GetProvinceByName(string name)
  {
    var provinces = await GetAllCacheProvinces();
    var province = provinces.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());

    if (province == null)
    {
      // If city is null, attempt to clear cache and reload
      provinces = await GetAllCacheProvinces(clearCacheOnRequest: true);
      province = provinces.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());
    }

    return province;
  }
  public async Task<State?> GetStateByName(string name)
  {
    var states = await GetAllCacheStates();
    var state = states.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());

    if (state == null)
    {
      // If city is null, attempt to clear cache and reload
      states = await GetAllCacheStates(clearCacheOnRequest: true);
      state = states.FirstOrDefault(x => x.Name?.Trim().ToLower() == name.Trim().ToLower());
    }

    return state;
  }
  public async Task<PinCode?> GetPinCodeByName(string name)
  {
    var pinCodes = await GetAllCachePinCodes();
    var pinCode = pinCodes.FirstOrDefault(x => x.PinCodeValue?.Trim().ToLower() == name.Trim().ToLower());

    if (pinCode == null)
    {
      // If city is null, attempt to clear cache and reload
      pinCodes = await GetAllCachePinCodes(clearCacheOnRequest: true);
      pinCode = pinCodes.FirstOrDefault(x => x.PinCodeValue?.Trim().ToLower() == name.Trim().ToLower());
    }

    return pinCode;
  }
  public async Task<List<PinCode>> GetPinCodesByCityId(int cityId)
  {
    var pinCodes = await GetAllCachePinCodes();
    var filteredPinCodes = pinCodes.Where(x => x.CityId == cityId).ToList();

    if (filteredPinCodes.Count == 0)
    {
      // If no pin codes found, attempt to clear cache and reload
      pinCodes = await GetAllCachePinCodes(clearCacheOnRequest: true);
      filteredPinCodes = pinCodes.Where(x => x.CityId == cityId).ToList();
    }

    return filteredPinCodes;
  }
  public async Task<List<City>> GetCitiesByProvinceAndCountryId(int countryId, int provinceId)
  {
    var cities = await GetAllCacheCities();
    var filteredCities = cities.Where(x => x.CountryId == countryId && x.ProvinceId == provinceId).ToList();

    if (filteredCities.Count == 0)
    {
      // If no cities found, attempt to clear cache and reload
      cities = await GetAllCacheCities(clearCacheOnRequest: true);
      filteredCities = cities.Where(x => x.CountryId == countryId && x.ProvinceId == provinceId).ToList();
    }

    return filteredCities;
  }
  public async Task<List<City>> GetCitiesByStateAndCountryIdQuery(int countryId, int stateId)
  {
    var cities = await GetAllCacheCities();
    var filteredCities = cities.Where(x => x.CountryId == countryId && x.StateId == stateId).ToList();

    if (filteredCities.Count == 0)
    {
      // If no cities found, attempt to clear cache and reload
      cities = await GetAllCacheCities(clearCacheOnRequest: true);
      filteredCities = cities.Where(x => x.CountryId == countryId && x.StateId == stateId).ToList();
    }

    return filteredCities;
  }
  public async Task<List<Province>> GetProvinceByCountryId(int countryId)
  {
    var provinces = await GetAllCacheProvinces();
    var filteredProvinces = provinces.Where(x => x.CountryId == countryId).ToList();

    if (filteredProvinces.Count == 0)
    {
      // If no provinces found, attempt to clear cache and reload
      provinces = await GetAllCacheProvinces(clearCacheOnRequest: true);
      filteredProvinces = provinces.Where(x => x.CountryId == countryId).ToList();
    }

    return filteredProvinces;
  }
  public async Task<List<State>> GetStatesByCountryId(int countryId)
  {
    var states = await GetAllCacheStates();
    var filteredStates = states.Where(x => x.CountryId == countryId).ToList();

    if (filteredStates.Count == 0)
    {
      // If no states found, attempt to clear cache and reload
      states = await GetAllCacheStates(clearCacheOnRequest: true);
      filteredStates = states.Where(x => x.CountryId == countryId).ToList();
    }

    return filteredStates;
  }
  public async Task<List<City>> GetCitiesByCountryId(int countryId)
  {
    var cities = await GetAllCacheCities();
    var filteredCities = cities.Where(x => x.CountryId == countryId).ToList();

    if (filteredCities.Count == 0)
    {
      // If no cities found, attempt to clear cache and reload
      cities = await GetAllCacheCities(clearCacheOnRequest: true);
      filteredCities = cities.Where(x => x.CountryId == countryId).ToList();
    }

    return filteredCities;
  }
  public async Task<List<City>> GetCities(int id, string? civilEntity)
  {
    List<City> cities = new List<City>();
    await Task.Delay(1);
    return cities;
  }
  public async Task<dynamic> GetCounrtyCityRegionIdByName(string countryName, string areaName, string cityName)
  {
    var country = await GetCountryByName(countryName);
    var city = await GetCityByName(cityName);
    var area = await GetAreaByName(areaName);

    dynamic data = new ExpandoObject();
    data.CountryId = (country != null ? country.CountryId : 0);
    data.CityId = (city != null ? city.CityId : 0);
    data.AreaId = (area != null ? area.AreaId : 0);
    return data;
  }
  public async Task<string> GetEntityNamesByOriginTypeId(int originTypeId, int id)
  {
    if (!System.Enum.IsDefined(typeof(EnumCivilEntityType), originTypeId))
      return string.Empty;

    var originType = (EnumCivilEntityType)originTypeId;

    switch (originType)
    {
      case EnumCivilEntityType.Country:
        var countries = await GetAllCacheCountries();
        return countries.FirstOrDefault(c => c.CountryId == id)?.Name ?? string.Empty;

      case EnumCivilEntityType.State:
        var states = await GetAllCacheStates();
        return states.FirstOrDefault(s => s.StateId == id)?.Name ?? string.Empty;

      case EnumCivilEntityType.Province:
        var provinces = await GetAllCacheProvinces();
        return provinces.FirstOrDefault(p => p.ProvinceId == id)?.Name ?? string.Empty;

      case EnumCivilEntityType.City:
        var cities = await GetAllCacheCities();
        return cities.FirstOrDefault(c => c.CityId == id)?.Name ?? string.Empty; ;

      case EnumCivilEntityType.Area:
        var areas = await GetAllCacheAreas();
        return areas.FirstOrDefault(a => a.AreaId == id)?.Name ?? string.Empty;

      case EnumCivilEntityType.PinCode:
        var pincodes = await GetAllCachePinCodes();
        return pincodes.FirstOrDefault(p => p.PinCodeId == id)?.PinCodeValue ?? string.Empty;

      default:
        return string.Empty; ;
    }
  }

  #region multiple || civil entity
  #region new code
  public async Task<List<AddressCommonLookupModel>> GetCivilEntityCommonLookupWithCarrierId(int? carrierId, List<long> intSelectedListIds, int? entityTypeId)
  {
    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped();

    return civilEntityCarrierMapped
        .Where(cee => cee.CarrierId == carrierId
            && intSelectedListIds.Contains(cee.ParentId.GetValueOrDefault())
            && cee.CivilEntityTypeId == entityTypeId)
        .OrderBy(x => x.CivilEntityExtendedId)
        .Select(x => new AddressCommonLookupModel
        {
          Id = (x.MappedId ?? 0) + "_" + x.CivilEntityExtendedId,
          Name = x.CivilEntityName,
          ParentId = x.ParentId 
        })
        .OrderBy(x => x.Id)
        .ToList();
  }

  #region city
  public async Task<dynamic> GetAllCacheCivilEntityCitiesWithCarrier(List<long> intSelectedListIds, int? carrierId = 0, int? dmsTypeId = 0, bool? clearCacheOnRequest = false)
  {
    var cities = await GetAllCacheCities();

    var dataList = new List<AddressCommonLookupModel>();
    if (carrierId > 0)
    {
      var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped(clearCacheOnRequest);
      dataList = await GetCivilEntityCommonLookupWithCarrierId(carrierId, intSelectedListIds, (int)EnumCivilEntityType.City);
    }
    else
    {

      dataList = cities.Where(x => intSelectedListIds.Contains(x.CountryId.GetValueOrDefault()))
                 .OrderBy(x => x.CityId)
                 .Select(x => new AddressCommonLookupModel
                 {
                   Id = $"{x.CityId}_0", // Corrected string interpolation
                   Name = x.Name
                 }).ToList();
    }
    return dataList!;
  }

  #endregion
  #region province
  public async Task<List<AddressCommonLookupModel>> GetAllCacheCivilEntityCarrierProvinceMapped(List<long> intSelectedListIds, int? carrierId = 0, int? dmsTypeId = 0, bool? clearCacheOnRequest = false)
  {
    var dataList = new List<AddressCommonLookupModel>();
    if (carrierId > 0)
    {
      var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped(clearCacheOnRequest);
      dataList = await GetCivilEntityCommonLookupWithCarrierId(carrierId, intSelectedListIds, (int)EnumCivilEntityType.Province);
    }
    else
    {
      var provinces = await GetAllCacheProvinces(clearCacheOnRequest);

      dataList = provinces.Where(x => intSelectedListIds.Contains(x.CountryId.GetValueOrDefault()))
                 .OrderBy(x => x.ProvinceId)
                 .Select(x => new AddressCommonLookupModel
                 {
                   Id = $"{x.ProvinceId}_0", // Corrected string interpolation
                   Name = x.Name
                 }).ToList();
    }
    return dataList;
  }
  #endregion 
  #region common
  public async Task<List<AddressCommonLookupModel>> GetAllCacheCivilEntityWithCarrierMappedByTypes(string? selectedEntityIds, int? selectedEntityType, int? entityType, int? carrierId = 0, int? dmsTypeId = 0, bool? clearCacheOnRequest = false)
  {
    var listIds = (selectedEntityIds ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    List<long> intSelectedListIds = new();
    // Process the list based on carrierId
    if (carrierId > 0)
    {
      intSelectedListIds = listIds
          .Select(id =>
          {
            var parts = id.Split('_');
            return parts.Length == 2 && long.TryParse(parts[1], out long result) ? result
            : (long.TryParse(id, out result) ? result : (long?)null);
          })
          .Where(id => id.HasValue)
          .Select(id => id!.Value)
          .ToList();
    }
    else
    {
      intSelectedListIds = listIds
          .Select(id =>
          {
            var parts = id.Split('_');
            return parts.Length == 2 && long.TryParse(parts[0], out long result) ? result
            : (long.TryParse(id, out result) ? result : (long?)null);
          })
          .Where(id => id.HasValue)
          .Select(id => id!.Value)
          .ToList();
    }

    var dataList = new List<AddressCommonLookupModel>();
    if (carrierId > 0)
    {
      //var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped(clearCacheOnRequest);
      dataList = await GetCivilEntityCommonLookupWithCarrierId(carrierId, intSelectedListIds, entityType);
    }
    else
    {
      bool isAllowUnderscore = carrierId != null && carrierId > 0;

        if (entityType == (int)EnumCivilEntityType.City)
        {
          var cities = await GetAllCacheCities();
          dataList = cities.Where(x => intSelectedListIds.Contains(x.CountryId.GetValueOrDefault()))
                      .OrderBy(x => x.CityId)
                      .Select(x => new AddressCommonLookupModel
                      {
                        Id = isAllowUnderscore ? $"{x.CityId}_0" : x.CityId,
                        Name = x.Name
                      }).ToList();
        }
        else if (entityType == (int)EnumCivilEntityType.Province)
        {
          var provinces = await GetAllCacheProvinces(clearCacheOnRequest);
          dataList = provinces.Where(x => intSelectedListIds.Contains(x.CountryId.GetValueOrDefault()))
                     .OrderBy(x => x.ProvinceId)
                     .Select(x => new AddressCommonLookupModel
                     {
                       Id = isAllowUnderscore ? $"{x.ProvinceId}_0" : x.ProvinceId,
                       Name = x.Name
                     }).ToList();
        }
        else if (entityType == (int)EnumCivilEntityType.State)
        {
          var states = await GetAllCacheStates();
          dataList = states.Where(x => intSelectedListIds.Contains(x.CountryId.GetValueOrDefault()))
                     .OrderBy(x => x.StateId)
                     .Select(x => new AddressCommonLookupModel
                     {
                       Id = isAllowUnderscore ? $"{x.StateId}_0" : x.StateId,
                       Name = x.Name
                     }).ToList();
        }

      if (selectedEntityType == (int)EnumCivilEntityType.State && entityType == (int)EnumCivilEntityType.City)
      {
        var cities = await GetAllCacheCities();
        dataList = cities.Where(x => intSelectedListIds.Contains(x.StateId.GetValueOrDefault()))
                    .OrderBy(x => x.CityId)
                    .Select(x => new AddressCommonLookupModel
                    {
                      Id = isAllowUnderscore ? $"{x.CityId}_0" : x.CityId,
                      Name = x.Name
                    }).ToList();
      }

      if (selectedEntityType == (int)EnumCivilEntityType.Province && entityType == (int)EnumCivilEntityType.City)
      {
        var cities = await GetAllCacheCities();
        dataList = cities.Where(x => intSelectedListIds.Contains(x.ProvinceId.GetValueOrDefault()))
                    .OrderBy(x => x.CityId)
                    .Select(x => new AddressCommonLookupModel
                    {
                      Id = isAllowUnderscore ? $"{x.CityId}_0" : x.CityId,
                      Name = x.Name
                    }).ToList();
      }

      if (selectedEntityType == (int)EnumCivilEntityType.City && entityType == (int)EnumCivilEntityType.Area)
      {
        var areas = await GetAllCacheAreas();
        dataList = areas.Where(x => intSelectedListIds.Contains(x.CityId))
                    .OrderBy(x => x.AreaId)
                    .Select(x => new AddressCommonLookupModel
                    {
                      Id = isAllowUnderscore ? $"{x.AreaId}_0" : x.AreaId,
                      Name = x.Name,
                      Latitude = x.Latitude,
                      Longitude = x.Longitude,
                      IsLatLng = true
                    }).ToList();
      }

      if (selectedEntityType == (int)EnumCivilEntityType.City && entityType == (int)EnumCivilEntityType.PinCode)
      {
        var pinCodes = await GetAllCachePinCodes();
        dataList = pinCodes.Where(x => intSelectedListIds.Contains(x.CityId.GetValueOrDefault()))
                    .OrderBy(x => x.PinCodeId)
                    .Select(x => new AddressCommonLookupModel
                    {
                      Id = isAllowUnderscore ? $"{x.PinCodeId}_0" : x.PinCodeId,
                      Name = x.PinCodeValue
                    }).ToList();
      }

      if (selectedEntityType == (int)EnumCivilEntityType.PinCode && entityType == (int)EnumCivilEntityType.City)
      {
        var pincodeStrList = (selectedEntityIds ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
        var pinCodes = await GetAllCachePinCodes();
        var cityIds = pinCodes.Where(x => x != null && x.CityId.HasValue &&
                                          (intSelectedListIds.Contains(x.PinCodeId) ||
                                           (x.PinCodeValue != null && pincodeStrList.Contains(x.PinCodeValue.Trim()))))
                              .Select(x => (long)x.CityId!.Value)
                              .Distinct()
                              .ToList();
        var cities = await GetAllCacheCities();
        dataList = cities.Where(x => x != null && cityIds.Contains(x.CityId))
                    .OrderBy(x => x.CityId)
                    .Select(x => new AddressCommonLookupModel
                    {
                      Id = isAllowUnderscore ? $"{x.CityId}_0" : x.CityId,
                      Name = x.Name
                    }).ToList();
      }

      if (selectedEntityType == (int)EnumCivilEntityType.PinCode && entityType == (int)EnumCivilEntityType.State)
      {
        var pincodeStrList = (selectedEntityIds ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
        var pinCodes = await GetAllCachePinCodes();
        var cityIds = pinCodes.Where(x => x != null && x.CityId.HasValue &&
                                          (intSelectedListIds.Contains(x.PinCodeId) ||
                                           (x.PinCodeValue != null && pincodeStrList.Contains(x.PinCodeValue.Trim()))))
                              .Select(x => x.CityId!.Value)
                              .Distinct()
                              .ToList();
        var cities = await GetAllCacheCities();
        var stateIds = cities.Where(x => x != null && cityIds.Contains(x.CityId) && x.StateId.HasValue)
                             .Select(x => (long)x.StateId!.Value)
                             .Distinct()
                             .ToList();
        var states = await GetAllCacheStates();
        dataList = states.Where(x => x != null && stateIds.Contains(x.StateId))
                    .OrderBy(x => x.StateId)
                    .Select(x => new AddressCommonLookupModel
                    {
                      Id = isAllowUnderscore ? $"{x.StateId}_0" : x.StateId,
                      Name = x.Name
                    }).ToList();
      }

    }
    return dataList;
  }
  #endregion
  #endregion
  public async Task<List<AddressCommonLookupModel>> GetAddressEntitiesByType(string? selectedEntityIds, string? selectedEntityType, string? nextEntityType, int? countryId, int? carrierId = 0)
  {
    selectedEntityIds = selectedEntityIds!.Trim();
    selectedEntityType = selectedEntityType!.Trim().ToLower();
    nextEntityType = nextEntityType!.Trim().ToLower();

    Carrier? oCarrier = null;
    if (carrierId > 0 && carrierId != null)
    {
      oCarrier = await _context.Carriers.FirstOrDefaultAsync(x => x.CarrierId == carrierId);
      if (oCarrier != null && oCarrier.IsDispatchExCompany.GetValueOrDefault()) // if dispatchex comany then we dont neeed to filter further address
      {
        oCarrier = null;
        carrierId = 0;
      }
    }

    //var listIds = selectedEntityIds.Split(',').ToList();
    //var intSelectedListIds = listIds.Select(id => int.Parse(id)).ToList();
    var result = new List<AddressCommonLookupModel>();

    //#region carrier related data
    //var mappedIds = string.Empty;
    //if (carrierId > 0 && carrierId != null)
    //{

    //  var cLocation = await _context.CarrierLocations.FirstOrDefaultAsync(x => x.CountryId == countryId);
    //  if (cLocation != null)
    //  {
    //    int entityTypeId = 0;
    //    EnumCivilEntityType entityType;
    //    if (Enum.TryParse(nextEntityType, true, out entityType)) // 'true' makes it case-insensitive
    //    {
    //      entityTypeId = (int)entityType;
    //    }

    //    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped();
    //    var dt = civilEntityCarrierMapped.Where(cecm => cecm.CivilEntityTypeId == entityTypeId && cecm.CarrierId == carrierId);
    //    mappedIds = string.Join(",", dt.Select(x => x.MappedId).ToList());
    //  }
    //  var carrierLocatoionAddressingScheme = cLocation?.AddressingScheme;
    //  // ["city","area"]
    //}
    //#endregion
    if (selectedEntityType == "country" && nextEntityType == "province")
    {
      //var provinces = await GetAllCacheProvinces();
      //#region carrier mapped provinces
      //if (carrierId > 0 && carrierId != null)
      //{
      //  provinces = await GetAllCacheCivilEntityCarrierMappedProvince(carrierId);
      //}
      //#endregion
      //var filteredProvinces = provinces.Where(x => intSelectedListIds.Contains(x.CountryId.GetValueOrDefault()))
      //                                  .Select(x => new { x.ProvinceId, x.Name })
      //                                  .OrderBy(x => x.ProvinceId)
      //                                  .ToList(); 
      var filteredProvinces = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, 0, (int)EnumCivilEntityType.Province, carrierId);
      result.AddRange(filteredProvinces);
    }
    else if (selectedEntityType == "country" && nextEntityType == "state")
    {
      //var states = await GetAllCacheStates();
      //#region carrier mapped states
      //if (carrierId > 0 && carrierId != null)
      //{
      //  states = await GetAllCacheCivilEntityCarrierMappedSates(carrierId);
      //}
      //#endregion
      //var filteredStates = states.Where(x => intSelectedListIds.Contains(x.CountryId.GetValueOrDefault()))
      //                           .Select(x => new { x.StateId, x.Name })
      //                           .OrderBy(x => x.StateId)
      //                           .ToList();

      var filteredData = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, 0, (int)EnumCivilEntityType.State, carrierId);
      result.AddRange(filteredData);
    }
    else if (selectedEntityType == "country" && nextEntityType == "city")
    {
      //var cities = await GetAllCacheCities();

      //#region carrier mapped city
      //if (carrierId > 0 && carrierId != null)
      //{
      //  cities = await GetAllCacheCivilEntityCarrierMappedCities(carrierId);
      //}
      //#endregion

      var filteredData = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, 0, (int)EnumCivilEntityType.City, carrierId);
      result.AddRange(filteredData);
    }
    else if (selectedEntityType == "state" && nextEntityType == "city")
    {
      //var cities = await GetAllCacheCities();
      //#region carrier mapped city
      //if (carrierId > 0 && carrierId != null)
      //{
      //  cities = await GetAllCacheCivilEntityCarrierMappedCities(carrierId);
      //}
      //#endregion
      //var filteredCities = cities.Where(x => intSelectedListIds.Contains(x.StateId.GetValueOrDefault()))
      //                           .Select(x => new { x.CityId, x.Name })
      //                           .OrderBy(x => x.CityId)
      //                           .ToList();

      var filteredData = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, (int)EnumCivilEntityType.State, (int)EnumCivilEntityType.City, carrierId);
      result.AddRange(filteredData);
    }
    else if (selectedEntityType == "province" && nextEntityType == "city")
    {
      //var cities = await GetAllCacheCities();
      //#region carrier mapped city
      //if (carrierId > 0 && carrierId != null)
      //{
      //  cities = await GetAllCacheCivilEntityCarrierMappedCities(carrierId);
      //}
      //#endregion
      //var filteredCities = cities.Where(x => intSelectedListIds.Contains(x.ProvinceId.GetValueOrDefault()))
      //                             .Select(x => new { x.CityId, x.Name })
      //                             .OrderBy(x => x.CityId)
      //                             .ToList();
      //result.AddRange(filteredCities);
      var filteredData = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, (int)EnumCivilEntityType.Province, (int)EnumCivilEntityType.City, carrierId);
      result.AddRange(filteredData);
    }
    else if (selectedEntityType == "city" && nextEntityType == "area")
    {
      //var areas = await GetAllCacheAreas();
      //#region carrier mapped areas
      //if (carrierId > 0 && carrierId != null)
      //{
      //  areas = await GetAllCacheCivilEntityCarrierMappedAreas(carrierId);
      //}
      //#endregion
      //var filteredAreas = areas.Where(x => intSelectedListIds.Contains(x.CityId))
      //                         .Select(x => new { x.AreaId, x.Name })
      //                         .OrderBy(x => x.AreaId)
      //                         .ToList();
      //result.AddRange(filteredAreas);
      var filteredData = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, (int)EnumCivilEntityType.City, (int)EnumCivilEntityType.Area, carrierId);
      result.AddRange(filteredData);
    }
    else if (selectedEntityType == "city" && nextEntityType == "pincode")
    {
      //var pinCodes = await GetAllCachePinCodes();
      //#region carrier mapped city
      //if (carrierId > 0 && carrierId != null)
      //{
      //  pinCodes = await GetAllCacheCivilEntityCarrierMappedPnCodes(carrierId);
      //}
      //#endregion
      //var filteredPinCodes = pinCodes.Where(x => intSelectedListIds.Contains(x.CityId.GetValueOrDefault()))
      //                               .Select(x => new { x.PinCodeId, x.PinCodeValue })
      //                               .OrderBy(x => x.PinCodeId)
      //                               .ToList();
      //result.AddRange(filteredPinCodes);
      var filteredData = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, (int)EnumCivilEntityType.City, (int)EnumCivilEntityType.PinCode, carrierId);
      result.AddRange(filteredData);
    }
    else if (selectedEntityType == "pincode" && nextEntityType == "city")
    {
      var filteredData = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, (int)EnumCivilEntityType.PinCode, (int)EnumCivilEntityType.City, carrierId);
      result.AddRange(filteredData);
    }
    else if (selectedEntityType == "pincode" && nextEntityType == "state")
    {
      var filteredData = await GetAllCacheCivilEntityWithCarrierMappedByTypes(selectedEntityIds, (int)EnumCivilEntityType.PinCode, (int)EnumCivilEntityType.State, carrierId);
      result.AddRange(filteredData);
    }
    else
    {

    }
    return result;
  }
  #endregion
  #region multiple || civil entity
  public async Task<dynamic> GetAddressEntitiesByType_backup(string? selectedEntityIds, string? selectedEntityType, string? nextEntityType, int? countryId, int? carrierId = 0)
  {
    selectedEntityIds = selectedEntityIds!.Trim();
    selectedEntityType = selectedEntityType!.Trim().ToLower();
    nextEntityType = nextEntityType!.Trim().ToLower();

    Carrier? oCarrier = null;
    if (carrierId > 0 && carrierId != null)
    {
      oCarrier = await _context.Carriers.FirstOrDefaultAsync(x => x.CarrierId == carrierId);
      if (oCarrier != null && oCarrier.IsDispatchExCompany.GetValueOrDefault()) // if dispatchex comany then we dont neeed to filter further address
      {
        oCarrier = null;
        carrierId = 0;
      }
    }

    var listIds = selectedEntityIds.Split(',').ToList();
    var intListIds = listIds.Select(id => int.Parse(id)).ToList();
    var result = new List<dynamic>();

    //#region carrier related data
    //var mappedIds = string.Empty;
    //if (carrierId > 0 && carrierId != null)
    //{

    //  var cLocation = await _context.CarrierLocations.FirstOrDefaultAsync(x => x.CountryId == countryId);
    //  if (cLocation != null)
    //  {
    //    int entityTypeId = 0;
    //    EnumCivilEntityType entityType;
    //    if (Enum.TryParse(nextEntityType, true, out entityType)) // 'true' makes it case-insensitive
    //    {
    //      entityTypeId = (int)entityType;
    //    }

    //    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped();
    //    var dt = civilEntityCarrierMapped.Where(cecm => cecm.CivilEntityTypeId == entityTypeId && cecm.CarrierId == carrierId);
    //    mappedIds = string.Join(",", dt.Select(x => x.MappedId).ToList());
    //  }
    //  var carrierLocatoionAddressingScheme = cLocation?.AddressingScheme;
    //  // ["city","area"]
    //}
    //#endregion
    if (selectedEntityType == "country" && nextEntityType == "province")
    {
      var provinces = await GetAllCacheProvinces();
      #region carrier mapped provinces
      if (carrierId > 0 && carrierId != null)
      {
        provinces = await GetAllCacheCivilEntityCarrierMappedProvince(carrierId);
      }
      #endregion
      var filteredProvinces = provinces.Where(x => intListIds.Contains(x.CountryId.GetValueOrDefault()))
                                        .Select(x => new { x.ProvinceId, x.Name })
                                        .OrderBy(x => x.ProvinceId)
                                        .ToList();
      result.AddRange(filteredProvinces);
    }
    else if (selectedEntityType == "country" && nextEntityType == "state")
    {
      var states = await GetAllCacheStates();
      #region carrier mapped states
      if (carrierId > 0 && carrierId != null)
      {
        states = await GetAllCacheCivilEntityCarrierMappedSates(carrierId);
      }
      #endregion
      var filteredStates = states.Where(x => intListIds.Contains(x.CountryId.GetValueOrDefault()))
                                 .Select(x => new { x.StateId, x.Name })
                                 .OrderBy(x => x.StateId)
                                 .ToList();
      result.AddRange(filteredStates);
    }
    else if (selectedEntityType == "country" && nextEntityType == "city")
    {
      var cities = await GetAllCacheCities();

      #region carrier mapped city
      if (carrierId > 0 && carrierId != null)
      {
        cities = await GetAllCacheCivilEntityCarrierMappedCities(carrierId);
      }
      #endregion

      var filteredCities = cities.Where(x => intListIds.Contains(x.CountryId.GetValueOrDefault()))
                                 .Select(x => new { x.CityId, x.Name })
                                 .OrderBy(x => x.CityId)
                                 .ToList();
      result.AddRange(filteredCities);
    }
    else if (selectedEntityType == "state" && nextEntityType == "city")
    {
      var cities = await GetAllCacheCities();
      #region carrier mapped city
      if (carrierId > 0 && carrierId != null)
      {
        cities = await GetAllCacheCivilEntityCarrierMappedCities(carrierId);
      }
      #endregion
      var filteredCities = cities.Where(x => intListIds.Contains(x.StateId.GetValueOrDefault()))
                                 .Select(x => new { x.CityId, x.Name })
                                 .OrderBy(x => x.CityId)
                                 .ToList();
      result.AddRange(filteredCities);
    }
    else if (selectedEntityType == "province" && nextEntityType == "city")
    {
      var cities = await GetAllCacheCities();
      #region carrier mapped city
      if (carrierId > 0 && carrierId != null)
      {
        cities = await GetAllCacheCivilEntityCarrierMappedCities(carrierId);
      }
      #endregion
      var filteredCities = cities.Where(x => intListIds.Contains(x.ProvinceId.GetValueOrDefault()))
                                   .Select(x => new { x.CityId, x.Name })
                                   .OrderBy(x => x.CityId)
                                   .ToList();
      result.AddRange(filteredCities);
    }
    else if (selectedEntityType == "city" && nextEntityType == "area")
    {
      var areas = await GetAllCacheAreas();
      #region carrier mapped areas
      if (carrierId > 0 && carrierId != null)
      {
        areas = await GetAllCacheCivilEntityCarrierMappedAreas(carrierId);
      }
      #endregion
      var filteredAreas = areas.Where(x => intListIds.Contains(x.CityId))
                               .Select(x => new { x.AreaId, x.Name })
                               .OrderBy(x => x.AreaId)
                               .ToList();
      result.AddRange(filteredAreas);
    }
    else if (selectedEntityType == "city" && nextEntityType == "pincode")
    {
      var pinCodes = await GetAllCachePinCodes();
      #region carrier mapped city
      if (carrierId > 0 && carrierId != null)
      {
        pinCodes = await GetAllCacheCivilEntityCarrierMappedPnCodes(carrierId);
      }
      #endregion
      var filteredPinCodes = pinCodes.Where(x => intListIds.Contains(x.CityId.GetValueOrDefault()))
                                     .Select(x => new { x.PinCodeId, x.PinCodeValue })
                                     .OrderBy(x => x.PinCodeId)
                                     .ToList();
      result.AddRange(filteredPinCodes);
    }
    return result;
  }
  #endregion

  #region excel export
  public async Task<dynamic> GetAddressEntitiesByTypeForExcel(int countryId, string? nextEntityType)
  {
    nextEntityType = nextEntityType!.Trim().ToLower();

    var result = new List<dynamic>();

    var countries = await GetAllCacheCountries();
    var states = await GetAllCacheStates();
    var provinces = await GetAllCacheProvinces();
    var pinCodes = await GetAllCachePinCodes();
    var cities = await GetAllCacheCities();
    var areas = await GetAllCacheAreas();

    if (nextEntityType == "province")
    {
      // LINQ query
      var query = from p in provinces
                  join c in countries on p.CountryId equals c.CountryId
                  where c.CountryId == countryId
                  select new
                  {
                    Country = c.Name,
                    Province = p.Name
                  };
      List<dynamic> listOfExpando = new List<dynamic>();
      foreach (var item in query)
      {
        dynamic expando = new ExpandoObject();
        expando.Country = item.Country;
        expando.Province = item.Province;

        listOfExpando.Add(expando);
      }
      return listOfExpando;

      //using (var connection = new SqlConnection(_connectionString))
      //{
      //  var dynamicParams = new DynamicParameters();

      //  string query = $@"SELECT c.Name AS CountryName,
      //                           p.Name AS ProvinceName
      //                    FROM dbo.Province AS p
      //                        INNER JOIN dbo.Country AS c
      //                            ON c.CountryId = p.CountryId ";
      //  string whereStart = $"WHERE ( c.CountryId = {countryId}  ";
      //  string whereEnd = ")";

      //  if (countryId > 0)
      //  {
      //    dynamicParams.Add("@countryId", countryId);
      //    whereStart += "And (c.CountryId = @countryId) ";
      //  }

      //  string where = whereStart + whereEnd;

      //  string queryData = query + where;

      //  var data = await connection.QueryAsync(queryData, dynamicParams);
      //  return data.ToList();
      //}

    }
    else if (nextEntityType == "state")
    {
      // LINQ query
      var query = from s in states
                  join c in countries on s.CountryId equals c.CountryId
                  where c.CountryId == countryId
                  select new
                  {
                    Country = c.Name,
                    State = s.Name
                  };
      List<dynamic> listOfExpando = new List<dynamic>();
      foreach (var item in query)
      {
        dynamic expando = new ExpandoObject();
        expando.Country = item.Country;
        expando.State = item.State;
        listOfExpando.Add(expando);
      }

      return listOfExpando;

      //using (var connection = new SqlConnection(_connectionString))
      //{
      //  var dynamicParams = new DynamicParameters();

      //  string query = $@"SELECT c.Name AS CountryName,
      //                           s.Name AS StateName
      //                    FROM dbo.State AS s 
      //                        INNER JOIN dbo.Country AS c
      //                            ON c.CountryId = s.CountryId ";
      //  string whereStart = $"WHERE ( c.CountryId = {countryId}  ";
      //  string whereEnd = ")";

      //  if (countryId > 0)
      //  {
      //    dynamicParams.Add("@countryId", countryId);
      //    whereStart += "And (c.CountryId = @countryId) ";
      //  }

      //  string where = whereStart + whereEnd;

      //  string queryData = query + where;

      //  var data = await connection.QueryAsync(queryData, dynamicParams);
      //  return data.ToList();
      //}
    }
    else if (nextEntityType == "city")
    {
      // LINQ query
      var query = from c2 in cities
                  join c in countries on c2.CountryId equals c.CountryId
                  where c.CountryId == countryId
                  select new
                  {
                    Country = c.Name,
                    City = c2.Name
                  };

      List<dynamic> listOfExpando = new List<dynamic>();
      foreach (var item in query)
      {
        dynamic expando = new ExpandoObject();
        expando.Country = item.Country;
        expando.City = item.City;
        listOfExpando.Add(expando);
      }
      return listOfExpando;

      //using (var connection = new SqlConnection(_connectionString))
      //{
      //  var dynamicParams = new DynamicParameters();

      //  string query = $@"SELECT c.Name AS CountryName,
      //                           c2.Name AS CityName
      //                    FROM dbo.City AS c2
      //                        INNER JOIN dbo.Country AS c
      //                            ON c.CountryId = c2.CountryId ";
      //  string whereStart = $"WHERE ( c.CountryId = {countryId}  ";
      //  string whereEnd = ")";

      //  if (countryId > 0)
      //  {
      //    dynamicParams.Add("@countryId", countryId);
      //    whereStart += "And (c.CountryId = @countryId) ";
      //  }

      //  string where = whereStart + whereEnd;

      //  string queryData = query + where;

      //  var data = await connection.QueryAsync(queryData, dynamicParams);
      //  return data.ToList();
      //}
    }

    else if (nextEntityType == "area")
    {
      // LINQ query
      var query = from a in areas
                  join c2 in cities on a.CityId equals c2.CityId
                  join c in countries on c2.CountryId equals c.CountryId
                  where c.CountryId == countryId
                  select new
                  {
                    Country = c.Name,
                    City = c2.Name,
                    Area = a.Name
                  };

      List<dynamic> listOfExpando = new List<dynamic>();
      foreach (var item in query)
      {
        dynamic expando = new ExpandoObject();
        expando.Country = item.Country;
        expando.City = item.City;
        expando.Area = item.Area;
        listOfExpando.Add(expando);
      }
      return listOfExpando;

      //using (var connection = new SqlConnection(_connectionString))
      //{
      //  var dynamicParams = new DynamicParameters();

      //  string query = $@"SELECT c.Name AS CountryName,
      //                           c2.Name AS CityName,
      //                           a.Name AS AreaName
      //                    FROM dbo.Area AS a
      //                        INNER JOIN dbo.City AS c2
      //                            ON c2.CityId = a.CityId
      //                        INNER JOIN dbo.Country AS c
      //                            ON c.CountryId = c2.CountryId ";
      //  string whereStart = $"WHERE ( c.CountryId = {countryId}  ";
      //  string whereEnd = ")";

      //  if (countryId > 0)
      //  {
      //    dynamicParams.Add("@countryId", countryId);
      //    whereStart += "And (c.CountryId = @countryId) ";
      //  }

      //  string where = whereStart + whereEnd;

      //  string queryData = query + where;

      //  var data = await connection.QueryAsync(queryData, dynamicParams);
      //  return data.ToList();
      //}

    }
    else if (nextEntityType == "pincode")
    {
      // LINQ query to perform the join and filter
      var query = from pc in pinCodes
                  join c2 in cities on pc.CityId equals c2.CityId
                  join c in countries on c2.CountryId equals c.CountryId
                  where countryId <= 0 || c.CountryId == countryId
                  select new
                  {
                    CountryName = c.Name,
                    CityName = c2.Name,
                    PinCode = pc.PinCodeValue
                  };
      List<dynamic> listOfExpando = new List<dynamic>();
      foreach (var item in query)
      {
        dynamic expando = new ExpandoObject();
        expando.CountryName = item.CountryName;
        expando.CityName = item.CityName;
        expando.PinCode = item.PinCode;
        listOfExpando.Add(expando);
      }
      return listOfExpando;

      //using (var connection = new SqlConnection(_connectionString))
      //{
      //  var dynamicParams = new DynamicParameters();

      //  string query = $@"SELECT c.Name AS CountryName,
      //                           c2.Name AS CityName,
      //                           pc.PinCodeValue AS PinCode
      //                    FROM dbo.PinCode AS pc 
      //                        INNER JOIN dbo.City AS c2
      //                            ON c2.CityId = pc.CityId
      //                        INNER JOIN dbo.Country AS c
      //                            ON c.CountryId = c2.CountryId ";
      //  string whereStart = $"WHERE ( c.CountryId = {countryId}  ";
      //  string whereEnd = ")";

      //  if (countryId > 0)
      //  {
      //    dynamicParams.Add("@countryId", countryId);
      //    whereStart += "And (c.CountryId = @countryId) ";
      //  }

      //  string where = whereStart + whereEnd;

      //  string queryData = query + where;

      //  var data = await connection.QueryAsync(queryData, dynamicParams);
      //  return data.ToList();
      //}

    }
    return result;
  }

  #endregion

  #region MyRegion
  public async Task<List<Country>> GetAllCountries()
  {
    return await GetAllCacheCountries()!;
  }
  public async Task<string> GetNameByKeyAsync(string key, int columnValue, SqlConnection connection)
  {
    var query = key.ToLower() switch
    {
      "area" => "SELECT Name FROM dbo.Area WHERE AreaId = @ColumnValue",
      "city" => "SELECT Name FROM dbo.City WHERE CityId = @ColumnValue",
      "province" => "SELECT Name FROM dbo.Province WHERE ProvinceId = @ColumnValue",
      "state" => "SELECT Name FROM dbo.State WHERE StateId = @ColumnValue",
      "pincode" => "SELECT PinCodeValue FROM dbo.PinCode WHERE PinCodeId = @ColumnValue",
      _ => throw new ArgumentException("Invalid key provided.")
    };

    return await connection.QueryFirstOrDefaultAsync<string>(query, new { ColumnValue = columnValue }) ?? "Not Found";
  }
  #endregion
  #region get all data entry
  public async Task<dynamic> GetAddressEntityNameByKey(string? key, int entityId)
  {
    using (var connection = new SqlConnection(_connectionString))
    {
      string name = await GetNameByKeyAsync(key!, entityId, connection);
      Console.WriteLine($"The name for '{key}' with ID {entityId} is: {name}");

      // Convert key to PascalCase

      return name;
    }
  }
  public async Task<List<City>> GetAllCities()
  {
    return await GetAllCacheCities()!;
  }
  public async Task<List<Province>> GetAllProvinces()
  {
    return await GetAllCacheProvinces()!;
  }
  public async Task<List<State>> GetAllStates()
  {
    return await GetAllCacheStates()!;
  }
  public async Task<List<PinCode>> GetAllPinCodes()
  {
    return await GetAllCachePinCodes();
  }
  public async Task<List<Area>> GetAllAreas()
  {
    return await GetAllCacheAreas()!;
  }
  public async Task<List<RegionTimeZone>> GetAllRegionTimeZones()
  {
    return await GetAllCacheRegionTimeZones()!;
  }
  #endregion
  #region get cache data
  public async Task<List<Country>> GetAllCacheCountries(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllCountries);
    }
    if (!_cache.TryGetValue(CacheKeys.AllCountries, out List<Country>? countries))
    {
      countries = await _context.Countries.ToListAsync();
      countries = countries.Where(x => x.CountryId != 0).ToList();
      _cache.Set(CacheKeys.AllCountries, countries, _cacheOptions);
    }
    return countries!;
  }
  public async Task<List<City>> GetAllCacheCities(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllCities);
    }

    if (!_cache.TryGetValue(CacheKeys.AllCities, out List<City>? cities))
    {
      cities = await _context.Cities.ToListAsync();
      cities = cities.Where(x => x.CityId != 0).ToList();
      _cache.Set(CacheKeys.AllCities, cities, _cacheOptions);
    }

    return cities!;
  }
  public async Task<List<Province>> GetAllCacheProvinces(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllProvinces);
    }

    if (!_cache.TryGetValue(CacheKeys.AllProvinces, out List<Province>? provinces))
    {
      provinces = await _context.Provinces.ToListAsync();
      provinces = provinces.Where(x => x.ProvinceId != 0).ToList();
      _cache.Set(CacheKeys.AllProvinces, provinces, _cacheOptions);
    }

    return provinces!;
  }
  public async Task<List<State>> GetAllCacheStates(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllStates);
    }

    if (!_cache.TryGetValue(CacheKeys.AllStates, out List<State>? states))
    {
      states = await _context.States.ToListAsync();
      states = states.Where(x => x.StateId != 0).ToList();
      _cache.Set(CacheKeys.AllStates, states, _cacheOptions);
    }
    return states!;
  }
  public async Task<List<PinCode>> GetAllCachePinCodes(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllPinCodes);
    }

    if (!_cache.TryGetValue(CacheKeys.AllPinCodes, out List<PinCode>? pinCodes))
    {
      pinCodes = await _context.PinCodes.ToListAsync();
      pinCodes = pinCodes.Where(x => x.PinCodeId != 0).ToList();

      _cache.Set(CacheKeys.AllPinCodes, pinCodes, _cacheOptions);
    }
    return pinCodes!;
  }
  public async Task<List<Area>> GetAllCacheAreas(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllAreas);
    }

    if (!_cache.TryGetValue(CacheKeys.AllAreas, out List<Area>? areas))
    {
      // Cache miss - fetch from database
      areas = await _context.Areas.ToListAsync();
      areas = areas.Where(x => x.AreaId != 0).ToList();
      _cache.Set(CacheKeys.AllAreas, areas, _cacheOptions);
    }
    // Cache hit - return cached areas
    return areas!;
  }

  #region carrier related
  public async Task<List<City>> GetAllCacheCivilEntityCarrierMappedCities(int? carrierId = 0, int? dmsTypeId = 0, bool? clearCacheOnRequest = false)
  {
    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped(clearCacheOnRequest);
    var cities = await GetAllCacheCities(clearCacheOnRequest);
    var mappedCities = cities
    .Join(
        civilEntityCarrierMapped
        .Where(cecm => cecm.CivilEntityTypeId == (int)EnumCivilEntityType.City && cecm.CarrierId == carrierId),
        city => city.CityId,
        cecm => cecm.MappedId,
        (city, cecm) => city
    )
    .ToList();

    // Cache hit - return cached areas
    return mappedCities!;
  }
  public async Task<List<Area>> GetAllCacheCivilEntityCarrierMappedAreas(int? carrierId = 0, int? dmsTypeId = 0, bool? clearCacheOnRequest = false)
  {
    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped(clearCacheOnRequest);
    var areas = await GetAllCacheAreas(clearCacheOnRequest);
    var mappedAreas = areas
    .Join(
        civilEntityCarrierMapped
        .Where(cecm => cecm.CivilEntityTypeId == (int)EnumCivilEntityType.Area && cecm.CarrierId == carrierId),
        area => area.AreaId,
        cecm => cecm.MappedId,
        (area, cecm) => area
    )
    .ToList();

    // Cache hit - return cached areas
    return mappedAreas!;
  }
  public async Task<List<Province>> GetAllCacheCivilEntityCarrierMappedProvince(int? carrierId = 0, int? dmsTypeId = 0, bool? clearCacheOnRequest = false)
  {
    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped(clearCacheOnRequest);
    var provinces = await GetAllCacheProvinces(clearCacheOnRequest);
    var mappedProvinces = provinces
    .Join(
        civilEntityCarrierMapped
        .Where(cecm => cecm.CivilEntityTypeId == (int)EnumCivilEntityType.Province && cecm.CarrierId == carrierId),
        city => city.ProvinceId,
        cecm => cecm.MappedId,
        (city, cecm) => city
    )
    .ToList();

    return mappedProvinces;
  }
  public async Task<List<State>> GetAllCacheCivilEntityCarrierMappedSates(int? carrierId = 0, int? dmsTypeId = 0, bool? clearCacheOnRequest = false)
  {
    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped(clearCacheOnRequest);
    var states = await GetAllCacheStates(clearCacheOnRequest);
    var mappedStats = states
    .Join(
        civilEntityCarrierMapped
        .Where(cecm => cecm.CivilEntityTypeId == (int)EnumCivilEntityType.State && cecm.CarrierId == carrierId),
        city => city.StateId,
        cecm => cecm.MappedId,
        (city, cecm) => city
    )
    .ToList();

    return mappedStats;
  }
  public async Task<List<PinCode>> GetAllCacheCivilEntityCarrierMappedPnCodes(int? carrierId = 0, int? dmsTypeId = 0, bool? clearCacheOnRequest = false)
  {
    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped(clearCacheOnRequest);
    var pinCodes = await GetAllCachePinCodes(clearCacheOnRequest);
    var mappedPinCodes = pinCodes
    .Join(
        civilEntityCarrierMapped
        .Where(cecm => cecm.CivilEntityTypeId == (int)EnumCivilEntityType.PinCode && cecm.CarrierId == carrierId),
        city => city.PinCodeId,
        cecm => cecm.MappedId,
        (city, cecm) => city
    )
    .ToList();

    return mappedPinCodes;
  }
  public async Task<string?> GetCivilEntityExtendedNameByIdAndTypeId(long? civilEntityExtendedId, int? civilEntityTypeId)
  {
    string name = string.Empty;
    var civilEntityExtended = await GetCivilEntityExtendedByIdAndTypeId(civilEntityExtendedId, civilEntityTypeId);
    if (civilEntityExtended is not null)
    {
      name = civilEntityExtended.CivilEntityName!;
    }
    return name!;
  }
  public async Task<CivilEntityExtended?> GetCivilEntityExtendedByIdAndTypeId(long? civilEntityExtendedId, int? civilEntityTypeId)
  {
    var civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped();
    var civilEntityExtended = civilEntityCarrierMapped.FirstOrDefault(x => x.CivilEntityExtendedId == civilEntityExtendedId && x.CivilEntityTypeId == civilEntityTypeId);
    return civilEntityExtended!;
  }
  public async Task<List<CivilEntityExtended>> GetAllCacheCivilEntityCarrierMapped(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllCivilEntityCarrierMapped);
    }
    if (!_cache.TryGetValue(CacheKeys.AllCivilEntityCarrierMapped, out List<CivilEntityExtended>? mappedCarriers))
    {
      // Cache miss - fetch from database
      mappedCarriers = await _context.CivilEntityExtendeds.ToListAsync();
      //mappedCarriers = mappedCarriers.Where(x => x.MappedId != 0).ToList();
      _cache.Set(CacheKeys.AllCivilEntityCarrierMapped, mappedCarriers, _cacheOptions);
    }
    // Cache hit - return cached areas
    return mappedCarriers!;
  }

  #endregion
  public async Task<List<RegionTimeZone>> GetAllCacheRegionTimeZones(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllRegionTimeZones);
    }

    if (!_cache.TryGetValue(CacheKeys.AllRegionTimeZones, out List<RegionTimeZone>? regionTimeZones))
    {
      regionTimeZones = await _context.RegionTimeZones.ToListAsync();
      _cache.Set(CacheKeys.AllRegionTimeZones, regionTimeZones, _cacheOptions);
    }

    return regionTimeZones!;
  }
  #endregion
  #region get address from google map 


  #region old map
  public async Task<dynamic> GetCivilEntityIdByLatitudeAndLongitude_old(decimal latitude, decimal longitude)
  {
    var data = await ReverseGeocodeAsync((double)latitude, (double)longitude);
    var countries = await GetAllCacheCountries();
    var provinces = await GetAllCacheProvinces();
    var states = await GetAllCacheStates();
    var cities = await GetAllCacheCities();
    var areas = await GetAllCacheAreas();
    var pinCodes = await GetAllCachePinCodes();

    CombinedAddressResponseModal responseModal = new CombinedAddressResponseModal();
    if (data is not null)
    {
      if (!string.IsNullOrEmpty(data.Country))
      {
        var country = await GetCountryByName(data.Country);
        if (country is not null)
        {
          responseModal.Country = country.CountryId;
        }
      }
      if (!string.IsNullOrEmpty(data.Province))
      {
        var province = provinces.FirstOrDefault(x => x.Name == data.Province);
        if (province is not null)
        {
          responseModal.Province = province.ProvinceId;
        }
      }
      if (!string.IsNullOrEmpty(data.State))
      {
        var state = await GetStateByName(data.State!);
        if (state is not null)
        {
          responseModal.State = state.StateId;
        }
      }
      if (!string.IsNullOrEmpty(data.City))
      {
        var city = await GetCityByName(data.City!);
        if (city is not null)
        {
          responseModal.City = city.CityId;
        }
      }
      if (!string.IsNullOrEmpty(data.Neighborhood))
      {
        var neighborhood = await GetAreaByName(data.Neighborhood!);
        if (neighborhood is not null)
        {
          responseModal.Area = neighborhood.AreaId;
        }
      }
      if (!string.IsNullOrEmpty(data.PinCode))
      {
        var pincode = await GetPinCodeByName(data.PinCode!);
        if (pincode is not null)
        {
          responseModal.PinCode = pincode.PinCodeId;
        }
      }
      // Remove trailing comma and space
      if (!string.IsNullOrEmpty(data.Street))
      {
        data.Street = data.Street!.TrimEnd(new char[] { ',', ' ' }); ;
      }
      responseModal.StreetAddress = data.Street;
    }
    return responseModal;
  }

  public async Task<CombinedAddressModal> ReverseGeocodeAsync(double latitude, double longitude)
  {
    using (var _httpClient = new HttpClient())
    {
      //string mapkey = "AIzaSyD-HG8jXSDrt7JoF_zaH30djk0H_JokVIQ";
      var mapkey = await GetMapKeys();

      var request = new HttpRequestMessage(HttpMethod.Get, $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={mapkey}&language=en");
      var response = await _httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();
      var responseBody = await response.Content.ReadAsStringAsync();
      GeocodingResponseModal? geocodingResponse = JsonConvert.DeserializeObject<GeocodingResponseModal>(responseBody);

      CombinedAddressModal addressModal = new CombinedAddressModal();

      foreach (GeocodingResultModal result in geocodingResponse!.results!)
      {
        if (result.address_components != null)
        {
          foreach (GeocodingAddressComponent component in result.address_components)
          {
            // Determine the address component type and set the corresponding property in the CombinedAddressModal
            if (component.types!.Contains("country"))
            {
              addressModal.Country = component.long_name;
            }
            else if (component.types!.Contains("administrative_area_level_1"))
            {
              addressModal.State = component.long_name;
            }
            else if (component.types!.Contains("locality"))
            {
              addressModal.City = component.long_name;
            }
            else if (component.types!.Contains("sublocality"))
            {
              addressModal.Neighborhood = component.long_name;
            }
            else if (component.types!.Contains("postal_code"))
            {
              addressModal.PinCode = component.long_name;
            }
            if (component!.types!.Contains("subpremise") || component!.types!.Contains("street_number") || component!.types!.Contains("route") || component!.types!.Contains("plus_code"))
            {
              addressModal.Street += $"{component.long_name}, ";
            }
            else if (component.types!.Contains("administrative_area_level_2"))
            {
              addressModal.Province = component.long_name;
            }
          }
        }
      }

      // You now have the CombinedAddressModal populated with the relevant data

      return addressModal;
    }
  }

  #endregion
  #region new map code 
  public async Task<dynamic> GetCivilEntityIdByLatitudeAndLongitude(decimal latitude, decimal longitude,int? carrierId = 0)
  {
    var responseModal = await GetCivilEntityIdByLatitudeAndLongitude_new(latitude, longitude,carrierId);
    return responseModal;
  }
  public string ProcessCivilEntitiesForMapLatLng(List<CivilEntityExtended> civilEntityCarrierMapped, string name, int civilEntityTypeId)
  {
    var civilKeyDataList = civilEntityCarrierMapped.Where(x => x.CivilEntityTypeId == civilEntityTypeId).ToList();

    var civilEnitityListByName = civilKeyDataList.Where(x => x.CivilEntityName!.ToLower() == name).ToList();

    if (civilEnitityListByName.Count > 1)
    {
      // Send email to Shipra 
    }

    var objData = civilEnitityListByName.FirstOrDefault();
    return objData == null ? "" : $"{objData?.CivilEntityExtendedId}";

  }
  public async Task<dynamic> GetCivilEntityIdByLatitudeAndLongitude_new(decimal latitude, decimal longitude, int? carrierId)
  {
    var geocodingResponse = await ReverseGeocodeAsync_new((double)latitude, (double)longitude);
    var countries = await GetAllCacheCountries();
    var provinces = await GetAllCacheProvinces();
    var states = await GetAllCacheStates();
    var cities = await GetAllCacheCities();
    var areas = await GetAllCacheAreas();
    var pinCodes = await GetAllCachePinCodes();

    CombinedAddressResponseModal responseModal = new CombinedAddressResponseModal();
    List<CivilEntityExtended>? civilEntityCarrierMapped = null;
    if (carrierId > 0)
    {
      civilEntityCarrierMapped = await GetAllCacheCivilEntityCarrierMapped();
    }
    var uniqueGoogleAddressStr = GetUniqueStringList(geocodingResponse);
    uniqueGoogleAddressStr.Reverse(); //reverse unique keys
    if (geocodingResponse is not null)
    {
      var countryName = GetCountryNameFromGeocode(geocodingResponse);

      Country? country = await GetCountryByName(countryName!);

      if (country is not null)
      {
        var keyList = JsonConvert.DeserializeObject<CountryAddressSchema>(country!.AddressingScheme!);
        responseModal.Country = country.CountryId;
        if (keyList is not null)
        {
          City? city = null;
          Province? province = null;
          State? state = null;
          PinCode? pinCode = null;
          Area? area = null;

          //reverse all key for address creation
          keyList!.keys!.Reverse();

          // Loop through the reversed list
          foreach (string key in keyList!.keys!)
          {
            // If-else statement to check the value of each string
            if (key == "city")
            {
              foreach (var dataKey in uniqueGoogleAddressStr)
              {
                var uniqueKey = dataKey.ToLower()!;
                if (carrierId.GetValueOrDefault() > 0)
                {
                  var civilArea = ProcessCivilEntitiesForMapLatLng(civilEntityCarrierMapped!, uniqueKey, (int)EnumCivilEntityType.Area);
                  responseModal.Area = civilArea;

                  break;
                }

                var cityListByName = cities
                    .Where(x => x.Name!.ToLower() == uniqueKey &&
                                (country == null || x.CountryId == country.CountryId))  
                    .ToList();
                if (cityListByName.Count > 0)
                {
                  if (cityListByName.Count > 1)
                  {
                    // send email to shipra  
                  }
                  city = cityListByName.FirstOrDefault();
                  responseModal.City = city?.CityId;
                  break;
                }
                else
                {
                  // insert to db
                }
              }
            }
            else if (key == "area")
            {

              foreach (string dataKey in uniqueGoogleAddressStr)
              {
                var uniqueKey = dataKey.ToLower()!;
                if (carrierId.GetValueOrDefault() > 0)
                {
                  var civilArea = ProcessCivilEntitiesForMapLatLng(civilEntityCarrierMapped!, uniqueKey, (int)EnumCivilEntityType.Area);
                  responseModal.Area = civilArea;

                  break;
                }
                var areaListByName = areas.Where(x => x.Name!.ToLower() == uniqueKey).ToList();
                if (areaListByName.Count > 0)
                {
                  if (areaListByName.Count > 1)
                  {
                    // send email to shipra  
                  }
                  area = areaListByName.FirstOrDefault();
                  responseModal.Area = area?.AreaId;

                  break;

                }
                else
                {
                  // insert to db
                }
              }
            }
            else if (key == "province")
            {
              foreach (var dataKey in uniqueGoogleAddressStr)
              {
                var uniqueKey = dataKey.ToLower()!;
                var provinceListByName = provinces.Where(x => x.Name!.ToLower() == uniqueKey).ToList();
                if (provinceListByName.Count > 0)
                {
                  if (provinceListByName.Count > 1)
                  {
                    // send email to shipra 
                  }
                  province = provinceListByName.FirstOrDefault();
                  responseModal.Province = province?.ProvinceId;

                  break;
                }
                else
                {
                  // insert to db
                }
              }
            }
            else if (key == "pinCode")
            {
              foreach (var dataKey in uniqueGoogleAddressStr)
              {
                var uniqueKey = dataKey.ToLower()!;
                var provinceListByName = pinCodes.Where(x => x.PinCodeValue!.ToLower() == uniqueKey && (city == null || x.CityId == city.CityId)).ToList();
                if (provinceListByName.Count > 0)
                {
                  if (provinceListByName.Count > 1)
                  {
                    // send email to shipra 
                  }
                  pinCode = provinceListByName.FirstOrDefault();
                  responseModal.PinCode = pinCode?.PinCodeId;
                  break;

                }
                else
                {
                  // insert to db
                }
              }
            }
            else if (key == "state")
            {
              foreach (var dataKey in uniqueGoogleAddressStr)
              {
                var uniqueKey = dataKey.ToLower()!;
                var stateListByName = states.Where(x => x.Name!.ToLower() == uniqueKey).ToList();
                if (stateListByName.Count > 0)
                {
                  if (stateListByName.Count > 1)
                  {
                    // send email to shipra 
                  }
                  state = stateListByName.FirstOrDefault();
                  responseModal.State = state?.StateId;
                  break;

                }
                else
                {
                  // insert to db
                }
              }
            }
            else
            {
              Console.WriteLine("Unknown key.");
            }
          }

        }
      }

      return responseModal;
    }

    return responseModal;
  }
  private List<string> GetUniqueStringList(GeocodingResponseModal geocodingResponse)
  {
    HashSet<string> uniqueAddressValues = new HashSet<string>();

    foreach (GeocodingResultModal result in geocodingResponse!.results!)
    {
      foreach (var component in result.address_components!)
      {
        uniqueAddressValues.Add(component.long_name!); // HashSet ensures uniqueness
      }
    }

    //// Convert to a List if needed
    List<string> addressValues = uniqueAddressValues.ToList();
    return addressValues;
  }
  private string? GetCountryNameFromGeocode(GeocodingResponseModal geocodingResponse)
  {
    string? country = null;

    foreach (var result in geocodingResponse!.results!)
    {
      foreach (var component in result.address_components!)
      {
        if (component.types!.Contains("country"))
        {
          country = component.long_name; // Get the full country name
          break;
        }
      }
      if (country != null) break; // Exit once found
    }
    return country;
  }
  public async Task<GeocodingResponseModal> ReverseGeocodeAsync_new(double latitude, double longitude)
  {
    using (var _httpClient = new HttpClient())
    {
      //string mapkey = "AIzaSyD-HG8jXSDrt7JoF_zaH30djk0H_JokVIQ";
      var mapkey = await GetMapKeys();

      var request = new HttpRequestMessage(HttpMethod.Get, $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={mapkey}&language=en");
      var response = await _httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();

      var responseBody = await response.Content.ReadAsStringAsync();
      GeocodingResponseModal? geocodingResponse = JsonConvert.DeserializeObject<GeocodingResponseModal>(responseBody);

      //CombinedAddressModal addressModal = new CombinedAddressModal(); 
      return geocodingResponse!;
    }
  }

  #endregion
  // public async Task<CombinedAddressModal> ReverseGeocodeAsync1(double latitude, double longitude)
  //{
  //  using (var _httpClient = new HttpClient())
  //  {
  //    //string mapkey = "AIzaSyD-HG8jXSDrt7JoF_zaH30djk0H_JokVIQ";
  //    var mapkey = await GetMapKeys();

  //    var request = new HttpRequestMessage(HttpMethod.Get, $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={mapkey}&language=en");
  //    var response = await _httpClient.SendAsync(request);
  //    response.EnsureSuccessStatusCode();

  //    var responseBody = await response.Content.ReadAsStringAsync();
  //    GeocodingResponseModal? geocodingResponse = JsonConvert.DeserializeObject<GeocodingResponseModal>(responseBody);

  //    CombinedAddressModal addressModal = new CombinedAddressModal();

  //    foreach (GeocodingResultModal result in geocodingResponse!.results!)
  //    {
  //      foreach (GeocodingAddressComponent component in result.address_components!)
  //      {
  //        if (component!.types!.Contains("subpremise") || component!.types!.Contains("street_number") || component!.types!.Contains("route") || component!.types!.Contains("plus_code"))
  //        {
  //          addressModal.Street += $"{component.long_name}, ";
  //        }


  //        if (component!.types!.Contains("country"))
  //        {
  //          addressModal.Country = component.long_name;
  //          #region uae
  //          if (component!.long_name == "United Arab Emirates")
  //          {
  //            Console.WriteLine($"Country: {component.long_name}");

  //            // find the city
  //            foreach (GeocodingAddressComponent cityComponent in result.address_components!)
  //            {
  //              if (cityComponent!.types!.Contains("locality"))
  //              {
  //                addressModal.City = cityComponent.long_name;
  //                Console.WriteLine($"City: {cityComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the neighborhood
  //            foreach (GeocodingAddressComponent neighborhoodComponent in result.address_components!)
  //            {
  //              if (neighborhoodComponent!.types!.Contains("neighborhood"))
  //              {
  //                addressModal.Neighborhood = neighborhoodComponent.long_name;
  //                Console.WriteLine($"Neighborhood: {neighborhoodComponent.long_name}");
  //                break;
  //              }
  //            }
  //          }
  //          #endregion

  //          #region pakistan
  //          else if (component.long_name == "Pakistan")
  //          {
  //            Console.WriteLine($"Country: {component.long_name}");

  //            // find the province
  //            foreach (GeocodingAddressComponent provinceComponent in result.address_components!)
  //            {
  //              if (provinceComponent!.types!.Contains("administrative_area_level_1"))
  //              {
  //                addressModal.Province = provinceComponent.long_name;
  //                Console.WriteLine($"Province: {provinceComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the city
  //            foreach (GeocodingAddressComponent cityComponent in result.address_components!)
  //            {
  //              if (cityComponent!.types!.Contains("locality"))
  //              {
  //                addressModal.City = cityComponent.long_name;
  //                Console.WriteLine($"City: {cityComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the area
  //            foreach (GeocodingAddressComponent areaComponent in result.address_components!)
  //            {
  //              if (areaComponent.types!.Contains("sublocality"))
  //              {
  //                addressModal.Neighborhood = areaComponent.long_name;
  //                Console.WriteLine($"Area: {areaComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the postal code
  //            foreach (GeocodingAddressComponent postalCodeComponent in result.address_components)
  //            {
  //              if (postalCodeComponent.types!.Contains("postal_code"))
  //              {
  //                addressModal.PinCode = postalCodeComponent.long_name;
  //                Console.WriteLine($"Postal Code: {postalCodeComponent.long_name}");
  //                break;
  //              }
  //            }
  //          }

  //          #endregion

  //          #region india
  //          else if (component.long_name == "India")
  //          {
  //            Console.WriteLine($"Country: {component.long_name}");

  //            // find the state
  //            foreach (GeocodingAddressComponent stateComponent in result.address_components!)
  //            {
  //              if (stateComponent!.types!.Contains("administrative_area_level_1"))
  //              {
  //                addressModal.State = stateComponent.long_name;
  //                Console.WriteLine($"State: {stateComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the city
  //            foreach (GeocodingAddressComponent cityComponent in result.address_components!)
  //            {
  //              if (cityComponent!.types!.Contains("locality"))
  //              {
  //                addressModal.City = cityComponent.long_name;
  //                Console.WriteLine($"City: {cityComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the area
  //            foreach (GeocodingAddressComponent areaComponent in result.address_components!)
  //            {
  //              if (areaComponent.types!.Contains("sublocality"))
  //              {
  //                addressModal.Neighborhood = areaComponent.long_name;
  //                Console.WriteLine($"Area: {areaComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the postal code
  //            foreach (GeocodingAddressComponent postalCodeComponent in result.address_components)
  //            {
  //              if (postalCodeComponent.types!.Contains("postal_code"))
  //              {
  //                addressModal.PinCode = postalCodeComponent.long_name;
  //                Console.WriteLine($"Postal Code: {postalCodeComponent.long_name}");
  //                break;
  //              }
  //            }
  //          }
  //          #endregion

  //          #region saudi arabia
  //          else if (component.long_name == "Saudi Arabia")
  //          {
  //            Console.WriteLine($"Country: {component.long_name}");
  //            // find the province
  //            foreach (GeocodingAddressComponent provinceComponent in result.address_components!)
  //            {
  //              if (provinceComponent.types!.Contains("administrative_area_level_1"))
  //              {
  //                addressModal.Province = provinceComponent.long_name;
  //                Console.WriteLine($"Province: {provinceComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the city
  //            foreach (GeocodingAddressComponent cityComponent in result.address_components!)
  //            {
  //              if (cityComponent.types!.Contains("locality"))
  //              {
  //                addressModal.City = cityComponent.long_name;
  //                Console.WriteLine($"City: {cityComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the area
  //            foreach (GeocodingAddressComponent areaComponent in result.address_components!)
  //            {
  //              if (areaComponent.types!.Contains("sublocality"))
  //              {
  //                addressModal.Neighborhood = areaComponent.long_name;
  //                Console.WriteLine($"Area: {areaComponent.long_name}");
  //                break;
  //              }
  //            }

  //            // find the postal code
  //            foreach (GeocodingAddressComponent postalCodeComponent in result.address_components)
  //            {
  //              if (postalCodeComponent.types!.Contains("postal_code"))
  //              {
  //                addressModal.PinCode = postalCodeComponent.long_name;
  //                Console.WriteLine($"Postal Code: {postalCodeComponent.long_name}");
  //                break;
  //              }
  //            }
  //          }

  //          #endregion
  //        }
  //      }
  //    }

  //    return addressModal;
  //  }
  //}

  #region google map key related

  public async Task<string> GetMapKeys()
  {
    var Date = DateTime.Now;
    string key = string.Empty;
    List<MapKeysHistory> NewRecords = new List<MapKeysHistory>();
    var list = await AllMapKeys();
    if (list.Count == 0)
    {
      list = await AllMapKeys(clearCacheOnRequest: true);
    }
    var allMapKeysInUses = await GetAllCacheMapKeys();
    if (allMapKeysInUses.Count == 0)
    {
      allMapKeysInUses = await GetAllCacheMapKeys(clearCacheOnRequest: true);
    }
    var CheckSameDayLogin = allMapKeysInUses
      .FirstOrDefault(x => x.StartDate.HasValue && x.StartDate.Value.Date == Date.Date &&
                           x.EndDate.HasValue && x.EndDate.Value.Date == Date.Date);
    if (CheckSameDayLogin == null)
    {
      var CheckHistory = _context.MapKeysHistories
          .FirstOrDefault(x => x.StartDate.HasValue && x.StartDate.Value.Date == Date.Date &&
                               x.EndDate.HasValue && x.EndDate.Value.Date == Date.Date);
      if (CheckHistory != null)
      {
        var Key = _context.MapKeys.FirstOrDefault(x => x.MapkeyId == CheckHistory.MapkeyId);
        if (Key != null)
        {
          key = Key!.ToString()!;
          var InUseKey = allMapKeysInUses.FirstOrDefault();
          if (InUseKey is not null)
          {
            InUseKey!.Update(Date, Date, CheckHistory.MapkeyId, CheckHistory.MapKeysHistoryid);
            await UpdateMapKeysInUse(InUseKey);
            _context.SaveChanges();
            //clear cache
          }
        }
      }
      else
      {
        var DayToAdd = 5;
        var counter = 0;
        foreach (var (item, index) in list.Select((value, i) => (value, i)))
        {
          for (int i = 0; i < DayToAdd; i++)
          {
            if (index == 0 && i == 0)
            {
              var obj = MapKeysHistory.Create(item.MapkeyId, DateTime.Now, DateTime.Now);
              NewRecords.Add(obj);
            }
            else
            {
              counter = counter + 1;

              var obj = MapKeysHistory.Create(item.MapkeyId, DateTime.Now.AddDays(counter), DateTime.Now.AddDays(counter));
              NewRecords.Add(obj);
            }
          }
        }
        _context.MapKeysHistories.AddRange(NewRecords);
        _context.SaveChanges();
        key = list!.FirstOrDefault()!.MapKey1!;
        var KeyInUse = allMapKeysInUses.FirstOrDefault();
        var KeyHistoryId = _context.MapKeysHistories
            .FirstOrDefault(x => x.StartDate.HasValue && x.StartDate.Value.Date == Date.Date &&
                                 x.EndDate.HasValue && x.EndDate.Value.Date == Date.Date);

        if (KeyInUse != null)
        {
          KeyInUse!.Update(DateTime.Now, DateTime.Now, list!.FirstOrDefault()!.MapkeyId, KeyHistoryId!.MapKeysHistoryid!);

          await UpdateMapKeysInUse(KeyInUse);
        }
        else
        {
          MapKeysInUse obj = MapKeysInUse.Create(DateTime.Now, DateTime.Now, list!.FirstOrDefault()!.MapkeyId, KeyHistoryId!.MapKeysHistoryid!);

          await CreateMapKeysInUse(obj);
        }
        _context.SaveChanges();
      }
    }
    else
    {
      var Mapkey = _context.MapKeys.FirstOrDefault(x => x.MapkeyId == CheckSameDayLogin.MapkeyId);
      key = Mapkey!.MapKey1!.ToString();
    }

    return key;
  }
  private async Task<bool> CreateMapKeysInUse(MapKeysInUse mapKeysInUse)
  {
    await _context.MapKeysInUses.AddAsync(mapKeysInUse);
    return await _context.SaveChangesAsync() > 0;
  }
  private async Task<bool> UpdateMapKeysInUse(MapKeysInUse mapKeysInUse)
  {
    _context.MapKeysInUses.Update(mapKeysInUse);
    return await _context.SaveChangesAsync() > 0;
  }


  public async Task<List<MapKey>> AllMapKeys(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllMapKeys);
    }

    if (!_cache.TryGetValue(CacheKeys.AllMapKeys, out List<MapKey>? mapKey))
    {
      mapKey = await _context.MapKeys.ToListAsync();
      _cache.Set(CacheKeys.AllMapKeys, mapKey, _cacheOptions);
    }

    return mapKey!;
  }

  public async Task<List<MapKeysInUse>> GetAllCacheMapKeys(bool? clearCacheOnRequest = false)
  {
    bool isClear = _cacheSettings.Value.ClearCacheOnRequest || clearCacheOnRequest.GetValueOrDefault();

    if (isClear)
    {
      _cache.Remove(CacheKeys.AllMapKeysInUse);
    }

    if (!_cache.TryGetValue(CacheKeys.AllMapKeysInUse, out List<MapKeysInUse>? mapKeyInuse))
    {
      mapKeyInuse = await _context.MapKeysInUses.ToListAsync();
      _cache.Set(CacheKeys.AllMapKeysInUse, mapKeyInuse, _cacheOptions);
    }

    return mapKeyInuse!;
  }
  #endregion

  public async Task<(double? Latitude, double? Longitude)> GetCoordinatesByAddressAsync(string address)
  {
    string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json"; 
    var mapkey = await GetMapKeys();

    if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(mapkey))
      return (null, null); // Return null if address or API key is empty

    using (HttpClient client = new HttpClient())
    {
      string url = $"{BaseUrl}?address={Uri.EscapeDataString(address)}&key={mapkey}";

      HttpResponseMessage response = await client.GetAsync(url);
      if (!response.IsSuccessStatusCode)
        return (null, null); // Return null if API call fails

      string responseBody = await response.Content.ReadAsStringAsync();
      JObject json;

      try
      {
        json = JObject.Parse(responseBody);
      }
      catch
      {
        return (null, null); // Return null if response is not valid JSON
      }

      if (json["status"]?.ToString() != "OK" || json["results"]?.HasValues != true)
        return (null, null); // Return null if address is invalid

      var location = json["results"]?.FirstOrDefault()?["geometry"]?["location"];
      if (location == null)
        return (null, null); // Return null if location data is missing

      double? latitude = location["lat"]?.ToObject<double?>();
      double? longitude = location["lng"]?.ToObject<double?>();

      return (latitude, longitude);
    }
  }

  #endregion

  #region Address
  public async Task<EntityAddressDto?> GetEntityAddressByOriginTypeId(int originTypeId, int id)
  {
    if (!Enum.IsDefined(typeof(EnumCivilEntityType), originTypeId))
      return null;

    var originType = (EnumCivilEntityType)originTypeId;
    var dto = new EntityAddressDto();

    switch (originType)
    {
      case EnumCivilEntityType.Country:
        var countries = await GetAllCacheCountries();
        dto.Country = countries.FirstOrDefault(c => c.CountryId == id);
        dto.DisplayName = dto.Country?.Name;
        break;

      case EnumCivilEntityType.State:
        var states = await GetAllCacheStates();
        dto.State = states.FirstOrDefault(s => s.StateId == id);
        if (dto.State != null)
        {
          var allCountries = await GetAllCacheCountries();
          dto.Country = allCountries.FirstOrDefault(x => x.CountryId == dto.State.CountryId);
          dto.DisplayName = string.Join(", ",
              new[] { dto.State?.Name, dto.Country?.Name }.Where(x => !string.IsNullOrEmpty(x))
          );
        }
        break;

      case EnumCivilEntityType.Province:
        var provinces = await GetAllCacheProvinces();
        dto.Province = provinces.FirstOrDefault(p => p.ProvinceId == id);
        if (dto.Province != null)
        {
          var allCountries = await GetAllCacheCountries();
          dto.Country = allCountries.FirstOrDefault(x => x.CountryId == dto.Province.CountryId);
          dto.DisplayName = string.Join(", ",
              new[] { dto.Province?.Name, dto.Country?.Name }.Where(x => !string.IsNullOrEmpty(x))
          );
        }
        break;

      case EnumCivilEntityType.City:
        var cities = await GetAllCacheCities();
        dto.City = cities.FirstOrDefault(c => c.CityId == id);
        if (dto.City != null)
        {
          var allCountries = await GetAllCacheCountries();
          dto.Country = allCountries.FirstOrDefault(x => x.CountryId == dto.City.CountryId);
          dto.DisplayName = string.Join(", ",
              new[] { dto.City?.Name, dto.Country?.Name }.Where(x => !string.IsNullOrEmpty(x))
          );
        }
        break;

      case EnumCivilEntityType.Area:
        var areas = await GetAllCacheAreas();
        dto.Area = areas.FirstOrDefault(a => a.AreaId == id);
        if (dto.Area != null)
        {
          var allCities = await GetAllCacheCities();
          dto.City = allCities.FirstOrDefault(c => c.CityId == dto.Area.CityId);
          if (dto.City != null)
          {
            var allCountries = await GetAllCacheCountries();
            dto.Country = allCountries.FirstOrDefault(x => x.CountryId == dto.City.CountryId);
          }
          dto.DisplayName = string.Join(", ",
              new[] { dto.Area?.Name, dto.City?.Name, dto.Country?.Name }.Where(x => !string.IsNullOrEmpty(x))
          );
        }
        break;
    }
    return dto;
  }
  #endregion

  public async Task<List<Zone>> GetAllZonesWithCoords()
  {
    return await _appDbContext.Zones.Where(x => x.Active == true).ToListAsync();
  }

  public async Task<Zone?> GetZoneById(int zoneId)
  {
    return await _appDbContext.Zones.FirstOrDefaultAsync(x => x.ZoneId == zoneId && x.Active == true);
  }

  public async Task<bool> SaveNewZone(Zone zone)
  {
    if (zone.ZoneId > 0)
    {
      var existing = await _appDbContext.Zones.FirstOrDefaultAsync(x => x.ZoneId == zone.ZoneId);
      if (existing != null)
      {
        existing.UpdateZone(zone.Name ?? "", zone.CityID, zone.Coords, zone.Code, zone.NameArabic);
        _appDbContext.Zones.Update(existing);
      }
      else
      {
        _appDbContext.Zones.Add(zone);
      }
    }
    else
    {
      _appDbContext.Zones.Add(zone);
    }
    return await _appDbContext.SaveChangesAsync() > 0;
  }

  public async Task<bool> UpdateZoneFromZoneBoundries(string zoneName, int zoneID, int cityID)
  {
    var existing = await _appDbContext.Zones.FirstOrDefaultAsync(x => x.ZoneId == zoneID);
    if (existing != null)
    {
      existing.UpdateZone(zoneName, cityID);
      _appDbContext.Zones.Update(existing);
      return await _appDbContext.SaveChangesAsync() > 0;
    }
    return false;
  }

  public async Task<bool> DeleteZoneByID(int zoneID)
  {
    var existing = await _appDbContext.Zones.FirstOrDefaultAsync(x => x.ZoneId == zoneID);
    if (existing != null)
    {
      existing.Deactivate();
      _appDbContext.Zones.Update(existing);
      return await _appDbContext.SaveChangesAsync() > 0;
    }
    return false;
  }
}

