using System.Text.RegularExpressions;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class ShopifyUrlsRepository : IShopifyUrlsRepository
{
  public ShopifyUrlsRepository(string hostDomain)
  {
    OauthRedirectUrl = JoinUrls(hostDomain, "/api/shopify/authresult");
    SubscriptionRedirectUrl = JoinUrls(hostDomain, "/api/shopify/chargeresult");
    AppUninstalledWebhookUrl = JoinUrls(hostDomain, "/api/shopifywebhooks/app-uninstalled");
    OrderCreatedWebhookUrl = JoinUrls(hostDomain, "/api/shopifywebhooks/order-created");
    OrderUpdatedWebhookUrl = JoinUrls(hostDomain, "/api/shopifywebhooks/order-updated");
    OrderDeletedWebhookUrl = JoinUrls(hostDomain, "/api/shopifywebhooks/order-deleted");
    CustomerUpdatedWebhookUrl = JoinUrls(hostDomain, "/api/shopifywebhooks/customer-updated");
    CustomerDeletedWebhookUrl = JoinUrls(hostDomain, "/api/shopifywebhooks/customer-deleted");
  }

  string JoinUrls(string left, string right)
  {
    var trimTrailingSlash = new Regex("/+$");
    var trimLeadingSlash = new Regex("^/+");

    return trimTrailingSlash.Replace(left, "") + "/" + trimLeadingSlash.Replace(right, "");
  }

  public string OauthRedirectUrl { get; }
  public string SubscriptionRedirectUrl { get; }
  public string AppUninstalledWebhookUrl { get; }
  public string OrderCreatedWebhookUrl { get; }
  public string OrderUpdatedWebhookUrl { get; }
  public string OrderDeletedWebhookUrl { get; }
  public string CustomerUpdatedWebhookUrl { get; }
  public string CustomerDeletedWebhookUrl { get; }
}
