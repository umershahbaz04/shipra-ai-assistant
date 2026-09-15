using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CountryUseCase.Request;
using Shipra.Backend.API.Application.DTOs.CountryUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCountryByIdQuery;
public class GetZoneByCityIdQuery : IRequest<ServiceResultDTOWithTypeModel<CityResponseModel>>
{
  public int CityId { get; set; }
}

public class GetZoneByCityIdQueryHandler : RequestHandlerBase<GetZoneByCityIdQuery, ServiceResultDTOWithTypeModel<CityResponseModel>>
{
  private readonly ICountryRepository _countryRepository;

  public GetZoneByCityIdQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetZoneByCityIdQuery> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<CityResponseModel>> HandleRequest(GetZoneByCityIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<CityResponseModel>();
    try
    {
      await Task.Delay(1);
      //int cityId = request.CityId;
      //var city = await _countryRepository.GetCityById(request.CityId);
      //if (city == null)
      //{
      //  throw new EntityNotFoundException("City ", request.CityId);
      //}

      //var cityModel = _mapper.Map<CityResponseModel>(city);
      //var zones = await _countryRepository.GetZoneByCityId(cityId);

      //var zoneMapping = _mapper.Map<List<ZoneRequestModel>>(zones);
      //cityModel.Zones = zoneMapping;
      //serviceResult = new ServiceResultDTOWithTypeModel<CityResponseModel>(cityModel);

      //serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
