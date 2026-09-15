using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.SaleChannelProcessFeature.Query.SaleChannelProductPreProcessor;
using Shipra.Backend.API.Application.Services.Interfaces;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;

namespace Shipra.Backend.API.Infrastructure.Services.Noon;

public class NoonProductPreProcessorService : ISaleChannelProductPreProcessorService
{
  private readonly ISaleChannelProductRepository _saleChannelProductRepository;
  private readonly ILogger<NoonProductPreProcessorService> _logger;
  private readonly ISaleChannelConfigRepository _saleChannelConfigRepository;


  public NoonProductPreProcessorService(ISaleChannelProductRepository saleChannelProductRepository, ILogger<NoonProductPreProcessorService> logger, ISaleChannelConfigRepository saleChannelConfigRepository)
  {
    _saleChannelProductRepository = saleChannelProductRepository;
    _logger = logger;
    _saleChannelConfigRepository = saleChannelConfigRepository;
  }
  public async Task<ServiceResultDTO> PreProcessProductsAsync(SaleChannelProductPreProcessorCommand request, SaleChannelConfig oSaleChannelConfig, ClientId clientId, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {

      var saleChannelConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(oSaleChannelConfig.Config!);
      var saleChannelSetting = Utils.ConvertKeysToCamelCase(saleChannelConfig!);

      #region Configuration
      var privateKey = Utils.GetValueFromDictionryByKey("privateKey", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("private_key", saleChannelSetting);
      var keyId = Utils.GetValueFromDictionryByKey("keyId", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("key_id", saleChannelSetting);
      var channelIdentifier = Utils.GetValueFromDictionryByKey("channelIdentifier", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("channel_identifier", saleChannelSetting);
      var projectCode = Utils.GetValueFromDictionryByKey("projectCode", saleChannelSetting) ?? Utils.GetValueFromDictionryByKey("project_code", saleChannelSetting);
      var saleChannelLookup = await _saleChannelConfigRepository.GetSaleChannelLookupById(oSaleChannelConfig.SaleChannelLookupId ?? (int)EnumSaleChannelLookup.Noon);
      var baseUrl = saleChannelLookup?.AppUrl ?? "";
      #endregion

      if (string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(channelIdentifier))
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Noon integration parameters (private_key, key_id, channel_identifier) are missing in SaleChannelConfig.");
      }

      #region GetAuthToken
      var jwtToken = NoonAuthHelper.GenerateSessionJwtToken(privateKey, keyId);
      var sessionCookie = await ExchangeJwtForSessionCookie(baseUrl, jwtToken, projectCode);
      #endregion


      if (string.IsNullOrEmpty(sessionCookie))
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Failed to retrieve Noon Access Token.");
      }

      var exportCode = await CreateExportAsync(baseUrl, sessionCookie);
      if (string.IsNullOrEmpty(exportCode))
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Failed to create Noon export.");
      }

      string downloadUrl = string.Empty;
      for (int i = 0; i < 10; i++) // Poll up to 10 times (e.g., 50 seconds)
      {
        await Task.Delay(5000, cancellationToken);
        var status = await GetExportStatusAsync(baseUrl, sessionCookie, exportCode);
        if (status?.ExportStatus == "COMPLETE")
        {
          downloadUrl = status.DownloadUrl ?? string.Empty;
          break;
        }
        else if (status?.ExportStatus == "FAILED")
        {
           throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Noon export failed.");
        }
      }

      if (string.IsNullOrEmpty(downloadUrl))
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Noon export did not complete in time.");
      }

      var noonProducts = await DownloadAndParseCsvAsync(downloadUrl);

      if (noonProducts != null && noonProducts.Count > 0)
      {
        dynamic data = noonProducts.Select(x => new
        {
          ImageSrc = string.Empty,
          ProductName = x.PartnerSku ?? "Noon Product",
          VariantCount = 1,
          Variants = new[] { new { SKU = x.PartnerSku, Price = 0, InventoryQuantity = x.Quantity } },
          Vendor = "Noon",
          CreatedAt = DateTime.UtcNow,
          ProductId = x.PartnerSku,
          ProductType = "Product",
          ProductPrice = 0,
          Description = string.Empty,
          InventoryQuantity = x.Quantity,
          SaleChannelLookupId = (int)EnumSaleChannelLookup.Noon, // Needs to be defined if missing, assuming it matches oSaleChannelConfig.SaleChannelLookupId
          request.StoreId,
          request.SaleChannelConfigId
        });

        serviceResult = new ServiceResultDTO(data);
        serviceResult.CreateSuccessResponse();
        return serviceResult;
      }
      else
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, "Noon Products stock not found or empty.");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error pre-processing Noon products");
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }


  private async Task<string?> ExchangeJwtForSessionCookie(string baseUrl, string jwtToken, string projectCode)
  {
    using (var client = new HttpClient())
    {
      var normalizedBaseUrl = baseUrl.TrimEnd('/');
      var requestUri = $"{normalizedBaseUrl}/identity/public/v1/api/login";
      var requestBody = new { token = jwtToken, default_project_code = projectCode };
      var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
      var response = await client.PostAsync(requestUri, content);

      if (response.IsSuccessStatusCode)
      {
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
          var cookieList = new List<string>();
          foreach (var cookie in cookies)
          {
            var parts = cookie.Split(';');
            if (parts.Length > 0)
            {
              cookieList.Add(parts[0]);
            }
          }
          return string.Join("; ", cookieList);
        }
      }
      return null;
    }
  }

  #region Export Products
  private async Task<string?> CreateExportAsync(string baseUrl, string accessToken)
  {
    using (var client = new HttpClient())
    {
      var normalizedBaseUrl = baseUrl.TrimEnd('/');
      var requestUri = $"{normalizedBaseUrl}/impex/v1/export/create";

      client.DefaultRequestHeaders.Add("Cookie", accessToken);
      client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.53.0");

      var requestBody = new
      {
        export_category_code = "noon_catalog_catalogexport",
        @params = new
        {
          country = "ae",
          noon_status = "active"
        }
      };

      var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
      var response = await client.PostAsync(requestUri, content);

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        var exportResponse = JsonConvert.DeserializeAnonymousType(responseContent, new { export_code = "" });
        return exportResponse?.export_code;
      }
      return null;
    }
  }

  private class ExportStatusResponse
  {
    [JsonProperty("export_status")]
    public string? ExportStatus { get; set; }
    [JsonProperty("download_url")]
    public string? DownloadUrl { get; set; }
  }

  private async Task<ExportStatusResponse?> GetExportStatusAsync(string baseUrl, string accessToken, string exportCode)
  {
    using (var client = new HttpClient())
    {
      var normalizedBaseUrl = baseUrl.TrimEnd('/');
      var requestUri = $"{normalizedBaseUrl}/impex/v1/export/status";

      client.DefaultRequestHeaders.Add("Cookie", accessToken);
      client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.53.0");

      var requestBody = new { export_code = exportCode };
      var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

      var response = await client.PostAsync(requestUri, content);

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ExportStatusResponse>(responseContent);
      }
      return null;
    }
  }

  private async Task<List<NoonStockResponseItem>> DownloadAndParseCsvAsync(string downloadUrl)
  {
    var products = new List<NoonStockResponseItem>();
    using (var client = new HttpClient())
    {
      var response = await client.GetAsync(downloadUrl);
      if (response.IsSuccessStatusCode)
      {
        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        var headerLine = await reader.ReadLineAsync();
        if (string.IsNullOrEmpty(headerLine)) return products;

        var headers = ParseCsvLine(headerLine).ToList();
        var skuIndex = headers.FindIndex(h => h.Equals("partner_sku", StringComparison.OrdinalIgnoreCase));
        var qtyIndex = headers.FindIndex(h => h.Equals("stock_quantity", StringComparison.OrdinalIgnoreCase) || h.Equals("quantity", StringComparison.OrdinalIgnoreCase));

        if (skuIndex == -1) return products; // Cannot process without SKU

        while (!reader.EndOfStream)
        {
          var line = await reader.ReadLineAsync();
          if (string.IsNullOrEmpty(line)) continue;

          var values = ParseCsvLine(line).ToList();
          if (values.Count > skuIndex)
          {
            var sku = values[skuIndex];
            int qty = 0;
            if (qtyIndex != -1 && values.Count > qtyIndex)
            {
              int.TryParse(values[qtyIndex], out qty);
            }

            if (!string.IsNullOrEmpty(sku))
            {
              products.Add(new NoonStockResponseItem
              {
                PartnerSku = sku,
                Quantity = qty
              });
            }
          }
        }
      }
    }
    return products;
  }

  private IEnumerable<string> ParseCsvLine(string line)
  {
    bool inQuotes = false;
    int startIndex = 0;
    for (int i = 0; i < line.Length; i++)
    {
      if (line[i] == '\"')
      {
        inQuotes = !inQuotes;
      }
      else if (line[i] == ',' && !inQuotes)
      {
        yield return line.Substring(startIndex, i - startIndex).Trim('\"');
        startIndex = i + 1;
      }
    }
    yield return line.Substring(startIndex).Trim('\"');
  }
  #endregion
}
