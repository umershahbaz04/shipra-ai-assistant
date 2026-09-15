using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Services.Interfaces.Shopify;
public interface IApplicationUrls
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
