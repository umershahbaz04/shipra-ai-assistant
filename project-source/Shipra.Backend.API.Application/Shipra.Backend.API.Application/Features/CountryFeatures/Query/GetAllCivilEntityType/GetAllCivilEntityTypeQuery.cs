using System;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCivilEntityType;
public class GetAllCivilEntityTypeQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllCivilEntityTypeQueryHandler : RequestHandlerBase<GetAllCivilEntityTypeQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetAllCivilEntityTypeQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetAllCivilEntityTypeQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCivilEntityTypeQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _countryRepository.GetAllCivilEntityType();
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
