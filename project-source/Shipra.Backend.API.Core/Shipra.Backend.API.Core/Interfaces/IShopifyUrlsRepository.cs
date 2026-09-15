namespace Shipra.Backend.API.Core.Interfaces;
public interface IShopifyUrlsRepository
{
  string OauthRedirectUrl { get; }
  string SubscriptionRedirectUrl { get; }
  string AppUninstalledWebhookUrl { get; }
  string OrderCreatedWebhookUrl { get; }
  string OrderUpdatedWebhookUrl { get; }
  string OrderDeletedWebhookUrl { get; }
  string CustomerUpdatedWebhookUrl { get; }
  string CustomerDeletedWebhookUrl { get; }
}
