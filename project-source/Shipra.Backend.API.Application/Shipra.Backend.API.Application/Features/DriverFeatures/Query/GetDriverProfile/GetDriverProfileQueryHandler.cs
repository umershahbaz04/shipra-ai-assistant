using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.AccountUserCase;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetDriverProfile;
public class GetDriverProfileQueryHandler : RequestHandlerBase<GetDriverProfileQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly IDriverRepository _driverRepository;

  public GetDriverProfileQueryHandler(IClientRepository clientRepository, IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetDriverProfileQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetDriverProfileQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var oDriver = await _driverRepository.GetDriverByEmployeeId(_currentUser.EmployeeId!);
      if (oDriver is not null)
      {
        var oDriverProfile = await _driverRepository.GetDriverProfile(oDriver.DriverId!.Value.ToString(), _currentUser.ClientIdStr!);
        if (oDriverProfile is not null)
        {
          var oClientConfigSetting = await _clientRepository.GetClientConfigSetting(_currentUser.ClientId!);

          oDriverProfile.settingConfig = (oClientConfigSetting != null
          ? new LoginSettingConfigDto
          {
            ShowOrderLabel = oClientConfigSetting.AllowShipperInvocie.GetValueOrDefault()
          } : new LoginSettingConfigDto()); 

          serviceResult = new ServiceResultDTO(oDriverProfile);
          serviceResult.CreateSuccessResponse();
        }
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
