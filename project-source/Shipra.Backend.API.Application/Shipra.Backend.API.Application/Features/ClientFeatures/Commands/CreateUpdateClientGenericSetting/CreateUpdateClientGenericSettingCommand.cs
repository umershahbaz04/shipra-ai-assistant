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
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
using Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.CheckDuplicationCarrierAliasByClient;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;
using Shipra.Backend.API.SharedKernel.Models;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateUpdateClientGenericSetting;
public class CreateUpdateClientGenericSettingCommand : IRequest<ServiceResultDTO>
{
  public List<GeneralSettingConfigModel>? SettingConfig { get; set; } = new();
}
public class CreateUpdateClientGenericSettingCommandHandler : RequestHandlerBase<CreateUpdateClientGenericSettingCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public CreateUpdateClientGenericSettingCommandHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<CreateUpdateClientGenericSettingCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateUpdateClientGenericSettingCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var oClientGenericSetting = await _clientRepository.ClientGenericSettingById(_currentUser.ClientId!);

      string? settingConfig = null;
      if (request.SettingConfig != null && request.SettingConfig!.Count > 0)
      {
        settingConfig = JsonConvert.SerializeObject(request.SettingConfig!, new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver(),
          Formatting = Formatting.Indented,
        });
      }
      if (oClientGenericSetting is null)
      {
        oClientGenericSetting = ClientGenericSetting.Create(_currentUser.ClientId!, settingConfig);
        serviceResult.IsSuccess = await _clientRepository.CreateClientGenericSetting(oClientGenericSetting!);
      }
      else
      {
        oClientGenericSetting.UpdateConfig(settingConfig);
        serviceResult.IsSuccess = await _clientRepository.UpdateClientGenericSetting(oClientGenericSetting);
      }

      if (serviceResult.IsSuccess)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = oClientGenericSetting?.ClientGenericSettingId, Message = "Action perform successfully" });
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
