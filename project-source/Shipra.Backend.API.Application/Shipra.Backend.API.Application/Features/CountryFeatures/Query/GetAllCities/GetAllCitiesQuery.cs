using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCities;

public class GetAllCitiesQuery : IRequest<ServiceResultDTO>
{
}

public class GetAllCitiesQueryHandler : RequestHandlerBase<GetAllCitiesQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetAllCitiesQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetAllCitiesQuery> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCitiesQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var cities = await _countryRepository.GetAllCities();
      var responseObj = new
      {
        List = cities
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
