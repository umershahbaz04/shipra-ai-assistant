using System;
using System.Collections.Generic;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;

public class OrderJsonWrapper
{
  public OrderWithArchiveDataDTOs? Order { get; set; }
  public List<OrderItemWithArchiveDTOs>? OrderItems { get; set; }

  public class OrderWithArchiveDataDTOs
  {
    public OrderId? OrderId { get; set; }
    public ClientId? ClientId { get; set; }
    public int? StoreId { get; set; }
    public int? SaleChannelConfigId { get; set; }
    public int? OrderTypeId { get; set; }
    public string? OrderNo { get; set; }
    public DateTime? OrderDate { get; set; }
    public long? OrderAddressId { get; set; }
    public decimal? Amount { get; set; }
    public int? CarrierId { get; set; }
    public int? ActiveCarrierId { get; set; }
    public int? ActiveCarrierPickupLocationId { get; set; }
    public string? CarrierTrackingNo { get; set; }
    public string? CarrierTrackingStatus { get; set; }
    public int? CarrierTrackingStatusId { get; set; }
    public DateTime? CarrierAssignDate { get; set; }
    public DateTime? CarrierLastUpdateDateTime { get; set; }
    public CarrierPaymentSettlementId? CarrierPaymentSettlementId { get; set; }
    public DateTime? CarrierPaymentSettlementDate { get; set; }
    public CarrierRRId? CarrierRRId { get; set; }
    public string? Description { get; set; }
    public string? Remarks { get; set; }
    public int? FullFillmentStatusId { get; set; }
    public EmployeeId? FulFiledById { get; set; }
    public DateTime? FulFilledDate { get; set; }
    public int? ItemsCount { get; set; }
    public decimal? DeliveryCharges { get; set; }
    public int? PaymentStatusId { get; set; }
    public string? PaymentRef { get; set; }
    public string? ReturnRef { get; set; }
    public decimal? Weight { get; set; }
    public decimal? ItemValue { get; set; }
    public int? OrderRequestVia { get; set; }
    public int? PaymentMethodId { get; set; }
    public int? StationId { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Vat { get; set; }
    public decimal? TotalTax { get; set; }
    public decimal? CShippingCharges { get; set; }
    public bool? TrackingLock { get; set; }
    public string? RefNo { get; set; }
    public string? OrderLabels { get; set; }
    public DateTime? CreatedOn { get; set; }
    public EmployeeId? CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public EmployeeId? UpdatedBy { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripeInvoiceHostURL { get; set; }
    public string? StripeInvoicePDFURL { get; set; }
    public string? StripeInvoiceId { get; set; }
    public int? OrderDeliveryTypeId { get; set; }
    public SaleChannelOrderId? SaleChannelOrderId { get; set; }
    public int? SaleChannelLookupId { get; set; }
    public int? CarrierContractTypeId { get; set; }
    public bool? IsShipraInvoiceCharged { get; set; }
    public Guid? InvoiceId { get; set; }
    public int? DInvoiceId { get; set; }
    public int? PInvoiceId { get; set; }
  }
  

  public class OrderItemWithArchiveDTOs
  {
    public OrderItemWithArchiveDTOs() { }

    public OrderItemId? OrderItemId { get; set; }
    public OrderId? OrderId { get; set; }
    public ProductId? ProductId { get; set; }
    public long? ProductVariantId { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? Remarks { get; set; }
    public int? Quantity { get; set; }
    public string? ItemBarcode { get; set; }
    public decimal? Discount { get; set; }
    public string? ReferenceNo { get; set; }
  }

  
}
