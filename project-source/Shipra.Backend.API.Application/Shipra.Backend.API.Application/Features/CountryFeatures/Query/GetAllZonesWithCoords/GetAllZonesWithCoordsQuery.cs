using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllZonesWithCoords;

public class GetAllZonesWithCoordsQuery : IRequest<ServiceResultDTO>
{
}

public class GetAllZonesWithCoordsQueryHandler : RequestHandlerBase<GetAllZonesWithCoordsQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetAllZonesWithCoordsQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetAllZonesWithCoordsQuery> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllZonesWithCoordsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var zones = await _countryRepository.GetAllZonesWithCoords();
      var responseObj = new
      {
        Result = zones != null && zones.Any(),
        Msg = "",
        Zones = zones
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
