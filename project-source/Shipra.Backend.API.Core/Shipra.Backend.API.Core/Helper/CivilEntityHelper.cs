using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.Helper;
public static class CivilEntityHelper
{
  public static Dictionary<string, object> ConvertCivilEntityToJson(string propertyName, string value)
  {
    int entityId;
    int bit = 0; // Default value

    if (value.Contains("_"))
    {
      string[] parts = value.Split('_');
      entityId = int.Parse(parts[0]);
      bit = int.Parse(parts[1]);
    }
    else
    {
      entityId = int.Parse(value);
    }

    EnumCivilEntityType entityType = GetEntityTypeWithPropertyName(propertyName);

    return new Dictionary<string, object>
        {
            { UtilityHelper.ToCamelCase(propertyName), entityId },
            { "entityId", bit },
            { "entityTypeId", (int)entityType }
        };
  }

  public static void AddToJsonList(List<Dictionary<string, object>> jsonList, string propertyName, string? value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      var json = ConvertCivilEntityToJson(propertyName, value);
      if (json != null)
      {
        jsonList.Add(json);
      }
    }
  }
  public static EnumCivilEntityType GetEntityTypeWithPropertyName(string propertyName)
  {
    return propertyName switch
    {
      "CityId" => EnumCivilEntityType.City,
      "AreaId" => EnumCivilEntityType.Area,
      "ProvinceId" => EnumCivilEntityType.Province,
      "StateId" => EnumCivilEntityType.State,
      "PinCodeId" => EnumCivilEntityType.PinCode,
      "StreetAddress" => EnumCivilEntityType.StreetAddress,
      _ => throw new ArgumentException($"Invalid property name: {propertyName}")
    };
  }

  public static EnumCivilEntityType? GetEntityTypeFromName(string name)
  {
    if (System.Enum.TryParse(name, true, out EnumCivilEntityType entityType))
    {
      return entityType;
    }
    return null; // Return null if not found
  }
  public static string? GetEntityKey(EnumCivilEntityType entityType)
  {
    var entityTypeMapping = GetEntityTypeTableStrucreMap();

    return entityTypeMapping.TryGetValue(entityType, out string? expectedKey) ? expectedKey : null;
  }
  public static Dictionary<EnumCivilEntityType, string> GetEntityTypeTableStrucreMap()
  {
    return new Dictionary<EnumCivilEntityType, string>
    {
        { EnumCivilEntityType.City, "CityId" },
        { EnumCivilEntityType.Area, "AreaId" },
        { EnumCivilEntityType.Province, "ProvinceId" },
        { EnumCivilEntityType.State, "StateId" },
        { EnumCivilEntityType.PinCode, "PinCodeId" },
        { EnumCivilEntityType.StreetAddress, "StreetAddress" },
    };
  }
  public static Dictionary<string, EnumCivilEntityType> GetEntityTypeTableEnumKeyStructure()
  {
    // Mapping dictionary for EnumCivilEntityType from keys
    var keyToEnumMapping = new Dictionary<string, EnumCivilEntityType>(StringComparer.OrdinalIgnoreCase)
        {
            { "country", EnumCivilEntityType.Country },
            { "city", EnumCivilEntityType.City },
            { "area", EnumCivilEntityType.Area },
            { "province", EnumCivilEntityType.Province },
            { "state", EnumCivilEntityType.State },
            { "pincode", EnumCivilEntityType.PinCode },
            { "streetAddress", EnumCivilEntityType.StreetAddress }
        };

    return keyToEnumMapping;
  }


  public static int GetCivilEntityTypeIdFromKey(string key)
  {
    // Ensure the key is trimmed and converted to lowercase 
    if (string.IsNullOrEmpty(key))
    {
      return 0;
    }
    key = key?.Trim().ToLower()!;

    // Map the key to the corresponding Enum value
    EnumCivilEntityType entityType = key switch
    {
      "city" => EnumCivilEntityType.City,
      "area" => EnumCivilEntityType.Area,
      "province" => EnumCivilEntityType.Province,
      "state" => EnumCivilEntityType.State,
      "pincode" => EnumCivilEntityType.PinCode,
      "streetaddress" => EnumCivilEntityType.StreetAddress,
      _ => throw new ArgumentException($"Invalid key: {key}")
    };

    return (int)entityType;
  }
  public static string GetTableEntityPropertyFromKey(string key, bool useCamelCase = true) // never change anything from this function
  {
    // Map the key to the corresponding Enum value
    EnumCivilEntityType entityType = key.ToLower() switch
    {
      "city" => EnumCivilEntityType.City,
      "area" => EnumCivilEntityType.Area,
      "province" => EnumCivilEntityType.Province,
      "state" => EnumCivilEntityType.State,
      "pincode" => EnumCivilEntityType.PinCode,
      "streetAddress" => EnumCivilEntityType.StreetAddress,
      _ => throw new ArgumentException($"Invalid key: {key}")
    };

    // Get the property name based on the Enum value
    string propertyName = entityType switch
    {
      EnumCivilEntityType.City => "CityId",
      EnumCivilEntityType.Area => "AreaId",
      EnumCivilEntityType.Province => "ProvinceId",
      EnumCivilEntityType.State => "StateId",
      EnumCivilEntityType.PinCode => "PinCodeId",
      EnumCivilEntityType.StreetAddress => "StreetAddress",
      _ => throw new ArgumentException($"Invalid entity type: {entityType}")
    };

    // If camelCase is requested, convert the first letter to lowercase
    if (useCamelCase && propertyName.Length > 0)
    {
      return char.ToLower(propertyName[0]) + propertyName.Substring(1);
    }

    return propertyName;
  }

  public static OrderJsonEntityTypeModifiedModel? GetOrderJsonEntityValueByKey(string json, string key)
  {
    var columnProperty = GetTableEntityPropertyFromKey(key);

    // Deserialize the JSON string to a JArray (array of JSON objects)
    JArray jsonArray = JArray.Parse(json);

    // Iterate through the array and look for the columnProperty in each object
    foreach (JObject obj in jsonArray)
    {
      // Check if the columnProperty exists in the current object
      if (obj.ContainsKey(columnProperty))
      {
        // Create the OrderJsonEntityTypeModifiedModel object and populate it with the data
        return new OrderJsonEntityTypeModifiedModel
        {
          // Assuming the columnProperty corresponds to the value you're looking for
          Value = obj[columnProperty]?.ToObject<int>(), // Convert the value to integer if exists
          CivilEntityTypeId = obj["entityTypeId"]?.ToObject<int>(), // Get the entityTypeId from the JSON
          CivilEntityExtendedId = obj["entityId"]?.ToObject<int>() ?? 0 // Get the entityId from the JSON, default to 0 if null
        };
      }
    }

    return null; // Return null if the columnProperty doesn't exist
  }
  // Function to generate queries based on the provided JSON

  #region address maping field
  public static void EnsureDefaultEntityMappings<T>(Dictionary<EnumCivilEntityType, string> mappings, T entity) where T : class
  {
    var entityTypeMap = GetEntityTypeTableStrucreMap();

    foreach (var kvp in entityTypeMap)
    {
      if (!mappings.ContainsKey(kvp.Key))
      {
        var property = typeof(T).GetProperty(kvp.Value);
        if (property != null)
        {
          var value = property.GetValue(entity) as int?;
          if (value.HasValue && value > 0)
          {
            mappings[kvp.Key] = $"{value.Value}_0";
          }
        }
      }
    }
  }
  public static Dictionary<EnumCivilEntityType, string> GetEntityMappings(string? entityAddressDataJson)
  {
    if (string.IsNullOrEmpty(entityAddressDataJson))
      return new Dictionary<EnumCivilEntityType, string>();

    var entities = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(entityAddressDataJson);
    var entityMappings = new Dictionary<EnumCivilEntityType, string>();

    if (entities != null)
    {
      foreach (var entity in entities)
      {
        if (entity.TryGetValue("entityId", out int entityId) &&
            entity.TryGetValue("entityTypeId", out int entityTypeId))
        {
          if (System.Enum.IsDefined(typeof(EnumCivilEntityType), entityTypeId))
          {
            var entityType = (EnumCivilEntityType)entityTypeId;

            // Get the first key that isn't "entityId" or "entityTypeId" (e.g., "cityId", "areaId")
            var propertyKey = entity.Keys.FirstOrDefault(k => k != "entityId" && k != "entityTypeId");

            if (propertyKey != null && entity.TryGetValue(propertyKey, out int entityValue))
            {
              entityMappings[entityType] = $"{entityValue}_{entityId}";
            }
          }
        }
      }
    }

    return entityMappings;
  }
  public static Dictionary<string, bool> ValidateEntityMappings(Dictionary<string, string> entityMappings)
  {
    var validationResults = new Dictionary<string, bool>();

    foreach (var mapping in entityMappings)
    {
      string propertyName = mapping.Key;
      string value = mapping.Value;

      bool isValid = false; // Default to invalid

      var parts = value.Split('_');
      if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
      {
        if (x == 0 && y > 0)
        {
          isValid = true; // Valid if first part is 0 and second part is > 0
        }
      }

      validationResults[propertyName] = !isValid; // true = Invalid, false = Valid
    }

    return validationResults;
  }

  public static string? GetEntityValue(Dictionary<EnumCivilEntityType, string> mappings, EnumCivilEntityType entityType)
  {
    return mappings.TryGetValue(entityType, out string? value) ? value : null;
  }
  #endregion
  public static bool CheckRequiredKeyInCountryAddressingShceck(string json, string key)
  {
    var jObject = JObject.Parse(json);

    if (jObject.ContainsKey(key) && jObject[key] is JObject obj && obj.ContainsKey("required"))
    {
      return obj["required"]?.ToObject<bool>() ?? false;
    }

    return false;
  }
  // Ensures default values when JSON is null
  public static void EnsureDefaultEntityMappings(Dictionary<EnumCivilEntityType, string> mappings, OrderAddress orderAddress)
  {
    if (!mappings.ContainsKey(EnumCivilEntityType.City) && orderAddress.CityId.HasValue && orderAddress.CityId > 0)
      mappings[EnumCivilEntityType.City] = $"{orderAddress.CityId}_0";

    if (!mappings.ContainsKey(EnumCivilEntityType.Area) && orderAddress.AreaId.HasValue && orderAddress.AreaId > 0)
      mappings[EnumCivilEntityType.Area] = $"{orderAddress.AreaId}_0";

    if (!mappings.ContainsKey(EnumCivilEntityType.Province) && orderAddress.ProvinceId.HasValue && orderAddress.ProvinceId > 0)
      mappings[EnumCivilEntityType.Province] = $"{orderAddress.ProvinceId}_0";

    if (!mappings.ContainsKey(EnumCivilEntityType.State) && orderAddress.StateId.HasValue && orderAddress.StateId > 0)
      mappings[EnumCivilEntityType.State] = $"{orderAddress.StateId}_0";

    if (!mappings.ContainsKey(EnumCivilEntityType.PinCode) && orderAddress.PinCodeId.HasValue && orderAddress.PinCodeId > 0)
      mappings[EnumCivilEntityType.PinCode] = $"{orderAddress.PinCodeId}_0";
  }
  // Retrieves entity value from mappings
  #region get enum proprty from entity
  public static Dictionary<string, long?> GetEnumValueFromAddressEntityMap<T>(T entity)
  {
    var result = new Dictionary<string, long?>(StringComparer.OrdinalIgnoreCase);

    if (entity == null)
      return result;

    foreach (EnumCivilEntityType type in System.Enum.GetValues(typeof(EnumCivilEntityType)))
    {
      string baseName = type.ToString();   // e.g., "City"
      string propertyName = baseName + "Id"; // e.g., "CityId"

      var prop = typeof(T)
          .GetProperties()
          .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

      if (prop != null)
      {
        var value = prop.GetValue(entity);

        if (value is long longValue)
        {
          if (longValue > 0) // only positive numbers
          {
            result[baseName.ToLower()] = longValue;
          }
        }
        else if (value is int intValue)
        {
          if (intValue > 0)
          {
            result[baseName.ToLower()] = intValue;
          }
        }
        else if (value is string s && long.TryParse(s, out var parsed) && parsed > 0)
        {
          result[baseName.ToLower()] = parsed;
        }
      }
    }

    return result;
  }

  private static readonly int[] OriginPriority =
{
    (int)EnumCivilEntityType.PinCode,
    (int)EnumCivilEntityType.Area,
    (int)EnumCivilEntityType.City,
    (int)EnumCivilEntityType.State,
    (int)EnumCivilEntityType.Province,
    (int)EnumCivilEntityType.Country
};
  
  private static bool TryGetMapValue(Dictionary<string, long?> map, EnumCivilEntityType type, out int value)
  {
    value = 0;

    // Accept both formats: "city" and "CityId"
    var candidates = type switch
    {
      EnumCivilEntityType.Country => new[] { "country", "CountryId" },
      EnumCivilEntityType.Province => new[] { "province", "ProvinceId" },
      EnumCivilEntityType.State => new[] { "state", "StateId" },
      EnumCivilEntityType.City => new[] { "city", "CityId" },
      EnumCivilEntityType.Area => new[] { "area", "AreaId" },
      EnumCivilEntityType.PinCode => new[] { "pincode", "PinCodeId", "PincodeId" },
      _ => Array.Empty<string>()
    };

    foreach (var k in candidates)
    {
      if (map.TryGetValue(k, out var v) && v.HasValue && v.Value > 0)
      {
        value = (int)v.Value;
        return true;
      }
    }

    return false;
  }

  public static (int originTypeId, int from, int to)? ResolveFromToByAvailableServiceGroups(
      Dictionary<string, long?> fromMap,
      Dictionary<string, long?> toMap,
      HashSet<int> availableOriginTypes)
  {
    // strict pass: must exist in availableOriginTypes
    foreach (var ot in OriginPriority)
    {
      if (!availableOriginTypes.Contains(ot)) continue;

      var type = (EnumCivilEntityType)ot;

      if (TryGetMapValue(fromMap, type, out var from) &&
          TryGetMapValue(toMap, type, out var to))
      {
        return (ot, from, to);
      }
    }

    // fallback: best common level
    foreach (var ot in OriginPriority)
    {
      var type = (EnumCivilEntityType)ot;

      if (TryGetMapValue(fromMap, type, out var from) &&
          TryGetMapValue(toMap, type, out var to))
      {
        return (ot, from, to);
      }
    }

    return null;
  }


  #endregion
}
