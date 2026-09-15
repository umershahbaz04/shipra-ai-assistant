using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetBlocksByCityId;
public class GetPinCodesByCityIdQuery : IRequest<ServiceResultDTO>
{
  public int CityId { get; set; }
}
public class GetPinCodesByCityIdQueryHandler : RequestHandlerBase<GetPinCodesByCityIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetPinCodesByCityIdQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetPinCodesByCityIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetPinCodesByCityIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var list = await _countryRepository.GetPinCodesByCityId(request.CityId);
      var obj = PinCode.AddDefault();
      list.Add(obj);

      var data = list.Select(x => new
      {
        x.PinCodeId,
        x.PinCodeValue
      }).OrderBy(x => x.PinCodeId).ToList();

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
public class GetPinCodesByCityIdQueryValidator : AbstractValidator<GetPinCodesByCityIdQuery>
{
  public GetPinCodesByCityIdQueryValidator()
  {
    RuleFor(x => x.CityId).NotEmpty().NotNull().GreaterThan(0);
  }
}
