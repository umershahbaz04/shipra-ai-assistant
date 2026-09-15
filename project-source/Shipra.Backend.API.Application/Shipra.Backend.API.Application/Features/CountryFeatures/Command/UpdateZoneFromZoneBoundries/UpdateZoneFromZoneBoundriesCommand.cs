using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Command.UpdateZoneFromZoneBoundries;

public class UpdateZoneFromZoneBoundriesCommand : IRequest<ServiceResultDTO>
{
  public string ZoneName { get; set; } = string.Empty;
  public int ZoneID { get; set; }
  public int CityID { get; set; }
}

public class UpdateZoneFromZoneBoundriesCommandHandler : RequestHandlerBase<UpdateZoneFromZoneBoundriesCommand, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public UpdateZoneFromZoneBoundriesCommandHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<UpdateZoneFromZoneBoundriesCommand> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(UpdateZoneFromZoneBoundriesCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      bool res = await _countryRepository.UpdateZoneFromZoneBoundries(request.ZoneName, request.ZoneID, request.CityID);
      var responseObj = new
      {
        Result = res
      };
      serviceResult = new ServiceResultDTO(responseObj);
      serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
