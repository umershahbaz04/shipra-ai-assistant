namespace Shipra.Backend.API.Core.Interfaces;
public interface IDashboardRepository
{
	Task<dynamic> GetToBePackedCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetToBeShippedCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetTopSellingItemsCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetAllItemCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetDeliveryRatioCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetTotalActiveCarrierCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetAllProductVariantsCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetLowStockItemsCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetTotalStoreCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetTotalCollected(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetTotalUncollected(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetTotalStockValue(DateTime? createdFrom, DateTime? createdTo, string clientId);
  Task<dynamic> GetTotalPurchaseStockValue(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetTotalCompletedOrderCountWithCarrier(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetAllOrderCountWithCarrier(DateTime? createdFrom, DateTime? createdTo, string clientId);
  Task<dynamic> GetAllOrderCountWithCarrierAndCODAmount(DateTime? createdFrom, DateTime? createdTo, string clientId);
  Task<dynamic> GetInProgressOrderCountWithCarrier(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetReturnedOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetInProgressOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetRegularOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetFulfillableOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetOrdersCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetDelieveredOrderCount(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetCarrierActivityWithDetail(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetCarrierStates(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetCarrierStatesStackedChart(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetCarrierDashboardStats(DateTime? createdFrom, DateTime? createdTo, string clientId);
	Task<dynamic> GetSaleDashboardChannels(int storeId, string clientId, DateTime? createdFrom = null, DateTime? createdTo = null);
	Task<dynamic> GetSaleDashboardProducts(int storeId, int? saleChannelConfigId, int start, int length, string search, string clientId, DateTime? createdFrom = null, DateTime? createdTo = null);
	Task<dynamic> GetSaleDashboardStores(string clientId);
}
