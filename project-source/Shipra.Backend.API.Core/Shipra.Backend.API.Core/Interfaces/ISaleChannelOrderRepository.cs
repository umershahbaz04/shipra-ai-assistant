using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface ISaleChannelOrderRepository
{
  Task<SaleChannelOrder> CreateSaleChannelOrder(SaleChannelOrder saleChannelOrder);
  Task<List<SaleChannelOrder>?> GetAllSaleChannelOrder(ClientId clientId, int saleChannelLookupId);
  Task<List<SaleChannelOrder>?> GetSaleChannelOrderListByOrderIds(string? orderIds, int saleChannelLookUpId, ClientId clientId);
}
