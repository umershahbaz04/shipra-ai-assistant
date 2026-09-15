using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ICountryRepository
{
  Task<List<CivilEntityType>> GetAllCivilEntityType();
  Task<dynamic> CreateCountry(Country country, List<City> city);
  Task<dynamic> UpdateCountry(Country country, List<City>? city);
  Task<Country?> GetCountryById(int? countryId);
  Task<Country?> GetCountryByCode(string code);
  Task<City?> GetCityByCode(string code);
  Task<List<Country>> GetAllCountries();
  Task<List<RegionTimeZone>> GetAllRegionTimeZone();
  Task<RegionTimeZone?> GetRegionTimeZoneById(int? regionTimeZoneId);

  //Task<List<Zone>?> GetZoneByCityId(long cityId);
  Task<City?> GetCityById(long cityId);
  Task<List<Area>?> GetAllAreaByCityId(long cityId);
  Task<List<City>> GetAllCities();
  Task<List<Province>> GetAllProvinces();
  Task<List<State>> GetAllStates();
  Task<List<Area>> GetAllAreas();
  Task<List<PinCode>> GetAllPinCodes();
  Task<string> GetFullAddress(string? streetAddress, int? countryId, int? cityId, int? areaId, int? provinceId = 0, int? pinCodeId = 0,int? stateId = 0, string? entityAddressDataJson = null);
  Task<Country?> GetCountryByName(string name);
  Task<City?> GetCityByName(string name);
  Task<Area?> GetAreaByName(string name);
  Task<Province?> GetProvinceByName(string name);
  Task<State?> GetStateByName(string name);
  Task<Area?> GetAreaById(int areaId);
  Task<dynamic> GetCounrtyCityRegionIdByName(string countryName, string areaName, string cityName);

  Task<dynamic> GetAllAreaByNameForSelection(int? cityId, string? search = "");

  Task<List<PinCode>> GetPinCodesByCityId(int cityId);
  Task<List<Province>> GetProvinceByCountryId(int countryId);
  Task<List<State>> GetStatesByCountryId(int countryId);
  Task<List<City>> GetCitiesByCountryId(int countryId);
  Task<List<City>> GetCitiesByProvinceAndCountryId(int counrtyId, int provinceId);
  Task<List<City>> GetCitiesByStateAndCountryIdQuery(int counrtyId, int stateId);
  Task<List<City>> GetCities(int id, string? civilEntity);
  Task<List<AddressCommonLookupModel>> GetAddressEntitiesByType(string? selectedEntityIds, string? selectedEntityType, string? nextEntityType,int? countryId, int? CarrierId = 0);
  Task<dynamic> GetAddressEntitiesByTypeForExcel(int countryId, string? nextEntityType);
  Task<dynamic> GetCivilEntityIdByLatitudeAndLongitude(decimal latitude, decimal longitude, int? carrierId=0);
  Task<dynamic> GetAddressEntityNameByKey(string? key, int entityId);
  Task<List<Country>> GetAllCacheCountries(bool? clearCacheOnRequest = false);
  Task<List<Province>> GetAllCacheProvinces(bool? clearCacheOnRequest = false);
  Task<List<State>> GetAllCacheStates(bool? clearCacheOnRequest = false);
  Task<List<Area>> GetAllCacheAreas(bool? clearCacheOnRequest = false);
  Task<List<City>> GetAllCacheCities(bool? clearCacheOnRequest = false);
  Task<List<CivilEntityExtended>> GetAllCacheCivilEntityCarrierMapped(bool? clearCacheOnRequest = false);
  Task<(double? Latitude, double? Longitude)> GetCoordinatesByAddressAsync(string address);
  Task<string> GetEntityNamesByOriginTypeId(int originTypeId, int id);
  Task<EntityAddressDto?> GetEntityAddressByOriginTypeId(int originTypeId, int id);
  Task<Country?> GetCountryByISOCode(string countryISOCode);
  Task<string> GetMapKeys();
  Task<List<Zone>> GetAllZonesWithCoords();
  Task<Zone?> GetZoneById(int zoneId);
  Task<bool> SaveNewZone(Zone zone);
  Task<bool> UpdateZoneFromZoneBoundries(string zoneName, int zoneID, int cityID);
  Task<bool> DeleteZoneByID(int zoneID);
}

