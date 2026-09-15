using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using ShopifySharp;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.CreateProductFromShipraToShopify;
public class CreateProductFromShipraToShopifyCommandHandler : RequestHandlerBase<CreateProductFromShipraToShopifyCommand, ServiceResultDTO>
{
  private readonly IShopifyRepository _shopifyRepository;
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;
  private readonly ISaleChannelProductRepository _saleChannelProductRepository;
  private readonly IProductRepository _productRepository;
  private readonly IProductCategoryRepository _productCategoryRepository;
  private readonly IStoreRepository _storeRepository;

  public CreateProductFromShipraToShopifyCommandHandler(IShopifyRepository shopifyRepository, ISaleChannelConfigRepository SaleChannelConfigRepository, ISaleChannelProductRepository saleChannelProductRepository, IProductRepository productRepository, IProductCategoryRepository productCategoryRepository, IStoreRepository storeRepository, IServiceProvider serviceProvider, ILogger<CreateProductFromShipraToShopifyCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shopifyRepository = shopifyRepository;
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
    _saleChannelProductRepository = saleChannelProductRepository;
    _productRepository = productRepository;
    _productCategoryRepository = productCategoryRepository;
    _storeRepository = storeRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateProductFromShipraToShopifyCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    var successProductList = new List<long?>();
    BaseResponseDto baseResponse = new BaseResponseDto();
    try
    {
      var oSaleChannelConfig = await _SaleChannelConfigRepository.GetSaleChannelConfigById(request.SaleChannelConfigId, _currentUser.ClientId!);
      if (oSaleChannelConfig == null)
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Sale Channel Config not found");
      }

      if (oSaleChannelConfig?.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
      {
        //1. Get the comma free list of productIds by splitting the productIds request
        List<string> productIds = request.ProductIds!.Split(',').ToList();
        List<Core.ProductAggregate.Product>? oProductList = await _productRepository.GetAllProductByProductIdsAsync(productIds);
        if (oProductList is not null && oProductList.Count > 0)
        {
          //2.3. Get the shoipfy config by saleChannelConfigId
          var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(request.SaleChannelConfigId, _currentUser.ClientId!);
          if (oShopifyConfig != null)
          {
            var service = new ProductService(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken!);
            foreach (var item in oProductList)
            {
              var oProductCat = await _productCategoryRepository.GetProductCategoryById(item.ProductCategoryId.GetValueOrDefault());
              var oStore = await _storeRepository.GetStoreById(request.StoreId.GetValueOrDefault(), _currentUser.ClientId!);
              ShopifySharp.Product oProduct = new ShopifySharp.Product()
              {
                Title = item.ProductName,
                BodyHtml = item.Description,
                Vendor = oStore!.StoreName,
                Images = new List<ShopifySharp.ProductImage>()
                  {
                     new ShopifySharp.ProductImage()
                     {
                       Src = item.FeatureImage
                     }
                  },
                ProductType = oProductCat!.CategoryName,
              };

              if (item.HaveOptions == true)
              {
                var oProductStockList = await _productRepository.GetProductStockByProductIdAsync(item.ProductId!);
                var oProductOptionList = await _productRepository.GetProductOptionByProductIdAsync(item.ProductId!);
                List<ShopifySharp.ProductVariant> oProductVariants = new List<ShopifySharp.ProductVariant>();
                int count = 0;
                foreach (var itemStock in oProductStockList!)
                {
                  count++;
                  ShopifySharp.ProductVariant oVariant = new ShopifySharp.ProductVariant()
                  {
                    Title = itemStock.VarientOption,
                    SKU = itemStock.Sku,
                    Position = count,
                    InventoryPolicy = "deny",
                    FulfillmentService = "manual",
                    InventoryManagement = "shopify",
                    Price = itemStock.Price,
                    Option1 = itemStock.VarientOption,
                    Taxable = true,
                    RequiresShipping = true,
                    InventoryQuantity = itemStock.QuantityAvailable,
                    Weight = item.Weight,
                    WeightUnit = "kg"
                  };
                  oProductVariants.Add(oVariant);
                }
                // Transform the list
                var oProductOptions = oProductOptionList!
                                      .GroupBy(po => po.OptionId)
                                      .Select((g, index) =>
                                              {
                                                // Check if the Key is an integer 
                                                string name = EnumProductOptionLookupHelper.GetEnumString(EnumProductOptionLookupHelper.GetEnumDefault(g.Key.GetValueOrDefault(5)));
                                                return new ShopifySharp.ProductOption
                                                {
                                                  Position = index + 1, // Incremental position starting from 1
                                                  Name = name, // Use the checked name
                                                  Values = g.Select(po => po.OptionValue).ToList() // Collect all OptionValues
                                                };
                                              })
                                              .ToList();

                if (oProductVariants != null && oProductVariants.Count > 0)
                {
                  oProduct.Variants = oProductVariants;
                }
                else
                {
                  oProduct.Variants = new List<ShopifySharp.ProductVariant>();
                }
                if (oProductOptions != null && oProductOptions.Count > 0)
                {
                  oProduct.Options = oProductOptions;
                }
                else
                {
                  oProduct.Options = new List<ShopifySharp.ProductOption>();
                }
              }
              var createdProduct = await service.CreateAsync(oProduct);
              if (createdProduct.Id > 0)
              {

              }
              //var (success, response) = await CreateProductAsync(oShopifyConfig.ShopDomain!, oShopifyConfig.AccessToken!, oProduct);
              //if (success)
              //{
              //  baseResponse = new BaseResponseDto
              //  {
              //    Data = response,
              //    Message = "Shopify product created successfully for the products."
              //  };
              //}
              //else
              //{
              //  baseResponse = new BaseResponseDto
              //  {
              //    Data = response,
              //    Message = "Failed to create product." + response
              //  };
              //  // Parse and display errors
              //  DisplayErrors(response);
              //}
            }
          }
          else
          {
            throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Shopify Config not found.");
          }

        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Product List not found");
        }
      }

      baseResponse = new BaseResponseDto
      {
        Data = successProductList,
        Message = "Shopify product created successfully for the products."
      };
      serviceResult = new ServiceResultDTO(baseResponse);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<(bool, string)> CreateProductAsync(string shopDomain, string accessToken, object product)
  {
    var executionPolicy = new LeakyBucketExecutionPolicy();
    var service = new ProductService(shopDomain, accessToken);
    using (var httpClient = new HttpClient())
    {
      httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

      var json = JsonConvert.SerializeObject(product);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      var url = $"https://{shopDomain}/admin/api/2024-01/products.json";
      var response = await httpClient.PostAsync(url, content);

      var responseContent = await response.Content.ReadAsStringAsync();
      if (response.IsSuccessStatusCode)
      {
        return (true, responseContent);
      }
      else
      {
        return (false, responseContent);
      }
    }
  }

  private void DisplayErrors(string responseContent)
  {
    try
    {
      var json = JObject.Parse(responseContent);

      if (json["errors"] != null)
      {
        Console.WriteLine("Errors:");
        foreach (var error in json["errors"]!)
        {
          Console.WriteLine($"{error.Path}: {error.First}");
        }
      }
      else
      {
        Console.WriteLine("Unknown error occurred.");
      }
    }
    catch (JsonException)
    {
      Console.WriteLine("Error parsing response content.");
    }
  }
}
