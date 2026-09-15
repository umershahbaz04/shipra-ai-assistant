using System.Reflection;
using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.Extensions.Logging;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.HelperOrder.UDTOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands;
using Shipra.Backend.API.Application.Features.PaymentProcessFeature.Query.GetStripeWebhook;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate; 
using static Shipra.Backend.API.Application.Features.OrderFeatures.Query.ValidatedOrderAddressForCarrier.ValidateOrderAddressForCarrierQueryHandler;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetValidatedOrderAddressByActiveCarrier;
public class GetValidatedOrderAddressByActiveCarrierQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
  public int CarrierId { get; set; }
  public int? ActiveCarrierId { get; set; }
}
public class GetValidatedOrderAddressByActiveCarrierQueryHandelr : RequestHandlerBase<GetValidatedOrderAddressByActiveCarrierQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;
  private readonly ICountryRepository _countryRepository;
  private readonly IStoreRepository _storeRepository;
  private readonly IOrderRepository _orderRepository;

  public GetValidatedOrderAddressByActiveCarrierQueryHandelr(ICarrierRepository carrierRepository, ICountryRepository countryRepository, IStoreRepository storeRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetValidatedOrderAddressByActiveCarrierQueryHandelr> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
    _countryRepository = countryRepository;
    _storeRepository = storeRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetValidatedOrderAddressByActiveCarrierQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      List<Country> countries = await _countryRepository.GetAllCacheCountries();
      List<CivilEntityExtended> civilEntityCarrierMapped = await _countryRepository.GetAllCacheCivilEntityCarrierMapped();
      List<CivilEntityExtended> filterdCivilEntityCarrier = civilEntityCarrierMapped.Where(x => x.CarrierId == request.CarrierId).ToList();

      List<OrderValidationResult> validationResults = new List<OrderValidationResult>();
      var carrier = await _carrierRepository.GetCarrierFromMasterDbById(request.CarrierId);

      CarrierLocation carrierLocation = await _carrierRepository.GetCarrierLocationByCarrier(carrier!);

      foreach (var oNO in request.OrderNos!.Split(","))
      {
        Dictionary<string, bool> validationResultsEntity = new();

        var order = await _orderRepository.GetOrderByOrderNo(oNO, _currentUser.ClientId!);
        if (order is null)
        {
          //throw new EntityNotFoundException("Order", oNO!);
        }
        string storeAddress = string.Empty;
        var store = await _storeRepository.GetStoreById(order!.StoreId.GetValueOrDefault(), _currentUser.ClientId!);
        var oAddress = await _orderRepository.GetOrderAddressById(order!.OrderAddressId.GetValueOrDefault());

        var country = countries.FirstOrDefault(x => x.CountryId == oAddress.CountryId);
        if (country is null)
        {
          Logger.LogWarning($"GetValidatedOrderAddress: OrderNo {oNO} skipped because country with Id {oAddress.CountryId} was not found in countries cache. Cache count: {countries.Count}. Cache country IDs: {string.Join(",", countries.Select(c => c.CountryId))}");
          continue;
        }
        var orderValidationResult = new OrderValidationAddressResult { Country = country.CountryId };
        if (carrier is not null && carrier.ValidateAddress.GetValueOrDefault())
        {
          #region testing 
          Dictionary<string, List<CivilEntityExtended>?> civilEntityConfigWithKeysData = new();

          List<string> keyList = OrderCommon.ExtractCarrierLocationKeys(carrierLocation.AddressingScheme!);
          if (keyList.Count == 0)
          {
            var counrtyKeyList = JsonConvert.DeserializeObject<CountryAddressSchema>(country.AddressingScheme!);
            keyList = counrtyKeyList!.keys!;
          } 

          var addressModel = new OrderAddressValidateAgainstCarrierWithNameModel();
          // Populate the model
          PopulateOrderAddressValidateModel(oAddress, addressModel, keyList);
          addressModel.StreetAddress = oAddress.StreetAddress;


          Dictionary<string, string?> countryConfigKeys = OrderCommon.GetPropertyValues(addressModel, keyList);


          foreach (var keyValue in countryConfigKeys)
          {
            string propertyName = keyValue.Key;
            string? propertyValue = keyValue.Value;

            string currentKeyParent = OrderCommon.GetParentKey(country.AddressingScheme!, propertyName!)!;
            string requiredParentKey = OrderCommon.GetRequiredParentKey(country.AddressingScheme!, propertyName);

            OrderCommon.AddCivilEntityData(requiredParentKey, country.CountryId, civilEntityCarrierMapped, civilEntityConfigWithKeysData);
            var typeId = CivilEntityHelper.GetCivilEntityTypeIdFromKey(propertyName);
            var parentIds = OrderCommon.GetCivilEntityValueByKey(civilEntityConfigWithKeysData, currentKeyParent)
                            ?.Select(c => c.CivilEntityExtendedId)
                            .ToList() ?? new List<long>();

            if (!carrier!.IsDispatchExCompany.GetValueOrDefault())
            {
              CivilEntityExtended? matchedEntity = null;

              long firstValue = 0, secondValue = 0;

              if (!string.IsNullOrEmpty(propertyValue))
              {
                string[] parts = propertyValue.Split('_');
                firstValue = long.TryParse(parts.ElementAtOrDefault(0), out long first) ? first : 0;
                secondValue = long.TryParse(parts.ElementAtOrDefault(1), out long second) ? second : 0;
              }

              if (firstValue > 0)
              {
                // When second value is 0, check against MappedId
                matchedEntity = filterdCivilEntityCarrier.FirstOrDefault(entity => entity.MappedId == firstValue &&
                                    entity.CivilEntityTypeId == typeId &&
                                    parentIds.Contains(entity.ParentId ?? -1));
              }
              else
              {
                // When second value > 0, check against CivilEntityExtendedId
                matchedEntity = filterdCivilEntityCarrier.FirstOrDefault(entity =>
                                   entity.CivilEntityExtendedId == secondValue &&
                                   entity.CivilEntityTypeId == typeId &&
                                   parentIds.Contains(entity.ParentId ?? -1));
              }


              if (!string.IsNullOrEmpty(carrierLocation.AddressingScheme))
              { 
                var selectedIds = string.Join(",", parentIds);
                List<AddressCommonLookupModel> data = await _countryRepository.GetAddressEntitiesByType(
                    selectedIds, requiredParentKey, propertyName, country.CountryId, request.CarrierId);

                OrderCommon.AddAddressEntities(propertyName, data, civilEntityCarrierMapped, civilEntityConfigWithKeysData);

                bool isCountryRequired = CivilEntityHelper.CheckRequiredKeyInCountryAddressingShceck(country.AddressingScheme!, propertyName);
                bool isCarrierRequired = OrderCommon.IsKeyRequired(carrierLocation.AddressingScheme, propertyName); 
                if ((isCountryRequired || isCarrierRequired) && matchedEntity is null)
                {
                  if (propertyName == "streetAddress" && !string.IsNullOrEmpty(addressModel.StreetAddress))
                  {
                    OrderCommon.SetProperty(orderValidationResult, propertyName, addressModel.StreetAddress); 
                  }
                  else
                  {
                    validationResultsEntity[propertyName] = true; 
                  }
                }
                else if (matchedEntity != null)
                {
                  OrderCommon.SetProperty(orderValidationResult, propertyName, $"{matchedEntity.MappedId}_{matchedEntity.CivilEntityExtendedId}");
                }
              }
            }

          }


          #endregion
        }
        var entityMappings = CivilEntityHelper.GetEntityMappings(oAddress.EntityAddressDataJson);
        if (store != null)
        {
          var storeAdd = await _storeRepository.GetStoreAddressById(store.StoreId);
          if (storeAdd != null)
          {
            storeAddress = storeAdd.FullAddress!;
          }
        }

        // Ensure default values when JSON is null or does not contain a mapping
        if (string.IsNullOrEmpty(oAddress.EntityAddressDataJson))
        {
          CivilEntityHelper.EnsureDefaultEntityMappings(entityMappings, oAddress);
        }

        bool isValidAddress = validationResultsEntity.Keys.Count == 0;

        validationResults.Add(new OrderValidationResult
        {
          OrderId = order.OrderId!.Value.ToString(),
          OrderNo = oNO,
          IsValidAddress = isValidAddress,
          OrderAddress = oAddress.CustomerFullAddress,
          StoreAddress = storeAddress,
          InvalidProperties = validationResultsEntity.Keys.ToList(),
          InvalidPropertiesWithData = validationResultsEntity,
          OrderTypeId = order.OrderTypeId,
          Address = orderValidationResult

        });
      }


      // Now `validationResults` contains the list of validated orders

      //throw error 
      serviceResult = new ServiceResultDTO(validationResults);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  public class OrderValidationResult
  {
    public string? OrderId { get; set; }
    public string? OrderNo { get; set; }
    public bool IsValidAddress { get; set; }
    public string? StoreAddress { get; set; }
    public string? OrderAddress { get; set; }
    public List<string> InvalidProperties { get; set; } = new List<string>();
    public Dictionary<string, bool> InvalidPropertiesWithData = new();
    public int? OrderTypeId { get; set; }
    public OrderValidationAddressResult? Address { get; set; }
  }
  public class CarrierMappedCivilEntity
  {
    [JsonExtensionData]
    public Dictionary<string, JToken> Entity { get; set; } = new();
    public bool Required { get; set; }
  }

  public class CarrierMappedEntityRoot
  {
    public List<CarrierMappedCivilEntity> MappedCivilEntity { get; set; } = new();
  }

  public Dictionary<string, string?> GetMatchedKeyValues(OrderAddress orderAddress, OrderAddressValidateAgainstCarrierWithNameModel addressModel, List<string> keysToMatch)
  {
    Dictionary<string, string?> matchedValues = new();

    // Get mapping dictionary
    var entityMapping = CivilEntityHelper.GetEntityTypeTableStrucreMap();

    // Iterate over the keys to match (e.g., "city", "area")
    foreach (var key in keysToMatch)
    {
      // Find corresponding property in OrderAddressValidateAgainstCarrierWithNameModel
      var modelProperty = typeof(OrderAddressValidateAgainstCarrierWithNameModel)
                          .GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

      // If the property exists and has a value
      if (modelProperty != null && modelProperty.GetValue(addressModel) is string propertyValue && !string.IsNullOrEmpty(propertyValue))
      {
        // Find the enum key in the dictionary that matches the property name
        var matchingKey = entityMapping.FirstOrDefault(e => e.Value.StartsWith(key, StringComparison.OrdinalIgnoreCase));

        if (!matchingKey.Equals(default(KeyValuePair<EnumCivilEntityType, string>)))
        {
          // Fetch the corresponding OrderAddress property dynamically
          var orderProperty = typeof(OrderAddress).GetProperty(matchingKey.Value, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

          if (orderProperty != null)
          {
            var propertyValueInOrder = orderProperty.GetValue(orderAddress);
            if (propertyValueInOrder is int matchedId)
            {
              matchedValues[key] = $"0_{matchedId}";
            }
          }
        }
      }
    }

    return matchedValues;
  }


  public void PopulateOrderAddressValidateModel(OrderAddress orderAddress,
                                                OrderAddressValidateAgainstCarrierWithNameModel addressModel,
                                                List<string> keysToMatch)
  {
    // Get mapping dictionary
    var entityMapping = CivilEntityHelper.GetEntityTypeTableStrucreMap();

    // Deserialize JSON if it exists
    List<dynamic>? entityAddressData = null;
    if (!string.IsNullOrEmpty(orderAddress.EntityAddressDataJson))
    {
      entityAddressData = JsonConvert.DeserializeObject<List<dynamic>>(orderAddress.EntityAddressDataJson);
    }

    // Iterate over the keys to match (e.g., "city", "area")
    foreach (var key in keysToMatch)
    {
      // Find corresponding property in OrderAddressValidateAgainstCarrierWithNameModel
      var modelProperty = typeof(OrderAddressValidateAgainstCarrierWithNameModel)
                          .GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

      if (modelProperty != null)
      {
        string? mappedOrderAddressKey = entityMapping!
            .Where(e => e.Value.StartsWith(key, StringComparison.OrdinalIgnoreCase))
            .Select(e => e.Value)
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(mappedOrderAddressKey))
        {
          string? matchedValue = null;

          // Try to get value from EntityAddressDataJson first
          if (entityAddressData != null)
          {
            var jsonMatch = entityAddressData.FirstOrDefault(e =>
               ((JObject)e).Properties().Any(p => p.Name.Equals(mappedOrderAddressKey, StringComparison.OrdinalIgnoreCase))
           );

            if (jsonMatch != null)
            {
              matchedValue = $"0_{jsonMatch["entityId"].ToString()}";
            }
          }

          // If no match found in JSON, fallback to OrderAddress properties
          if (matchedValue == null)
          {
            var orderProperty = typeof(OrderAddress).GetProperty(mappedOrderAddressKey, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (orderProperty != null)
            {
              var orderPropertyValue = orderProperty.GetValue(orderAddress);
              if (orderPropertyValue != null)
              {
                matchedValue = $"0_{orderPropertyValue.ToString()}";
              }
            }
          }

          // Assign formatted value if found
          if (!string.IsNullOrEmpty(matchedValue))
          {
            modelProperty.SetValue(addressModel, matchedValue);
          }
        }
      }
    }
  }
}
