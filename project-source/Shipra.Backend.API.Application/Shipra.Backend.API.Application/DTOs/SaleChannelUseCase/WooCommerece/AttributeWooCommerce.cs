using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
public class AttributeWooCommerceModal
{
  [JsonProperty("id")]
  public int? Id { get; set; }

  [JsonProperty("name")]
  public string? Name { get; set; }

  [JsonProperty("option")]
  public string? Option { get; set; }
}
