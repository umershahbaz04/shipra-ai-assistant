using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.OrderAggregate;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelOrderAggregate;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;
using Shipra.Backend.API.Core.StoresAggregate;
using Shipra.Backend.API.SharedKernel.Models;
using Product = Shipra.Backend.API.Core.ProductAggregate.Product;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IJobRepository
{
  Task<Order> CreateOrder(Order order, ClientId clientId);
  Task<OrderAddress> CreateOrderAddress(OrderAddress orderAddress, ClientId clientId);
  Task<OrderNote> CreateOrderNote(OrderNote orderNote, ClientId clientId);
  Task<bool> CreateOrderTax(OrderTax orderTax, ClientId clientId);
  Task<OrderTrackingHistory> CreateOrderTrackingHistory(OrderTrackingHistory orderHistory, ClientId clientId); 
  Task<List<SaleChannelConfigResponseModel>> GetAllAutoSyncSaleChannelsForJob(List<ClientSummaryResponseModel> clientSummaryResponseModels);
  Task<Client?> GetClientById(ClientId clientId);
  Task<string> GetClientNextOrderNo(ClientId clientId);
  Task<string?> GetEmployeeNameById(EmployeeId? employeeId, ClientId clientId);
  Task<List<SaleChannelOrder>?> GetSaleChannelOrderListByOrderIds(string? orderIds, int saleChannelLookUpId, ClientId clientId);
  Task<ShopifyConfig?> GetShopifyConfigByClientId(int? SaleChannelConfigId, ClientId clientId);
  Task<Store?> GetStoreById(int storeid, ClientId? clientId);
  Task<bool> CreateSaleChannelOrder(SaleChannelOrder oSaleChannelOrder, ClientId? clientId);
  Task<SaleChannelOrder> GetSaleChannelOrderBySaleChannelNoByClient(string? refNo, ClientId clientId);
  Task<List<SaleChannelProduct>> GetAllSaleChannelProduct(ClientId clientId, int? saleChannelLookupId);
  Task<List<SaleChannelProduct>?> GetSaleChannelProductListByProductIds(string? productIds, int? saleChannelLookUpId, ClientId clientId);
  Task<SaleChannelProduct> CreateSaleChannelProduct(SaleChannelProduct saleChannelProduct, ClientId clientId); 
  Task<dynamic> CreateProductOptionAsync(ProductOption option, ClientId clientId);
  Task<dynamic> CreateProductStockHistory(ProductStockHistory stockHistory, ClientId clientId); 
  Task<ProductStock> CreateProductStockAsync(ProductStock productStock, ClientId clientId);
  Task<dynamic> CreateProductAsync(Product product);
  Task<List<dynamic>?> GetAllProductStocksForSaleChannelInventorySync(string? clientId, string? productSKUs);
  Task<ProductStock?> GetProductStockBySKUAsync(string sku, ClientId? clientId);
  Task<ProductStock> UpdateProductStockAsync(ProductStock productStock, ClientId? clientId);
  Task<OrderItem> CreateOrderItem(OrderItem orderItem, ClientId clientId);
  Task<ProductStock> GetProductStockByIdAsync(long productStockId, ClientId clientId);
  Task<Product?> GetProductByIdAsync(ProductId productId, ClientId clientId);
  Task<dynamic> UpdateOrder(Order order);
  Task<List<Order>> GetOrdersWithOrderNos(string trackingNos, ClientId clientId);
}
