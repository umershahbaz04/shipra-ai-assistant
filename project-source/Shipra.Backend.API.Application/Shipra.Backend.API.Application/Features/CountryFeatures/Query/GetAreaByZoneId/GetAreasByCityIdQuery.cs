using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CommonUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAreaByZoneIdQuery;
public class GetAreasByCityIdQuery : IRequest<ServiceResultDTO>
{ 
  public long CityId { get; set; }
}

public class GetAreasByCityIdQueryHandler : RequestHandlerBase<GetAreasByCityIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetAreasByCityIdQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetAreasByCityIdQuery> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAreasByCityIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var areas = await _countryRepository.GetAllAreaByCityId(request.CityId);

      var mapperDto = areas!.Select(x =>  new { x.AreaId, x.Name }).ToList();

      serviceResult = new ServiceResultDTO(mapperDto);

      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetAreasByCityIdQueryValidator : AbstractValidator<GetAreasByCityIdQuery>
{
  public GetAreasByCityIdQueryValidator()
  {
    RuleFor(x => x.CityId).NotEmpty().NotNull();
  }
}
