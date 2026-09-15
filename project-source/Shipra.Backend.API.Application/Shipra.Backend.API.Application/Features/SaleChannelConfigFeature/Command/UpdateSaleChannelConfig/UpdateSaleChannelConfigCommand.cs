using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.Helpers;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.ShopifyAggregate;

namespace Shipra.Backend.API.Application.Features.SaleChannelConfigFeature.Command.UpdateSaleChannelConfig;
public class UpdateSaleChannelConfigCommand : IRequest<ServiceResultDTO>
{
  public bool? IsActive { get; set; }
  public bool IsAllowToDisplayInSaleChannel { get; set; }
  public Dictionary<string, string>? InputParameters { get; set; }
  public List<GeneralSettingConfigModel>? SettingConfig { get; set; } = new();
  public int StoreId { get; set; }
  public int SaleChannelLookupId { get; set; }
  public int SaleChannelConfigId { get; set; }
  public string? SaleChannelName { get; set; }
  public string? AccessToken { get; set; }
}
public class UpdateSaleChannelConfigCommandHandler : RequestHandlerBase<UpdateSaleChannelConfigCommand, ServiceResultDTO>
{
  private readonly IShopifyRepository _shopifyRepository;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly ISaleChannelConfigRepository _SaleChannelConfigRepository;

  public UpdateSaleChannelConfigCommandHandler(IShopifyRepository shopifyRepository,IEmployeeRepository employeeRepository, ISaleChannelConfigRepository saleChannelConfigRepository, IServiceProvider serviceProvider, ILogger<UpdateSaleChannelConfigCommandHandler> logger) : base(serviceProvider, logger)
  {
    _shopifyRepository = shopifyRepository;
    _employeeRepository = employeeRepository;
    _SaleChannelConfigRepository = saleChannelConfigRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateSaleChannelConfigCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    { 
      string? settingConfig = null;
      if (request.SettingConfig != null && request.SettingConfig!.Count > 0)
      {
        settingConfig = JsonConvert.SerializeObject(request.SettingConfig!, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
      }
      var oSaleChannelConfig = await _SaleChannelConfigRepository.GetSaleChannelConfigForUpdateById(request.SaleChannelConfigId, _currentUser.ClientId!);
      if (oSaleChannelConfig is null)
      {
        serviceResult.StatusCode = (int)HttpStatusCode.ExpectationFailed;
        serviceResult.Errors?.Add("SaleChannelConfig", new[] { "Sale Channel Config not found" });
      }

      var requestDictionry = request.InputParameters! != null && request.InputParameters!.Count > 0 ? Utils.ConvertKeysToCamelCase(request.InputParameters!) : new Dictionary<string, string>();
      var saleChannelJsonData = JsonConvert.SerializeObject(requestDictionry, Formatting.Indented);

      string? saleChannelKey = null;



      if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.SalePerson)
      {
        var salePersonName = Utils.GetValueFromDictionryByKey("salePersonName", requestDictionry);
        var phoneNo = Utils.GetValueFromDictionryByKey("phoneNo", requestDictionry);
        //var email = Utils.GetValueFromDictionryByKey("email", requestDictionry);
        var address = Utils.GetValueFromDictionryByKey("address", requestDictionry);


        #region update config model
        requestDictionry["salePersonName"] = salePersonName;
        requestDictionry["phoneNo"] = phoneNo;
        requestDictionry["address"] = address; 
        #endregion

        #region Update Employee
        var oEmployee = await _employeeRepository.GetEmployeeBySaleChannelConfigId(request.SaleChannelConfigId, _currentUser.ClientId!);
        if (oEmployee is not null)
        {
          oEmployee.UpdateSaleChannel(salePersonName, phoneNo, address, _currentUser.EmployeeId);
          await _employeeRepository.UpdateEmployee(oEmployee);
        }
        #endregion
        var saleChannelJson = JsonConvert.SerializeObject(requestDictionry, Formatting.Indented);

        oSaleChannelConfig!.UpdateSaleChannelConfig(request.StoreId, saleChannelJson, request!.SaleChannelName!, request.IsActive.GetValueOrDefault(), request.IsAllowToDisplayInSaleChannel, _currentUser.EmployeeId,settingConfig!); 
        await _SaleChannelConfigRepository.UpdateSaleChannelConfig(oSaleChannelConfig);

        serviceResult.CreateSuccessResponse(HttpStatusCode.OK); 
      }
      else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.Shopify)
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
        var oShopifyConfig = await _shopifyRepository.GetShopifyConfigById(request.SaleChannelConfigId, _currentUser.ClientId!);
        
        if (oShopifyConfig != null && oSaleChannelConfig!.SaleChannelKey != saleChannelKey) 
        {
          //delete flag set
          oShopifyConfig.DeleteShopifyConfig(_currentUser.EmployeeId);
          bool? isDeleted = await _shopifyRepository.DeleteShopifyConfig(oShopifyConfig);

          if (isDeleted == true)
          {
            oSaleChannelConfig!.UpdateSaleChannelConfig(request.StoreId, saleChannelJsonData, request!.SaleChannelName!, oSaleChannelConfig.Active.GetValueOrDefault(), request.IsAllowToDisplayInSaleChannel, _currentUser.EmployeeId,settingConfig!, saleChannelKey);
            oSaleChannelConfig = await _SaleChannelConfigRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
            if (oSaleChannelConfig is not null)
            {
              oShopifyConfig = await _shopifyRepository.CreateShopifyConfig(ShopifyConfig.CreateShopifyConfig(request.AccessToken!, oSaleChannelConfig.SaleChannelKey!.ToString(), oSaleChannelConfig.SaleChannelConfigId, oSaleChannelConfig.ClientId!, _currentUser.EmployeeId!));
              if (oShopifyConfig is not null)
              {
                oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(_currentUser.EmployeeId!);
                await _SaleChannelConfigRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
                serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
              }              
            }           
          }
        }
        else
        {
          oSaleChannelConfig!.UpdateSaleChannelConfig(request.StoreId, saleChannelJsonData, request!.SaleChannelName!, request.IsActive.GetValueOrDefault(true), request.IsAllowToDisplayInSaleChannel, _currentUser.EmployeeId,settingConfig!, saleChannelKey);
          await _SaleChannelConfigRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
          if (oShopifyConfig is not null)
          {
            oShopifyConfig.UpdateShopifyConfig(request.AccessToken!, _currentUser.EmployeeId!);
            oShopifyConfig = await _shopifyRepository.UpdateShopifyConfig(oShopifyConfig);
            if (oShopifyConfig is not null)
            {
              oSaleChannelConfig.UpdateSaleChannelConfigWhileActivate(_currentUser.EmployeeId!);
              await _SaleChannelConfigRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
              serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
            }           
          }
        }        
      }
      else if (request.SaleChannelLookupId == (int)EnumSaleChannelLookup.WooCommerce)
      {
        var shopURL = Utils.GetValueFromDictionryByKey("shopURL", requestDictionry);
        if (!string.IsNullOrEmpty(shopURL) && !Utility.IsValidWooCommerceUrl(shopURL.ToLower()))
        {
          throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Invalid ShopURL Please enter valid shopURL with https e.g. https://shipra.woocommerce.com");
        }
        else
        {
          saleChannelKey = shopURL.ToLower();
        }

        oSaleChannelConfig!.UpdateSaleChannelConfig(request.StoreId, saleChannelJsonData, request!.SaleChannelName!, oSaleChannelConfig.Active.GetValueOrDefault(), request.IsAllowToDisplayInSaleChannel, _currentUser.EmployeeId,settingConfig!, saleChannelKey);
        await _SaleChannelConfigRepository.UpdateSaleChannelConfig(oSaleChannelConfig);
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oSaleChannelConfig?.SaleChannelConfigId, Message = NotificationConstants.Success });
      }
      else
      {
        throw new ShipraApplicationException(System.Net.HttpStatusCode.BadRequest, "Please enter valid sale channel.");
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
public class UpdateSaleChannelConfigCommandValidator : AbstractValidator<UpdateSaleChannelConfigCommand>
{
  public UpdateSaleChannelConfigCommandValidator()
  {
    RuleFor(v => v.SettingConfig).NotNull().NotEmpty();
    RuleFor(v => v.StoreId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.SaleChannelName).NotNull().NotEmpty();
  }
}
