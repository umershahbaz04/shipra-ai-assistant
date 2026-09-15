using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IShopifyRepository
{
  //Shopify Config
  Task<ShopifyConfig> CreateShopifyConfig(ShopifyConfig shopifyConfig);
  Task<ShopifyConfig> UpdateShopifyConfig(ShopifyConfig shopifyConfig);
  Task<bool> DeleteShopifyConfig(ShopifyConfig shopifyConfig);
  Task<ShopifyConfig?> GetShopifyConfigById(int saleChannelConfigId, ClientId clientId);
  Task<dynamic> GetAllShopifyConfig(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId);
  Task<ShopifyConfig?> GetShopifyConfigByClientId(int? SaleChannelConfigId, ClientId clientId); 
}
