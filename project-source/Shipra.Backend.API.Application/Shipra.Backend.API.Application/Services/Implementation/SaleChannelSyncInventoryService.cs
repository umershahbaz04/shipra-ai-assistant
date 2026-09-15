using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class SaleChannelSyncInventoryService : ISaleChannelSyncInventoryService
{
  private readonly IJobRepository _jobRepository;
  private readonly ISharedClientRepository _sharedClientRepository;
  private readonly IConfigRepository _configRepository;

  private readonly ICountryRepository _countryRepository;
  private readonly IInventoryLevelServiceFactory _inventoryLevelServiceFactory;
  private readonly IProductServiceFactory _productServiceFactory;

  public SaleChannelSyncInventoryService(IJobRepository jobRepository, ISharedClientRepository sharedClientRepository, IConfigRepository configRepository, ICountryRepository countryRepository, IInventoryLevelServiceFactory inventoryLevelServiceFactory, IProductServiceFactory productServiceFactory)
  {
    _jobRepository = jobRepository;
    _sharedClientRepository = sharedClientRepository;
    _configRepository = configRepository;
    _countryRepository = countryRepository;
    _inventoryLevelServiceFactory = inventoryLevelServiceFactory;
    _productServiceFactory = productServiceFactory;
  }
  public async Task SaleChannelsInventorySyncManageQueuesAsync()
  {
    var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.AdminControlPanKey, (int)EnumEnvironmentType.Dev);
    if (mcconfig is null)
    {
      throw new EntityNotFoundException("Mcconfig", "Admin ControlPan Value");
    }
    var result = await _sharedClientRepository.GetAllClients(mcconfig.Value!);
    if (!string.IsNullOrEmpty(result))
    {
      var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<List<ClientSummaryResponseModel>>>(result);

      var saleChannelConfig = await _jobRepository.GetAllAutoSyncSaleChannelsForJob(deseralisedResponse!.result!);
      //group by SaleChannelLookupId for better identification
      var groupSaleLookups = saleChannelConfig.GroupBy(x => x.SaleChannelLookupId);
      foreach (var oSaleChannelLookups in groupSaleLookups)
      {
        // group by client for client id 
        //var allSameSalechannelByClients = saleChannelConfig.Where(x => x.SaleChannelConfigId == saleChannelConfigId).GroupBy(x => x.ClientId);

        foreach (var oSaleChannelsLookupResponse in oSaleChannelLookups!)
        {
          var oSaleChannelConfig = SaleChannelConfig.ConverSaleChannelConfig(oSaleChannelsLookupResponse.SaleChannelConfigId, oSaleChannelsLookupResponse.StoreId, oSaleChannelsLookupResponse.SaleChannelLookupId, oSaleChannelsLookupResponse.SaleChannelName!, new ClientId(oSaleChannelsLookupResponse!.ClientId!.Value), new EmployeeId(oSaleChannelsLookupResponse!.ClientId!.Value), oSaleChannelsLookupResponse.SCSettingConfig, oSaleChannelsLookupResponse.SaleChannelKey, oSaleChannelsLookupResponse.UserName, oSaleChannelsLookupResponse.Password);

          int storeId = 0;

          int? saleChannelConfigId = oSaleChannelConfig.SaleChannelConfigId;

          var clientId = new ClientId(oSaleChannelsLookupResponse!.ClientId!.Value);
          var employeeId = new EmployeeId(oSaleChannelsLookupResponse!.ClientId!.Value);
          var oStore = await _jobRepository.GetStoreById(storeId, clientId!);
          var oClient = await _jobRepository.GetClientById(clientId!);
          if (oSaleChannelsLookupResponse.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
          {
            //1. Validate to get shopify config
            var oShopifyConfig = await _jobRepository.GetShopifyConfigByClientId(saleChannelConfigId, clientId);
            if (oShopifyConfig != null)
            {
              bool? SaleChannelTowardsShipra = true;
              bool? ShipraTowardsSaleChannel = false;
              string? ProductStockSkus = string.Empty;
              await ShopifyRequestProcessor(oSaleChannelConfig.SaleChannelConfigId, SaleChannelTowardsShipra, ShipraTowardsSaleChannel, clientId, employeeId, ProductStockSkus);
            }
            else
            {
              continue;
            }
          }
        }
      }
      // Schedule a recurring job that runs every 5 minutes
      //RecurringJob.AddOrUpdate(
      //    "RecurringJob_Every1Minutes",            // A unique identifier for the recurring job
      //    () => RecurringJobMethod(mcconfig.Value!),              // Method to call
      //   "*/1 * * * *"                          // Cron expression for every 5 minutes
      //); 
      //foreach (var item in deseralisedResponse!.result!)
      //{

      //} 
    }
  }
  public void RecurringJobMethod(string val)
  {
    Console.WriteLine("Recurring job executed every 5 minutes");
  }


  #region Shopify
  private async Task ShopifyRequestProcessor(int? saleChannelConfigId, bool? saleChannelTowardsShipra, bool? shipraTowardsSaleChannel, ClientId clientId, EmployeeId employeeId, string? productSKUs = null)
  {
    var oShopifyConfig = await _jobRepository.GetShopifyConfigByClientId(saleChannelConfigId, clientId!);
    if (oShopifyConfig is not null)
    {
      if (oShopifyConfig.AccessToken is not null)
      {
        //Get all product of requested store
        var oShopifyShopProductList = await ListAllProductsOnShop(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken);
        if (oShopifyShopProductList is not null)
        {
          //Get current user inventory stock list
          List<dynamic>? oProductStockInventoryList = await _jobRepository.GetAllProductStocksForSaleChannelInventorySync(clientId.Value.ToString()!, productSKUs);
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
                      var oProductStock = await _jobRepository.GetProductStockBySKUAsync(variant.SKU, clientId);
                      if (oProductStock is not null)
                      {
                        if ((int)variant.InventoryQuantity! != oProductStock.QuantityAvailable)
                        {
                          int? previousQty = oProductStock.QuantityAvailable;
                          oProductStock.UpdateProductStockQuantityAvailable((int)variant.InventoryQuantity!, employeeId!);
                          var productStock = await _jobRepository.UpdateProductStockAsync(oProductStock, clientId);
                          if (productStock is not null)
                          {
                            await _jobRepository.CreateProductStockHistory(ProductStockHistory.CreateProductStockHistory((int)InventoryTransactionType.ExternalSync, productStock.ProductStockId, previousQty, (int)variant.InventoryQuantity, "Shopify Inventory Sync", employeeId!), clientId);
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
  private async Task<List<ShopifySharp.Product>> ListAllProductsOnShop(string shopDomain, string accesstoken)
  {
    shopDomain = "demoimepress.myshopify.com";
    accesstoken = "shpua_489b8265d3732d74aa797c537fc9dffc";
    var productService = _productServiceFactory.Create(shopDomain, accesstoken);
    var allProducts = new List<ShopifySharp.Product>();
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
