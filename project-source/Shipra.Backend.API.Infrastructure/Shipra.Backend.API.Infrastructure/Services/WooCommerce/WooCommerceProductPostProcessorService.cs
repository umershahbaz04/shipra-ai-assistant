using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Command.SaleChannelProductPostProcessor;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;
using Shipra.Backend.API.Core.SaleChannelProductAggregate;
using Formatting = Newtonsoft.Json.Formatting;

namespace Shipra.Backend.API.Infrastructure.Services.WooCommerce;

public class WooCommerceProductPostProcessorService : ISaleChannelProductPostProcessorService
{
  private readonly IProductRepository _productRepository;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;
  private readonly ISaleChannelProductRepository _saleChannelProductRepository;
  private readonly IClientRepository _clientRepository;
  private readonly ILogger<WooCommerceProductPostProcessorService> _logger;

  public WooCommerceProductPostProcessorService(
    IProductRepository productRepository,
    ISaleChannelConfigRepository saleChannelConfigRepository,
    ISaleChannelProductRepository saleChannelProductRepository,
    IClientRepository clientRepository,
    ILogger<WooCommerceProductPostProcessorService> logger)
  {
    _productRepository = productRepository;
    _saleChannelConfigRepository = saleChannelConfigRepository;
    _saleChannelProductRepository = saleChannelProductRepository;
    _clientRepository = clientRepository;
    _logger = logger;
  }

  public async Task<ServiceResultDTO> PostProcessProductsAsync(
    SaleChannelProductPostProcessorCommand request,
    ClientId clientId,
    EmployeeId? employeeId,
    CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    var successProductList = new List<long?>();
    BaseResponseDto baseResponse = new BaseResponseDto();
    try
    {
      List<string> skusInput = (request.Skus ?? request.ProductIds ?? "")
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim())
        .ToList();

      List<ProductWooCommerceModal>? oWooCommerceProductList = null;

      if (skusInput.Count > 0)
      {
        var oSaleChannelConfig = await _saleChannelConfigRepository.GetSaleChannelConfigById(request.SaleChannelConfigId, clientId);
        if (oSaleChannelConfig is not null && oSaleChannelConfig.StoreId == request.StoreId && oSaleChannelConfig.SaleChannelConfigId == request.SaleChannelConfigId)
        {
          var saleChannelConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(oSaleChannelConfig.Config!);
          var saleChannelSetting = Utils.ConvertKeysToCamelCase(saleChannelConfig!);

          var consumerKey = Utils.GetValueFromDictionryByKey("consumerKey", saleChannelSetting);
          var consumerSecret = Utils.GetValueFromDictionryByKey("consumerSecret", saleChannelSetting);
          var shopURL = Utils.GetValueFromDictionryByKey("shopURL", saleChannelSetting);

          oWooCommerceProductList = await GetAllProductOnShopForWooCommerce(shopURL, consumerKey, consumerSecret);
          if (oWooCommerceProductList is not null)
          {
            var filteredWooCommerceProducts = oWooCommerceProductList
              .Where(x => !string.IsNullOrEmpty(x.SKU) && skusInput.Contains(x.SKU.Trim()))
              .ToList();

            if (filteredWooCommerceProducts.Count > 0)
            {
              var oSaleChannelProductList = await _saleChannelProductRepository.GetAllSaleChannelProduct(clientId, request.SaleChannelLookupId);
              var existingProductIds = oSaleChannelProductList?.Select(x => x.ProductId).Where(id => id != null).ToHashSet() ?? new HashSet<string?>();
              var productsToCreate = filteredWooCommerceProducts.Where(item => !existingProductIds.Contains(item.Id.ToString())).ToList();

              if (productsToCreate.Count > 0)
              {
                #region salechannelproduct
                foreach (var item in productsToCreate)
                {
                  var oShipraProduct = Core.ProductAggregate.Product.CreateProductForWooCommerce(
                    item.Name!, item.SKU, item.Description!, item.Images!.FirstOrDefault()!.Source!,
                    (int)item.RatingCount!, clientId, request?.StoreId, employeeId);
                  await _productRepository.CreateProductAsync(oShipraProduct);

                  var oProductStock = Core.ProductAggregate.ProductStock.CreateProductStock(
                    oShipraProduct.ProductId, item.SKU, item.Price!.ToDecimal(), item.StockQuantity,
                    (int)item.LowStockAmount!, (int)EnumStationLookup.Dubai, item.ShortDescription, employeeId!);
                  await _productRepository.CreateProductStockAsync(oProductStock);

                  if ((int)item.StockQuantity! > 0 && (bool)item.ShippingRequired!)
                  {
                    var productStockHistory = ProductStockHistory.CreateProductStockHistory(
                      (int)InventoryTransactionType.ExternalSync, oProductStock.ProductStockId,
                      item.StockQuantity, item.StockQuantity, "Shopify Product ", employeeId!);
                    var addedHistory = await _productRepository.CreateProductStockHistory(productStockHistory);
                  }

                  foreach (var productOption in item.Categories!)
                  {
                    var oProductOption = Core.ProductAggregate.ProductOption.CreateProductOption(
                      oShipraProduct.ProductId, productOption.Id.ToString(), productOption.Name, 1);
                    await _productRepository.CreateProductOptionAsync(oProductOption);
                  }

                  await _saleChannelProductRepository.CreateSaleChannelProduct(
                    SaleChannelProduct.CreateSaleChannelProduct(
                      request?.SaleChannelLookupId!, item.Id.ToString(), item.Id.ToString(),
                      JsonConvert.SerializeObject(item, Formatting.Indented),
                      item.DateCreated, oShipraProduct.ProductId!, clientId, employeeId!));
                  successProductList.Add(item.Id);
                }
                #endregion
              }
            }
            else
            {
              throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "WooCommerce Product not found.");
            }
          }
          else
          {
            throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "WooCommerce Product not found.");
          }
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Sale Channel Config not found.");
        }
      }

      baseResponse = new BaseResponseDto { Data = successProductList, Message = "Products created successfully." };
      serviceResult = new ServiceResultDTO(baseResponse);
      return serviceResult;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error post-processing WooCommerce products");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<List<ProductWooCommerceModal>> GetAllProductOnShopForWooCommerce(string baseURL, string consumerKey, string consumerSecret)
  {
    var allProducts = new List<ProductWooCommerceModal>();
    int page = 1;
    int perPage = 100;
    using (HttpClient client = new HttpClient())
    {
      client.BaseAddress = new Uri(baseURL + "//wp-json/wc/v3/");
      var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

      while (true)
      {
        try
        {
          HttpResponseMessage response = await client.GetAsync($"products?page={page}&per_page={perPage}");
          if (response.IsSuccessStatusCode)
          {
            string responseBody = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<ProductWooCommerceModal>>(responseBody);
            if (data == null || data.Count == 0) break;
            allProducts.AddRange(data);
            if (data.Count < perPage) break;
            page++;
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
    return allProducts;
  }
}
