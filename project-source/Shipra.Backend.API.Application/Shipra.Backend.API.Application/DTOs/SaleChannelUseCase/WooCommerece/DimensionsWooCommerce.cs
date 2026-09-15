using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
public class DimensionsWooCommerceModal
{
  [JsonProperty("length")]
  public string? Length { get; set; }
  [JsonProperty("width")]
  public string? Width { get; set; }
  [JsonProperty("height")]
  public string? Height { get; set; }
}
