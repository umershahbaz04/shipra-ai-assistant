
using Nancy.Extensions;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Common.Helpers;
public class Utils
{
  public static Dictionary<string, string> ConvertKeysToCamelCase(
    Dictionary<string, string> dictionaries)
  {
    var convertedDictionatry = new Dictionary<string, string>();
    foreach (string key in dictionaries.Keys)
    {
      convertedDictionatry.Add(key.ToCamelCase(), dictionaries[key]);
    }
    return convertedDictionatry;
  }
  public static string GetValueFromDictionryByKey(string key, Dictionary<string, string>? dict)
  {
    return dict!.ContainsKey(key) ? dict[key] : "";
  }  
  public static string GetValueFromDictionryByKey(string key, Dictionary<string, string[]>? dict)
  {
    if (dict == null)
    {
      return "";
    }
    return dict!.ContainsKey(key) ? string.Join(",",dict[key]) : "";
  } 
  public static bool CheckKeyExistFromDictionryByKey(string key, Dictionary<string, string>? dict)
  {
    return dict!.ContainsKey(key) ? true : false;
  }
  public static string GetValueFromDictionryByKey(string key, Dictionary<string, object>? dict)
  {
    if (dict == null)
      return "";

    return dict.ContainsKey(key) ? dict[key]?.ToString() ?? "" : "";
  }

  public static string ImageToBase64(string imagePath)
  {
    byte[] imageBytes = File.ReadAllBytes(imagePath);
    string base64String = Convert.ToBase64String(imageBytes);
    return base64String;
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
}
