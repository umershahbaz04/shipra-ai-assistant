using Microsoft.Extensions.Configuration;
using Shipra.Backend.API.Application.Services.Interfaces.Shopify;

namespace Shipra.Backend.API.Application.Services.Implementation.Shopify;
public class Secrets : ISecrets
{
  public IConfiguration _configuration { get; set; }
  public Secrets(IConfiguration configuration)
  {
    _configuration = configuration;
    ShopifyClientSecret = _configuration.GetValue<string>("Shopify:ClientSecret")!;
    ShopifyClientID = _configuration.GetValue<string>("Shopify:ClientId")!;
    HostDomain = _configuration.GetValue<string>("Shopify:HostDomain")!;
    ShopName = _configuration.GetValue<string>("Shopify:ShopName")!;
    AccessToken = null;
  }
  public string ShopifyClientSecret { get; }
  public string ShopifyClientID { get; }
  public string HostDomain { get; }
  public string? AccessToken { get; }
  public string ShopName { get; }
}
