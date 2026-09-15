using System.Linq;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.CreateDriverCTSSetting;
public class CreateDriverCTSSettingCommand : IRequest<ServiceResultDTO>
{
  public string? CarrierTrackingStatusIds { get; set; }
}
public class CreateDriverCTSSettingCommandHandler : RequestHandlerBase<CreateDriverCTSSettingCommand, ServiceResultDTO>
{
  private readonly IDriverRepository _driverRepository;

  public CreateDriverCTSSettingCommandHandler(IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<CreateDriverCTSSettingCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateDriverCTSSettingCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      if (!string.IsNullOrEmpty(request.CarrierTrackingStatusIds))
      {
        await _driverRepository.DeleteDriverCTSSetting(_currentUser.ClientId!);
        var newCarrierTrackingStatusIdList = request?.CarrierTrackingStatusIds?.Split(",").ToList();

        foreach (var carrierTrackingStatusId in newCarrierTrackingStatusIdList!)
        {
          _ = await _driverRepository.CreateDriverCTSSetting(DriverCTSSetting.Create(_currentUser.ClientId!, Int32.Parse(carrierTrackingStatusId), _currentUser.EmployeeId!));
        }
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = "", Message = "Record added successfully." });
      }
      else
      { 
        serviceResult.CreateError("Wrong", new string[] { "Something went wrong!" });
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
