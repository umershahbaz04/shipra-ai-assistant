using Newtonsoft.Json;

namespace Shipra.Backend.API.Application.DTOs.SaleChannelUseCase.WooCommerece;
public class LinksWooCommerceModal
{
  [JsonProperty("self")]
  public List<LinkWooCommerceModal>? Self { get; set; }
  [JsonProperty("collection")]
  public List<LinkWooCommerceModal>? Collection { get; set; }
  [JsonProperty("up")]
  public List<LinkWooCommerceModal>? Up { get; set; }
}
