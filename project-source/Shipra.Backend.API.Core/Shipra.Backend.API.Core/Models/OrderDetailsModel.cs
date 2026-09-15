namespace Shipra.Backend.API.Core.Models;

public class OrderDetailsModel
{
  public string? OrderId { get; set; }
  public string? OrderNo { get; set; }
  public string? RefNo { get; set; }
  public DateTime? OrderDate { get; set; }
  public DateTime? CreatedOn { get; set; }
  public decimal? Amount { get; set; }
  public int? CarrierTrackingStatusId { get; set; }
  public int? CarrierId { get; set; }
  public string? CarrierTrackingNo { get; set; }
  public string? CarrierTrackingStatus { get; set; }
  public string? TrackingStatus { get; set; }
  public string? CarrierName { get; set; }
  public string? Description { get; set; }
  public string? Remarks { get; set; }
  public string? FullFillmentStatus { get; set; }
  public int ItemsCount { get; set; }
  public string? PaymentStatus { get; set; }
  public string? PaymentMethod { get; set; }
  public int? PaymentMethodId { get; set; }
  public decimal? Weight { get; set; }
  public int? OrderDeliveryTypeId { get; set; }
  public string? DeliveryTypeName { get; set; }
  public decimal? ItemValue { get; set; }
  public string? ProductStationName { get; set; }
  public decimal Discount { get; set; }
  public decimal? VAT { get; set; }
  public string? StoreName { get; set; }
  public string? StoreImage { get; set; }
  public string? URLs { get; set; }
  public string? CustomerServiceNo { get; set; }
  public string? StoreCompany { get; set; }
  public string? StoreEmail { get; set; }
  public string? StoreAddress { get; set; }
  public int StoreId { get; set; }
  public string? StoreCountry { get; set; }
  public string? OrderTypeName { get; set; }
  public int? OrderAddressId { get; set; }
  public string? CustomerName { get; set; }
  public int OrderTypeId { get; set; }
  public string? Mobile1 { get; set; }
  public string? CustomerEmail { get; set; }
  public string? Mobile2 { get; set; }
  public string? CustomerAddress { get; set; }
  public string? ConsigneCountryName { get; set; }
  public string? ClientName { get; set; }
  public string? ClientCode { get; set; }
  public decimal? TotalTax { get; set; }
  public string? ClientCompanyName { get; set; }
  public string? Email { get; set; }
  public string? ClientId { get; set; }
  public string? StripeInvoiceHostURL { get; set; }
  public string? StripeInvoicePDFURL { get; set; }
}
