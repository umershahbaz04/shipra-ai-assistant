namespace Shipra.Backend.API.Core.Interfaces;
public interface IInventoryRepository
{
  Task<dynamic> GetAllInventorySales(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? productStationIds, string? productSKUs, string? trackingStatusId, bool? IsFulfilled, bool? IsInTransit, string? regionIds, string? saleChannelConfigIds, string storeIds, string clientId);
  Task<dynamic> GetInventorySalesSummary(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? productStationIds, string? productSKUs, string? trackingStatusId, bool? IsFulfilled, bool? IsInTransit, string? regionIds, string? saleChannelConfigIds, string storeIds, string clientId);
}
