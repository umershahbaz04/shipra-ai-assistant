using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetAllDriverCtssetting;
public class GetAllDriverCTSSettingQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllDriverCTSSettingQueryHandler : RequestHandlerBase<GetAllDriverCTSSettingQuery, ServiceResultDTO>
{
  private readonly IDriverRepository _driverRepository;

  public GetAllDriverCTSSettingQueryHandler(IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<GetAllDriverCTSSettingQueryHandler> logger) : base(serviceProvider, logger)
  {
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDriverCTSSettingQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var list = await _driverRepository.GetAllDriverCTSSetting(_currentUser?.ClientIdStr!);
      serviceResult = new ServiceResultDTO(list);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
