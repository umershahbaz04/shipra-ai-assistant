using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;

namespace Shipra.Backend.API.Core.OrderAggregate;
public class Order
{
  public Order() { }
  public OrderId? OrderId { get; set; }
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
  public int? ActiveCarrierPickupLocationId { get; private set; }
  public string? CarrierTrackingNo { get; private set; }
  public string? CarrierTrackingStatus { get; private set; }
  public int? CarrierTrackingStatusId { get; set; }
  public DateTime? CarrierAssignDate { get; private set; }
  public DateTime? CarrierLastUpdateDateTime { get; private set; }
  public CarrierPaymentSettlementId? CarrierPaymentSettlementId { get; private set; }
  public DateTime? CarrierPaymentSettlementDate { get; private set; }
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
  public string? OrderLabels { get; private set; }
  public DateTime? CreatedOn { get; private set; }
  public EmployeeId? CreatedBy { get; private set; }
  public DateTime? UpdatedOn { get; private set; }
  public EmployeeId? UpdatedBy { get; private set; }
  public string? StripeCustomerId { get; private set; }
  public string? StripeInvoiceHostURL { get; private set; }
  public string? StripeInvoicePDFURL { get; private set; }
  public string? StripeInvoiceId { get; private set; }
  public int? OrderDeliveryTypeId { get; private set; }
  public SaleChannelOrderId? SaleChannelOrderId { get; private set; }
  public int? SaleChannelLookupId { get; private set; }
  public int? CarrierContractTypeId { get; private set; }
  public bool? IsShipraInvoiceCharged { get; private set; }
  public Guid? InvoiceId { get; private set; }
  public int? DInvoiceId { get; private set; }
  public int? PInvoiceId { get; private set; } 

  public static Order CreateOrder(ClientId clientId, int? storeId, int? saleChannelConfigId, int? orderTypeId, string? orderNo, DateTime? orderDate, long? orderAddressId, decimal? amount, string? description, string? remarks, int? itemsCount, decimal? cShippingCharges, int? paymentStatusId, decimal? weight, decimal? itemValue, int? orderRequestVia, decimal? discount, decimal totalTax, int? paymentMethodId, int staionId, int? carrierTrackingStatusId, string? refNo, EmployeeId? createdBy, string? carrierTrackingStatus = null, int? saleChannelLookupId = null, int? orderDeliveryTypeId = (int)EnumOrderDeliveryType.Forward)
  {
    var order = new Order()
    {
      OrderId = OrderId.New,
      ClientId = clientId,
      StoreId = storeId,
      SaleChannelConfigId = saleChannelConfigId,
      SaleChannelLookupId = saleChannelLookupId,
      OrderTypeId = orderTypeId,
      OrderNo = orderNo,
      OrderDate = orderDate,
      OrderAddressId = orderAddressId,
      Amount = amount,
      Description = description,
      Remarks = remarks,
      ItemsCount = itemsCount,
      CShippingCharges = cShippingCharges,
      CarrierTrackingStatusId = carrierTrackingStatusId,
      CarrierTrackingStatus = carrierTrackingStatus,
      CarrierLastUpdateDateTime = DateTime.UtcNow,
      PaymentStatusId = paymentStatusId,
      Weight = weight,
      ItemValue = itemValue,
      OrderRequestVia = orderRequestVia,
      Discount = discount,
      TotalTax = totalTax,
      RefNo = refNo,
      FullFillmentStatusId = (int)EnumFullfillmentStatus.Unfulfilled,//order will be unfulfill when create
      PaymentMethodId = paymentMethodId,
      OrderDeliveryTypeId = orderDeliveryTypeId,
      CreatedOn = DateTime.UtcNow,
      StationId = staionId == 0 ? null : staionId,
      CreatedBy = createdBy,
    };
    return order;
  }

  public static Order CreateOrderForShopify(ClientId clientId, int? storeId, string? orderNumber, int? saleChannelConfigId, int? stationId, DateTime? orderDate, string description, string? remarks, decimal? amount, decimal? discount, decimal? vAT, decimal? weight,
  string? financialStatus, long? orderAddressId, decimal? lineItemPrice, EmployeeId? createdBy, int? orderTypeId, string? fulfillmentStatus)
  {
    Order oOrder = new Order();
    if (orderTypeId != null && orderTypeId == (int)EnumOrderType.Regular)
    {
      oOrder = new Order()
      {
        OrderId = OrderId.New,
        ClientId = clientId,
        StoreId = storeId,
        OrderNo = orderNumber,
        SaleChannelConfigId = saleChannelConfigId,
        StationId = stationId == 0 ? null : stationId,
        OrderDate = orderDate,
        Description = description,
        Remarks = remarks,
        Amount = amount,
        Discount = discount,
        Vat = vAT,
        Weight = weight,
        PaymentStatusId = (financialStatus == null || financialStatus == "pending") ? (int)EnumPaymentStatus.Unpaid : (int)EnumPaymentStatus.Paid,
        PaymentMethodId = (financialStatus == null || financialStatus == "pending") ? (int)EnumPaymentMethod.COD : (int)EnumPaymentMethod.PP,
        OrderTypeId = (int)EnumOrderType.Regular,
        OrderRequestVia = (int)EnumOrderRequestVia.Shopify,
        CarrierTrackingStatusId = (int)EnumCarrierTrackingStatus.OrderPlaced,
        OrderAddressId = orderAddressId,
        ItemValue = lineItemPrice,
        DeliveryCharges = 0,
        CreatedBy = createdBy,
        CreatedOn = DateTime.UtcNow,
      };
    }
    else if (orderTypeId != null && orderTypeId == (int)EnumOrderType.FullFilable)
    {
      oOrder = new Order()
      {
        OrderId = OrderId.New,
        ClientId = clientId,
        StoreId = storeId,
        OrderNo = orderNumber,
        SaleChannelConfigId = saleChannelConfigId,
        StationId = stationId == 0 ? null : stationId,
        OrderDate = orderDate,
        Description = description,
        Remarks = remarks,
        Amount = amount,
        Discount = discount,
        Vat = vAT,
        Weight = weight,
        PaymentStatusId = (financialStatus == null && financialStatus == "pending") ? (int)EnumPaymentStatus.Unpaid : (int)EnumPaymentStatus.Paid,
        PaymentMethodId = (financialStatus == null && financialStatus == "pending") ? (int)EnumPaymentMethod.COD : (int)EnumPaymentMethod.PP,
        OrderTypeId = (int)EnumOrderType.FullFilable,
        OrderRequestVia = (int)EnumOrderRequestVia.Shopify,
        CarrierTrackingStatusId = (int)EnumCarrierTrackingStatus.OrderPlaced,
        OrderAddressId = orderAddressId,
        ItemValue = lineItemPrice,
        DeliveryCharges = 0,
        CreatedBy = createdBy,
        CreatedOn = DateTime.UtcNow,
        FullFillmentStatusId = fulfillmentStatus == null ? (int)EnumFullfillmentStatus.Unfulfilled : (int)EnumFullfillmentStatus.Fulfilled
      };
    }
    return oOrder;
  }

  public void UnAssignFromCarrier(int carrierTrackingStatusId, string? carrierTrackingStatus, EmployeeId? userId)
  {
    CarrierId = null;
    ActiveCarrierId = null;
    CarrierAssignDate = null;
    CarrierTrackingNo = null;
    CarrierTrackingStatus = CarrierTrackingStatus;
    CarrierLastUpdateDateTime = DateTime.UtcNow;
    CarrierTrackingStatusId = carrierTrackingStatusId;
    UpdatedBy = userId;
    CarrierTrackingStatus = "UnAssign from Carrier";
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateAssignToCarrierTrackingNos(int carrierId, int activeCarrierId,int activeCarrierPickupLocationId, int carrierContractTypeId, string barcode, string trackingStatus, int carrierTrackingStatusId, EmployeeId? userId)
  {
    CarrierId = carrierId;
    ActiveCarrierId = activeCarrierId;
    ActiveCarrierPickupLocationId = activeCarrierPickupLocationId;
    CarrierAssignDate = DateTime.UtcNow;
    CarrierTrackingNo = barcode;
    CarrierTrackingStatus = trackingStatus;
    CarrierTrackingStatusId = carrierTrackingStatusId;
    CarrierContractTypeId = carrierContractTypeId;
    //remove srtipe link when assign to carrier
    StripeInvoiceHostURL = null;
    StripeInvoicePDFURL = null;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateCreateCarrierPaymentSettlement(CarrierPaymentSettlementId? carrierPaymentSettlementId, EmployeeId? userId)
  {
    CarrierPaymentSettlementId = carrierPaymentSettlementId;
    //TrackingLock = true;
    //PaymentStatusId = (int)EnumPaymentStatus.Paid;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void DeleteCarrierPaymentSettlement(int paymentMethodId, EmployeeId? userId)
  {
    if (paymentMethodId == (int)EnumPaymentMethod.COD)
    {
      PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
    }
    CarrierPaymentSettlementId = null;
    TrackingLock = false;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateOrder(int? storeId, int? orderTypeId, DateTime? orderDate, long? orderAddressId, decimal? amount, string? description, string? remarks, int? itemsCount, decimal? cShippingCharges, int? paymentStatusId, decimal? weight, decimal? itemValue, int? orderRequestVia, decimal discount, decimal vAT, int? paymentMethodId, int staionId, string? refNo, EmployeeId? updatedBy,int? saleChannelConfigId,int? saleChannelLookupId)
  {
    StoreId = storeId;
    OrderTypeId = orderTypeId;
    OrderDate = orderDate;
    OrderAddressId = orderAddressId;
    Amount = amount;
    Description = description;
    Remarks = remarks;
    ItemsCount = itemsCount;
    PaymentStatusId = paymentStatusId;
    CShippingCharges = cShippingCharges;
    Weight = weight;
    ItemValue = itemValue;
    OrderRequestVia = orderRequestVia;
    Discount = discount;
    RefNo = refNo;
    Vat = vAT;
    PaymentMethodId = paymentMethodId;
    StationId = staionId == 0 ? null : staionId;
    UpdatedBy = updatedBy;
    UpdatedOn = DateTime.UtcNow;
    SaleChannelConfigId = saleChannelConfigId;
    SaleChannelLookupId = saleChannelLookupId;
  }

  public void UpdateOrderAmount(decimal? amount, EmployeeId? userId)
  {
    Amount = amount;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateOrderCarrierStatus(int? carrierTrackingStatusId, EmployeeId employeeId, string? carrierTrackingStatus)
  {
    CarrierTrackingStatusId = carrierTrackingStatusId;
    CarrierLastUpdateDateTime = DateTime.UtcNow;
    CarrierTrackingStatus = carrierTrackingStatus;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateSaleChannelConfigId(int? saleChannelConfigId, EmployeeId employeeId)
  {
    SaleChannelConfigId = saleChannelConfigId;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateOrderStationId(int? stationId)
  {
    StationId = stationId == 0 ? null : stationId;
  }  
  public void RemoveSaleChannelConfigId(EmployeeId employeeId)
  {
    SaleChannelConfigId = null;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateOrderFulfillmentStatus(int fullfilmentStatusId, EmployeeId? employeeId = null, DateTime? fulFilledDate = null)
  {
    FullFillmentStatusId = fullfilmentStatusId;
    FulFiledById = employeeId;
    FulFilledDate = fulFilledDate;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateOrderPaymentStatus(int? paymentStatusId, EmployeeId employeeId)
  {
    PaymentStatusId = paymentStatusId;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateStatusForReturnReport(CarrierRRId? carrierRrid, EmployeeId? userId, int trackingStatusId, string? carrierTrackingStatus, bool trackingLock = false)
  {
    CarrierRRId = carrierRrid;
    CarrierTrackingStatusId = trackingStatusId;
    CarrierLastUpdateDateTime = DateTime.UtcNow;
    CarrierTrackingStatus = carrierTrackingStatus;
    UpdatedBy = userId;
    UpdatedOn = DateTime.UtcNow;
    TrackingLock = trackingLock;
  }

  public void UpdateOrderByStripeInvoiceInformation(string stripeCustomerId, string stripeInvoiceHostURL, string stripeInvoicePDFURL, string stripeInvoiceId)
  {
    StripeCustomerId = stripeCustomerId;
    StripeInvoiceHostURL = stripeInvoiceHostURL;
    StripeInvoicePDFURL = stripeInvoicePDFURL;
    StripeInvoiceId = stripeInvoiceId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateOrderPaymentStatusViaStripeInvoicePaymentStatus(int? paymentStatusId, int? paymentMethodId)
  {
    PaymentStatusId = paymentStatusId;
    PaymentMethodId = paymentMethodId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateOrderForCreateDeliveryTask(EmployeeId employeeId, int? carrierId)
  {
    CarrierId = carrierId;
    ActiveCarrierId = carrierId;
    CarrierTrackingStatusId = (int)EnumCarrierTrackingStatus.AssignInHouse;
    CarrierTrackingStatus = "Assign in house";
    CarrierLastUpdateDateTime = DateTime.UtcNow;
    //remove srtipe link when assign to carrier
    StripeInvoiceHostURL = null;
    StripeInvoicePDFURL = null;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateOrderForUnAssignDeliveryTask(EmployeeId employeeId)
  {
    CarrierId = null;
    CarrierTrackingStatusId = (int)EnumCarrierTrackingStatus.UnAssignfromcarrier;
    CarrierTrackingStatus = "UnAssign from carrier";
    CarrierLastUpdateDateTime = DateTime.UtcNow;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateOrderStatusAsPendingForReturn(EmployeeId? employeeId)
  {
    CarrierTrackingStatusId = (int)EnumCarrierTrackingStatus.PendingForReturn;
    CarrierTrackingStatus = "Pending For Return";
    CarrierLastUpdateDateTime = DateTime.UtcNow;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateOrderForRevertDeliveryTask(EmployeeId? employeeId)
  {
    CarrierId = null;
    CarrierTrackingStatusId = (int)EnumCarrierTrackingStatus.OrderPlaced;
    CarrierTrackingStatus = "Order Placed";
    CarrierLastUpdateDateTime = DateTime.UtcNow;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateOrderForSaleChannel(SaleChannelOrderId? saleChannelOrderId, int? saleChannelLookupId, EmployeeId? employeeId)
  {
    SaleChannelLookupId = saleChannelLookupId;
    SaleChannelOrderId = saleChannelOrderId;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void MarkStatusPaidAndLockOrder(EmployeeId? employeeId, bool trackingLock = true)
  {
    PaymentStatusId = (int)EnumPaymentStatus.Paid;
    TrackingLock = trackingLock;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void MarkStatusUnPaidAndUnLockOrder(int? paymentMethodId, EmployeeId? employeeId)
  {
    if (paymentMethodId == (int)EnumPaymentMethod.COD)
    {
      PaymentStatusId = (int)EnumPaymentStatus.Unpaid;
    }
    TrackingLock = false;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateAssignToCarrierFaild(int assignedTocarrierFaild, EmployeeId? employeeId, string? carrierTrackingStatus)
  {
    CarrierTrackingStatusId = assignedTocarrierFaild;
    CarrierTrackingStatus = carrierTrackingStatus;
    CarrierLastUpdateDateTime = DateTime.UtcNow;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }

  public void UpdateSerViceCharges(decimal deliveryCharges)
  {
    DeliveryCharges = deliveryCharges;
  }

  public void TotalProcessingUpdateOrderWithPrepaidStatusAndPaid()
  {
    PaymentMethodId = (int)EnumPaymentMethod.PP;
    PaymentStatusId = (int)EnumPaymentStatus.Paid;
  }

  public void UpdateOrderLabel(string labelsToAddCsv, EmployeeId employeeId)
  {
    OrderLabels = labelsToAddCsv;
    UpdatedBy = employeeId;
    UpdatedOn = DateTime.UtcNow;
  }
  public void UpdateCarrierManual(int? carrierId, int? activeCarrierId, string? carrierTrackingNo)
  {
    CarrierId = carrierId;
    ActiveCarrierId = activeCarrierId;
    CarrierAssignDate = DateTime.UtcNow;
    CarrierTrackingStatus = "Assigned To Carrier";
    CarrierTrackingStatusId = (int)EnumCarrierTrackingStatus.AssignedTocarrier;
    CarrierTrackingNo = carrierTrackingNo;

  }
  public static Order CreateOrderWithArchiveData(
    OrderId? orderId = null,
    ClientId? clientId = null,
    int? storeId = null,
    int? saleChannelConfigId = null,
    int? orderTypeId = null,
    string? orderNo = null,
    DateTime? orderDate = null,
    long? orderAddressId = null,
    decimal? amount = null,
    int? carrierId = null,
    int? activeCarrierId = null,
    int? activeCarrierPickupLocationId = null,
    string? carrierTrackingNo = null,
    string? carrierTrackingStatus = null,
    int? carrierTrackingStatusId = null,
    DateTime? carrierAssignDate = null,
    DateTime? carrierLastUpdateDateTime = null,
    CarrierPaymentSettlementId? carrierPaymentSettlementId = null,
    DateTime? carrierPaymentSettlementDate = null,
    CarrierRRId? carrierRRId = null,
    string? description = null,
    string? remarks = null,
    int? fullFillmentStatusId = null,
    EmployeeId? fulFilledById = null,
    DateTime? fulFilledDate = null,
    int? itemsCount = null,
    decimal? deliveryCharges = null,
    int? paymentStatusId = null,
    string? paymentRef = null,
    string? returnRef = null,
    decimal? weight = null,
    decimal? itemValue = null,
    int? orderRequestVia = null,
    int? paymentMethodId = null,
    int? stationId = null,
    decimal? discount = null,
    decimal? vat = null,
    decimal? totalTax = null,
    decimal? cShippingCharges = null,
    bool? trackingLock = null,
    string? refNo = null,
    string? orderLabels = null,
    DateTime? createdOn = null,
    EmployeeId? createdBy = null,
    DateTime? updatedOn = null,
    EmployeeId? updatedBy = null,
    string? stripeCustomerId = null,
    string? stripeInvoiceHostURL = null,
    string? stripeInvoicePDFURL = null,
    string? stripeInvoiceId = null,
    int? orderDeliveryTypeId = null,
    SaleChannelOrderId? saleChannelOrderId = null,
    int? saleChannelLookupId = null,
    int? carrierContractTypeId = null,
    bool? isShipraInvoiceCharged = null,
    Guid? invoiceId = null,
    int? dInvoiceId = null,
    int? pInvoiceId = null
)
  {
    return new Order
    {
      OrderId = orderId ?? OrderId.New,
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
      ActiveCarrierPickupLocationId = activeCarrierPickupLocationId,
      CarrierTrackingNo = carrierTrackingNo,
      CarrierTrackingStatus = carrierTrackingStatus,
      CarrierTrackingStatusId = carrierTrackingStatusId,
      CarrierAssignDate = carrierAssignDate,
      CarrierLastUpdateDateTime = carrierLastUpdateDateTime ?? DateTime.UtcNow,
      CarrierPaymentSettlementId = carrierPaymentSettlementId,
      CarrierPaymentSettlementDate = carrierPaymentSettlementDate,
      CarrierRRId = carrierRRId,
      Description = description,
      Remarks = remarks,
      FullFillmentStatusId = fullFillmentStatusId,
      FulFiledById = fulFilledById,
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
      StationId = stationId == 0 ? null : stationId,
      Discount = discount,
      Vat = vat,
      TotalTax = totalTax,
      CShippingCharges = cShippingCharges,
      TrackingLock = trackingLock,
      RefNo = refNo,
      OrderLabels = orderLabels,
      CreatedOn = createdOn ?? DateTime.UtcNow,
      CreatedBy = createdBy,
      UpdatedOn = updatedOn,
      UpdatedBy = updatedBy,
      StripeCustomerId = stripeCustomerId,
      StripeInvoiceHostURL = stripeInvoiceHostURL,
      StripeInvoicePDFURL = stripeInvoicePDFURL,
      StripeInvoiceId = stripeInvoiceId,
      OrderDeliveryTypeId = orderDeliveryTypeId,
      SaleChannelOrderId = saleChannelOrderId,
      SaleChannelLookupId = saleChannelLookupId,
      CarrierContractTypeId = carrierContractTypeId,
      IsShipraInvoiceCharged = isShipraInvoiceCharged,
      InvoiceId = invoiceId,
      DInvoiceId = dInvoiceId,
      PInvoiceId = pInvoiceId
    };
  }
   
}

public sealed record OrderId(Guid Value)
{
  public static OrderId New => new(Guid.NewGuid());
}
