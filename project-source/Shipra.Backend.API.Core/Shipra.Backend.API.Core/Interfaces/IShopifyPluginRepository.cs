using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.StoresAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IShopifyPluginRepository : IShopifyRepository
{
  Task<List<Store>?> GetStoresForSelection(ClientId? clientId);
  Task<SaleChannelConfig> UpdateSaleChannelConfig(SaleChannelConfig SaleChannelConfig);
  Task<SaleChannelConfig?> GetSaleChannelConfigForUpdateById(int SaleChannelConfigId, ClientId clientId);
  Task<bool> DeleteSaleChannelConfig(SaleChannelConfig SaleChannelConfig);
  Task<Client?> GetClientById(ClientId clientId);
  Task<SaleChannelConfig> CreateSaleChannelConfig(SaleChannelConfig SaleChannelConfig);
  Task<SaleChannelConfig?> GetSaleChannelNameValidate(string? saleChannelName, ClientId clientId);
  Task<SaleChannelConfig?> GetSaleChannelConfigByKey(string saleChannelKey,string clientId);
}
