using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAllCityByNameForSelection;
public class GetAllCityByNameForSelectionQuery : IRequest<ServiceResultDTO>
{
  public string? Search { get; set; }
  public int? RegionId { get; set; }
}
public class GetAllCityByNameForSelectionQueryHandler : RequestHandlerBase<GetAllCityByNameForSelectionQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetAllCityByNameForSelectionQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetAllCityByNameForSelectionQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCityByNameForSelectionQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      dynamic list = await _countryRepository.GetAllAreaByNameForSelection(request.RegionId,request.Search);
      serviceResult = new ServiceResultDTO(list);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
