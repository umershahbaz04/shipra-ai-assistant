using System.Text.RegularExpressions;
using Shipra.Backend.API.Application.Services.Interfaces.Shopify;

namespace Shipra.Backend.API.Application.Services.Implementation.Shopify;
public class ApplicationUrls : IApplicationUrls
{
  public ApplicationUrls(ISecrets secrets)
  {
    OauthRedirectUrl = JoinUrls(secrets.HostDomain, "/api/authresult");
    SubscriptionRedirectUrl = JoinUrls(secrets.HostDomain, "/subscription/chargeresult");
    AppUninstalledWebhookUrl = JoinUrls(secrets.HostDomain, "/webhooks/app-uninstalled");
    OrderCreatedWebhookUrl = JoinUrls(secrets.HostDomain, "/webhooks/order-created");
    OrderUpdatedWebhookUrl = JoinUrls(secrets.HostDomain, "/webhooks/order-updated");
    OrderDeletedWebhookUrl = JoinUrls(secrets.HostDomain, "/webhooks/order-deleted");
    CustomerUpdatedWebhookUrl = JoinUrls(secrets.HostDomain, "/webhooks/customer-updated");
    CustomerDeletedWebhookUrl = JoinUrls(secrets.HostDomain, "/webhooks/customer-deleted");
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
