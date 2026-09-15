using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.ShopifyOrder;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Application.Features.ProductFeatures.Commands;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;
using ShopifySharp;
using ShopifySharp.Filters;

namespace Shipra.Backend.API.Application.Services.Implementation;
public class SaleChannelProductService : ISaleChannelProductService
{
  private readonly IJobRepository _jobRepository;
  private readonly ISharedClientRepository _sharedClientRepository;
  private readonly IConfigRepository _configRepository;

  private readonly ICountryRepository _countryRepository;

  public SaleChannelProductService(IJobRepository jobRepository, ISharedClientRepository sharedClientRepository, IConfigRepository configRepository, ICountryRepository countryRepository)
  {
    _jobRepository = jobRepository;
    _sharedClientRepository = sharedClientRepository;
    _configRepository = configRepository;
    _countryRepository = countryRepository;
  }
  public async Task SaleChannelsProductManageQueuesAsync()
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
            decimal? productPrice = 0;
            decimal? weight = 0;
            long qtyAvailable = 0;
            bool isTrackInventory = false;

            //1. Validate to get shopify config
            var oShopifyConfig = await _jobRepository.GetShopifyConfigByClientId(saleChannelConfigId, clientId);
            if (oShopifyConfig != null)
            {
              List<SaleChannelProduct> saleChannelExistingProductList = await _jobRepository.GetAllSaleChannelProduct(clientId!, oSaleChannelConfig.SaleChannelLookupId);


              var shopifyProductList = await ListAllProductsOnShop(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken!, Utility.ConvertUTCDateToDateTimeOffSet(DateTime.UtcNow.AddDays(-30)), Utility.ConvertUTCDateToDateTimeOffSet(DateTime.UtcNow));
              var shopifyProductListWithSKU = shopifyProductList.Where(x => x.Variants.Any(v => v.SKU != null && v.SKU != "")).ToList();
              //var productIds = string.Join(',', shopifyProductListWithSKU.Select(x => x.Id));


              var productIds = string.Join(',', shopifyProductListWithSKU.Select(x => x.Id).ToList());
              //2. Check for the existing product List
              var oSaleChannelProductList = await _jobRepository.GetSaleChannelProductListByProductIds(productIds, oSaleChannelsLookupResponse.SaleChannelLookupId!, clientId); 
              //remove existing product
              if (oSaleChannelProductList!.Count > 0)
              {
                foreach (var item in oSaleChannelProductList)
                {
                  var oExistTarget = shopifyProductListWithSKU.FirstOrDefault(x => x.Id == long.Parse(item.ProductId!));
                  if (oExistTarget is not null)
                  {
                    shopifyProductListWithSKU.Remove(oExistTarget);
                  }
                }
              }
              if (shopifyProductListWithSKU is not null && shopifyProductListWithSKU.Count > 0)
              {
                //2.6. Create shipra product for the given shopify products
                #region salechannelproduct
                foreach (var item in shopifyProductListWithSKU!)
                {
                  DateTimeOffset dateTimeOffset = (DateTimeOffset)item.CreatedAt!;
                  DateTime productDate = dateTimeOffset.DateTime;
                  if (item.Variants.Count() > 0)
                  {
                    qtyAvailable = item.Variants.Sum(x => x.InventoryQuantity).GetValueOrDefault();

                    bool allSamePrice = item.Variants.Select(x => x.Price).Distinct().Count() == 1;
                    if (allSamePrice)
                    {
                      productPrice = item.Variants.Select(x => x.Price).FirstOrDefault();
                    }
                    bool allSameWeight = item.Variants.Select(x => x.Weight).Distinct().Count() == 1;
                    if (allSameWeight)
                    {
                      weight = item.Variants.Select(x => x.Weight).FirstOrDefault();
                    }
                    isTrackInventory = item.Variants.Select(x => x.InventoryManagement).Distinct().Count() == 1;
                  }

                  var oShipraProduct = Core.ProductAggregate.Product.CreateProductForShopify(item.Variants.Select(v => v.SKU).FirstOrDefault(), item.Title, ProductCommon.GetDescriptionForShopifyProductItem(item), item.Images.Select(s => s.Src).FirstOrDefault(), item.Variants.Count(), qtyAvailable, storeId, oSaleChannelConfig!.SaleChannelConfigId, productPrice, isTrackInventory, oClient!.DefaultCurrencyId, weight, oClient.DefaultProductCategoryId, clientId, employeeId);
                  await _jobRepository.CreateProductAsync(oShipraProduct);
                  //2.7. Create productStocks of each shopify product
                  foreach (var productStock in item.Variants)
                  {
                    long? longQuantityAvailable = productStock.InventoryQuantity;
                    int intQuantityAvailable = Convert.ToInt32(longQuantityAvailable);
                    var oProductStock = Core.ProductAggregate.ProductStock.CreateProductStock(oShipraProduct.ProductId, productStock.SKU, productStock!.Price!, intQuantityAvailable, 0, oClient!.DefaultProductStationId, ProductCommon.GetVarientDescriptionForShopifyProductItem(productStock), employeeId!, productStock.Id!);
                    await _jobRepository.CreateProductStockAsync(oProductStock,clientId);
                    //2.8. Check for quantity available
                    if (intQuantityAvailable > 0 && productStock.RequiresShipping.GetValueOrDefault())
                    {
                      //2.9. Create productHistory
                      var productStockHistory = GetProductStockHistory(intQuantityAvailable, oProductStock.ProductStockId,employeeId);
                      var addedHistory = await _jobRepository.CreateProductStockHistory(productStockHistory, clientId);
                    }
                  }
                  //2.10. Create productOption
                  foreach (var productOption in item.Options)
                  {
                    foreach (var optionName in productOption.Values)
                    {
                      var oProductOption = Core.ProductAggregate.ProductOption.CreateProductOption(oShipraProduct.ProductId, productOption.Id.ToString(), optionName, 1);
                      await _jobRepository.CreateProductOptionAsync(oProductOption, clientId);
                    }
                  }
                  //2.11. Create sale channel product for shopify products
                  await _jobRepository.CreateSaleChannelProduct(SaleChannelProduct.CreateSaleChannelProduct(oSaleChannelsLookupResponse.SaleChannelLookupId!, item.Id.ToString(), "", JsonConvert.SerializeObject(item, Formatting.Indented), productDate, oShipraProduct.ProductId!, clientId!, employeeId!),clientId); 
                }
              }
              #endregion
              else
              {
                // "Shopify Product not found.";
              }

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
  private ProductStockHistory GetProductStockHistory(int? quantityAvailable, long productStockId,EmployeeId? employeeId)
  {
    return ProductStockHistory.CreateProductStockHistory((int)InventoryTransactionType.ExternalSync, productStockId, quantityAvailable, quantityAvailable, "Shopify Product ", employeeId!);
  }
  public void RecurringJobMethod(string val)
  {
    Console.WriteLine("Recurring job executed every 5 minutes");
  }

  #region Shopify
  public ShopifyOrderDetailResponse FilterOrdersWithNoErrors(ShopifyOrderDetailResponse response)
  {
    // Initialize a new response object to store the filtered results
    var filteredResponse = new ShopifyOrderDetailResponse
    {
      ShopifyOrderDetail = new List<CreateUploadOrderResponseModel>(),
      Errors = new List<ShopifyOrderError>(),
      IsSuccessed = response.IsSuccessed
    };

    // Filter the records where the error messages have count 0
    for (var i = 0; i < response.Errors?.Count; i++)
    {
      if (response.Errors[i].Msg.Count == 0)
      {
        // Add corresponding ShopifyOrderDetail and Errors entries to the new response
        filteredResponse.ShopifyOrderDetail?.Add(response.ShopifyOrderDetail?[i]!);
        filteredResponse.Errors?.Add(response.Errors[i]);
      }
    }

    return filteredResponse;
  }
  #region Shopify
  public async Task<List<ShopifySharp.Product>> ListAllProductsOnShop(string shopDomain, string accesstoken, DateTimeOffset createdFrom, DateTimeOffset createdTo)
  {
    shopDomain = "demoimepress.myshopify.com";
    accesstoken = "shpua_489b8265d3732d74aa797c537fc9dffc";
    var executionPolicy = new LeakyBucketExecutionPolicy();
    var service = new ProductService(shopDomain, accesstoken);
    var allProducts = new List<ShopifySharp.Product>();
    var page = await service.ListAsync(new ProductListFilter
    {
      Limit = 250,
      CreatedAtMin = createdFrom,
      CreatedAtMax = createdTo, 
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
      page = await service.ListAsync(page.GetNextPageFilter());
    }
    return allProducts;
    // TODO: do something with the `allOrders` variable
  }
  #endregion

  #endregion

}
