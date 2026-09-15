using System.Dynamic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase.Response;
using Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.MetaFieldAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderById;
public class GetOrderByIdQueryHandler : RequestHandlerBase<GetOrderByIdQuery, ServiceResultDTO>
{
  private readonly IOrderBoxRepository _orderBoxRepository;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IMetaFieldRepository _metaFieldRepository;
  private readonly ICountryRepository _countryRepository;
  public GetOrderByIdQueryHandler(IOrderBoxRepository orderBoxRepository, IProductRepository productRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, IMetaFieldRepository metaFieldRepository, ICountryRepository countryRepository, ILogger<GetOrderByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderBoxRepository = orderBoxRepository;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
    _metaFieldRepository = metaFieldRepository;
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var order = await _orderRepository.GetOrderById(new OrderId(new Guid(request.OrderId!)), _currentUser.ClientId!);
      if (order is null)
      {
        throw new EntityNotFoundException("Order ", request.OrderId!);
      }
      var oOrderItems = await _orderRepository.GetOrderItemsByOrderId(order?.OrderId);
      var oAddress = await _orderRepository.GetOrderAddressById(order!.OrderAddressId.GetValueOrDefault());
      var oNote = await _orderRepository.GetOrderNoteByOrderId(order?.OrderId!);
      var oOrderTax = await _orderRepository.GetAllOrderTaxByOrderId(order?.OrderId!);
      var orderBoxes = await _orderBoxRepository.GetOrderBoxsByOrderId(order?.OrderId!);

      var orderMap = _mapper.Map<OrderResponseModel>(order);
      var oItemMap = _mapper.Map<List<OrderItemModel>>(oOrderItems);

      var oAddressMap = await ConvertToResponseModelAsync(oAddress);

      var oNoteMap = _mapper.Map<OrderNoteModel>(oNote);
      var orderBoxesMap = _mapper.Map<List<OrderBoxesResponseModel>>(orderBoxes);
      var oOrderTaxMap = oOrderTax!.Select(x => new OrderTaxModel
      {
        OrderTaxId = x.OrderTaxId!.Value.ToString(),
        TaxValue = x.TaxValue,
        ClientTaxId = x.TaxId
      }).ToList();

      #region MetaField
      var clientmetafield = await _metaFieldRepository.GetClientMetaDataByClientId(_currentUser.ClientId!);
      var MetaFieldData = await _metaFieldRepository.GetMetaFieldDataByOrderId(order!.OrderId!.Value.ToString());
      var clientSchema = clientmetafield?.Select(c => JsonConvert.DeserializeObject<List<SettingConfigItem>>(c.SettingConfig!)).SelectMany(x => x!).ToList();
      var savedValues = string.IsNullOrEmpty(MetaFieldData?.SettingConfig)? new List<SettingConfigItem>(): JsonConvert.DeserializeObject<List<SettingConfigItem>>(MetaFieldData.SettingConfig!) ?? new List<SettingConfigItem>();
      var mergedSchema = clientSchema?.Select(c =>
      {
        var existing = savedValues?.FirstOrDefault(m => m.name == c.name);

        if (existing != null && existing.value != null)
        {
          // If it's a select type, try to deserialize into the same object
          if (c.type!.label!.Equals("select", StringComparison.OrdinalIgnoreCase))
          {
            try
            {
              // existing.value might already be an object or a stringified object
              if (existing.value is string stringVal && stringVal.TrimStart().StartsWith("{"))
              {
                c.value = JsonConvert.DeserializeObject<Dictionary<string, object>>(stringVal);
              }
              else
              {
                c.value = existing.value;
              }
            }
            catch
            {
              // fallback, just assign string
              c.value = existing.value;
            }
          }
          else
          {
            // For simple fields, just assign the value
            c.value = existing.value;
          }
        }

        return c;
      }).ToList();

      string mergedSettingConfigJson = JsonConvert.SerializeObject(mergedSchema);
      #endregion
      dynamic resData = new ExpandoObject();
      resData.order = orderMap;
      resData.orderItems = oItemMap;
      resData.orderAddress = oAddressMap;
      resData.orderNote = oNoteMap;
      resData.orderTax = oOrderTaxMap;
      resData.orderBoxes = orderBoxesMap;
      #region Metafield
      resData.metafields = new
      {
        metaFieldId = MetaFieldData?.MetaFieldId,
        clientId = MetaFieldData?.ClientId,
        entityId = MetaFieldData?.EntityId,
        createdOn = MetaFieldData?.CreatedOn,
        updatedOn = MetaFieldData?.UpdatedOn,
        settingConfig = mergedSettingConfigJson
      };
      #endregion
      serviceResult = new ServiceResultDTO(resData);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  public async Task<OrderAddressWithEntityResponseModel?> ConvertToResponseModelAsync(OrderAddress orderAddress)
  {
    if (orderAddress == null) return null;

    var entityMappings = CivilEntityHelper.GetEntityMappings(orderAddress.EntityAddressDataJson);
    // Ensure default values when JSON is null or does not contain a mapping
    if (string.IsNullOrEmpty(orderAddress.EntityAddressDataJson))
    {
      CivilEntityHelper.EnsureDefaultEntityMappings(entityMappings, orderAddress);
    }

    string? pinCodeValue = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.PinCode);

    if (orderAddress.CountryId.HasValue)
    {
      var country = await _countryRepository.GetCountryById(orderAddress.CountryId.Value);
      if (country != null && country.Name != null && country.Name.Equals("India", StringComparison.OrdinalIgnoreCase))
      {
        if (!string.IsNullOrEmpty(pinCodeValue))
        {
          var parts = pinCodeValue.Split('_');
          if (parts.Length > 0 && int.TryParse(parts[0], out int pinCodeId) && pinCodeId > 0)
          {
            var actualPinCode = await _countryRepository.GetEntityNamesByOriginTypeId((int)EnumCivilEntityType.PinCode, pinCodeId);
            if (!string.IsNullOrEmpty(actualPinCode))
            {
              pinCodeValue = actualPinCode;
            }
          }
        }
      }
    }

    return new OrderAddressWithEntityResponseModel
    {
      OrderAddressId = orderAddress.OrderAddressId,
      CustomerName = orderAddress.CustomerName,
      CustomerFullAddress = orderAddress.CustomerFullAddress,
      Email = orderAddress.Email,
      Mobile1 = orderAddress.Mobile1,
      Mobile2 = orderAddress.Mobile2,
      Country = orderAddress.CountryId,
      City = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.City),
      Area = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.Area),
      Province = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.Province),
      State = CivilEntityHelper.GetEntityValue(entityMappings, EnumCivilEntityType.State),
      PinCode = pinCodeValue,
      StreetAddress = orderAddress.StreetAddress,
      StreetAddress2 = orderAddress.StreetAddress2,
      HouseNo = orderAddress.HouseNo,
      BuildingName = orderAddress.BuildingName,
      Landmark = orderAddress.Landmark,
      Latitude = orderAddress.Latitude,
      Longitude = orderAddress.Longitude,
      EntityAddressDataJson = orderAddress.EntityAddressDataJson,
      SelectedCarrierId = orderAddress.SelectedCarrierId
    };
  }

}
