using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.DeliveryNoteAggregate;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IDeliveryNoteRepository
{
  #region DeliveryNote
  Task<DeliveryNote> CreateDeliveryNote(DeliveryNote deliverNote);
  Task<DeliveryNote> UpdateDeliveryNote(DeliveryNote deliveryNote);
  Task<dynamic> GetAllDeliveryNote(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, int deliveryNoteStatusId, string? clientId, string? driverIds = null);
  Task<string> GetDeliveryNoteNo(ClientId clientId);
  Task<DeliveryNote?> GetDeliveryNoteById(DeliveryNoteId deliveryNoteId);
  Task<DeliveryNote?> GetDeliveryNoteByNoteNo(string? NoteNo,string? clientId); 
  Task<DeliveryNote?> GetDriverActiveDeliveryNoteToday(DriverId driverId, ClientId clientId);
  Task<bool> DeleteDeliveryNoteById(DeliveryNote deliveryNote);
  Task<bool> MergeDeliveryNotes(string targetNoteId, List<string> sourceNoteIds, DateTime? newCreatedDate, EmployeeId employeeId);
  #endregion

  #region DeliveryNoteDetail
  Task<DeliveryNoteDetail> CreateDeliveryNoteDetail(DeliveryNoteDetail deliveryNoteDetail);
  Task<dynamic> GetDeliveryNoteDetailForDebrief(string deliveryNoteId,string clientId);
  Task<dynamic> GetCompletedDeliveryNoteExpenses(string deliveryNoteId);
  Task<DeliveryNoteDetail?> GetDeliveryNoteDetailByOrderId(OrderId orderId);
  Task<DeliveryNoteDetail?> UpdateDeliveryNoteDetail(DeliveryNoteDetail? deliveryNoteDetail);
  Task<string> GetClientNextNoteNumber(ClientId? clientId);
  Task<dynamic> GetDeliveryNotePaymentInfo(string clinetId);
  Task<dynamic> GetRunSheetInfoByDeliveryNoteId(string deliveryNoteId,string clientid);
  Task<bool> CheckOrderExistsInDND(OrderId? orderId, EmployeeId employeeId);
  Task<dynamic> GetAllDeliveryNoteForDriverById(string? driverId,string clientId);
  Task<dynamic> GetDeliveryNoteDetailForDriverById(string? driverId, string? deliveryNoteId, string clientId);
  Task<dynamic> GetDriverOrdersByDriverId(string? driverId, string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir);
  Task<DeliveryNoteDetail?> GetDeliveryNoteDetailById(DeliveryNoteDetailId? deliveryNoteDetailId);
  Task<IEnumerable<dynamic>> GetDriverLatestDeliveryNoteToday(string driverId, string clientId);
  Task<List<DeliveryNoteDetail>>? GetAllDeliveryNoteDetailByNoteId(DeliveryNoteId? deliveryNoteId);
  Task<List<DeliveryNoteDetail>?> GetAllDeliveryNoteDetailByDeliveryNoteId(DeliveryNoteId? deliveryNoteId);
  Task<bool> DeleteDeliveryNoteDetail(DeliveryNoteDetail deliveryNoteDetail);
  Task<DeliveryNoteDetail> AssignOrderToDeliveryNoteAndCleanup(OrderId orderId, DeliveryNoteId targetNoteId, EmployeeId employeeId);
  #endregion
}
