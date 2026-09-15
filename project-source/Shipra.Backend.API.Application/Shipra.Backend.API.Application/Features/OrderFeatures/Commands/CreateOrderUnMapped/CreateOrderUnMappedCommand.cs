using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.CustomBinder;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.UnMappedOrderUseCase;
using Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderUnMapped;
public class CreateOrderUnMappedCommand : IRequest<ServiceResultDTO>
{
  public List<CreateUnMappedOrderRequestModel>? orderList { get; set; }
  public long? OrderDraftId { get; set; } = 0;
  public bool? IsSaleChannelOrder { get; set; } = false;
  public bool? IsAssigCarrier { get; set; } = false;
  public bool? WithThirdPartyResponse { get; set; } = false;
}
public class CreateUnMappedOrderRequestModel : CreateUpdateOrderCommonRequestModel
{
  public virtual UnMappedOrderAddressModel? OrderAddress { get; set; } = new();
}
public class CreateUpdateOrderCommonRequestModel
{
  public string? OrderId { get; set; }
  public int? StoreId { get; set; }
  public int? OrderTypeId { get; set; } = (int)EnumOrderType.Regular;
  public DateTime? OrderDate { get; set; }
  public string? Description { get; set; }
  public string? Remarks { get; set; }
  public decimal? Amount { get; set; }
  //this column used for CShippingcharges
  public decimal? CShippingCharges { get; set; }
  public int? PaymentStatusId { get; set; }
  public decimal? Weight { get; set; }
  public decimal? ItemValue { get; set; }
  public int? OrderRequestVia { get; set; } = (int)EnumOrderRequestVia.Api;
  public int? PaymentMethodId { get; set; }
  public int StationId { get; set; }
  public decimal? Discount { get; set; }
  public decimal? VAT { get; set; }
  public string? RefNo { get; set; }
  public int? SaleChannelConfigId { get; set; }
  public int? SaleChannelLookupId { get; set; }
  public long? OrderDraftId { get; set; }
  public int? OrderDeliveryTypeId { get; set; } = (int)EnumOrderDeliveryType.Forward;
  public OrderNoteModel? OrderNote { get; set; } = new();
  public List<OrderBoxRequestModel>? OrderBoxs { get; set; } = new();
  public List<SettingConfigItem>? settingConfig { get; set; }
  public List<OrderItemModel>? OrderItems { get; set; } = new();
  public List<OrderTaxModel>? OrderTaxes { get; set; } = new();
  public AssignToCarrierOrderInfoRequestModel? CarrierData { get; set; }

}

public class CreateOrderUnMappedCommandHandler : RequestHandlerBase<CreateOrderUnMappedCommand, ServiceResultDTO>
{
  private readonly IMediator _mediator;
  private readonly ICountryRepository _countryRepository;


  public CreateOrderUnMappedCommandHandler(IMediator mediator, ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<CreateOrderUnMappedCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateOrderUnMappedCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new();

    try
    {
      if (request.orderList == null || !request.orderList.Any())
      {
        return serviceResultDTO;
      }

      CreateOrderCommand createOrderCommand = new()
      {
        OrderDraftId = request.OrderDraftId,
        IsSaleChannelOrder = request.IsSaleChannelOrder,
        IsAssigCarrier = request.IsAssigCarrier,
        WithThirdPartyResponse = request.WithThirdPartyResponse,
        orderList = new List<CreateOrderRequestModel>()
      };

      foreach (var order in request.orderList)
      {
        var address = order.OrderAddress;

        Country? countryObj = null;
        City? cityObj = null;
        Province? provinceObj = null;
        Area? areaObj = null;
        State? stateObj = null;

        // Country
        if (!string.IsNullOrWhiteSpace(address?.Country))
        {
          countryObj = await _countryRepository
              .GetCountryByName(address.Country);
        }
        if (countryObj is null && !string.IsNullOrWhiteSpace(address?.CountryCodeISO3))
        {
          countryObj = await _countryRepository
              .GetCountryByISOCode(address.CountryCodeISO3);
        }

        // City
        if (!string.IsNullOrWhiteSpace(address?.City))
        {
          cityObj = await _countryRepository
              .GetCityByName(address.City);
        }

        // Province
        if (!string.IsNullOrWhiteSpace(address?.Province))
        {
          provinceObj = await _countryRepository
              .GetProvinceByName(address.Province);
        }

        // Area
        if (!string.IsNullOrWhiteSpace(address?.Area))
        {
          areaObj = await _countryRepository
              .GetAreaByName(address.Area);
        }

        // State
        if (!string.IsNullOrWhiteSpace(address?.State))
        {
          stateObj = await _countryRepository
              .GetStateByName(address.State);
        }

        // Construct mapped order
        CreateOrderRequestModel mappedOrder = new()
        {
          StoreId = order.StoreId,
          OrderTypeId = order.OrderTypeId,
          OrderDate = order.OrderDate,
          Description = order.Description,
          Remarks = order.Remarks,
          Amount = order.Amount,
          CShippingCharges = order.CShippingCharges,
          PaymentStatusId = order.PaymentStatusId,
          Weight = order.Weight,
          ItemValue = order.ItemValue,
          OrderRequestVia = order.OrderRequestVia,
          PaymentMethodId = order.PaymentMethodId,
          StationId = order.StationId,
          Discount = order.Discount,
          VAT = order.VAT,
          RefNo = order.RefNo,
          SaleChannelConfigId = order.SaleChannelConfigId,
          SaleChannelLookupId = order.SaleChannelLookupId,
          OrderDraftId = order.OrderDraftId,
          OrderDeliveryTypeId = order.OrderDeliveryTypeId,

          OrderNote = order.OrderNote,
          OrderBoxs = order.OrderBoxs,
          settingConfig = order.settingConfig,
          OrderItems = order.OrderItems,
          OrderTaxes = order.OrderTaxes,
          CarrierData = order.CarrierData,

          OrderAddress = new OrderAddressModel
          {
            OrderAddressId = address?.OrderAddressId ?? 0,
            CustomerName = address?.CustomerName,
            Email = address?.Email,
            Mobile1 = address?.Mobile1,
            Mobile2 = address?.Mobile2,
            EntityAddressDataJson = address?.EntityAddressDataJson,
            SelectedCarrierId = address?.SelectedCarrierId,
            CustomerFullAddress = address?.CustomerFullAddress,

            CountryId = countryObj?.CountryId,
            CityId = cityObj?.CityId,
            ProvinceId = provinceObj?.ProvinceId,
            AreaId = areaObj?.AreaId,
            StateId = stateObj?.StateId,

            StreetAddress = address?.StreetAddress,
            StreetAddress2 = address?.StreetAddress2,
            HouseNo = address?.HouseNo,
            BuildingName = address?.BuildingName,
            Landmark = address?.Landmark,
            Zip = address?.Zip,
            AddressTypeId = address?.AddressTypeId,
            Latitude = address?.Latitude,
            Longitude = address?.Longitude
          }
        };
        var addressModel = new OrderAddressClientSideModel
        {
          CountryId = mappedOrder.OrderAddress?.CountryId,
          CityId = mappedOrder.OrderAddress?.CityId?.ToString(),
          AreaId = mappedOrder.OrderAddress?.AreaId?.ToString(),
          StateId = mappedOrder.OrderAddress?.StateId?.ToString(),
          ProvinceId = mappedOrder.OrderAddress?.ProvinceId?.ToString(),
        };
        mappedOrder.OrderAddress!.EntityAddressDataJson = AddressConversionHelper.GetAddressJson(addressModel);
        createOrderCommand.orderList.Add(mappedOrder);
      }

      // Send mapped command
      serviceResultDTO = await _mediator.Send(createOrderCommand);

      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
