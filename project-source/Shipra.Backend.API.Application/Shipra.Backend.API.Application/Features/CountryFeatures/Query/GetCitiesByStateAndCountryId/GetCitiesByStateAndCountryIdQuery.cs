using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetStates;
public class GetCitiesByStateAndCountryIdQuery : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
  public int StateId { get; set; }
}
public class GetCitiesByStateAndCountryIdQueryHandler : RequestHandlerBase<GetCitiesByStateAndCountryIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetCitiesByStateAndCountryIdQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetCitiesByStateAndCountryIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCitiesByStateAndCountryIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var list = await _countryRepository.GetCitiesByStateAndCountryIdQuery(request.CountryId, request.StateId);
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
public class GetCitiesByStateAndCountryIdQueryValidator : AbstractValidator<GetCitiesByStateAndCountryIdQuery>
{
  public GetCitiesByStateAndCountryIdQueryValidator()
  {
    RuleFor(x => x.CountryId).NotNull().NotEmpty();
    RuleFor(x => x.StateId).NotNull().NotEmpty();
  }
}
