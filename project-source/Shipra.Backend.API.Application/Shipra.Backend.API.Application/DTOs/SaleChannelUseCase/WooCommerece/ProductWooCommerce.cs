using System.Text.Json;
using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
public class ProductWooCommerceModal
{
  [JsonProperty("id")]
  public int Id { get; set; }
  [JsonProperty("name")]
  public string? Name { get; set; } = "";
  [JsonProperty("slug")]
  public string? Slug { get; set; } = "";
  [JsonProperty("permalink")]
  public string? Permalink { get; set; } = "";
  [JsonProperty("date_created")]
  public DateTime DateCreated { get; set; }
  [JsonProperty("date_created_gmt")]
  public DateTime? DateCreatedGmt { get; set; }
  [JsonProperty("date_modified")]
  public DateTime? DateModified { get; set; }
  [JsonProperty("date_modified_gmt")]
  public DateTime? DateModifiedGmt { get; set; }
  [JsonProperty("type")]
  public string? Type { get; set; } = "";
  [JsonProperty("status")]
  public string? Status { get; set; } = "";
  [JsonProperty("featured")]
  public bool Featured { get; set; }=false;
  [JsonProperty("catalog_visibility")]
  public string? CatalogVisibility { get; set; } = "";
  [JsonProperty("description")]
  public string? Description { get; set; } = "";
  [JsonProperty("short_description")]
  public string? ShortDescription { get; set; } = "";
  [JsonProperty("sku")]
  public string? SKU { get; set; } = "";
  [JsonProperty("price")]
  public string? Price { get; set; } = "0";
  [JsonProperty("regular_price")]
  public string? RegularPrice { get; set; } = "0";
  [JsonProperty("sale_price")]
  public string? SalePrice { get; set; } = "0";
  [JsonProperty("date_on_sale_from")]
  public DateTime? DateOnSaleFrom { get; set; }
  [JsonProperty("date_on_sale_from_gmt")]
  public DateTime? DateOnSaleFromGmt { get; set; }
  [JsonProperty("date_on_sale_to")]
  public DateTime? DateOnSaleTo { get; set; }
  [JsonProperty("date_on_sale_to_gmt")]
  public DateTime? DateOnSaleToGmt { get; set; }
  [JsonProperty("on_sale")]
  public bool? OnSale { get; set; }=false;
  [JsonProperty("purchasable")]
  public bool? Purchasable { get; set; } = false;
  [JsonProperty("total_sales")]
  public int? TotalSales { get; set; } = 0;
  [JsonProperty("virtual")]
  public bool? Virtual { get; set; } = false;
  [JsonProperty("downloadable")]
  public bool? Downloadable { get; set; } = false;
  [JsonProperty("downloads")]
  public List<object>? Downloads { get; set; }
  [JsonProperty("download_limit")]
  public int? DownloadLimit { get; set; }
  [JsonProperty("download_expiry")]
  public int? DownloadExpiry { get; set; }
  [JsonProperty("external_url")]
  public string? ExternalUrl { get; set; }
  [JsonProperty("button_text")]
  public string? ButtonText { get; set; }
  [JsonProperty("tax_status")]
  public string? TaxStatus { get; set; }
  [JsonProperty("tax_class")]
  public string? TaxClass { get; set; }
  [JsonProperty("manage_stock")]
  public bool? ManageStock { get; set; }
  [JsonProperty("stock_quantity")]
  public int? StockQuantity { get; set; }
  [JsonProperty("backorders")]
  public string? Backorders { get; set; }
  [JsonProperty("backorders_allowed")]
  public bool? BackordersAllowed { get; set; }
  [JsonProperty("backordered")]
  public bool? Backordered { get; set; }
  [JsonProperty("low_stock_amount")]
  public int? LowStockAmount { get; set; }
  [JsonProperty("sold_individually")]
  public bool? SoldIndividually { get; set; }
  [JsonProperty("weight")]
  public string? Weight { get; set; }
  [JsonProperty("dimensions")]
  public DimensionsWooCommerceModal? Dimensions { get; set; }
  [JsonProperty("shipping_required")]
  public bool? ShippingRequired { get; set; }
  [JsonProperty("shipping_taxable")]
  public bool? ShippingTaxable { get; set; }
  [JsonProperty("shipping_class")]
  public string? ShippingClass { get; set; }
  [JsonProperty("shipping_class_id")]
  public int? ShippingClassId { get; set; }
  [JsonProperty("reviews_allowed")]
  public bool? ReviewsAllowed { get; set; }
  [JsonProperty("average_rating")]
  public string? AverageRating { get; set; }
  [JsonProperty("rating_count")]
  public int? RatingCount { get; set; }
  [JsonProperty("upsell_ids")]
  public List<object>? UpsellIds { get; set; }
  [JsonProperty("cross_sell_ids")]
  public List<object>? CrossSellIds { get; set; }
  [JsonProperty("parent_id")]
  public int? ParentId { get; set; }
  [JsonProperty("purchase_note")]
  public string? PurchaseNote { get; set; }
  [JsonProperty("categories")]
  public List<CategoryWooCommerceModal>? Categories { get; set; }
  [JsonProperty("tags")]
  public List<object>? Tags { get; set; }
  [JsonProperty("images")]
  public List<ImageWooCommerceModal>? Images { get; set; }
  [JsonProperty("attributes")]
  public List<AttributeWooCommerceModal>? Attributes { get; set; }
  [JsonProperty("default_attributes")]
  public List<object>? DefaultAttributes { get; set; }
  [JsonProperty("variations")]
  public List<object>? Variations { get; set; }
  [JsonProperty("grouped_products")]
  public List<object>? GroupedProducts { get; set; }
  [JsonProperty("menu_order")]
  public int? MenuOrder { get; set; }
  [JsonProperty("price_html")]
  public string? PriceHtml { get; set; }
  [JsonProperty("related_ids")]
  public List<object>? RelatedIds { get; set; }

  [JsonProperty("stock_status")]
  public string? StockStatus { get; set; }
  [JsonProperty("has_options")]
  public bool? HasOptions { get; set; }
  [JsonProperty("post_password")]
  public string? PostPassword { get; set; }
  [JsonProperty("aioseo_notices")]
  public List<object>? AioseoNotices { get; set; }
  [JsonProperty("links")]
  public LinksWooCommerceModal? _Links { get; set; }
}
