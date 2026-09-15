using Microsoft.AspNetCore.Mvc;
using Shipra.Backend.API.Application.Features.ShopifyFeature.Command.CreateShopifyConfig;
using Shipra.Backend.API.Application.Services.Interfaces.Shopify;
using ShopifySharp;
using ShopifySharp.Utilities;

namespace Shipra.Backend.API.Web.Api;
[Route("api")]
public class ShopSyncController : BaseApiController
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly IApplicationUrls _appUrls;
  private readonly ISecrets _secrets;
  public ShopSyncController(IServiceProvider serviceProvider, IApplicationUrls appUrls, ISecrets secrets, IWebHostEnvironment webHostEnvironment) : base(serviceProvider)
  {
    this._webHostEnvironment = webHostEnvironment;
    _appUrls = appUrls;
    _secrets = secrets;
  }

  #region Command

  [HttpGet("callback")]
  public ActionResult CallBack(string? shop = null)
  {
    ShopifyOauthUtility shopifyOauthUtility = new ShopifyOauthUtility();
    ShopifyDomainUtility shopifyDomainUtility = new ShopifyDomainUtility();
    // Check to make sure the domain they entered is a real Shopify store. This will prevent accidentally redirecting
    // the user away to a bad website.
    if (!string.IsNullOrEmpty(shop))
    {
      bool isValidShop = true;
      Task.Run(async () =>
      {
        if (!await shopifyDomainUtility.IsValidShopDomainAsync(shop))
        {
          isValidShop = false;
        }
      }).Wait();

      if (!isValidShop)
      {
        return RedirectToAction("AppError", "Home");
      }
    }

    // This user account doesn't exist. Send them to Shopify to start the OAuth installation process
    // 1. Create a list of permissions to request from them when installing
    // 2. Create an OauthState record to ensure the login request can only be used once
    // 3. Save the new oauth state record
    // 4. Redirect them to the OAuth URL
    //    private const string Scopes = "read_content,read_themes,read_products,read_customers,read_orders,read_draft_orders,read_script_tags,read_fulfillments,read_shipping,read_analytics,read_checkouts,read_reports,read_price_rules,read_marketing_events,read_resource_feedbacks,read_shopify_payments_payouts,read_shopify_payments_disputes,read_shopify_payments_balance,read_translations,read_locales,read_merchants,read_webhooks";
    var requiredPermissions = new[] { "read_orders,read_products,write_products,read_inventory,write_inventory,read_customers,read_fulfillments,read_shipping" };
    var Token = Guid.NewGuid().ToString();

    var oauthUrl = shopifyOauthUtility.BuildAuthorizationUrl(
        requiredPermissions,
        shop!,
        _secrets.ShopifyClientID,
        _appUrls.OauthRedirectUrl,
        Token);

    return Redirect(oauthUrl.ToString());
  }

  [HttpGet("authresult")]
  public async Task<ActionResult> CreateSaleChannelConfig([FromQuery] CreateShopifyConfigCommand request, CancellationToken cancellationToken = default)
  {
    var response = await Mediator.Send(request, cancellationToken);
    if (response.StatusCode == (int)System.Net.HttpStatusCode.OK)
    {
      return RedirectToAction("AppSuccess", "Home");
    }
    return RedirectToAction("AppError", "Home");
  }

  #endregion
}
