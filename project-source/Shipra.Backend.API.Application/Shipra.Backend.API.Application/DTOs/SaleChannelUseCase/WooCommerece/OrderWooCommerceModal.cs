using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;

public class Billing
{
  public string? first_name { get; set; }
  public string? last_name { get; set; }
  public string? company { get; set; }
  public string? address_1 { get; set; }
  public string? address_2 { get; set; }
  public string? city { get; set; }
  public string? state { get; set; }
  public string? postcode { get; set; }
  public string? country { get; set; }
  public string? email { get; set; }
  public string? phone { get; set; }
}

public class Image
{
  public string? id { get; set; }
  public string? src { get; set; }
}

public class LineItem
{
  [JsonProperty("id")]
  public int? Id { get; set; }
  [JsonProperty("name")]
  public string? Name { get; set; }
  [JsonProperty("product_id")]
  public int? ProductId { get; set; }
  [JsonProperty("variation_id")]
  public int? VariationId { get; set; }
  [JsonProperty("quantity")]
  public int? Quantity { get; set; }
  [JsonProperty("tax_class")]
  public string? TaxClass { get; set; }
  [JsonProperty("subtotal")]
  public string? SubTotal { get; set; }
  [JsonProperty("subtotal_tax")]
  public string? SubTotalTax { get; set; }
  [JsonProperty("total")]
  public string? Total { get; set; }
  [JsonProperty("total_tax")]
  public string? TotalTax { get; set; }
  [JsonProperty("Taxes")]
  public List<object>? Taxes { get; set; }
  [JsonProperty("meta_data")]
  public List<object>? MetaData { get; set; }
  [JsonProperty("sku")]
  public string? SKU { get; set; }
  [JsonProperty("price")]
  public int Price { get; set; }
  [JsonProperty("image")]
  public ImageWooCommerceModal? Image { get; set; }
  [JsonProperty("parent_name")]
  public object? ParentName { get; set; }
}

public class OrderWooCommerceModal
{
  [JsonProperty("id")]
  public int? OrderId { get; set; }
  [JsonProperty("parent_id")]
  public int? ParentId { get; set; }
  [JsonProperty("status")]
  public string? Status { get; set; }
  [JsonProperty("currency")]
  public string? Currency { get; set; }
  [JsonProperty("version")]
  public string? Version { get; set; }
  [JsonProperty("prices_include_tax")]
  public bool PricesIncludeTax { get; set; }
  [JsonProperty("date_created")]
  public DateTime? DateCreated { get; set; }
  [JsonProperty("date_modified")]
  public DateTime? DateModified { get; set; }
  [JsonProperty("discount_total")]
  public decimal? DiscountTotal { get; set; }
  [JsonProperty("discount_tax")]
  public decimal? DiscountTax { get; set; }
  [JsonProperty("shipping_total")]
  public decimal? ShippingTotal { get; set; }
  [JsonProperty("shipping_tax")]
  public decimal? ShippingTax { get; set; }
  [JsonProperty("cart_tax")]
  public decimal?  CartTax { get; set; }
  [JsonProperty("total")]
  public decimal? Total { get; set; }
  [JsonProperty("total_tax")]
  public decimal? TotalTax { get; set; }
  [JsonProperty("customer_id")]
  public int CustomerId { get; set; }
  [JsonProperty("order_key")]
  public string? OrderKey { get; set; }
  [JsonProperty("billing")]
  public Billing? Billing { get; set; }
  [JsonProperty("shipping")]
  public Billing? Shipping { get; set; }
  [JsonProperty("payment_method")]
  public string? PaymentMethod { get; set; }
  [JsonProperty("payment_method_title")]
  public string? PaymentMethodTitle { get; set; }
  [JsonProperty("transaction_id")]
  public string? TransactionId { get; set; }
  [JsonProperty("customer_ip_address")]
  public string? CustomerIpAddress { get; set; }
  [JsonProperty("customer_user_agent")]
  public string? CustomerUserAgent { get; set; }
  [JsonProperty("created_via")]
  public string? CreatedVia { get; set; }
  [JsonProperty("customer_note")]
  public string? CustomerNote { get; set; }
  [JsonProperty("date_completed")]
  public DateTime? DateCompleted { get; set; }
  [JsonProperty("date_paid")]
  public DateTime? DatePaid { get; set; }
  [JsonProperty("cart_hash")]
  public string? CartHash { get; set; }
  [JsonProperty("number")]
  public string? Number { get; set; }
  [JsonProperty("meta_data")]
  public List<object>? MetaData { get; set; }
  [JsonProperty("line_items")]
  public List<LineItem>? LineItems { get; set; }
  [JsonProperty("tax_lines")]
  public List<object>? TaxLines { get; set; }
  [JsonProperty("shipping_lines")]
  public List<object>? ShippingLines { get; set; }
  [JsonProperty("fee_lines")]
  public List<object>? FeeLines { get; set; }
  [JsonProperty("coupon_lines")]
  public List<object>? CouponLines { get; set; }
  [JsonProperty("refunds")]
  public List<object>? Refunds { get; set; }
  [JsonProperty("payment_url")]
  public string? PaymentURL { get; set; }
  [JsonProperty("is_editable")]
  public bool IsEditable { get; set; }
  [JsonProperty("needs_payment")]
  public bool NeedsPayment { get; set; }
  [JsonProperty("needs_processing")]
  public bool NeedsProcessing { get; set; }
  [JsonProperty("date_created_gmt")]
  public DateTime? DateCreatedGMT { get; set; }
  [JsonProperty("date_modified_gmt")]
  public DateTime? DateModifiedGMT { get; set; }
  [JsonProperty("date_completed_gmt")]
  public DateTime? DateCompletedGMT { get; set; }
  [JsonProperty("date_paid_gmt")]
  public DateTime? DatePaidGMT { get; set; }
  [JsonProperty("currency_symbol")]
  public string? CurrencySymbol { get; set; }
  [JsonProperty("_links")]
  public LinksWooCommerceModal? Links { get; set; }
}

