using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
public class CategoryWooCommerceModal
{
  [JsonProperty("id")]
  public int? Id { get; set; }
  [JsonProperty("name")]
  public string? Name { get; set; }
  [JsonProperty("slug")]
  public string? Slug { get; set; }
}
