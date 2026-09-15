using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCities;
public class GetCitiesQuery : IRequest<ServiceResultDTO>
{
  public int Id { get; set; }
  public string? CivilEntity { get; set; }
}
public class GetCitiesQueryHandler : RequestHandlerBase<GetCitiesQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetCitiesQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetCitiesQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCitiesQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var list = await _countryRepository.GetCities(request.Id,request.CivilEntity);
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
public class GetCitiesQueryValidator : AbstractValidator<GetCitiesQuery>
{
  public GetCitiesQueryValidator()
  {
    RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(x => x.CivilEntity).NotEmpty().NotNull(); 
  }
}
