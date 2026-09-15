//using System.Net;
//using System.Net.Http.Headers;
//using Shipra.Backend.API.Application.Common.Exceptions;
//using Shipra.Backend.API.Core.EmployeeAggregate;
//using Shipra.Backend.API.Core.Enum;
//using Shipra.Backend.API.Core.ProductAggregate;
//using ShopifySharp;
//using ShopifySharp.Filters;
//using Product = ShopifySharp.Product;

//namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.SaleChannelCommon;
//public class SaleChannelCommon
//{
//  private async Core.ProductAggregate.ProductStockHistory GetProductStockHistory(int? quantityAvailable, long productStockId, EmployeeId createdBy)
//  {
//    return ProductStockHistory.CreatProductStockHistory((int)EnumLookupAdjustReason.AdjustStock, productStockId, quantityAvailable, quantityAvailable, "Sale Channel Product", createdBy);
//  }
//  public async Task<List<Product>> ListAllProductsOnShop(string shopDomain, List<string> productIds, string accesstoken)
//  {
//    var filterProductIds = productIds.Select(s => long.Parse(s!)).ToList();
//    var executionPolicy = new LeakyBucketExecutionPolicy();
//    var service = new ProductService(shopDomain, accesstoken);
//    var allProducts = new List<Product>();
//    var page = await service.ListAsync(new ProductListFilter
//    {
//      Limit = 250,
//      Ids = filterProductIds
//    });
//    // Keep adding the orders to the list of all Product until there are no

//    while (true)
//    {
//      allProducts.AddRange(page.Items);
//      if (!page.HasNextPage)
//      {
//        // We've reached the end of the list
//        break;
//      }
//      // There is at least one more page, list it and loop again
//      page = await service.ListAsync(page.GetNextPageFilter());
//    }
//    return allProducts;
//    // TODO: do something with the `allOrders` variable
//  }

//  #region WooCommerce
//  public async Task<dynamic> GetAllProductOnShopForWooCommerce(string baseURL, string consumerKey, string consumerSecret, List<string> productIds)
//  {
//    // Initialize HttpClient
//    using (HttpClient client = new HttpClient())
//    {
//      // Set the base URL for the WooCommerce API
//      client.BaseAddress = new Uri(baseURL + "//wp-json/wc/v3/");

//      // Set the authorization header
//      var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
//      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

//      try
//      {
//        var filterParam = string.Join(",", productIds);
//        var requestUrl = $"products?filter[product_id]={filterParam}";
//        // Make a request to retrieve orders (adjust the endpoint as needed)
//        HttpResponseMessage response = await client.GetAsync(requestUrl);

//        // Check if the request was successful
//        if (response.IsSuccessStatusCode)
//        {
//          // Read and display the response content
//          string responseBody = await response.Content.ReadAsStringAsync();
//          return responseBody;
//        }
//        else
//        {
//          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Error: {response.StatusCode} - {response.ReasonPhrase}");
//        }
//      }
//      catch (Exception ex)
//      {
//        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Exception: {ex.Message}");
//      }
//    }
//  }
//  #endregion

//}
