using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetRegionTimeZoneByClient;
public class GetRegionTimeZoneByClientQuery : IRequest<ServiceResultDTO>
{
}
public class GetRegionTimeZoneByClientQueryHandler : RequestHandlerBase<GetRegionTimeZoneByClientQuery, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;
  private readonly ICountryRepository _countryRepository;

  public GetRegionTimeZoneByClientQueryHandler(IClientRepository clientRepository ,ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetRegionTimeZoneByClientQueryHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetRegionTimeZoneByClientQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResultDTO = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      RegionTimeZone? oRegionTimeZone = new RegionTimeZone();
      if (client is not null)
      {
        oRegionTimeZone = await _countryRepository.GetRegionTimeZoneById(client.RegionTimeZoneId);
      }  
      serviceResultDTO = new ServiceResultDTO(oRegionTimeZone!);

      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}
