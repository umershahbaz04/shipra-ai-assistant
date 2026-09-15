using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCountry;
public class GetAllCountryQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllCountryQueryHandler : RequestHandlerBase<GetAllCountryQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;
  public GetAllCountryQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetAllCountryQuery> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCountryQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      dynamic countryList = await _countryRepository.GetAllCountries(); 
      if (countryList is not null)
      {
        serviceResult = new ServiceResultDTO(countryList);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
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
