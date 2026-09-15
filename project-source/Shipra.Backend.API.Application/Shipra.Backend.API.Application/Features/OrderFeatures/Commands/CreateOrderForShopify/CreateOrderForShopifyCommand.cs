using MediatR;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderForShopify;
public class CreateOrderForShopifyCommand : IRequest<ServiceResultDTO>
{
  public List<CreateShopifyOrderRequestModel>? OrderList { get; set; }
}
public class CreateShopifyOrderRequestModel
{
  public int? StoreId { get; set; } = 0;
  public int? ChannelId { get; set; } = 0;
  public int? OrderTypeId { get; set; } = (int)EnumOrderType.Regular;
  public DateTime? OrderDate { get; set; } = DateTime.UtcNow;
  public string? Description { get; set; }
  public string? Remarks { get; set; }
  public decimal? Amount { get; set; }
  public decimal? DeliveryCharges { get; set; } = 0;
  public int? PaymentStatusId { get; set; } = (int)EnumPaymentStatus.Unpaid;
  public decimal? Weight { get; set; } = 0;
  public decimal? ItemValue { get; set; } = 0;
  public int? OrderRequestVia { get; set; } = (int)EnumOrderRequestVia.Api;
  public int? PaymentMethodId { get; set; } = (int)EnumPaymentMethod.COD;
  public int StationId { get; set; } = 0;
  public decimal Discount { get; set; } = 0;
  public decimal VAT { get; set; } = 0;
  public OrderNoteShopifyModel? OrderNote { get; set; } = new();
  public OrderAddressShopifyModel? OrderAddress { get; set; } = new();
  public List<OrderItemShopifyModel>? OrderItems { get; set; } = new();
}
public class OrderNoteShopifyModel
{
  public string? Note { get; set; }
}
public class OrderAddressShopifyModel
{
  public string? CustomerName { get; set; }
  public string? Email { get; set; }
  public string? Mobile1 { get; set; }
  public string? Mobile2 { get; set; }
  public string? Country { get; set; }
  public string? Region { get; set; }
  public string? City { get; set; }
  public string? StreetAddress { get; set; }
  public decimal? Latitude { get; set; }
  public decimal? Longitude { get; set; }
}
public class OrderItemShopifyModel
{
  public string? ProductId { get; set; }
  public long? ProductStockId { get; set; }
  public long? ProductVariantId { get; set; }
  public long? InventoryBalanceId { get; set; }
  public decimal? Price { get; set; }
  public string? Description { get; set; }
  public string? Remarks { get; set; }
  public int? Quantity { get; set; }
  public decimal? Discount { get; set; }
}
