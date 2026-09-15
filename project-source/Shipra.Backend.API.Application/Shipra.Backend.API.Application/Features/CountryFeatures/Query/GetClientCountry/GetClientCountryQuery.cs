using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetClientCountry;
public class GetClientCountryQuery : IRequest<ServiceResultDTO>
{
}
public class GetClientCountryQueryHandler : RequestHandlerBase<GetClientCountryQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;
  private readonly IClientRepository _clientRepository;

  public GetClientCountryQueryHandler(ICountryRepository countryRepository, IClientRepository clientRepository, IServiceProvider serviceProvider, ILogger<GetClientCountryQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetClientCountryQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var client = await _clientRepository.GetClientById(_currentUser.ClientId!);
      if (client is not null)
      {
        ClientAddress? oClientAddress = await _clientRepository.GetClientAddress(_currentUser.ClientId);
        if (oClientAddress is not null)
        {
          var oCountry = await _countryRepository.GetCountryById(oClientAddress!.CountryId!);
          serviceResult = new ServiceResultDTO(oCountry!);
        }

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
