using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Core.Helper;
public static class UtilityHelper
{
  public const int WeightDimensionalFactor = 5000;
  public static string RemoveAllSpaces(string input)
  {
    return string.IsNullOrEmpty(input)
        ? input
        : input.Replace(" ", "");
  }
  public static int GetMediaTypeId(IFormFile file)
  {
    if (file == null || string.IsNullOrWhiteSpace(file.ContentType))
      throw new ArgumentException("Invalid file.");

    var contentType = file.ContentType.ToLower();

    if (contentType.StartsWith("image/"))
      return (int)EnumMediaType.Image; // Image

    if (contentType.StartsWith("video/"))
      return (int)EnumMediaType.Video; // Video

    return (int)EnumMediaType.Other; // Other
  }

  public static decimal? TrimToPrecision(decimal? value, int totalDigits = 8, int decimalPlaces = 6) //8,6 for db saving
  {
    if (!value.HasValue)
      return null; // Return null if value is null

    decimal maxValue = (decimal)Math.Pow(10, totalDigits - decimalPlaces) - (decimal)Math.Pow(10, -decimalPlaces);
    decimal scale = (decimal)Math.Pow(10, decimalPlaces);

    decimal trimmedValue = Math.Truncate(value.Value * scale) / scale;

    // Ensure the trimmed value doesn't exceed the allowed max digits
    return trimmedValue > maxValue ? maxValue : trimmedValue;
  }
  public static string CleanPhoneNumber(string? input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return string.Empty;

    string noWhitespace = Regex.Replace(input, @"\s+", "");
    return noWhitespace.Replace("+", "00");
  }

  public static int GetColumnValueByKey<T>(T address, string key) where T : class
  {
    if (address == null)
      throw new ArgumentNullException(nameof(address), "Address object cannot be null");

    // Dictionary to map the key to the corresponding property name
    var keyColumnMapping = new Dictionary<string, Func<T, int>>
    {
        { "area", (addr) => GetPropertyValue(addr, "AreaId") },
        { "city", (addr) => GetPropertyValue(addr, "CityId") },
        { "province", (addr) => GetPropertyValue(addr, "ProvinceId") },
        { "country", (addr) => GetPropertyValue(addr, "CountryId") },
        { "state", (addr) => GetPropertyValue(addr, "StateId") },
        { "pinCode", (addr) => GetPropertyValue(addr, "PinCodeId") }
    };

    // Check if the provided key exists
    if (keyColumnMapping.ContainsKey(key.ToLower()))
    {
      return keyColumnMapping[key.ToLower()](address);
    }

    throw new ArgumentException("Invalid key provided.");
  }
  // Helper function to fetch property value dynamically
  private static int GetPropertyValue<T>(T obj, string propertyName)
  {
    var property = typeof(T).GetProperty(propertyName);
    if (property == null)
      throw new ArgumentException($"Property '{propertyName}' not found in {typeof(T).Name}");

    return (int)property.GetValue(obj)!;
  }
  public static string? GetEnumNameById<TEnum>(int id) where TEnum : System.Enum
  {
    return System.Enum.GetName(typeof(TEnum), id);
  }

  public static string GenerateEmployeeCode(string employeeName, bool toUpperCase = false, int charCount = 2)
  {
    if (string.IsNullOrWhiteSpace(employeeName))
      throw new ArgumentException("Employee name cannot be null or empty.");

    charCount = Math.Max(1, Math.Min(charCount, employeeName.Length));

    var code = employeeName.Substring(0, charCount);

    return toUpperCase ? code.ToUpper() : code.ToLower();
  }
  public static string ToCamelCase(string input)
  {
    if (string.IsNullOrEmpty(input))
      return input; 
    return char.ToLowerInvariant(input[0]) + input.Substring(1);
  }
  public static string UpdateJsonCarrierSettingConfigValue(string jsonString, string sectionKey, string key, string value = "")
  {
    if (!string.IsNullOrEmpty(jsonString))
    {

      if (string.IsNullOrEmpty(value))
      {
        value = string.Empty;
      }
      // Deserialize the JSON string to an array of StoreInfo objects 
      List<GeneralSettingConfigModel>? storeInfoArray = JsonConvert.DeserializeObject<List<GeneralSettingConfigModel>>(jsonString!);
      // Find the appropriate section and update the value
      foreach (var storeInfo in storeInfoArray!)
      {
        if (storeInfo!.Key!.Equals(sectionKey, StringComparison.OrdinalIgnoreCase))
        {
          foreach (var inputData in storeInfo.InputData!)
          {
            if (inputData!.Key!.Trim().ToLower() == key.Trim().ToLower())
            {
              inputData.Value = value;
              break;
            }
          }
        }
      }
      // Serialize the updated array back to a JSON string with camelCase property names
      var settings = new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };
      // Serialize the updated array back to a JSON string
      return JsonConvert.SerializeObject(storeInfoArray, settings);
    }
    return string.Empty;
  }
  public static string GetClientSettingValueWithByKey(string? clientSettingConfigJsonBody, string sectionKey, string inputValueKey)
  {
    string res = string.Empty;
    if (!string.IsNullOrEmpty(clientSettingConfigJsonBody))
    {

      List<GeneralSettingConfigModel>? oClientCarrierSettingConfigList = JsonConvert.DeserializeObject<List<GeneralSettingConfigModel>>(clientSettingConfigJsonBody!);

      if (!string.IsNullOrEmpty(sectionKey) && !string.IsNullOrEmpty(inputValueKey))
      {
        var section = oClientCarrierSettingConfigList!.FirstOrDefault(x => x.Key!.Trim().ToLower() == sectionKey!.Trim().ToLower());

        if (section is not null)
        {
          var selectedInput = section!.InputData!.FirstOrDefault(x => x.Key!.Trim().ToLower() == inputValueKey.Trim().ToLower());
          if (selectedInput is not null)
          {
            res = selectedInput!.Value!;
          }
        }
      }
    }

    return res;
  } 
  public static bool GetBoolFromString(string? value)
  {
    bool res = false;
    if (!string.IsNullOrEmpty(value))
    {
      res = value == "true" ? true : false;
    }

    return res;
  }
  #region get updated date
  public static string GetConfigUpdatedJson(string json1, string json2)
  {
    JArray array1 = JArray.Parse(json1);
    JArray array2 = JArray.Parse(json2);

    foreach (var section2 in array2)
    {
      // Trim and convert section names to lowercase for comparison
      string section2Name = section2!["sectionName"]!.ToString().Trim().ToLower();

      // Find corresponding section in array1
      var section1 = array1.FirstOrDefault(s =>
          s["sectionName"]!.ToString().Trim().ToLower() == section2Name);

      if (section1 != null)
      {
        UpdateSection(section1, section2);
      }
    }

    return array1.ToString(Formatting.Indented);
  }
  private static void UpdateSection(JToken section1, JToken section2)
  {
    var inputData1 = section1["inputData"] as JArray;
    var inputData2 = section2["inputData"] as JArray;

    if (inputData1 != null)
    {
      foreach (var input2 in inputData2!)
      {
        var input1 = inputData1.FirstOrDefault(i =>
            i != null &&
            i["key"]?.Value<string>() == input2["key"]?.Value<string>());

        if (input1 != null)
        {
          // Update existing input1 with input2's value
          input1["value"] = input2["value"];
        }
        else
        {
          // Add input2 to inputData1 if input1 is not found
          inputData1.Add(input2);
        }
      }
    }
  }
  #endregion
  public static int GetLeadingInteger(string input)
  {
    if (string.IsNullOrEmpty(input))
      return 0;

    var number = new string(input.TakeWhile(char.IsDigit).ToArray());

    return int.TryParse(number, out int result) ? result : 0;
  }
  public static string? ToBase64(object model)
  {
    if (model == null) return null;

    // Serialize the object to JSON
    string json = JsonConvert.SerializeObject(model);

    // Convert JSON to Base64
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
  }
  #region setting config
  public static string? GetSelectedConfigByKey(JArray settings, string key)
  {
    if (settings == null || string.IsNullOrWhiteSpace(key))
      return null;

    var token = settings.SelectToken($"$..InputData[?(@.Key == '{key}')].Value");
    return token?.ToString();
  }
  public static Dictionary<string, string> FlattenSettingConfigToDictionary(string? settingConfig)
  {
    var configDict = new Dictionary<string, string>();
    if (string.IsNullOrEmpty(settingConfig))
      return configDict;

    try
    {
      var settingsArray = JArray.Parse(settingConfig);
      foreach (var section in settingsArray)
      {
        var inputData = section["inputData"] as JArray;
        if (inputData != null)
        {
          foreach (var input in inputData)
          {
            var key = input["key"]?.ToString();
            var value = input["value"]?.ToString();
            if (!string.IsNullOrEmpty(key))
            {
              configDict[key] = value ?? string.Empty;
            }
          }
        }
      }
      return configDict;
    }
    catch
    {
      return configDict;
    }
  }
  public static List<string> GetSettingConfigByKey(JArray settings, string key, string arrayField = "Data")
  {
    if (settings == null || string.IsNullOrWhiteSpace(key))
      return new List<string>();

    var token = settings.SelectToken($"$..InputData[?(@.Key == '{key}')].{arrayField}");
    return token != null ? token.ToObject<List<string>>() ?? new List<string>() : new List<string>();
  }

  public static Random GetRandomNumber()
  {
    Random _random = new Random();
    return _random;
  }
  #endregion
}



public static class EnumHelper
{
  public static string? GetFormattedEnumName<TEnum>(int id, bool isSplit = false) where TEnum : System.Enum
  {
    if (!System.Enum.IsDefined(typeof(TEnum), id)) return null;

    string enumName = System.Enum.GetName(typeof(TEnum), id)!;
    enumName = string.IsNullOrEmpty(enumName) ? "Unknown" : enumName;
    return isSplit ? Regex.Replace(enumName, "([a-z])([A-Z])", "$1 $2") : enumName;
  } 
  public static string ToCamelCase(string input)
  {
    if (string.IsNullOrEmpty(input))
      return input;

    return char.ToLowerInvariant(input[0]) + input.Substring(1);
  }
   
}
