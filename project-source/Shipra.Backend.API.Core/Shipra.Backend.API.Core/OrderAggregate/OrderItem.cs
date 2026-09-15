using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderItem
{
  public OrderItem()
  {

  }
  public OrderItemId? OrderItemId { get; private set; }
  public OrderId? OrderId { get; private set; }
  public ProductId? ProductId { get; private set; }
  public long? ProductVariantId { get; private set; }
  public decimal? Price { get; private set; }
  public string? Description { get; private set; }
  public string? Remarks { get; private set; }
  public int? Quantity { get; private set; }
  public string? ItemBarcode { get; private set; }
  public decimal? Discount { get; private set; }
  public string? ReferenceNo { get; private set; }
  public string? HsCode { get; private set; }
  public string? OriginCountryCode { get; private set; }
  public decimal? UnitRate { get; private set; }
  public decimal? Weight { get; private set; }

  public static OrderItem CreateOrderItemFullFilable(OrderId orderId, ProductId productId, decimal? price, string? description, string? remarks, int? quantity, decimal? discount, string? hsCode = null, string? originCountryCode = null, decimal? unitRate = null, decimal? weight = null, long? productVariantId = null)
  {
    price = price.GetValueOrDefault() ;
    return new OrderItem()
    {
      OrderItemId = OrderItemId.New,
      OrderId = orderId,
      ProductId = productId,
      ProductVariantId = productVariantId,
      Price = price,
      Description = description,
      Remarks = remarks,
      Quantity = quantity,
      Discount = discount,
      HsCode = hsCode,
      OriginCountryCode = originCountryCode,
      UnitRate = unitRate,
      Weight = weight
    };
  }

  public static OrderItem? CreateOrderItemRegular(OrderId orderId, decimal? price, string? description, string? remarks, int? quantity, decimal? discount, string? hsCode = null, string? originCountryCode = null, decimal? unitRate = null, decimal? weight = null)
  {
    return new OrderItem()
    {
      OrderItemId = OrderItemId.New,
      OrderId = orderId,
      Price = price,
      Description = description,
      Remarks = remarks,
      Quantity = quantity,
      Discount = discount,
      HsCode = hsCode,
      OriginCountryCode = originCountryCode,
      UnitRate = unitRate,
      Weight = weight
    };
  }

  public void UpdateOrderItem(ProductId productId, decimal? price, string? description, string? remarks, int? quantity, decimal? discount, long? productVariantId = null)
  {
    ProductId = productId;
    ProductVariantId = productVariantId;
    Price = price;
    Description = description;
    Remarks = remarks;
    Quantity = quantity;
    Discount = discount;
  }
  public void UpdateOrderItemRegular(decimal? price, string? description, string? remarks, int? quantity, decimal? discount)
  {
    Price = price.GetValueOrDefault();
    Description = description;
    Remarks = remarks;
    Quantity = quantity;
    Discount = discount;
  }
  public static OrderItem CreateOrderItemWithArchiveData(
    OrderItemId? orderItemId,
      OrderId orderId,
      ProductId productId,
      decimal? price,
      string? description,
      string? remarks,
      int? quantity,
      string? itemBarcode,
      decimal? discount,
    string? referenceNo,
    long? productVariantId = null
  )
  {
    return new OrderItem
    {
      OrderItemId = orderItemId,
      OrderId = orderId,
      ProductId = productId,
      ProductVariantId = productVariantId,
      Price = price ?? 0,
      Description = description,
      Remarks = remarks,
      Quantity = quantity,
      ItemBarcode = itemBarcode,
      Discount = discount,
      ReferenceNo = referenceNo,
    };
  }
}
public sealed record OrderItemId(Guid Value)
{
  public static OrderItemId New => new(Guid.NewGuid());
}
