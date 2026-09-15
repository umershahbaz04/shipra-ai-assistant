using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
 
using Shipra.Backend.API.Core.Models; 
 
  
using Shipra.Backend.API.Core.StoresAggregate;
 

namespace Shipra.Backend.API.Core.Interfaces;
public interface ICarrierRepository
{
  #region Carrier
  Task<dynamic> CreateCarrier(Carrier model);
  Task<dynamic> UpdateCarrier(Carrier model);
  Task<bool> DeleteCarrier(Carrier model);
  Task<Carrier?> GetCarrierById(int id);
  Task<Carrier?> GetCarrierFromMasterDbById(int id);
  Task<dynamic> GetAllCarriers(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId);
  Task<List<dynamic>> DashbaordGetAllCarriers(DateTime? createdFrom, DateTime? createdT);
  #endregion

  #region Active carrier
  Task<dynamic> CreateActiveCarrier(ActiveCarrier model);
  Task<dynamic> UpdateActiveCarrier(ActiveCarrier model);

  Task<bool> DeleteActiveCarrier(ActiveCarrier model);
  Task<bool> deletepickLocation(ActiveCarrierPickupLocation model);
 
  Task<dynamic> UpdatepickLocation(ActiveCarrierPickupLocation model);

  Task<ActiveCarrier?> GetActiveCarrierById(int activeCarrierId, ClientId? clientId);
  Task<ActiveCarrier?> GetActiveCarrierByCarrierId(int carrierId, ClientId? clientId);
  Task<ActiveCarrier?> GetActiveCarrierByActiveCarrierId(int activeCarrierId, ClientId? clientId);
  Task<ActiveCarrier?> GetActiveCarrierByCarrierIdAndUserName(int carrierId, string? userName, ClientId? clientId);
  Task<dynamic> GetAllActiveCarriers(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, int countryId, int deliveryServiceId, ClientId? clientId = null);
  Task<List<ActiveCarriersForSelectionResponseModel>> GetAllActiveCarriersForSelection(string clientId);
  Task<ActiveCarrierPickupLocation?> GetActiveCarrierLocationbyId(int ActiveCarrierPickupLocationId, ClientId client);
  Task<List<dynamic>> DashbaordGetAllActiveCarriers(string clientId);

  Task<ActiveCarrier> GetActiveCarrierByCarrierAlias(int? carrierId, string? carrierAlias, ClientId? clientId);
  Task<ActiveCarrier> GetActiveCarrierByAlias(string? carrierAlias, ClientId? clientId);
  Task<List<ActiveCarrier>> GetActiveCarrieriersByCarrierId(int? carrierId, ClientId? clientId);
  #endregion

  #region Webhook
  Task<bool> CreateWebhookUrlHash(WebhookUrlHash webhookUrlHash);
  Task<WebhookUrlHash> CheckWebhookUrlHashByClientAndCarrierId(int carrierId, Guid clientId);
  Task<WebhookUrlHash> CheckWebhookUrlHashByContractIdAndCarrierId(int carrierId, int carrierContractTypeId);
  #endregion
  #region carrier with location
  //Task<List<Carrier>> GetAllClientCarrier();
  //Task<List<Carrier>> GetAllCarrierWithoutClientCarrier();
  Task<dynamic> GetAllCarrierWithServiceAndLocation(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, int countryId, int deliveryServiceId, string clientId);
  Task<dynamic> GetCarrierWithServiceAndLocationByCarrierId(int carrierId);
  Task<List<DeliveryService>> GetAllDeliveryService();
  Task<List<CarrierDeliveryService>> GetAllCarrierDeliveryServices(int carrierId);
  Task<List<CarrierLocation>> GetAllCarrierLocations(int carrierId);
  Task<ShipraContractCarrier?> GetShipraContractCarrierByCarrierId(int carrierId);
  Task<ShipraContractCarrier?> GetShipraContractCarrierByContractId(int shipraContractCarrierId);
  Task<dynamic> GetAllShipraContractCarrierWithServiceAndLocation(int start, int length, string search, int sortCol, string sortDir, int countryId, int deliveryServiceId, int deliveryTypeId, bool? isForAdmin, ClientId? clientId = null);
  Task<dynamic> GetShipraContractByShipraContractCarrierId(int shipraContractCarrierId);
  Task<ShipraContractCarrier> GetShipraContractCarrierByAlias(string? carrierAlias, int? carrierId);
  Task<bool> CreateShipraContractCarrier(ShipraContractCarrier shipraContractCarrier);
  Task<ShipraContractCarrier> GetShipraCarrierContractByCarrierId(int? shipraContractCarrierId);
  Task<bool> UpdateShipraCarrierContractByCarrierId(ShipraContractCarrier shipraContractCarrier);
  #endregion
  Task<List<ShipraContractClientCarrier>?> GetAllShipraContractClientCarriersByClientId(ClientId? clientId);
  Task<bool> DeleteShipraContractClientCarrierAsync(ShipraContractClientCarrier carrier);
  Task<ShipraContractClientCarrier> GetShipraContractClientCarrierById(int shipraContractClientCarrierId);
  Task<bool> AddShipraContractClientCarrierAsync(ShipraContractClientCarrier carrierc);
  Task<List<ShipraContractClientCarrier>> GetAllShipraContractClientCarrier(ClientId clientId);
  Task<ShipraContractClientCarrier?> GetShipraContractClientCarrier(ClientId clientId, int shipraContractCarrierId);
  Task<dynamic> GetAllActiveCarrierForCreateOrderSelectionFilterByCountry(string? clientIdStr, int? countryId);
  #region MyRegion

  Task<bool> CreateActiveCarrierPickupLocation(ActiveCarrierPickupLocation acp);
  Task<List<ActiveCarrierPickupLocationForSelectionResponseModel>> GetAllActiveCarrierPickupLocationByActiveCarrierIdForSelection(int? activeCarrierId, string clientId);
  Task<CarrierLocation> GetCarrierLocationByCarrier(Carrier carrier);
  Task<List<CivilEntityExtended>> GetCivilEntityExtended(int carrierId);
  Task<List<CarrierLocationWithCountryResponseModel>> GetAllCarrierLocationsByCarrier(Carrier carrier);
  #endregion
  Task<dynamic> GetAllClientRateAsync(PriceCalculatorFilter filter);
}
