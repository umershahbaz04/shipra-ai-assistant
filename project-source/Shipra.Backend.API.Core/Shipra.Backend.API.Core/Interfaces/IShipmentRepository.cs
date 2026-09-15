using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.SettingOperationDashboardAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IShipmentRepository
{
  Task<dynamic> GetAllShipments(DateTime? createdFrom, DateTime? createdTo, DateTime? orderFromDate, DateTime? orderToDate, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, string? carrierTrackingStatusIds, string? saleChannelConfigIds, string? salePersonIds, string? countryIds = null, Dictionary<string, AddressFilterModel>? addressFilter = null);
  Task<dynamic> GetAllShipmentTabsCount(DateTime? createdFrom, DateTime? createdTo, DateTime? orderFromDate, DateTime? orderToDate, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeIds, int? orderTypeId, string? carrierIds, int? fullFillmentStatusId, int? paymentStatusId, int? paymentMethodId, string? stationIds, string? carrierTrackingStatusIds, string? saleChannelConfigIds, string? salePersonIds, string? countryIds = null, Dictionary<string, AddressFilterModel>? addressFilter = null);
  Task<List<DefaultShipmentDashboard>?> GetDefaultShipmentDashboard();
  Task<dynamic> GetOrderAddressInforForMapByOrderId(OrderId orderId);
  Task<dynamic?> GetOrderInfoForPopupByOrderNo(string orderNo, string clientId);
  Task<ActiveCarrierPickupLocation?> GetActiveCarrierPickupLocationById(int? activecarrierpickuplocationid);

  #region dashboard tabs related
  Task<dynamic> GetAllShipmentGridClientSettingForDashboard(string? clientId);
  Task<List<ShipmentGridClientSetting>> GetAllShipmentGridClientSetting(ClientId clientId);
  Task<bool> DeleteShipmentGridClientSetting(ShipmentGridClientSetting model);
  Task<ShipmentGridClientSetting> GetShipmentGridClientSettingById(ShipmentGridClientSettingId? ShipmentGridClientSettingId, ClientId clientId);
  Task<bool> CreateShipmentGridClientSetting(ShipmentGridClientSetting oShipmentGridClientSetting);
  Task<ShipmentGridClientSetting> GetShipmentGridClientSettingByShipmentGridColumnId(int shipmentGridColumnId, ClientId clientId);

  Task<bool> UpdateShipmentGridClientSetting(ShipmentGridClientSetting item);
  Task<List<ShipmentGridColumn>> GetAllShipmentGridColumn(ClientId clientId);
  Task<bool> CreateShipmentGridColumn(ShipmentGridColumn oShipmentGridColumn);
  Task<bool> DeleteShipmentGridColumn(ShipmentGridColumn oShipmentGridColumn);
  Task<bool> UpdateShipmentGridColumn(ShipmentGridColumn oShipmentGridColumn);
  Task<ShipmentGridColumn> GetShipmentGridColumnById(int shipmentGridColumnId, ClientId clientId);
  Task<ShipmentGridColumn> GetShipmentGridColumnByName(string? columnName, ClientId clientId);
  Task<dynamic> GetAllShipmentsByDriverReceivableId(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir,string? driverReceivableId, string? clientId);
  Task<dynamic> GetAllShipmentsByReturnReportId(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string? carrierRRId, string? clientId);
  Task<dynamic> GetAllShipmentsDetailByDriverReceivableId(string? driverReceivableId, string clientId);
  Task<List<OrderPODFile>> GetOrderPodFilesByOrderId(OrderId orderId);
  #endregion
}
