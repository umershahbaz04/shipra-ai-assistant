using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.SaleChannelConfigAggregate;
using Shipra.Backend.API.Core.ShopifyAggregate;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.CreateShopifySaleChannelConfig;
public class CreateShopifySaleChannelConfigCommandHandler : RequestHandlerBase<CreateShopifySaleChannelConfigCommand, ServiceResultDTO>
{
  private readonly IShopifyPluginRepository _shopifyPluginRepository;   
  public CreateShopifySaleChannelConfigCommandHandler(IShopifyPluginRepository shopifyPluginRepository, IServiceProvider serviceProvider, ILogger<CreateShopifySaleChannelConfigCommand> logger) : base(serviceProvider, logger)
  {
    _shopifyPluginRepository = shopifyPluginRepository;   
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateShopifySaleChannelConfigCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var clientId = new ClientId(new Guid(request.ClientId!));
      var oClient = await _shopifyPluginRepository.GetClientById(clientId);
      var employeeId = new EmployeeId(new Guid(request.ClientId!));

      string? settingConfig = null;
      if (request.SettingConfig != null && request.SettingConfig!.Count > 0)
      {
        settingConfig = JsonConvert.SerializeObject(request.SettingConfig!, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
      }
      if (oClient is not null)
      {
        if (oClient.SecretKey == request.SecretKey)
        {
          string? saleChannelKey = null;
          if (request.InputParameters!.Count == 0)
          {
            throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Invalid Data");
          }
          var requestDictionry = Utils.ConvertKeysToCamelCase(request.InputParameters!);
          var saleChannelJsonData = JsonConvert.SerializeObject(requestDictionry, Formatting.Indented);
          if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
          {
            var shopName = Utils.GetValueFromDictionryByKey("shop", requestDictionry);
            if (!string.IsNullOrEmpty(shopName) && !Utility.IsValidShopifyUrl(shopName.ToLower()))
            {
              throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Invalid Shopname Please enter valid shop name e.g. shipra.myshopify.com");
            }
            else
            {
              saleChannelKey = shopName.ToLower();
            }
            if (!string.IsNullOrEmpty(request.SaleChannelName))
            {
              var oSaleChannel = await _shopifyPluginRepository.GetSaleChannelNameValidate(request.SaleChannelName, clientId);
              if (oSaleChannel is not null)
              {
                throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "The sale channel name already exist with the name of " + request.SaleChannelName + ".");
              }
            }
          }
          var oSaleChannelKey = await _shopifyPluginRepository.GetSaleChannelConfigByKey(saleChannelKey!, request.ClientId!);
          if (oSaleChannelKey == null)
          {
            var oSaleChannelConfig = await _shopifyPluginRepository.CreateSaleChannelConfig(SaleChannelConfig.CreateSaleChannelConfig(request.StoreId, request.SaleChannelLookupId, saleChannelJsonData, request.SaleChannelName!, clientId, request.IsAllowToDisplayInSaleChannel, employeeId,settingConfig!, saleChannelKey));
            if (oSaleChannelConfig is not null)
            {
              if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
              {
                var oShopifyConfig = await _shopifyPluginRepository.GetShopifyConfigByClientId(oSaleChannelConfig.SaleChannelConfigId, clientId!);
                if (oShopifyConfig is not null)
                {
                  oShopifyConfig.UpdateShopifyConfig(request.AccessToken!, employeeId!);
                  oShopifyConfig = await _shopifyPluginRepository.UpdateShopifyConfig(oShopifyConfig);
                }
                else
                {
                  oShopifyConfig = await _shopifyPluginRepository.CreateShopifyConfig(ShopifyConfig.CreateShopifyConfig(request.AccessToken!, oSaleChannelConfig.SaleChannelKey!.ToString(), oSaleChannelConfig.SaleChannelConfigId, oSaleChannelConfig.ClientId!, employeeId!));

                  oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(employeeId!);
                  await _shopifyPluginRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
                }
              }
              serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
            }
            return serviceResult;
          }
          else
          {
            throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "The sale channel record already exist with the name of " + saleChannelKey + ".");
          }
        }
        else
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Invalid User secret key.");
        }
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Invalid User Entity or Not found.");
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
