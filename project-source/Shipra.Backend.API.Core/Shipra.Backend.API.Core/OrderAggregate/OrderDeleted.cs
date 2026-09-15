using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class OrderDeleted
{
  public OrderDeletedId? OrderDeletedId { get; private set; }

  public OrderId? OrderId { get; private set; }
  public ClientId? ClientId { get; private set; }
  public int? StoreId { get; private set; }
  public int? SaleChannelConfigId { get; private set; }
  public int? OrderTypeId { get; private set; }
  public string? OrderNo { get; private set; }
  public DateTime? OrderDate { get; private set; }
  public long? OrderAddressId { get; private set; }
  public decimal? Amount { get; private set; }
  public int? CarrierId { get; private set; }
  public int? ActiveCarrierId { get; private set; }
  public string? CarrierTrackingNo { get; private set; }
  public string? CarrierTrackingStatus { get; private set; }
  public int? CarrierTrackingStatusId { get; set; }
  public DateTime? CarrierAssignDate { get; private set; }
  public DateTime? CarrierLastUpdateDateTime { get; private set; }
  public CarrierPaymentSettlementId? CarrierPaymentSettlementId { get; private set; }
  public CarrierRRId? CarrierRRId { get; private set; }
  public string? Description { get; private set; }
  public string? Remarks { get; private set; }
  public int? FullFillmentStatusId { get; private set; }
  public EmployeeId? FulFiledById { get; private set; }
  public DateTime? FulFilledDate { get; private set; }
  public int? ItemsCount { get; private set; }
  public decimal? DeliveryCharges { get; private set; }
  public int? PaymentStatusId { get; private set; }
  public string? PaymentRef { get; private set; }
  public string? ReturnRef { get; private set; }
  public decimal? Weight { get; private set; }
  public decimal? ItemValue { get; private set; }
  public int? OrderRequestVia { get; private set; }
  public int? PaymentMethodId { get; private set; }
  public int? StationId { get; private set; }
  public decimal? Discount { get; private set; }
  public decimal? Vat { get; private set; }
  public decimal? TotalTax { get; private set; }
  public decimal? CShippingCharges { get; private set; }
  public bool? TrackingLock { get; private set; }
  public string? RefNo { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public string? StripeCustomerId { get; private set; }
  public string? StripeInvoiceHostURL { get; private set; }
  public string? StripeInvoicePDFURL { get; private set; }
  public string? StripeInvoiceId { get; private set; }
  public SaleChannelOrderId? SaleChannelOrderId { get; private set; }
  public int? SaleChannelLookupId { get; private set; }

  public static OrderDeleted CreateOrderDeleted(OrderId? orderId, ClientId? clientId, int? storeId, int? saleChannelConfigId, int? orderTypeId, string? orderNo, DateTime? orderDate, long? orderAddressId, decimal? amount, int? carrierId, int? activeCarrierId, string? carrierTrackingNo, string? carrierTrackingStatus, int? carrierTrackingStatusId, DateTime? carrierAssignDate, DateTime? carrierLastUpdateDateTime, CarrierPaymentSettlementId? carrierPaymentSettlementId, CarrierRRId? carrierRRId, string? description, string? remarks, int? fullFillmentStatusId, EmployeeId? fulFiledById, DateTime? fulFilledDate, int? itemsCount, decimal? deliveryCharges, int? paymentStatusId, string? paymentRef, string? returnRef, decimal? weight, decimal? itemValue, int? orderRequestVia, int? paymentMethodId, int? stationId, decimal? discount, decimal? vat, decimal? totalTax, decimal? cShippingCharges, bool? trackingLock, string? refNo, DateTime? createdOn, EmployeeId? createdBy, DateTime? updatedOn, EmployeeId? updatedBy, string? stripeCustomerId, string? stripeInvoiceHostURL, string? stripeInvoicePDFURL, string? stripeInvoiceId, SaleChannelOrderId? saleChannelOrderId, int? saleChannelLookupId)
  {
    return new OrderDeleted
    {
      OrderDeletedId = OrderDeletedId.New,
      OrderId = orderId,
      ClientId = clientId,
      StoreId = storeId,
      SaleChannelConfigId = saleChannelConfigId,
      OrderTypeId = orderTypeId,
      OrderNo = orderNo,
      OrderDate = orderDate,
      OrderAddressId = orderAddressId,
      Amount = amount,
      CarrierId = carrierId,
      ActiveCarrierId = activeCarrierId,
      CarrierTrackingNo = carrierTrackingNo,
      CarrierTrackingStatus = carrierTrackingStatus,
      CarrierTrackingStatusId = carrierTrackingStatusId,
      CarrierAssignDate = carrierAssignDate,
      CarrierLastUpdateDateTime = carrierLastUpdateDateTime,
      CarrierPaymentSettlementId = carrierPaymentSettlementId,
      CarrierRRId = carrierRRId,
      Description = description,
      Remarks = remarks,
      FullFillmentStatusId = fullFillmentStatusId,
      FulFiledById = fulFiledById,
      FulFilledDate = fulFilledDate,
      ItemsCount = itemsCount,
      DeliveryCharges = deliveryCharges,
      PaymentStatusId = paymentStatusId,
      PaymentRef = paymentRef,
      ReturnRef = returnRef,
      Weight = weight,
      ItemValue = itemValue,
      OrderRequestVia = orderRequestVia,
      PaymentMethodId = paymentMethodId,
      StationId = stationId,
      Discount = discount,
      Vat = vat,
      TotalTax = totalTax,
      CShippingCharges = cShippingCharges,
      TrackingLock = trackingLock,
      RefNo = refNo,
      CreatedOn = createdOn,
      CreatedBy = createdBy,
      UpdatedOn = updatedOn,
      UpdatedBy = updatedBy,
      StripeCustomerId = stripeCustomerId,
      StripeInvoiceHostURL = stripeInvoiceHostURL,
      StripeInvoicePDFURL = stripeInvoicePDFURL,
      StripeInvoiceId = stripeInvoiceId,
      SaleChannelOrderId = saleChannelOrderId,
      SaleChannelLookupId = saleChannelLookupId
    };
  }
}
public sealed record OrderDeletedId(Guid Value)
{
  public static OrderDeletedId New => new(Guid.NewGuid());
}
