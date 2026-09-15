using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.Common.Helpers;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CheckDuplicationCarrierAliasByClient;
using Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierWebHookUrlByCarrierId;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.SharedKernel.Interfaces;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CreateActiveCarrier;
public class CreateActiveCarrierCommandHandler : RequestHandlerBase<CreateActiveCarrierCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly IMediator _mediator;
  private readonly IConfigRepository _configRepository;
  private readonly ICarrierSharedRepository _carrierSharedRepository;
  ICarrierRepository _carrierRepository;
  public CreateActiveCarrierCommandHandler(IMediator mediator, IConfigRepository configRepository, ICarrierSharedRepository carrierSharedRepository, ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<CreateActiveCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _mediator = mediator;
    _configRepository = configRepository;
    _carrierSharedRepository = carrierSharedRepository;
    _carrierRepository = carrierRepository;
  }
  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(CreateActiveCarrierCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    try
    {
      var dict = request.InputParameters! != null && request.InputParameters!.Count > 0 ? Utils.ConvertKeysToCamelCase(request.InputParameters!) : new Dictionary<string, string>();//JsonConvert.DeserializeObject<Dictionary<string, string>>(request.InputParameter!); 

      if (request.CarrierId > 0)
      {
        //check if dipatchex carrier already create

        var carrier = await _carrierRepository.GetCarrierFromMasterDbById(request.CarrierId.GetValueOrDefault());
        #region exist carrier and config check
        if (carrier is null)
        {
          throw new EntityNotFoundException("Carrier ", request.CarrierId);
        }
        if (string.IsNullOrEmpty(carrier!.Config!))
        {
          throw new EntityNotFoundException("CarrierConfig ", carrier!.Config!);
        }

        #endregion

        #region cehck duplication
        CheckDuplicationCarrierAliasByClientCommand checkDuplicationCarrierAlias = new();
        checkDuplicationCarrierAlias.CarrierId = request.CarrierId;
        checkDuplicationCarrierAlias.CarrierAlias = request.CarrierAlias;
        checkDuplicationCarrierAlias.CarrierContractTypeId = request.CarrierContractTypeId;
        var serviceResultDTO = await _mediator.Send(checkDuplicationCarrierAlias);

        if (!serviceResultDTO.IsSuccess)
        {
          serviceResult.Errors = serviceResultDTO.Errors;
          serviceResult.IsSuccess = serviceResultDTO.IsSuccess;

          return serviceResult;
        }

        #endregion


        // Step 1: Deserialize into Dictionary<string, object>
        var originalDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(carrier!.Config!);

        // Step 2: Keep only string-string pairs
        var stringOnlyDict = originalDict!
            .Where(kvp => kvp.Value is string)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!.ToString()!);
         
        var carrierDic = Utils.ConvertKeysToCamelCase(stringOnlyDict);

        #region update config model
        foreach (var entry in dict)
        {
          // do something with entry.Value or entry.Key 
          if (Utils.CheckKeyExistFromDictionryByKey(entry.Key, carrierDic))
          {
            carrierDic[entry.Key] = entry.Value;
          }
        }
        string? userName = Utils.GetValueFromDictionryByKey("userName", carrierDic);
        //if alias not exist then username will be alias
        if (string.IsNullOrEmpty(request.CarrierAlias))
        {
          request.CarrierAlias = userName;
        }
        #endregion

        var jsonStr = JsonConvert.SerializeObject(carrierDic, Formatting.Indented);
        if (carrier!.IsDispatchExCompany.GetValueOrDefault())
        {
          var mcconfig = await _configRepository.GetMcconfigByKey(ApplicationConstants.IntegrationKey, _currentUser.EnvironmentTypeId);
          if (mcconfig is null)
          {
            throw new EntityNotFoundException("Mcconfig", "Integration Value");
          }
          #region if dispatch carrir then we must validate and give response accordingly
          dynamic validationResponse = await _carrierSharedRepository.ValidateCarrierForActivation(request.CarrierId.GetValueOrDefault(), jsonStr, mcconfig.Value!, _currentUser.ClientIdStr!);
          var deseralisedResponse = JsonConvert.DeserializeObject<AuthResponseModel<ValidateCarrierResponseModel>>(validationResponse);

          if (deseralisedResponse!.isSuccess)
          {
            serviceResult = await CreateCarrier(request, carrier, jsonStr, request.CarrierAlias, userName, request.CarrierContractTypeId);
            return serviceResult;
          }
          else
          {
            serviceResult.StatusCode = 400;
            serviceResult.Errors?.Add("InvalidUserNamePassword", new[] { "Invalid UserName or Password" });
            serviceResult.IsSuccess = deseralisedResponse!.isSuccess;
            return serviceResult;
          }
          #endregion
        }
        else
        {
          //for all other compnies create create and update 
          serviceResult = await CreateCarrier(request, carrier, jsonStr, request.CarrierAlias, userName, request.CarrierContractTypeId);
          return serviceResult;
        }
      }
      else
      {
        serviceResult.StatusCode = 404;
        serviceResult.Errors?.Add("CarrierNotFound", new[] { "Carrier Key No Found" });
        serviceResult.IsSuccess = false;
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> CreateCarrier(CreateActiveCarrierCommand request, Carrier carrier, string jsonStr, string carrierAlias, string userName, int carrierContractTypeId)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    int? regionTimeZoneId = (int)EnumRegionTimeZone.GulfRegion;
    if (carrier.RegionTimeZoneId != null)
    {
      regionTimeZoneId = carrier.RegionTimeZoneId;
    }

    string? settingConfig = null;
    if (request.SettingConfig != null && request.SettingConfig!.Count > 0)
    {
      settingConfig = JsonConvert.SerializeObject(request.SettingConfig!, new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      });
    }
    int activeCarrierId = 0;
    if (carrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
    {
     var obj = ShipraContractCarrier.CreateShipraContractCarrier(request.FlatRate.GetValueOrDefault(),carrier!.CarrierId, carrier.RegionTimeZoneId, carrier.IsWebhookSupported, settingConfig, jsonStr, request.CarrierAlias, userName);
      await _carrierRepository.CreateShipraContractCarrier(obj);
    }
    else
    {
      var activateCarrier = ActiveCarrier.CreateActiveCarrier(carrier!.CarrierId, _currentUser.ClientId!, _currentUser.EmployeeId!, carrier.RegionTimeZoneId, carrier.IsWebhookSupported, settingConfig, false, jsonStr, true, request.CarrierAlias, userName,request.CarrierLocationId);
      await _carrierRepository.CreateActiveCarrier(activateCarrier);
      activeCarrierId = activateCarrier.ActiveCarrierId;
    }

    #region create webhook url hash 
    GetCarrierWebHookUrlByCarrierIdQuery getCarrierWebHookUrlByCarrierId = new GetCarrierWebHookUrlByCarrierIdQuery()
    {

      CarrierId = carrier.CarrierId,
      ActiveCarrierId = activeCarrierId,
      CarrierContractTypeId = carrierContractTypeId
    };
    await _mediator.Send(getCarrierWebHookUrlByCarrierId);
    #endregion

    serviceResult.CreateSuccessResponse();
    return serviceResult;
  }
  #region create and update
  private async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> CreateOrUpdateCarrier(CreateActiveCarrierCommand request, Carrier carrier, string jsonStr, string carrierAlias, string userName)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    int? regionTimeZoneId = (int)EnumRegionTimeZone.GulfRegion;
    if (carrier.RegionTimeZoneId != null)
    {
      regionTimeZoneId = carrier.RegionTimeZoneId;
    }
    var oActiveCarrier = await _carrierRepository.GetActiveCarrierByCarrierIdAndUserName(carrier!.CarrierId, userName, _currentUser.ClientId!);
    if (oActiveCarrier is null)
    {
      string? settingConfig = null;
      if (request.SettingConfig != null && request.SettingConfig!.Count > 0)
      {
        settingConfig = JsonConvert.SerializeObject(request.SettingConfig!, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
      }

      var activateCarrier = ActiveCarrier.CreateActiveCarrier(carrier!.CarrierId, _currentUser.ClientId!, _currentUser.EmployeeId!, carrier.RegionTimeZoneId, carrier.IsWebhookSupported, settingConfig, false, jsonStr, true, request.CarrierAlias, userName,request.CarrierLocationId);
      await _carrierRepository.CreateActiveCarrier(activateCarrier);

      #region create webhook url hash 
      GetCarrierWebHookUrlByCarrierIdQuery getCarrierWebHookUrlByCarrierId = new GetCarrierWebHookUrlByCarrierIdQuery()
      {

        CarrierId = carrier.CarrierId,
        ActiveCarrierId = activateCarrier.ActiveCarrierId
      };
      await _mediator.Send(getCarrierWebHookUrlByCarrierId);
      #endregion
    }
    else
    {
      oActiveCarrier.UpdateActiveCarrier(carrier!.CarrierId, oActiveCarrier.ClientId!, jsonStr, true, request.CarrierAlias, userName, _currentUser.EmployeeId!);

      await _carrierRepository.UpdateActiveCarrier(oActiveCarrier);
    }
    serviceResult.CreateSuccessResponse();
    return serviceResult;
  }

  #endregion
}
