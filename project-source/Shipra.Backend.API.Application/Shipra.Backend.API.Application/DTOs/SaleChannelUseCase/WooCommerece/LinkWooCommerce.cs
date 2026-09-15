using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
public class LinkWooCommerceModal
{
  [JsonProperty("href")]
  public string? Href { get; set; }
}
