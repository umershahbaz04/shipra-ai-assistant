using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.Amazon;

/// <summary>
/// Shared Amazon SP-API response models used by ProductPreProcessor and ProductPostProcessor services.
/// </summary>
public class AmazonTokenResponse
{
  [JsonProperty("access_token")]
  public string? AccessToken { get; set; }
}

public class AmazonCreateReportResponse
{
  [JsonProperty("reportId")]
  public string? ReportId { get; set; }
}

public class AmazonReportStatusResponse
{
  [JsonProperty("processingStatus")]
  public string? ProcessingStatus { get; set; }

  [JsonProperty("reportDocumentId")]
  public string? ReportDocumentId { get; set; }
}

public class AmazonDocumentResponse
{
  [JsonProperty("url")]
  public string? Url { get; set; }
}

public class AmazonReportProduct
{
  public string? Asin { get; set; }
  public string? SellerSku { get; set; }
  public string? ItemName { get; set; }
  public string? Description { get; set; }
  public decimal Price { get; set; }
  public int Quantity { get; set; }
  public string? ImageUrl { get; set; }
}
