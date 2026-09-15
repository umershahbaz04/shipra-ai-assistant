using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.DriverAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.DeleteDriverCTSSetting;
public class DeleteDriverCTSSettingCommand : IRequest<ServiceResultDTO>
{
  public string? driverCtsid { get; set; }
}
public class DeleteDriverCtssettingCommandHandler : RequestHandlerBase<DeleteDriverCTSSettingCommand, ServiceResultDTO>
{
  private readonly IDriverRepository _driverRepository;

  public DeleteDriverCtssettingCommandHandler(IDriverRepository driverRepository, IServiceProvider serviceProvider, ILogger<DeleteDriverCtssettingCommandHandler> logger) : base(serviceProvider, logger)
  {
    _driverRepository = driverRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteDriverCTSSettingCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var isDeleted = await _driverRepository.DeleteDriverCTSSetting(new DriverCTSSettingId(new Guid(request.driverCtsid!)));
      if (isDeleted)
      {
        serviceResult.CreateSuccessResponse();
        serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = "", Message = "Record deleted successfully." });
      }
      else {
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
