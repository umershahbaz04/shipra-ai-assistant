using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetAddressEntitiesByType;
public class GetAddressEntitiesByTypeQuery : IRequest<ServiceResultDTO>
{
  public string? SelectedEntityIds { get; set; }
  public string? SelectedEntityType { get; set; } 
  public string? NextEntityType { get; set; }
  public int? CarrierId { get; set; }
  public int? CountryId { get; set; }
}
public class GetAddressEntitiesByTypeQueryHandler : RequestHandlerBase<GetAddressEntitiesByTypeQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetAddressEntitiesByTypeQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetAddressEntitiesByTypeQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAddressEntitiesByTypeQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      dynamic data = await _countryRepository.GetAddressEntitiesByType(request.SelectedEntityIds,request.SelectedEntityType,request.NextEntityType,request.CountryId,request.CarrierId);
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
public class GetAddressEntitiesByTypeQueryValidator : AbstractValidator<GetAddressEntitiesByTypeQuery>
{
  public GetAddressEntitiesByTypeQueryValidator()
  {
    RuleFor(x => x.SelectedEntityIds).NotEmpty().NotNull();
    RuleFor(x => x.SelectedEntityType).NotEmpty().NotNull();
    RuleFor(x => x.NextEntityType).NotEmpty().NotNull();
  }
}
