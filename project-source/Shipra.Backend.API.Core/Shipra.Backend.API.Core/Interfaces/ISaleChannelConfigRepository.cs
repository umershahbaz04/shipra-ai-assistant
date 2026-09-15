using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ISaleChannelConfigRepository
{
  Task<dynamic> GetAllSaleChannelConfig(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId);
  Task<SaleChannelConfig> CreateSaleChannelConfig(SaleChannelConfig SaleChannelConfig);
  Task<SaleChannelConfig?> GetSaleChannelConfigById(int SaleChannelConfigId, ClientId clientId);
  Task<SaleChannelConfig?> GetSaleChannelConfigForUpdateById(int SaleChannelConfigId, ClientId clientId);
  Task<bool> DeleteSaleChannelConfig(SaleChannelConfig SaleChannelConfig);
  Task<SaleChannelConfig> UpdateSaleChannelConfig(SaleChannelConfig SaleChannelConfig);
  Task<SaleChannelLookup?> GetSaleChannelLookupById(int SaleChannelLookupId);
  Task<List<SaleChannelLookup>?> GetAllSaleChannelLookupForSelection();
  Task<List<SaleChannelConfig>?> GetSaleChannelConfigBySaleChannelLookupId(int? SaleChannelLookupId, ClientId clientId);
  Task<SaleChannelConfig?> GetClientDefaultSaleChannelConfigById(ClientId clientId);
  Task<SaleChannelConfig?> GetSaleChannelConfigByKey(string SaleChannelKey);
  Task<dynamic> GetSaleChannelByStoreIdForSelection(int storeId, string? clientId,int? roleId = 0);
  Task<dynamic> GetAllSaleChannelForSelection(string? storeId, string? clientId);
  Task<dynamic> GetAllSaleChannelByLookupIdForSelection(int saleChannelLookupId, string? clientId);
  Task<SaleChannelConfig?> GetSaleChannelNameValidate(string? saleChannelName, ClientId clientId);
  Task<string> GetUniqueSaleChannelNameAsync(string saleChannelName, ClientId clientId);
  Task<dynamic> GetSaleChannelInforByConfigId(int? saleChannelConfigId, string? clientId);
}
