using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IOrderTrackingHistoryRepository
{
  Task<OrderTrackingHistory> CreateOrderTrackingHistory(OrderTrackingHistory orderHistory);
  Task<OrderNote> CreateOrderNote(OrderNote orderNote);
  Task<OrderNote> GetOrderNoteById(OrderNoteId orderNoteId);
  Task<bool> DeleteOrderById(OrderNote orderNote);
  Task<dynamic?> GetOrderNoteByOrderNo(string orderNo, string clientId);
  Task<OrderNote?> GetOrderNoteByOrderId(OrderId orderId);
  Task<dynamic?> GetOrderTrackingHistoryByOrderNo(string orderId, string clientId);
  Task<List<OrderTrackingHistory>?> GetOrderTrackingHistoryByOrderId(OrderId orderId);
  Task<bool> DeleteOrderTrackingHistory(List<OrderTrackingHistory> orderTrackingHistories);
  Task<dynamic?> GetOrderTrackingHistoryByOrderNoForView(string orderNo, string clientId);
  Task<OrderNote> UpdateOrderNote(OrderNote orderNote);
}
