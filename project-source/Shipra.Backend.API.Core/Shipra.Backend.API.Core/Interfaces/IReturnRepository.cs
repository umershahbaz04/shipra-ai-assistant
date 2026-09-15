using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ReturnAggregate;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IReturnRepository
{
  Task<bool> CreateClientReturnReson(ClientReturnReason clientReturnReason);
  Task<dynamic> GetAllClientReturnReason(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string v);
  Task<ClientReturnReason> GetClientReturnResonByClientReasonId(int clientReturnReasonId, ClientId clientId);
  Task<bool> UpdateClientReturnReson(ClientReturnReason clientReturnReason);
  Task<dynamic> GetAllOrderReturn(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId, int? returnReasonId = null, int? returnStatusId = null);

  Task<List<ClientReturnReason>> GetAllClientReturnReasonForSelection(ClientId clientId);
  Task<List<RefundTypeLookup>> GetAllRefundTypeLookupForSelection(ClientId clientId);
  Task<List<ReturnStatusLookup>> GetAllReturnStatusLookupForSelection(ClientId clientId);
  Task<bool> CreateReturn(Return returnObj, string? clientId);
  Task<Order> GetOrderById(OrderId orderId, ClientId clientId);
  Task<Order> GetOrderByOrderNo(string orderNo, ClientId clientId);
  Task<ReturnOrderWrapperResponseModel> GetReturnReportDataByOrderIdForTracking(string? orderNo, string? clientId);
  Task<bool> CreateReturnProduct(ReturnProduct returnProduct, string? clientId);
  Task<Return> GetReturnByReturnId(ReturnId? returnId);
  Task<bool> UpdateReturn(Return objReturn);
  Task<bool> CreateActivity(ReturnActivityLog returnActivityLog, string? clientId);
  Task<bool> CreateReturnTrackingHistory(ReturnTrackingHistory model, string? clientId);
  Task<dynamic> GetReturnTrackingHistory(string? returnId,string? clientId);
  Task<PaymentLink?> GetPaymentLinkByOrderId(OrderId? orderId, ClientId clientId);

  Task<dynamic?> GetStoreAndCustomerAddressByOrderId(string? orderId, string clientId);
  Task<dynamic> GetAllOrderItems(string orderId, string clientId);
  Task<dynamic> GetOrderTaxInfo(string orderId, string clientId);
}
