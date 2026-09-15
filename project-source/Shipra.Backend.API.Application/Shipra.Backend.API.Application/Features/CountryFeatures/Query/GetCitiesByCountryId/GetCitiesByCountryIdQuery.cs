using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCitiesByCountryId;
public class GetCitiesByCountryIdQuery : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
}
public class GetCitiesByCountryIdQueryHandler : RequestHandlerBase<GetCitiesByCountryIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetCitiesByCountryIdQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetCitiesByCountryIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCitiesByCountryIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<City> list = await _countryRepository.GetCitiesByCountryId(request.CountryId);
      var obj = City.AddDefault();
      list.Add(obj);

      var data = list.Select(x => new
      {
        x.CityId,
        x.Name
      }).OrderBy(x => x.CityId).ToList();

      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
