using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
public class MetaDataWooCommerceModal
{
  [JsonProperty("id")]
  public int? Id { get; set; }
  [JsonProperty("key")]
  public string? Key { get; set; }
  [JsonProperty("value")]
  public string? Value { get; set; }
}
