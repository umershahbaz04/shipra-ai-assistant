using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetStatesByCountryId;
public class GetStatesByCountryIdQuery : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
}
public class GetStatesByCountryIdQueryHandler : RequestHandlerBase<GetStatesByCountryIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetStatesByCountryIdQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetStatesByCountryIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetStatesByCountryIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<State> list = await _countryRepository.GetStatesByCountryId(request.CountryId);
      var obj = State.AddDefault();
      list.Add(obj);

      var data = list.Select(x => new
      {
        x.StateId,
        x.Name
      }).OrderBy(x => x.StateId).ToList();

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
public class GetStatesByCountryIdQueryValidator : AbstractValidator<GetStatesByCountryIdQuery>
{
}
