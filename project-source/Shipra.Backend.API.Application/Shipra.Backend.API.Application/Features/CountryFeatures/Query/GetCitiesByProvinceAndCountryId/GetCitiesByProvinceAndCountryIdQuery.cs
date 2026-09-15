using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCitiesByProvinceAndCountryId;
public class GetCitiesByProvinceAndCountryIdQuery : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
  public int ProvinceId { get; set; }
}
public class GetCitiesByProvinceAndCountryIdQueryHandler : RequestHandlerBase<GetCitiesByProvinceAndCountryIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetCitiesByProvinceAndCountryIdQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetCitiesByProvinceAndCountryIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCitiesByProvinceAndCountryIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var list = await _countryRepository.GetCitiesByProvinceAndCountryId(request.CountryId,request.ProvinceId);
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
public class GetCitiesByProvinceAndCountryIdQueryValidator : AbstractValidator<GetCitiesByProvinceAndCountryIdQuery>
{
  public GetCitiesByProvinceAndCountryIdQueryValidator()
  {
    RuleFor(x => x.CountryId).NotNull().NotEmpty();
    RuleFor(x => x.ProvinceId).NotNull().NotEmpty();
  }
}
