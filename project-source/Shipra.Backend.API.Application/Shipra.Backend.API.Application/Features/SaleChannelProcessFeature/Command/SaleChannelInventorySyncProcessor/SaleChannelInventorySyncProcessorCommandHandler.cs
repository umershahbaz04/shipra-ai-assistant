using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml.Drawing.Slicer.Style;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelProductPostProcessor;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;
using Product = ShopifySharp.Product;

namespace Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelInventorySyncProcessor;
public class SaleChannelInventorySyncProcessorCommandHandler : RequestHandlerBase<SaleChannelInventorySyncProcessorCommand, ServiceResultDTO>
{
  private readonly IShopifyRepository _shopifyRepository;
  private readonly IProductRepository _productRepository;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly ISaleChannelProductRepository _saleChannelProductRepository;
  private readonly IProductServiceFactory _productServiceFactory;
  private readonly IInventoryLevelServiceFactory _inventoryLevelServiceFactory;
  public SaleChannelInventorySyncProcessorCommandHandler(IShopifyRepository shopifyRepository, IProductRepository productRepository, ISaleChannelConfigRepository saleChannelConfigRepository, ISaleChannelProductRepository saleChannelProductRepository, IProductServiceFactory productServiceFactory, IInventoryLevelServiceFactory inventoryLevelServiceFactory, IServiceProvider serviceProvider, ILogger<SaleChannelProductPostProcessorCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shopifyRepository = shopifyRepository;
    _productRepository = productRepository;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _saleChannelProductRepository = saleChannelProductRepository;
    _productServiceFactory = productServiceFactory;
    _inventoryLevelServiceFactory = inventoryLevelServiceFactory;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(SaleChannelInventorySyncProcessorCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    BaseResponseDto baseResponse = new BaseResponseDto();
    if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
    {
      if (request.SaleChannelConfigId == 0)
      {
        //Case When Sync Inventory of All Sale Channels of Shopify
        var oSaleChannelConfigList = await _saleChannelConfigRepository.GetSaleChannelConfigBySaleChannelLookupId(request.SaleChannelLookupId, _currentUser.ClientId!);
        if (oSaleChannelConfigList != null)
        {
          foreach (var oSaleChannelConfig in oSaleChannelConfigList)
          {
            await ShopifyRequestProcessor(oSaleChannelConfig.SaleChannelConfigId, request.SaleChannelTowardsShipra, request.ShipraTowardsSaleChannel, request.ProductStockSkus);
          }
        }
      }
      else
      {
        //Case When Sync Inventory of Specific Sale Channels of Shopify
        await ShopifyRequestProcessor(request.SaleChannelConfigId, request.SaleChannelTowardsShipra, request.ShipraTowardsSaleChannel, request.ProductStockSkus);
      }
    }
    else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.WooCommerce)
    {
      if (request.SaleChannelConfigId == 0)
      {
        //Case When Sync Inventory of All Sale Channels of Shopify
        var oSaleChannelConfigList = await _saleChannelConfigRepository.GetSaleChannelConfigBySaleChannelLookupId(request.SaleChannelLookupId, _currentUser.ClientId!);
        if (oSaleChannelConfigList != null)
        {
          foreach (var oSaleChannelConfig in oSaleChannelConfigList)
          {
            await WooCommerceRequestProcessor(oSaleChannelConfig.SaleChannelConfigId, request.SaleChannelTowardsShipra, request.ShipraTowardsSaleChannel, request.ProductStockSkus);
          }
        }
      }
      else
      {
        //Case When Sync Inventory of Specific Sale Channels of Shopify
        await WooCommerceRequestProcessor(request.SaleChannelConfigId, request.SaleChannelTowardsShipra, request.ShipraTowardsSaleChannel, request.ProductStockSkus);
      }
    }
    serviceResult = new ServiceResultDTO(baseResponse);
    return serviceResult;
  }


  #region WooCommerce

  private async Task WooCommerceRequestProcessor(int? saleChannelConfigId, bool? saleChannelTowardsShipra, bool? shipraTowardsSaleChannel, string? productSKUs = null)
  {
    List<ProductWooCommerceModal>? oWooCommerceProductList = null;
    List<ProductVariationWooCommerceModal>? productVariationWooCommerceModals = null;
    //Get the saleChannelConfig by saleChannelConfigId
    var oSaleChannelConfig = await _saleChannelConfigRepository.GetSaleChannelConfigById((int)saleChannelConfigId!, _currentUser.ClientId!);
    if (oSaleChannelConfig is not null && oSaleChannelConfig.SaleChannelConfigId == saleChannelConfigId)
    {
      //Deserialize salechannel config values
      var saleChannelConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(oSaleChannelConfig.Config!);

      //Convert keys To camelcase with deserialize salechannel config  object
      var saleChannelSetting = Utils.ConvertKeysToCamelCase(saleChannelConfig!);

      //Sale channel keys
      var consumerKey = Utils.GetValueFromDictionryByKey("consumerKey", saleChannelSetting);
      var consumerSecret = Utils.GetValueFromDictionryByKey("consumerSecret", saleChannelSetting);
      var shopURL = Utils.GetValueFromDictionryByKey("shopURL", saleChannelSetting);

      oWooCommerceProductList = await GetAllProductOnShopForWooCommerce(shopURL, consumerKey, consumerSecret);
      if (oWooCommerceProductList is not null)
      {
        //Get current user inventory stock list
        List<dynamic>? oProductStockInventoryList = await _productRepository.GetAllProductStocksForSaleChannelInventorySync(_currentUser.ClientIdStr!, productSKUs);
        if (oProductStockInventoryList is not null)
        {
          List<dynamic>? SKUs = null;
          if (!string.IsNullOrEmpty(productSKUs))
          {
            SKUs = productSKUs!.Split(',').Select(item => (dynamic)item).ToList();
          }
          else
          {
            // Extract existing SKUs in a list string
            SKUs = oProductStockInventoryList.Select(x => x.SKU).ToList();
          }

          // Find the product with the specified SKU
          var foundSimpleProductList = oWooCommerceProductList.Where(x => x.Type == "simple" && x.SKU != "" && SKUs!.Contains(x.SKU!)).ToList();

          // Find the product with the specified Type
          var oVariationProductList = oWooCommerceProductList!.Where(x => x.Type == "variable").Select(s => new { s.Variations, s.Id }).ToList();

          if (foundSimpleProductList is not null)
          {
            foreach (var product in foundSimpleProductList)
            {
              if (saleChannelTowardsShipra == true)
              {
                  var oProductVariant = await _productRepository.GetProductVariantBySKUAsync(product.SKU!);
                  if (oProductVariant is not null)
                  {
                    var oInventoryBalance = await _productRepository.GetInventoryBalanceByVariantIdAsync(oProductVariant.ProductVariantId);
                    if (oInventoryBalance is not null)
                    {
                      if ((int)product.StockQuantity! != oInventoryBalance.QuantityAvailable)
                      {
                        int previousQty = oInventoryBalance.QuantityAvailable;
                        oInventoryBalance.UpdateQuantities((int)product.StockQuantity!, (int)product.StockQuantity!, oInventoryBalance.QuantityCommitted);
                        var inventoryBalance = await _productRepository.UpdateInventoryBalanceAsync(oInventoryBalance);
                        if (inventoryBalance is not null)
                        {
                          await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> {
                            InventoryTransaction.Create(
                              oProductVariant.ProductVariantId, 
                              oInventoryBalance.ProductStationId, 
                              (int)InventoryTransactionType.ExternalSync, 
                              Math.Abs((int)product.StockQuantity! - previousQty), 
                              previousQty, 
                              (int)product.StockQuantity!, 
                              "WooCommerce Inventory Sync", 
                              _currentUser.EmployeeId!
                            )
                          });
                        }
                      }
                    }
                  }
                }
                else if (shipraTowardsSaleChannel == true)
                {
                  var oProductVariant = await _productRepository.GetProductVariantBySKUAsync(product.SKU!);
                  if (oProductVariant is not null)
                  {
                    var oInventoryBalance = await _productRepository.GetInventoryBalanceByVariantIdAsync(oProductVariant.ProductVariantId);
                    if (oInventoryBalance is not null)
                    {
                      if ((int)product.StockQuantity! != oInventoryBalance.QuantityAvailable)
                      {
                        await UpdateWooCommerceSimpleProductStockQuantity(shopURL, consumerKey, consumerSecret, product.Id, oInventoryBalance.QuantityAvailable);
                      }
                    }
                  }
                }
            }
          }
          if (oVariationProductList is not null)
          {
            productVariationWooCommerceModals = new List<ProductVariationWooCommerceModal>();
            foreach (var item in oVariationProductList)
            {
              var newList = await GetAllProductVariationOnShopForWooCommerce(shopURL, consumerKey, consumerSecret, item.Id, item.Variations!);
              productVariationWooCommerceModals.AddRange(newList);
            }

            var foundVariationProductList = productVariationWooCommerceModals.Where(x => x.SKU != "" && SKUs!.Contains(x.SKU!)).ToList();
            if (foundVariationProductList is not null)
            {
              foreach (var product in foundVariationProductList)
              {
                if (saleChannelTowardsShipra == true)
                {
                  var oProductVariant = await _productRepository.GetProductVariantBySKUAsync(product.SKU!);
                  if (oProductVariant is not null)
                  {
                    var oInventoryBalance = await _productRepository.GetInventoryBalanceByVariantIdAsync(oProductVariant.ProductVariantId);
                    if (oInventoryBalance is not null)
                    {
                      var stockQuantity = Convert.ToInt32(product.StockQuantity!);
                      if (stockQuantity! != oInventoryBalance.QuantityAvailable)
                      {
                        int previousQty = oInventoryBalance.QuantityAvailable;
                        oInventoryBalance.UpdateQuantities(stockQuantity!, stockQuantity!, oInventoryBalance.QuantityCommitted);
                        var inventoryBalance = await _productRepository.UpdateInventoryBalanceAsync(oInventoryBalance);
                        if (inventoryBalance is not null)
                        {
                          await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> {
                            InventoryTransaction.Create(
                              oProductVariant.ProductVariantId, 
                              oInventoryBalance.ProductStationId, 
                              (int)InventoryTransactionType.ExternalSync, 
                              Math.Abs(stockQuantity! - previousQty), 
                              previousQty, 
                              stockQuantity!, 
                              "WooCommerce Inventory Sync", 
                              _currentUser.EmployeeId!
                            )
                          });
                        }
                      }
                    }
                  }
                }
                else if (shipraTowardsSaleChannel == true)
                {
                  var oProductVariant = await _productRepository.GetProductVariantBySKUAsync(product.SKU!);
                  if (oProductVariant is not null)
                  {
                    var oInventoryBalance = await _productRepository.GetInventoryBalanceByVariantIdAsync(oProductVariant.ProductVariantId);
                    if (oInventoryBalance is not null)
                    {
                      if (Convert.ToInt32(product.StockQuantity!) != oInventoryBalance.QuantityAvailable)
                      {
                        await UpdateWooCommerceVariationsProductStockQuantity(shopURL, consumerKey, consumerSecret, product.ProductId, product.VariantId, oInventoryBalance.QuantityAvailable);
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  }

  private async Task<List<ProductWooCommerceModal>> GetAllProductOnShopForWooCommerce(string baseURL, string consumerKey, string consumerSecret)
  {
    // Initialize HttpClient
    using (HttpClient client = new HttpClient())
    {
      // Set the base URL for the WooCommerce API
      client.BaseAddress = new Uri(baseURL + "//wp-json/wc/v3/");

      // Set the authorization header
      var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

      try
      {
        var requestUrl = $"products";
        // Make a request to retrieve orders (adjust the endpoint as needed)
        HttpResponseMessage response = await client.GetAsync(requestUrl);

        // Check if the request was successful
        if (response.IsSuccessStatusCode)
        {
          // Read and display the response content
          string responseBody = await response.Content.ReadAsStringAsync();
          var data = JsonConvert.DeserializeObject<List<ProductWooCommerceModal>>(responseBody);
          return data!;
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Error: {response.StatusCode} - {response.ReasonPhrase}");
        }
      }
      catch (Exception ex)
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Exception: {ex.Message}");
      }
    }
  }
  private async Task<List<ProductVariationWooCommerceModal>> GetAllProductVariationOnShopForWooCommerce(string baseURL, string consumerKey, string consumerSecret, int productId, List<object> variationIds)
  {
    // Initialize HttpClient
    using (HttpClient client = new HttpClient())
    {
      // Set the base URL for the WooCommerce API
      client.BaseAddress = new Uri(baseURL + "//wp-json/wc/v3/");

      // Set the authorization header
      var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

      try
      {
        var requestUrl = $"products/{productId}/variations?include={string.Join(",", variationIds.Select(item => item.ToString()))}";
        // Make a request to retrieve orders (adjust the endpoint as needed)
        HttpResponseMessage response = await client.GetAsync(requestUrl);

        // Check if the request was successful
        if (response.IsSuccessStatusCode)
        {
          // Read and display the response content
          string responseBody = await response.Content.ReadAsStringAsync();
          var dataList = JsonConvert.DeserializeObject<List<ProductVariationWooCommerceModal>>(responseBody);
          foreach (var product in dataList!)
          {
            product.ProductId = productId;
          }
          return dataList!;
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Error: {response.StatusCode} - {response.ReasonPhrase}");
        }
      }
      catch (Exception ex)
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Exception: {ex.Message}");
      }
    }
  }
  private async Task UpdateWooCommerceSimpleProductStockQuantity(string baseURL, string consumerKey, string consumerSecret, int? productId, int? newQty)
  {

    // Product data for inventory update (modify as needed)
    string updatedInventoryData = $@"
        {{
            ""stock_quantity"": {newQty}
        }}";


    // Create HttpClient instance with WooCommerce API credentials
    using (HttpClient client = new HttpClient())
    {
      // Set the base URL for the WooCommerce API
      client.BaseAddress = new Uri(baseURL + "//wp-json/wc/v3/");

      // Create URL for updating inventory for the specific product ID
      var updateInventoryUrl = $"products/{productId}";
      // Set the base URL for the WooCommerce API
      var byteArray = Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}");
      client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

      // Prepare the content
      var content = new StringContent(updatedInventoryData, Encoding.UTF8, "application/json");

      // Make the request to update the product inventory
      HttpResponseMessage response = await client.PutAsync(updateInventoryUrl, content);

      // Check if the request was successful
      if (response.IsSuccessStatusCode)
      {
        Console.WriteLine("Product inventory updated successfully!");
      }
      else
      {
        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
      }
    }
  }
  private async Task UpdateWooCommerceVariationsProductStockQuantity(string baseURL, string consumerKey, string consumerSecret, int? productId, int? variantId, int? newQty)
  {

    // Product data for inventory update (modify as needed)
    string updatedInventoryData = $@"
        {{
            ""stock_quantity"": {newQty}
        }}";

    // Create HttpClient instance with WooCommerce API credentials
    using (HttpClient client = new HttpClient())
    {
      // Set the base URL for the WooCommerce API
      client.BaseAddress = new Uri(baseURL + "//wp-json/wc/v3/");
      // Create URL for updating inventory for the specific product ID
      string updateInventoryUrl = $"products/{productId}/variations/{variantId}";
      // Set the base URL for the WooCommerce API
      var byteArray = Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}");
      client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

      // Prepare the content
      var content = new StringContent(updatedInventoryData, Encoding.UTF8, "application/json");

      // Make the request to update the product inventory
      HttpResponseMessage response = await client.PutAsync(updateInventoryUrl, content);

      // Check if the request was successful
      if (response.IsSuccessStatusCode)
      {
        Console.WriteLine("Product inventory Variations updated successfully!");
      }
      else
      {
        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
      }
    }
  }

  #endregion
  #region Shopify
  private async Task ShopifyRequestProcessor(int? saleChannelConfigId, bool? saleChannelTowardsShipra, bool? shipraTowardsSaleChannel, string? productSKUs = null)
  {
    var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(saleChannelConfigId, _currentUser.ClientId!);
    if (oShopifyConfig is not null)
    {
      if (oShopifyConfig.AccessToken is not null)
      {
        //Get all product of requested store
        var oShopifyShopProductList = await ListAllProductsOnShop(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken);
        if (oShopifyShopProductList is not null)
        {
          //Get current user inventory stock list
          List<dynamic>? oProductStockInventoryList = await _productRepository.GetAllProductStocksForSaleChannelInventorySync(_currentUser.ClientIdStr!, productSKUs);
          if (oProductStockInventoryList is not null)
          {
            List<dynamic>? SKUs = null;
            if (!string.IsNullOrEmpty(productSKUs))
            {
              SKUs = productSKUs!.Split(',').Select(item => (dynamic)item).ToList();
            }
            else
            {
              // Extract existing SKUs in a list string
              SKUs = oProductStockInventoryList.Select(x => x.SKU).ToList();
            }
            // Find the product with the specified SKU
            var foundProductList = oShopifyShopProductList.Where(p => p.Variants.Any(v => SKUs!.Contains(v.SKU!) && v.SKU != ""));
            if (foundProductList is not null)
            {
              foreach (var product in foundProductList)
              {
                foreach (var variant in product.Variants)
                {
                  if (variant is not null)
                  {
                    if (saleChannelTowardsShipra == true)
                    {
                      var oProductVariant = await _productRepository.GetProductVariantBySKUAsync(variant.SKU);
                      if (oProductVariant is not null)
                      {
                        var oInventoryBalance = await _productRepository.GetInventoryBalanceByVariantIdAsync(oProductVariant.ProductVariantId);
                        if (oInventoryBalance is not null)
                        {
                          if ((int)variant.InventoryQuantity! != oInventoryBalance.QuantityAvailable)
                          {
                            int previousQty = oInventoryBalance.QuantityAvailable;
                            oInventoryBalance.UpdateQuantities((int)variant.InventoryQuantity!, (int)variant.InventoryQuantity!, oInventoryBalance.QuantityCommitted);
                            var inventoryBalance = await _productRepository.UpdateInventoryBalanceAsync(oInventoryBalance);
                            if (inventoryBalance is not null)
                            {
                              await _productRepository.CreateInventoryTransactionsAsync(new List<InventoryTransaction> {
                                InventoryTransaction.Create(
                                  oProductVariant.ProductVariantId, 
                                  oInventoryBalance.ProductStationId, 
                                  (int)InventoryTransactionType.ExternalSync, 
                                  Math.Abs((int)variant.InventoryQuantity! - previousQty), 
                                  previousQty, 
                                  (int)variant.InventoryQuantity!, 
                                  "Shopify Inventory Sync", 
                                  _currentUser.EmployeeId!
                                )
                              });
                            }
                          }
                        }
                      }
                    }
                    else if (shipraTowardsSaleChannel == true)
                    {
                      var listAllInventoryItems = await ListAllInventoryItemsOnShop(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken);
                      // Fetch the current product variant
                      //var productVariant = listAllInventoryItems.Select(x => x.InventoryItemId);
                      //if (productVariant is not null)
                      //{
                      //  // Update the inventory quantity (modify as needed)
                      //  productVariant.InventoryQuantity = 50;
                      //  // Save the changes
                      //  ProductVariant updatedVariant = await service.Up((long)variant!.Id!, productVariant);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  }
  private async Task<List<InventoryLevel>> ListAllInventoryItemsOnShop(string shopDomain, string accesstoken)
  {
    var service = _inventoryLevelServiceFactory.Create(shopDomain, accesstoken);
    var allInventoryLevel = new List<InventoryLevel>();
    var page = await service.ListAsync(new InventoryLevelListFilter
    {
      Limit = 250
    });
    // Keep adding the orders to the list of all Product until there are no

    while (true)
    {
      allInventoryLevel.AddRange(page.Items);
      if (!page.HasNextPage)
      {
        // We've reached the end of the list
        break;
      }
      // There is at least one more page, list it and loop again
      page = await service.ListAsync(page.GetNextPageFilter());
    }
    return allInventoryLevel;
    // TODO: do something with the `allOrders` variable
  }
  private async Task<List<Product>> ListAllProductsOnShop(string shopDomain, string accesstoken)
  {
    var productService = _productServiceFactory.Create(shopDomain, accesstoken);
    var allProducts = new List<Product>();
    var page = await productService.ListAsync(new ProductListFilter
    {
      Limit = 250
    });
    // Keep adding the orders to the list of all Product until there are no

    while (true)
    {
      allProducts.AddRange(page.Items);
      if (!page.HasNextPage)
      {
        // We've reached the end of the list
        break;
      }
      // There is at least one more page, list it and loop again
      page = await productService.ListAsync(page.GetNextPageFilter());
    }
    return allProducts;
    // TODO: do something with the `allOrders` variable
  }

  #endregion
}
