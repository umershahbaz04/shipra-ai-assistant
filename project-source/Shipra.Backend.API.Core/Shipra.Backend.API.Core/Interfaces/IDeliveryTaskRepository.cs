using Shipra.Backend.API.Core.DeliveryTaskAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IDeliveryTaskRepository
{
  Task<DeliveryTask?> CheckDeliveryTaskExists(OrderId? orderId);
  Task<DeliveryTask> CreateDeliveryTask(DeliveryTask deliverytask);
  Task<DeliveryTask?> UpdateDeliveryTask(DeliveryTask deliveryTask);
  Task<bool> DeleteDeliveryTaskById(DeliveryTask deliveryTask);
  Task<dynamic> GetAllDeliveryTask(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId, int? driverAssignedStatus, string? countryIds = null, string? carrierTrackingStatusIds = null, string? storeIds = null, string? deliveryTaskStatusIds = null, string? driverIds = null, bool? includeDriver = true, Dictionary<string, AddressFilterModel>? addressFilter = null, string? salePersonIds = null, string? orderLabels = null, int? duplicateStatus = null, int? deliveryNoteStatusId = null);
  Task<DeliveryTask?> GetDeliveryTaskById(DeliveryTaskId deliveryTaskId);
  Task<DeliveryTask?> GetDeliveryTaskByOrderId(OrderId orderId);
  Task<List<DeliveryTask>?> GetUnAssignedDeliveryTaskListByOrderId(List<OrderId> orderIds);
  Task<dynamic> GetAllPendingForReturnShipments(DateTime? createdFrom, DateTime? createdTo, int start, int length, string? search, int sortCol, string? sortDir, string clientId);
  Task<dynamic> GetAllCODCollectionPendingsMyCarrier(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId);
  Task<dynamic> GetAllCODCollectionsMyCarrier(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, string? carrierIds, int? orderTypeId);
  Task<List<DeliveryTask>?> GetDeliveryTaskListByOrderId(List<OrderId> orderIds);
  Task<List<DeliveryTaskStatusLookup>> GetAllDeliveryTaskStatusForSelection();
}
