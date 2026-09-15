using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Query.GetAllRegionTimeZone;
public class GetAllRegionTimeZoneQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllRegionTimeZoneQueryHandler : RequestHandlerBase<GetAllRegionTimeZoneQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetAllRegionTimeZoneQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetAllRegionTimeZoneQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllRegionTimeZoneQuery request, CancellationToken cancellationToken)
  {           
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _countryRepository.GetAllRegionTimeZone();
      var newList = data.Select(x => new
      {
        id = x.RegionTimeZoneId,
        text = x.TimeZoneName
      });
      serviceResult = new ServiceResultDTO(newList);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
