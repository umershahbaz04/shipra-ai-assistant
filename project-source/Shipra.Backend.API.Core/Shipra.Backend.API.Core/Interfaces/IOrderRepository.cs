using Org.BouncyCastle.Asn1.Ocsp;
using Shipra.Backend.API.Core.AccountAggregate;
using Shipra.Backend.API.Core.CarrierReturnReportAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IOrderRepository : IOrderTrackingHistoryRepository
{
  Task<Order> CreateOrder(Order order);
  Task<dynamic> UpdateOrder(Order order);
  Task<dynamic> DeleteOrder(Order order);
  Task<Order?> GetOrderById(OrderId orderId, ClientId clientId);
  Task<bool> ArchiveOrderAsync(OrderArchive archive);
  Task<dynamic> GetAllArchveOrders(string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<dynamic> ExcelExportOrdersArchive(string clientId, DateTime? date);
  Task<List<OrderArchive>> GetArchiveOrdersByArchiveNo(ClientId clientId, string? ArchiveNo);

  Task<List<Order>?> GetOrdersByReturnReportId(CarrierRRId carrierRRId, ClientId clientId);
  Task<dynamic> DashboardGetTotalNoofOrdersPlaced(DateTime? createdFrom, DateTime? createdTo, string clientId);
  Task<int> GetOrderCountByClientId(ClientId clientId);
  Task<List<Order>> GetAllOrdersByPaymentSettlementId(CarrierPaymentSettlementId? CarrierPaymentSettlementId);
  #region store
  Task<Store?> GetStoreByid(int storeid);
  #endregion
  Task<List<ClientCarrierTrackingStatus>> GetAllCarrierTrackingStatusesByClientId(ClientId clientId);
  #region order address
  Task<OrderAddress> CreateOrderAddress(OrderAddress orderAddress);
  Task<OrderAddress> GetOrderAddressById(long orderAddressId);
  Task<OrderAddress> UpdateOrderAddress(OrderAddress orderAddress);
  Task<StoreWitAddresshModel> GetStoreWithAddress(int? storeId, string? clientId);
  #endregion
  #region order item
  Task<OrderItem> CreateOrderItem(OrderItem orderItem);
  Task<List<OrderItem>> GetOrderItemsByOrderId(OrderId? orderId);
  Task<OrderItem> GetOrderItemByIdAndOrderItemId(OrderId orderId, OrderItemId orderItemId);
  Task<OrderItem> UpdateOrderItem(OrderItem? orderItem);
  Task<List<Order>> GetOrdersWithOrderNos(string? trackingNos, ClientId clientId);
  Task<dynamic> BatchUpdateOrders(List<Order> orders);
  Task<Order?> GetOrderByOrderNo(string orderNo, ClientId clientId);
  Task<Order?> GetOrderByStripeInvoiceId(string stripeInvoiceId);
  Task<List<AllOrderPaymentLinkResponseModel>> GetAllOrderPaymentLinks(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, int? paymentLinkStatusId = 0, bool? IsForPayoutRequest = false, string trackingPageUrl = "", DateTime? schedualDateFrom = null, DateTime? schedualDateTo = null);
  Task<dynamic> GetAllOrdersForGeneratePaymentLink(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, string clientId);
  Task<string?> GetAllSaleChannelConfigIdsByEmployeeIDs(string? clientId, string employeeIds);
  Task<dynamic> GetAllOrders(DateTime? createdFrom, DateTime? createdTo, DateTime? orderFromDate, DateTime? orderToDate, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, bool readyForAssignment, int? carrierAssign = 0, string? saleChannelConfigIds = "", string? salePersonIds = "", string? countryIds = null, string? carrierTrackingStatusIds = null, Dictionary<string, AddressFilterModel>? addressFilter = null, string? OrderLabels = null, bool? isWithoutStation = false);
  Task<dynamic> GetAllOrdersForSalePerson(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, bool readyForAssignment, bool assigned, string salePersonId);
  Task<dynamic> GetAllOrdersByTenantId(string? clientId, DateTime? createdFrom, DateTime? createdTo);
  Task<dynamic> GetOrderInfoByOrderNo(string orderNo, string clientId);
  Task<dynamic> GetOrderItemsInfoByOrderId(string orderId, string clientId);
  Task<dynamic> UpdateOrderPaymentStatus(List<Order> orders);
  Task<List<Order>> GetOrdersByOrderNos(string orderNos, ClientId clientId);
  Task<List<Order>> GetOrdersByOrderIds(string orderIds, ClientId clientId);
  Task<dynamic> GetAllCODPendings(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, string? carrierIds, int? orderTypeId);
  Task<dynamic> UpdateCustomerEmail(OrderAddress orderAddress);
  Task<List<OrderAggregate.Dto.AdvanceSearchOrderDto>> AdvanceSearchOrders(string? searchType, string? searchQuery, int start, int length, string clientId);
  Task<Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel> CheckMobileNoDuplicate(string mobileNo, string clientId);
  Task<List<Shipra.Backend.API.Core.Models.CheckMobileNoDuplicateResponseModel>> CheckMobileNosDuplicateBulk(List<string> mobileNos, string clientId);
  #endregion

  #region return file
  Task<dynamic> GetOrderDetailByReturnReportFile(string orderIds, string trackingNosc, string clientId);
  Task<string> GetClientNextOrderNo(ClientId clientId, EmployeeId employeeId, int roleId = 0, int? saleChannelConfigId = 0);
  Task<dynamic> GetOrderForDriverById(string? orderId, string? driverId, string clientId);
  Task<OrderItem> DeleteOrderItem(OrderItem oOrderItem);
  Task<OrderPODFile> CreateOrderPODFiles(OrderPODFile orderPODFile);
  #region order tax
  Task<bool> CreateOrderTax(OrderTax orderTax);
  Task<OrderTax?> GetOrderTaxById(OrderTaxId orderTaxId);
  Task<List<OrderTax>?> GetAllOrderTaxByOrderId(OrderId orderId);
  Task<bool> UpdateOrderTax(OrderTax orderTax);
  Task<dynamic> GetAllCarrierPendingForReturnShipments(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId);
  Task<List<OrderUplaodSampleFile>> GetOrderUplaodSampleFile();
  Task<bool> CreateOrderDeleted(OrderDeleted orderDeleted);
  Task<long> GetOrderCount(ClientId clientId);
  Task<List<Order>> GetAllOrdersByRefNos(string? refNos, ClientId? clientId);
  #endregion
  #endregion
  #region Order label
  Task<List<ClientOrderLabel>> GetAllClientOrderLabelByOrderId(ClientId clientId, OrderId orderId);
  Task<ClientOrderLabel> GetClientOrderLabelById(ClientOrderLabelId? ClientOrderLabelId, ClientId clientId);
  Task<bool> CreateClientOrderLabel(ClientOrderLabel clientOrderLabel);
  Task<bool> DeleteClientOrderLabel(ClientOrderLabel clientOrderLabel);
  Task<List<ClientOrderLabel>> GetAllClientOrderLabelForSelection(ClientId clientId);
  #endregion
  #region client Order label lookp
  Task<bool> CreateClientOrderLabelLookup(ClientOrderLabelLookup clientOrderLabel);
  Task<bool> UpdateClientOrderLabelLookup(ClientOrderLabelLookup clientOrderLabel);
  Task<bool> DeleteClientOrderLabelLookup(ClientOrderLabelLookup clientOrderLabel);
  Task<ClientOrderLabelLookup> GetClientOrderLabelLookupId(int? clientOrderLabelLookupId, ClientId clientId);
  Task<dynamic> GetAllClientOrderLabelLookup(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string? clientId);
  Task<List<ClientOrderLabelLookup>> GetAllClientOrderLabelLookupForSelection(ClientId clientId);
  Task<dynamic> GetAllOrderStatusReport(DateTime? filterDate, string clientId, int carrierId = 0);
  #endregion
  #region OrderDraft


  Task<bool> CreateOrderDraft(OrderDraft orderDraft);
  Task<bool> UpdateOrderDraft(OrderDraft orderDraft);
  Task<OrderDraft> GetDraftOrderById(long OrderDraftId, ClientId clientId);
  Task<bool> DeleteDraftOrder(OrderDraft orderDraft);
  Task<List<OrderDraft>> GetAllDraftOrders(ClientId? clientId);
  Task<bool> DeleteArchiveOrder(OrderArchive archive);
  Task<int> GetAllDraftOrdersCount(ClientId? clientId);
  Task<ClientConfigSetting?> GetClientConfigSetting(ClientId clientId);
  Task<string?> GetMapApiKey();
  #endregion
}


