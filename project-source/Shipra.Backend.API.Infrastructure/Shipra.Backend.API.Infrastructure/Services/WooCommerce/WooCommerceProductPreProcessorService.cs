using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelProductPreProcessor;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Infrastructure.Services.WooCommerce;

public class WooCommerceProductPreProcessorService : ISaleChannelProductPreProcessorService
{
  private readonly ILogger<WooCommerceProductPreProcessorService> _logger;

  public WooCommerceProductPreProcessorService(ILogger<WooCommerceProductPreProcessorService> logger)
  {
    _logger = logger;
  }

  public async Task<ServiceResultDTO> PreProcessProductsAsync(
    SaleChannelProductPreProcessorCommand request,
    SaleChannelConfig oSaleChannelConfig,
    ClientId clientId,
    CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var saleChannelConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(oSaleChannelConfig.Config!);
      var saleChannelSetting = Utils.ConvertKeysToCamelCase(saleChannelConfig!);

      var consumerKey = Utils.GetValueFromDictionryByKey("consumerKey", saleChannelSetting);
      var consumerSecret = Utils.GetValueFromDictionryByKey("consumerSecret", saleChannelSetting);
      var shopURL = Utils.GetValueFromDictionryByKey("shopURL", saleChannelSetting);

      var oWooCommerceProduct = await GetAllProductOnShopForWooCommerce(shopURL, consumerKey, consumerSecret);
      if (oWooCommerceProduct is not null)
      {
        var wooCommerceProductList = JsonConvert.DeserializeObject<List<ProductWooCommerceModal>>(oWooCommerceProduct);
        var wooCommerceProductCastList = wooCommerceProductList as List<ProductWooCommerceModal>;
        var shopifyProductListWithSKU = wooCommerceProductCastList!
          .Where(x => x.SKU != null && x.SKU != "" && x.Price != null && x.Price != "")
          .ToList();

        dynamic data = shopifyProductListWithSKU.Select(x => new
        {
          ImageSrc = x.Images!.Select(s => s.Source).FirstOrDefault(),
          ProductName = x.Name,
          VariantCount = x.Variations!.Count(),
          Variants = x.Variations,
          Vendor = x.SKU,
          CreatedAt = x.DateCreated,
          ProductId = x.Id,
          ProductType = x.Type,
          ProductPrice = x.Price,
          x.Description,
          InventoryQuantity = x.StockQuantity,
          SaleChannelLookupId = (int)EnumSaleChannelLookup.WooCommerce,
          request.StoreId,
          request.SaleChannelConfigId
        });
        serviceResult = new ServiceResultDTO(data);
        serviceResult.CreateSuccessResponse();
        return serviceResult;
      }
      else
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "WooCommerce Product not found");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error pre-processing WooCommerce products");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<dynamic> GetAllProductOnShopForWooCommerce(string baseURL, string consumerKey, string consumerSecret)
  {
    using (HttpClient client = new HttpClient())
    {
      client.BaseAddress = new Uri(baseURL + "//wp-json/wc/v3/");
      var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

      try
      {
        HttpResponseMessage response = await client.GetAsync("products");
        if (response.IsSuccessStatusCode)
        {
          string responseBody = await response.Content.ReadAsStringAsync();
          return responseBody;
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
}
