using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.Features.MetaFieldFeature.Command;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.OrderBoxAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
public class CreateOrderCommand : IRequest<ServiceResultDTO>
{
  public List<CreateOrderRequestModel>? orderList { get; set; } 
  public long? OrderDraftId { get; set; } = 0;
  public bool? IsSaleChannelOrder { get; set; } = false;
  public bool? IsAssigCarrier { get; set; } = false;
  public bool? WithThirdPartyResponse { get; set; } = false;
}
public class CreateOrderRequestModel : CreateUpdateOrderCommonRequestModel
{
  public virtual OrderAddressModel? OrderAddress { get; set; } = new();
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

