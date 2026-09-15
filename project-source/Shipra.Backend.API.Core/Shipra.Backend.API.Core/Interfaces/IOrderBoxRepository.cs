using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.OrderBoxAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IOrderBoxRepository
{
  Task<ClientOrderBox?> CreateClientOrderBox(ClientOrderBox model);
  Task<List<ClientOrderBox>> GetAllClientClientOrderBox(ClientId? clientId);
  Task<ClientOrderBox?> GetClientOrderBoxById(int clientOrderBoxId, ClientId clientId);
  Task<ClientOrderBox?> GetDefaultClientOrderBoxById(ClientId clientId);
  Task<bool> UpdateClientOrderBox(ClientOrderBox model);
  Task<bool> CreateOrderBox(OrderBox orderBox);
  Task<List<OrderBox>> GetOrderBoxsByOrderId(OrderId? OrderId);
  Task<bool> UpdateOrderBox(OrderBox orderBox);
  Task<bool> DeleteOrderBox(OrderBox orderBox);
  Task<bool> IsOrderBoxExist(string v, ClientId clientId);
  Task<List<BoxTypeLookup>> GetAllBoxTypeLookup();
}
