using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ISaleChannelProductRepository
{
  Task<SaleChannelProduct> CreateSaleChannelProduct(SaleChannelProduct saleChannelProduct);
  Task<List<SaleChannelProduct>?> GetAllSaleChannelProduct(ClientId clientId, int? saleChannelLookupId);
  Task<List<SaleChannelProduct>?> GetSaleChannelProductListByProductIds(string? productIds, int? saleChannelLookUpId, ClientId clientId);
}
