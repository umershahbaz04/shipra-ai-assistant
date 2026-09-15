using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
using Shipra.Backend.API.Core.ShopifyAggregate;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.UpdateShopifySaleChannelConfig;
public class UpdateShopifySaleChannelConfigCommandHandler : RequestHandlerBase<UpdateShopifySaleChannelConfigCommand, ServiceResultDTO>
{
  private readonly IShopifyPluginRepository _shopifyPluginRepository; 

  public UpdateShopifySaleChannelConfigCommandHandler(IShopifyPluginRepository shopifyPluginRepository, IServiceProvider serviceProvider, ILogger<UpdateShopifySaleChannelConfigCommand> logger) : base(serviceProvider, logger)
  {
    _shopifyPluginRepository = shopifyPluginRepository; 
  }
  protected override async Task<ServiceResultDTO> HandleRequest(UpdateShopifySaleChannelConfigCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var clientId = new ClientId(new Guid(request.ClientId!));
      var employeeId = new EmployeeId(new Guid(request.ClientId!));

      var oClient = await _shopifyPluginRepository.GetClientById(clientId);
      if (oClient != null)
      {
        if (oClient.SecretKey == request.SecretKey)
        {
          if (request.InputParameters!.Count == 0)
          {
            throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Invalid Data");
          }
          var requestDictionry = Utils.ConvertKeysToCamelCase(request.InputParameters!);
          var saleChannelJsonData = JsonConvert.SerializeObject(requestDictionry, Formatting.Indented);
          string? saleChannelKey = null;

          var oSaleChannelConfig = await _shopifyPluginRepository.GetSaleChannelConfigForUpdateById(request.SaleChannelConfigId, clientId!);
          if (oSaleChannelConfig is null)
          {
            serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            serviceResult.Errors?.Add("SaleChannelConfig", new[] { "Sale Channel Config not found" });
          }

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
            var oShopifyConfig = await _shopifyPluginRepository.GetShopifyConfigById(request.SaleChannelConfigId, clientId);

            if (oShopifyConfig != null && oSaleChannelConfig!.SaleChannelKey != saleChannelKey)
            {
              //delete flag set
              oShopifyConfig.DeleteShopifyConfig(_currentUser.EmployeeId);
              bool? isDeleted = await _shopifyPluginRepository.DeleteShopifyConfig(oShopifyConfig);
              if (isDeleted == true)
              {
                oSaleChannelConfig!.UpdateSaleChannelConfig(request.StoreId, saleChannelJsonData, request!.SaleChannelName!, oSaleChannelConfig.Active.GetValueOrDefault(), request.IsAllowToDisplayInSaleChannel, _currentUser.EmployeeId, saleChannelKey);
                oSaleChannelConfig = await _shopifyPluginRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
                if (oSaleChannelConfig is not null)
                {
                  oShopifyConfig = await _shopifyPluginRepository.CreateShopifyConfig(ShopifyConfig.CreateShopifyConfig(request.AccessToken!, oSaleChannelConfig.SaleChannelKey!.ToString(), oSaleChannelConfig.SaleChannelConfigId, clientId, employeeId!));
                  if (oShopifyConfig is not null)
                  {
                    oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(_currentUser.EmployeeId!);
                    await _shopifyPluginRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
                    serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
                  }
                  else
                  {
                    oShopifyConfig = await _shopifyPluginRepository.CreateShopifyConfig(ShopifyConfig.CreateShopifyConfig(request.AccessToken!, oSaleChannelConfig.SaleChannelKey!.ToString(), oSaleChannelConfig.SaleChannelConfigId, oSaleChannelConfig.ClientId!, employeeId!));

                    oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(employeeId!);
                    await _shopifyPluginRepository.UpdateSaleChannelConfig(oSaleChannelConfig);

                    serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
                  }
                }
              }
            }
            else
            {
              oSaleChannelConfig!.UpdateSaleChannelConfig(request.StoreId, saleChannelJsonData, request!.SaleChannelName!, request.IsActive.GetValueOrDefault(true), request.IsAllowToDisplayInSaleChannel, _currentUser.EmployeeId, saleChannelKey);
              await _shopifyPluginRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
              if (oShopifyConfig is not null)
              {
                oShopifyConfig.UpdateShopifyConfig(request.AccessToken!, employeeId!);
                oShopifyConfig = await _shopifyPluginRepository.UpdateShopifyConfig(oShopifyConfig);
                if (oShopifyConfig is not null)
                {
                  oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(employeeId!);
                  await _shopifyPluginRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
                  serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
                }
              }
              else
              {
                oShopifyConfig = await _shopifyPluginRepository.CreateShopifyConfig(ShopifyConfig.CreateShopifyConfig(request.AccessToken!, oSaleChannelConfig.SaleChannelKey!.ToString(), oSaleChannelConfig.SaleChannelConfigId, oSaleChannelConfig.ClientId!, employeeId!));

                oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(employeeId!);
                await _shopifyPluginRepository.UpdateSaleChannelConfig(oSaleChannelConfig);

                serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
              }
            }
            return serviceResult;
          }
          else
          {
            throw new ShipraApplicationException(System.Net.HttpStatusCode.ExpectationFailed, "Invalid Shopify Entity or Not found.");
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
