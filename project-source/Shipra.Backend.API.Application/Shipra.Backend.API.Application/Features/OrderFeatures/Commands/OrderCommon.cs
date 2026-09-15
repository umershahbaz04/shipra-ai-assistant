using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using static Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetValidatedOrderAddressByActiveCarrier.GetValidatedOrderAddressByActiveCarrierQueryHandelr;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
public class OrderCommon
{
  public static string GetDescriptionForShipmentItem(Product? p, ProductStock ps)
  {
    StringBuilder sbf = new StringBuilder($"Name :{p!.ProductName} SKU: {ps.Sku} - ");

    if (p!.HaveOptions.HasValue)
    {
      sbf.Append($"{ps.VarientOption} - ");
    }
    sbf.Append("Q2");
    return sbf.ToString();
  }

  public static string GetDescriptionForShipmentItem(Product? p, ProductVariant pv)
  {
    StringBuilder sbf = new StringBuilder($"Name :{p!.ProductName} SKU: {pv.SKU} - ");

    if (p!.HaveOptions.HasValue)
    {
      sbf.Append($"{pv.VariantOptionText} - ");
    }
    sbf.Append("Q2");
    return sbf.ToString();
  }

  public static string GetDescriptionForShopifyOrderItems(List<ShopifySharp.LineItem> lineItems)
  {
    StringBuilder productDes = new StringBuilder();
    foreach (var item in lineItems)
    {
      productDes.AppendLine($"Name :{item.Name} - SKU: {item.SKU} - QTY: {item.Quantity}");
    }
    return productDes.ToString();
  }
  public static string GetDescriptionForShopifyOrderItem(ShopifySharp.LineItem item)
  {
    StringBuilder productDes = new StringBuilder();
    productDes.AppendLine($"Name :{item.Name} - SKU: {item.SKU} - QTY: {item.Quantity}");

    return productDes.ToString();
  }

  public static string GetDescriptionForWooCommerceOrderItems(List<Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece.LineItem> lineItems)
  {
    StringBuilder productDes = new StringBuilder();
    foreach (var item in lineItems)
    {
      productDes.AppendLine($"Name :{item.Name} - SKU: {item.SKU} - QTY: {item.Quantity}");
    }
    return productDes.ToString();
  }

  public static Dictionary<string, bool> ValidateEntityMappings(
  List<Country> countries,
  OrderAddress orderAddress,
  Dictionary<EnumCivilEntityType, string> entityMappings,
  List<CivilEntityExtended> filteredCivilEntityCarrier,
  Carrier carrier,
  CarrierLocation carrierLocation)
  {
    // Get country keys
    var countryConfigKeys = new List<string>();

    var country = countries?.FirstOrDefault(x => x.CountryId == orderAddress.CountryId);
    if (country is not null)
    {
      var form = JsonConvert.DeserializeObject<CountryMainForm>(country!.AddressingScheme!);
      countryConfigKeys = form?.Keys ?? new List<string>();
    }

    // Ensure all countryKeys exist in entityMappings
    if (countryConfigKeys != null)
    {
      foreach (var key in countryConfigKeys)
      {
        if (CivilEntityHelper.GetEntityTypeTableEnumKeyStructure().TryGetValue(key, out var enumValue))
        {
          if (!entityMappings.ContainsKey(enumValue))
          {
            entityMappings[enumValue] = "0_0"; // Default value
          }
        }
      }
    }

    var entityMappingsStringKey = entityMappings.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
    Dictionary<string, bool> validationResultsEntity = new Dictionary<string, bool>();

    // Validate against CivilEntityExtended
    foreach (var mapping in entityMappingsStringKey)
    {
      string propertyName = mapping.Key;
      string[] parts = mapping.Value.Split('_');

      if (parts.Length == 2 && int.TryParse(parts[0], out int firstValue) && int.TryParse(parts[1], out int secondValue))
      {
        if (!carrier!.IsDispatchExCompany.GetValueOrDefault())
        {
          CivilEntityExtended? matchedEntity = null;

          if (secondValue > 0)
          {
            // When second value > 0, check against CivilEntityExtendedId
            matchedEntity = filteredCivilEntityCarrier.FirstOrDefault(x => x.CivilEntityExtendedId == secondValue);
          }
          else if (firstValue > 0)
          {
            // When second value is 0, check against MappedId
            matchedEntity = filteredCivilEntityCarrier.FirstOrDefault(x => x.MappedId == firstValue);
          }

          if (!string.IsNullOrEmpty(carrierLocation.AddressingScheme))
          {
            var isRequired = IsKeyRequired(carrierLocation.AddressingScheme, propertyName);
            if (isRequired && matchedEntity is null)
            {
              validationResultsEntity[propertyName] = true; // Property is invalid
            }
          }
        }
      }
    }

    return validationResultsEntity;
  }
  public static bool IsKeyRequired(string json, string keyToCheck)
  {
    if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(keyToCheck))
      return false;

    var obj = JsonConvert.DeserializeObject<CarrierMappedEntityRoot>(json);
    if (obj?.MappedCivilEntity == null || obj.MappedCivilEntity.Count == 0)
      return false;

    return obj.MappedCivilEntity.Any(entity =>
        entity?.Entity != null &&
        entity.Entity.Keys.Any(k => k.Equals(keyToCheck, StringComparison.OrdinalIgnoreCase)) &&
        entity.Required);
  }


  #region order validation for thirdparty
  public static void AddAddressEntities(string propertyName, List<AddressCommonLookupModel> data, List<CivilEntityExtended> civilEntityCarrierMapped,
   Dictionary<string, List<CivilEntityExtended>?> civilEntityConfigWithKeysData)
  {
    if (data == null || data.Count == 0) return;

    var ids = data
              .Select(x => x.Id?.ToString()?.Split('_').LastOrDefault())
              .Where(id => !string.IsNullOrEmpty(id))
              .Select(id => long.Parse(id!))
              .ToList();

    var newData = civilEntityCarrierMapped.Where(x => ids.Contains(x.CivilEntityExtendedId)).ToList();
    if (civilEntityConfigWithKeysData.ContainsKey(propertyName))
    {
      civilEntityConfigWithKeysData[propertyName]?.AddRange(newData);
    }
    else
    {
      civilEntityConfigWithKeysData[propertyName] = newData;
    }
  }

  public static void SetProperty<T>(T obj, string propertyName, object value)
  {
    var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    propertyInfo?.SetValue(obj, value);
  }

  public static void AddCivilEntityData(string requiredParentKey, long countryId, List<CivilEntityExtended> civilEntityCarrierMapped,
      Dictionary<string, List<CivilEntityExtended>?> civilEntityConfigWithKeysData)
  {
    if (requiredParentKey == "country")
    {
      var newData = civilEntityCarrierMapped.Where(x => x.CivilEntityExtendedId == countryId).ToList();
      if (civilEntityConfigWithKeysData.ContainsKey(requiredParentKey))
      {
        civilEntityConfigWithKeysData[requiredParentKey]?.AddRange(newData);
      }
      else
      {
        civilEntityConfigWithKeysData[requiredParentKey] = newData;
      }
    }
  }
  public static List<CivilEntityExtended>? GetCivilEntityValueByKey(
    Dictionary<string, List<CivilEntityExtended>?> civilEntityConfigWithKeysData,
    string key)
  {
    // Check if key is null or empty
    if (string.IsNullOrEmpty(key))
    {
      return null;  // or return an empty list depending on your preference
    }
    return civilEntityConfigWithKeysData.TryGetValue(key, out var value) ? value : null;
  }
  public static Dictionary<string, string?> GetPropertyValues<T>(T obj, List<string> keys)
  {
    var type = typeof(T);
    var result = new Dictionary<string, string?>();

    foreach (var key in keys)
    {
      // Find a matching property (case-insensitive)
      PropertyInfo? property = type.GetProperties()
                                   .FirstOrDefault(p => string.Equals(p.Name, key, StringComparison.OrdinalIgnoreCase));

      if (property != null)
      {
        var value = property.GetValue(obj)?.ToString();
        result[key] = value;
      }
    }
    return result;
  }
  public static string GetRequiredParentKey(string json, string key)
  {
    JObject obj = JObject.Parse(json);
    if (!obj.TryGetValue(key, out JToken? current) || current?.Type != JTokenType.Object)
      return "country";

    while (current?["parentKey"] != null)
    {
      string parentKey = current["parentKey"]?.ToString() ?? "";
      if (!obj.TryGetValue(parentKey, out JToken? parent) || parent?.Type != JTokenType.Object)
        break;

      if (parent["required"]?.ToObject<bool>() == true)
        return parentKey;

      current = parent;
    }
    return "country";
  }
  public static string? GetParentKey(string json, string key)
  {
    JObject obj = JObject.Parse(json);

    if (!obj.TryGetValue(key, out JToken? current) || current?.Type != JTokenType.Object)
      return null;

    return current["parentKey"]?.ToString();
  }
  public static List<string> ExtractCarrierLocationKeys(string json)
  {
    if (string.IsNullOrEmpty(json)) return new List<string>();

    try
    {
      var jObject = JObject.Parse(json);
      var mappedCivilEntities = jObject["mappedCivilEntity"] as JArray;

      if (mappedCivilEntities == null) return new List<string>();

      return mappedCivilEntities
          .OfType<JObject>()
          .SelectMany(obj => obj.Properties())
          .Where(prop => prop.Name != "required")
          .Select(prop => prop.Name)
          .Distinct()
          .ToList();
    }
    catch (Exception)
    {
      return new List<string>();
    }
  }
  #endregion
}
