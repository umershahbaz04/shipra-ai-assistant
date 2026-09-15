using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Infrastructure.Helpers;
public static class CacheKeys
{ 
  public const string AllCountries = "AllCountries";
  public const string AllProvinces = "AllProvinces";
  public const string AllStates = "AllStates";
  public const string AllCities = "AllCities";
  public const string UserPoolClientsCacheKey = "UserPoolClientsCacheKey";
  public const string AllPinCodes = "AllPinCodes";
  public const string AllAreas = "AllAreas";
  public const string AllCivilEntityCarrierMapped = "AllCivilEntityCarrierMappedAreas";
  public const string AllRegionTimeZones = "AllRegionTimeZones";
  public const string AllMapKeysInUse = "AllMapKeysInUse";
  public const string AllMapKeys = "AllMapKeys";
  public const string Catalogues = "Catalogues";
  public const string CatalogueDatabases = "CatalogueDatabases";
  // Add other cache keys here as needed
}
