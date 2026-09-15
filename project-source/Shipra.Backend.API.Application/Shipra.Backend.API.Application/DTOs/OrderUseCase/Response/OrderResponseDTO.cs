using Shipra.Backend.API.Application.DTOs.Common.Base.Response;

namespace Shipra.Backend.API.Application.DTOs.OrderUseCase.Response;
 
public class OrderResponseModel
{
  public string? OrderId { get; set; }
  public int? StoreId { get; set; }
  public int? OrderTypeId { get; set; }
  public DateTime? OrderDate { get; set; }
  public string? Description { get; set; }
  public string? Remarks { get; set; }
  public decimal? Amount { get; set; }
  public decimal? ActualAmount { get; set; }
  public decimal? DeliveryCharges { get; set; }
  public decimal? CShippingCharges { get; set; }
  public int? PaymentStatusId { get; set; }
  public decimal? Weight { get; set; }
  public int? ItemsCount { get; set; }
  public decimal? ItemValue { get; set; }
  public int? OrderRequestVia { get; set; }
  public int? PaymentMethodId { get; set; }
  public int StationId { get; set; }
  public int? SaleChannelConfigId { get; set; }
  public decimal Discount { get; set; }
  public decimal VAT { get; set; }
  public string? RefNo { get; set; }
  public string? StripeInvoiceHostURL { get; set; }
  public int? CarrierId { get; set; }
  public string? StripeInvoicePDFURL { get; set; }
}
