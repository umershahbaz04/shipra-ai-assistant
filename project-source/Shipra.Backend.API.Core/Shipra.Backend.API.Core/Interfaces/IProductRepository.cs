using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IProductRepository
{
  #region product

  Task<dynamic> CreateProductAsync(Product product);
  Task<dynamic> CreateProductOptionsAsync(List<ProductOption> productOptions);
  Task<dynamic> CreateProductStocksAsync(List<ProductStock> productStocks);
  Task<dynamic> CreateProductVariantsAsync(List<ProductVariant> productVariants);
  Task<List<ProductVariant>> GetProductVariantsByProductIdAsync(ProductId productId);
  Task<dynamic> UpdateProductVariantAsync(ProductVariant productVariant);
  Task<dynamic> CreateProductVariantAsync(ProductVariant productVariant);
  Task<dynamic> CreateInventoryBalancesAsync(List<InventoryBalance> inventoryBalances);
  Task<List<InventoryBalance>> GetInventoryBalancesByProductIdAsync(ProductId productId);
  Task<dynamic> UpdateInventoryBalanceAsync(InventoryBalance inventoryBalance);
  Task<dynamic> CreateInventoryBalanceAsync(InventoryBalance inventoryBalance);
  Task<dynamic> CreateInventoryTransactionsAsync(List<InventoryTransaction> inventoryTransactions);
  Task<dynamic> CreateProductVariantOptionsAsync(List<ProductVariantOption> productVariantOptions);
  Task<dynamic> UpdateProductAsync(Product product, List<ProductOption>? productOptions);
  Task<dynamic> UpdateProductAsync(Product product);
  Task<Product?> GetProductByIdAsync(ProductId productId, ClientId? clientId);
  Task<Product?> GetProductBySKUAsync(string sku, ClientId? clientId);
  Task<ProductStock?> GetProductStockBySaleChannelVariantIdAsync(long? saleChannelVariantId);
  Task<Product?> GetProductStockOptionByIdAsync(ProductId productId);
  Task<List<Product>?> GetAllProductAsync(ClientId? clientId);
  Task<Product?> CheckUniqueProductSku(string sku, ClientId? clientId);
  Task<List<Product>?> GetAllProductsByIdsAsync(string productid, string clientid);
  Task<(List<ProductStock> Stock, List<ProductOption> Options, List<ProductMedia> ProductMedia)> GetAllProductStockAndOptionsByIdAsync(Product model, string clientid);
  Task<string?> GetProductNameByProductIdAsync(ProductId productId);

  #endregion

  #region related to product 
  Task<List<ProductOption>?> GetProductOptionByProductIdAsync(ProductId productId);
  Task<List<ProductOptionLookup>?> GetProductOptionLookups();
  Task<List<ProductStock>?> GetProductStockByProductIdAsync(ProductId productId);
  Task<List<dynamic>> GetProductVariantsAndBalancesAsync(ProductId productId, string clientId);
  Task<List<Product>?> GetAllProductByProductIdsAsync(List<string> productIds);
  Task<ProductStock> GetProductStockByIdAsync(long? productStockId);
  //Task<List<dynamic>> DashbaordGetAllProducts();
  Task<ProductStock> UpdateProductStockAsync(ProductStock productStock);
  Task<ProductStock> CreateProductStockAsync(ProductStock productStock);
  Task<dynamic> CreateQuickAddProductOption(List<ProductOption> productOptions, List<ProductStock> productStocks);
  Task<dynamic> GetAllProducts(DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId, string? storeId = "", bool? addOptions = false, int? productStationId = 0);
  Task<List<dynamic>?> GetProductOptionsbYProductIdAsync(string productId, string clientId);
  Task<dynamic> CreateProductOptionAsync(ProductOption option);
  Task<dynamic> CreateProductStockHistory(ProductStockHistory stockHistory);
  Task<dynamic> GetProductStockHistoryByStockIdAsync(long ProductStockId, int TransactionTypeId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir, string clientId);
  Task<dynamic> GetAllLowQuantityProductStock(string? storeId, int? productStationId, bool? isActive, int? availableQty, string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<dynamic> GetAllProductInventoryAsync(string? storeId, int? productStationId, bool? isActive, bool? isAvailable, int? availableQty, string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<dynamic> GetAllProductInventorySummaryAsync(string? storeId, int? productStationId, bool? isActive, bool? isAvailable, int? availableQty, string clientId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string search, int sortCol, string sortDir);
  Task<dynamic> GetProductStockCountAvailability(string productId, DateTime? createdFrom, DateTime? createdTo, int start, int length, string v1, int sortCol, string v2, string clientId);
  Task<dynamic?> GetProductStockPriceByProductStockId(long? productStockId);
  Task<dynamic?> GetProductStocksForSelection(string clientId, int storeId, int stationId = 0, string? search = null, long? productVariantId = null, long? inventoryBalanceId = null);
  Task<ProductStock?> GetProductStockBySKUAsync(string sku);
  Task<ProductVariant?> GetProductVariantBySKUAsync(string sku);
  Task<InventoryBalance?> GetInventoryBalanceByVariantIdAsync(long variantId);
  Task<InventoryBalance?> GetInventoryBalanceByIdAsync(long inventoryBalanceId);
  Task<InventoryBalance?> GetInventoryBalanceByVariantAndStationAsync(long variantId, int stationId);
  Task<ProductVariant?> GetProductVariantByIdAsync(long productVariantId);
  Task<List<dynamic>> GetVariantsWithoutStationAsync(string clientId);

  Task<List<LookupProductStockStatus>> GetLookupProductStockStatusForSelectionQuery();
  Task<bool> CheckAutogeneratedUniqueProductSKU(string? clientId, string generatedSku);
  Task<bool> CheckUniqueProductStockSKU(string sku, string clientId);
  Task<ProductOption> CheckUniqueOptionNameByOptionIdNameAndProductId(string? optionId, ProductId productId, string? optionValue);
  Task<List<dynamic>?> GetAllProductStocksForSaleChannelInventorySync(string? clientId, string? productSKUs);

  #endregion
  #region product link token 
  Task<ProductLinkToken?> GetProductLinkTokenByProductId(ProductId productId, int? storeId, ClientId clientId);
  Task<List<ProductLinkToken>?> GetAllProductLinkTokenByProductId(ProductId productId, ClientId clientId);
  Task<bool> CreateProductLinkToken(ProductLinkToken productLink);
  Task<bool> UpdateProductLinkToken(ProductLinkToken productLink);
  Task<string> GenerateUniqueShortTokenForProductAsync(ClientId clientId, int length = 8);
  Task<dynamic?> GetAllStoreWithTokenByProductId(string productId, string clientId, int storeId = 0);
  #endregion
  #region image gallery
  Task<bool> CreateProductMedia(ProductMedia model);
  Task<List<dynamic>?> GetAllProductMedia(string? clientId);
  Task<bool> CreateImageGallery(ImageGallery model);
  Task<List<ImageGallery>> GetAllImageGalleries(ClientId clientId);
  Task<bool> DeleteProductMediaByID(ProductMedia productMedia);
  Task<ProductMedia> GetProductMediaById(long productMediaId);
  Task<bool> UpdateProductMedia(ProductMedia productMedia);
  Task<List<ProductMedia>> GetProductMediaByProductId(ProductId roductId);
  #endregion
}
