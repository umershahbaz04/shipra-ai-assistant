using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCountryCityRegionIdByName;
public class GetCountryCityAreaIdByNameQuery : IRequest<ServiceResultDTO>
{
  public string? CountryName { get; set; }
  public string? CityName { get; set; }
  public string? AreaName { get; set; }
}
public class GetCountryCityAreaIdByNameQueryHandler : RequestHandlerBase<GetCountryCityAreaIdByNameQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetCountryCityAreaIdByNameQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetCountryCityAreaIdByNameQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCountryCityAreaIdByNameQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      dynamic oCountryRegionCity = await _countryRepository.GetCounrtyCityRegionIdByName(request.CountryName!, request.AreaName!, request.CityName!);
      if (oCountryRegionCity is not null)
      {
        serviceResult = new ServiceResultDTO(oCountryRegionCity);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        return serviceResult;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.Error));
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
