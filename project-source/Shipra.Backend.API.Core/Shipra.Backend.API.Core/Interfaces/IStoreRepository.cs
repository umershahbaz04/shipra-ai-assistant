using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IStoreRepository
{
  Task<dynamic?> CreateStore(Store store);
  Task<Store?> UpdateStore(Store store);
  Task<dynamic> DisableStore(Store store);
  Task<dynamic> EnableStore(Store store);
  Task<Store?> GetStoreById(int storeid, ClientId client);
  Task<StoreAddress?> GetStoreAddressById(int storeid);
  Task<dynamic?> GetAllStore(string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<List<Store>?> GetStoresForSelection(ClientId? clientId);
  Task<Store> CheckStoreExistAginstClientByStoreName(string? storeName, ClientId? clientId);
  Task<string> GetClientNextStoreCode(ClientId? clientId);
  Task<dynamic> GetStoreAddressByStoreId(int storeId,string clientId);
  Task<bool> CreateStoreAddress(StoreAddress oStoreAddress);
  Task<bool> UpdateStoreAddress(StoreAddress oStoreAddress);
  Task<List<Store>?> GetAllStoreForid(ClientId clientId);
  Task<List<Store>?> GetAllStoresByClient(ClientId clientId);
  Task<List<StoreUploadSampleFile>> GetSampleExcelFileForStoreUpload(int countryId);
  #region store product

  Task<StoreProduct> CheckExistProductOnStore(int? storeId, ProductId? productId);
  Task<bool> CreateStoreProduct(StoreProduct storeProduct);
  Task<bool> UpdateStoreProduct(StoreProduct storeProduct);
  Task<StoreProduct> GetProductStoreById(StoreProductId? storeProductId); 

  #endregion
}
