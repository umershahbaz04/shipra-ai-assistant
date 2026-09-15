using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;

public class CreateUploadOrderResponseModel
{
  public int? StoreId { get; set; } 
  public int? ChannelId { get; set; }
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
  public string? SCOrderId { get; set; }
  public string? SCOrderNo { get; set; }
  public int ItemsCount { get; set; }
  public OrderNoteModel? OrderNote { get; set; } = new();
  public OrderAddressResponseModel? OrderAddress { get; set; } = new();
  public List<OrderItemModel>? OrderItems { get; set; } = new();
  public List<OrderTaxModel>? OrderTaxes { get; set; } = new();
  //public List<OrderBoxRequestModel>? OrderBoxs { get; set; } = new();
  public List<ClientOrderBoxResponseModel>? OrderBoxs { get; set; } = new();

}
