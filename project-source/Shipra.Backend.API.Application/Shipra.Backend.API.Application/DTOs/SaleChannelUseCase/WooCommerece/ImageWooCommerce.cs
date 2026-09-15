using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
public class ImageWooCommerceModal
{
  [JsonProperty("id")]
  public int? Id { get; set; }

  [JsonProperty("date_created")]
  public DateTime? DateCreated { get; set; }

  [JsonProperty("date_created_gmt")]
  public DateTime? DateCreatedGmt { get; set; }

  [JsonProperty("date_modified")]
  public DateTime? DateModified { get; set; }

  [JsonProperty("date_modified_gmt")]
  public DateTime? DateModifiedGmt { get; set; }

  [JsonProperty("src")]
  public string? Source { get; set; }

  [JsonProperty("name")]
  public string? Name { get; set; }

  [JsonProperty("alt")]
  public string? Alt { get; set; }
}
