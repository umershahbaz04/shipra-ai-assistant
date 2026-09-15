using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCityByRegionIdQuery;
public class GetCityByRegionIdQuery : IRequest<ServiceResultDTO>
{
  public int RegionId { get; set; }
}

public class GetCityByRegionIdQueryHandler : RequestHandlerBase<GetCityByRegionIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;
  public GetCityByRegionIdQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetCityByRegionIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCityByRegionIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      await Task.Delay(1);
      //var cityList = await _countryRepository.GetCityByRegionId(request.RegionId);
      //cityList!.Insert(0, new City { CityId = ApplicationConstants.DropDownPlaceHolderId, Name = ApplicationConstants.DropDownPlaceHolderName });

      //if (cityList is not null)
      //{
      //  serviceResult = new ServiceResultDTO(cityList);
      //  serviceResult.CreateSuccessResponse();
      //}
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
