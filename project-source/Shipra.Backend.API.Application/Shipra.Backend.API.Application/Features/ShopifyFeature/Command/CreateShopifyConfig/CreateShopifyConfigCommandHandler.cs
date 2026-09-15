using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ShopifyAggregate;
using ShopifySharp.Utilities;

namespace Shipra.Backend.API.Application.Features.ShopifyFeature.Command.CreateShopifyConfig;
public class CreateShopifyConfigCommandHandler : RequestHandlerBase<CreateShopifyConfigCommand, ServiceResultDTO>
{
  private readonly IShopifyRepository _shopifyRepository;
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;
  private readonly IAppConfigRepository _appConfigRepository;

  public CreateShopifyConfigCommandHandler(IShopifyRepository shopifyRepository, ISaleChannelConfigRepository SaleChannelConfigRepository, IServiceProvider serviceProvider, IAppConfigRepository appConfigRepository, ILogger<CreateShopifyConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shopifyRepository = shopifyRepository;
    _SaleChannelConfigRepository = SaleChannelConfigRepository;
    _appConfigRepository = appConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateShopifyConfigCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oSaleChannelConfig = await _SaleChannelConfigRepository.GetSaleChannelConfigByKey(request.shop!.ToString());
      if (oSaleChannelConfig is not null)
      {
        var oAppConfig = await _appConfigRepository.GetAppConfigByKey(ApplicationConstants.ShopifyConfigKey);
        //Deserialize SaleChannel Config values
        var appConfigJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(oAppConfig!.AppConfigValue!);

        //Convert Keys To CamelCase with Deserialize SaleChannel object
        var resultAppConfigJson = Utils.ConvertKeysToCamelCase(appConfigJson!);

        //Shopify keys
        var shopifyClientId = Utils.GetValueFromDictionryByKey("clientId", resultAppConfigJson);
        var shopifyClientSecret = Utils.GetValueFromDictionryByKey("clientSecret", resultAppConfigJson);

        if (!string.IsNullOrEmpty(shopifyClientSecret) && !string.IsNullOrEmpty(shopifyClientId))
        {
          ShopifyOauthUtility shopifyOauthUtility = new ShopifyOauthUtility();
          var oAuthorizationResult =  await shopifyOauthUtility.AuthorizeAsync(request.code!, request.shop!, shopifyClientId, shopifyClientSecret);
          if (oAuthorizationResult is not null)
          {
            var oShopifyConfig = await _shopifyRepository.GetShopifyConfigByClientId(oSaleChannelConfig.SaleChannelConfigId, oSaleChannelConfig.ClientId!);
            if (oShopifyConfig is not null)
            {
              oShopifyConfig.UpdateShopifyConfig(oAuthorizationResult.AccessToken, _currentUser.EmployeeId!);
              oShopifyConfig = await _shopifyRepository.UpdateShopifyConfig(oShopifyConfig);
            }
            else
            {
              oShopifyConfig = await _shopifyRepository.CreateShopifyConfig(ShopifyConfig.CreateShopifyConfig(oAuthorizationResult.AccessToken, request.shop!.ToString(), oSaleChannelConfig.SaleChannelConfigId, oSaleChannelConfig.ClientId!, _currentUser.EmployeeId!));

              oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(_currentUser.EmployeeId!);
              await _SaleChannelConfigRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
            }
            serviceResult = new ServiceResultDTO(oShopifyConfig);
            serviceResult.CreateSuccessResponse(HttpStatusCode.OK);
          }
          else
          {
            throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Request failed while getting Shopify Access Token.");
          }
        }
        else
        {
          throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Request failed while getting Shopify keys.");
        }
      }
      else
      {
        throw new ShipraApplicationException(HttpStatusCode.ExpectationFailed, $"Request failed while getting Shopify info against {request.shop}.");
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
