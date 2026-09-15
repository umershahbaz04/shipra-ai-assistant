using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.Core.WalletAggregate;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetSettingConfigByActiveCarrierId;
public class GetSettingConfigByActiveCarrierIdQuery : IRequest<ServiceResultDTO>
{
  public int ActiveCarrierId { get; set; }
  public int CarrierContractTypeId { get; set; } = (int)EnumCarrierContractType.OwnContractType;
  public int? ShipraContractCarrierId { get; set; }
}
public class GetSettingConfigByActiveCarrierIdQueryHandler : RequestHandlerBase<GetSettingConfigByActiveCarrierIdQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetSettingConfigByActiveCarrierIdQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetSettingConfigByActiveCarrierIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetSettingConfigByActiveCarrierIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      ActiveCarrierContractResponseModel? activeCarrier = null;
      if (request.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType)
      {
        ShipraContractCarrier? shipraContractCarrier = await _carrierRepository.GetShipraContractCarrierByContractId(request.ShipraContractCarrierId.GetValueOrDefault());

        if (shipraContractCarrier == null)
        {
          throw new EntityNotFoundException("ShipraContractCarrier", request.ShipraContractCarrierId.GetValueOrDefault());
        }
        activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(shipraContractCarrier);

      }
      else
      {

        ActiveCarrier? oActiveCarrier = await _carrierRepository.GetActiveCarrierByActiveCarrierId(request.ActiveCarrierId, _currentUser.ClientId!);
        if (oActiveCarrier == null)
        {
          throw new EntityNotFoundException("ActiveCarrier", request.ActiveCarrierId);
        }
        activeCarrier = _mapper.Map<ActiveCarrierContractResponseModel>(oActiveCarrier);

      }

      var oCarrier = await _carrierRepository.GetCarrierFromMasterDbById(activeCarrier.CarrierId);
      if (oCarrier == null)
      {
        throw new EntityNotFoundException("Carrier", activeCarrier.CarrierId);
      }
      string? jsonString = !string.IsNullOrEmpty(activeCarrier.SettingConfig!) ? activeCarrier.SettingConfig! : "";
      string? jsonConfigString = !string.IsNullOrEmpty(activeCarrier.SettingConfig!) ? activeCarrier.SettingConfig! : "";
      #region MyRegion 
      if (!string.IsNullOrEmpty(oCarrier.SettingConfig!))
      {
        List<GeneralSettingConfigModel>? carrierSetting = JsonConvert.DeserializeObject<List<GeneralSettingConfigModel>>(oCarrier.SettingConfig!);

        List<GeneralSettingConfigModel>? activeCarrierSetting = JsonConvert.DeserializeObject<List<GeneralSettingConfigModel>>(activeCarrier.SettingConfig!);

        string jsonCarrierSetting = JsonConvert.SerializeObject(carrierSetting, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        string jsonActiveCarrierSetting = JsonConvert.SerializeObject(activeCarrierSetting, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        jsonString = GetUpdatedJson(jsonCarrierSetting!, jsonActiveCarrierSetting!);
      }
      #endregion
      #region MyRegion 
      if (!string.IsNullOrEmpty(oCarrier.Config!))
      {
        CarrierSettingsConfig? carrierSetting = JsonConvert.DeserializeObject<CarrierSettingsConfig>(oCarrier.Config!);

        CarrierSettingsConfig? activeCarrierSetting = JsonConvert.DeserializeObject<CarrierSettingsConfig>(activeCarrier.Config!);


        // Update setting1 values with setting2 values
        carrierSetting!.AccountNumber = activeCarrierSetting!.AccountNumber;
        carrierSetting!.UserName = activeCarrierSetting!.UserName;
        carrierSetting!.Password = activeCarrierSetting!.Password;
        carrierSetting!.DomainProdURL = activeCarrierSetting!.DomainProdURL;
        carrierSetting!.DomainTestURL = activeCarrierSetting!.DomainTestURL;

        // Serialize back to JSON
        jsonConfigString = JsonConvert.SerializeObject(carrierSetting!, Formatting.Indented);


      }
      #endregion

      List<GeneralSettingConfigModel>? oClientCarrierSettingConfigList = JsonConvert.DeserializeObject<List<GeneralSettingConfigModel>>(jsonString); 
      serviceResult = new ServiceResultDTO(new { carrier = activeCarrier, settingConfig = oClientCarrierSettingConfigList! , config = jsonConfigString, isDispatchExCompany = oCarrier.IsDispatchExCompany.GetValueOrDefault(false)});

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
  public static string GetUpdatedJson(string json1, string json2)
  {
    JArray array1 = JArray.Parse(json1);
    JArray array2 = JArray.Parse(json2);

    foreach (var section2 in array2)
    {
      // Trim and convert section names to lowercase for comparison
      string section2Name = section2!["sectionName"]!.ToString().Trim().ToLower();

      // Find corresponding section in array1
      var section1 = array1.FirstOrDefault(s =>
          s["sectionName"]!.ToString().Trim().ToLower() == section2Name);

      if (section1 != null)
      {
        UpdateSection(section1, section2);
      }
    }

    return array1.ToString(Formatting.Indented);
  }
  private static void UpdateSection(JToken section1, JToken section2)
  {
    var inputData1 = section1["inputData"] as JArray;
    var inputData2 = section2["inputData"] as JArray;

    if (inputData1 != null)
    {
      foreach (var input2 in inputData2!)
      {
        var input1 = inputData1.FirstOrDefault(i =>
            i != null &&
            i["key"]?.Value<string>() == input2["key"]?.Value<string>());

        if (input1 != null)
        {
          // Update existing input1 with input2's value
          input1["value"] = input2["value"];
        }
        else
        {
          // Add input2 to inputData1 if input1 is not found
          inputData1.Add(input2);
        }
      }
    }
  }
}
public class GetSettingConfigByActiveCarrierIdQueryValidator : AbstractValidator<GetSettingConfigByActiveCarrierIdQuery>
{
  public GetSettingConfigByActiveCarrierIdQueryValidator()
  {
    When(v => v.CarrierContractTypeId > 0 && v.CarrierContractTypeId == (int)EnumCarrierContractType.OwnContractType, () =>
    {
      RuleFor(x => x.ActiveCarrierId).NotEmpty().NotNull().GreaterThan(0);
    });
    When(v => v.CarrierContractTypeId > 0 && v.CarrierContractTypeId == (int)EnumCarrierContractType.ShipraContractType, () =>
    {
      RuleFor(x => x.ShipraContractCarrierId).NotNull().NotEmpty().GreaterThan(0);
    });
  }
}
