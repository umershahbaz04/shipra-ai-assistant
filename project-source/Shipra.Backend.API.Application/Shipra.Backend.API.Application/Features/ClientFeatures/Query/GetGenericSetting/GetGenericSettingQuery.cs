using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Helper;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetGenericSetting;
public class GetGenericSettingQuery : IRequest<ServiceResultDTO>
{
}
public class GetGenericSettingQueryHandler : RequestHandlerBase<GetGenericSettingQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public GetGenericSettingQueryHandler(IClientRepository clientRepository,IServiceProvider serviceProvider, ILogger<GetGenericSettingQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetGenericSettingQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var clientGenericSetting = await _clientRepository.ClientGenericSettingById(_currentUser.ClientId!);
      var genericSettingLookups = await _clientRepository.GetClientGenericSettingLookup();

      if (clientGenericSetting is null)
      {
        clientGenericSetting = ClientGenericSetting.Create(_currentUser.ClientId!, genericSettingLookups?.SettingConfig);
        serviceResult.IsSuccess = await _clientRepository.CreateClientGenericSetting(clientGenericSetting!);
      }

      // Default to empty JSON string if SettingConfig is null or empty
      string jsonString = clientGenericSetting?.SettingConfig ?? "";

      if (!string.IsNullOrEmpty(genericSettingLookups?.SettingConfig))
      {
        var carrierSetting = JsonConvert.DeserializeObject<List<GeneralSettingConfigModel>>(genericSettingLookups.SettingConfig);
        var activeCarrierSetting = JsonConvert.DeserializeObject<List<GeneralSettingConfigModel>>(clientGenericSetting?.SettingConfig ?? "[]");

        string jsonCarrierSetting = JsonConvert.SerializeObject(carrierSetting, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver(),
          Formatting = Formatting.Indented,
        });

        string jsonActiveCarrierSetting = JsonConvert.SerializeObject(activeCarrierSetting, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver(),
          Formatting = Formatting.Indented, 
        });

        jsonString = GetConfigUpdatedJsonWithoutAppend(jsonCarrierSetting, jsonActiveCarrierSetting);

        var mergedSettings = JsonConvert.DeserializeObject<List<GeneralSettingConfigModel>>(jsonString);
        int activeKeysCount = activeCarrierSetting?.Sum(s => s.InputData?.Count ?? 0) ?? 0;
        int mergedKeysCount = mergedSettings?.Sum(s => s.InputData?.Count ?? 0) ?? 0;

        if (activeKeysCount != mergedKeysCount)
        {
          clientGenericSetting!.UpdateConfig(jsonString);
          await _clientRepository.UpdateClientGenericSetting(clientGenericSetting);
        }
      }

      // Ensure `clientGenericSetting` is not null before creating a success response
      serviceResult = new ServiceResultDTO(jsonString);  
      return serviceResult; 
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private string GetConfigUpdatedJsonWithoutAppend(string json1, string json2)
  {
    Newtonsoft.Json.Linq.JArray array1 = Newtonsoft.Json.Linq.JArray.Parse(json1);
    Newtonsoft.Json.Linq.JArray array2 = Newtonsoft.Json.Linq.JArray.Parse(json2);

    foreach (var section2 in array2)
    {
      string section2Name = section2!["sectionName"]!.ToString().Trim().ToLower();

      var section1 = array1.FirstOrDefault(s =>
          s["sectionName"]!.ToString().Trim().ToLower() == section2Name);

      if (section1 != null)
      {
        var inputData1 = section1["inputData"] as Newtonsoft.Json.Linq.JArray;
        var inputData2 = section2["inputData"] as Newtonsoft.Json.Linq.JArray;

        if (inputData1 != null && inputData2 != null)
        {
          foreach (var input2 in inputData2)
          {
            var input1 = inputData1.FirstOrDefault(i =>
                i != null &&
                (string?)i["key"] == (string?)input2["key"]);

            if (input1 != null)
            {
              input1["value"] = input2["value"];
            }
          }
        }
      }
    }

    return array1.ToString(Formatting.Indented);
  }
}
