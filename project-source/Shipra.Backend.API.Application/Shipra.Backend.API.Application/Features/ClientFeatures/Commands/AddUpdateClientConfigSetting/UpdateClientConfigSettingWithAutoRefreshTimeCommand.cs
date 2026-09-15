using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.AddUpdateClientConfigSetting;
public class UpdateClientConfigSettingWithAutoRefreshTimeCommand : IRequest<ServiceResultDTO>
{
  public string? ClientId { get; set; }
  public bool? AutoOrderStatusUpdate { get; set; }
  public int RefreshOrderMinut { get; set; }
}
public class AddUpdateClientConfigSettingCommandHandler : RequestHandlerBase<UpdateClientConfigSettingWithAutoRefreshTimeCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public AddUpdateClientConfigSettingCommandHandler(IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<AddUpdateClientConfigSettingCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateClientConfigSettingWithAutoRefreshTimeCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is null)
      {
        throw new EntityNotFoundException("Client", _currentUser.ClientIdStr!);
      }

      ClientConfigSetting clientSettingConfig = await _clientRepository.GetClientConfigSetting(_currentUser.ClientId!);
      if (clientSettingConfig is null)
      {
        clientSettingConfig = ClientConfigSetting.Create(_currentUser.ClientId!,request.AutoOrderStatusUpdate, request.RefreshOrderMinut);
        var a = await _clientRepository.CreateClientConfigSetting(clientSettingConfig);

      }
      else
      {
        clientSettingConfig.UpdateAutoStatusUpdateSetting(request.AutoOrderStatusUpdate, request.RefreshOrderMinut);
        await _clientRepository.UpdateClientConfigSetting(clientSettingConfig);
      }
      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = true, Message = "Update successfully" });
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
