using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetGoogleMapRestrictedCountry;
public class GetGoogleMapRestrictedCountryQuery  : IRequest<ServiceResultDTO>
{
}
public class GetGoogleMapRestrictedCountryQueryHandler : RequestHandlerBase<GetGoogleMapRestrictedCountryQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetGoogleMapRestrictedCountryQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetGoogleMapRestrictedCountryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetGoogleMapRestrictedCountryQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var countries = await _countryRepository.GetAllCountries();
      serviceResult = new ServiceResultDTO(countries.Select(x => x.MapCountryCode).ToList());
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
