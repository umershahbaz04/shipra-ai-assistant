using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCivilEntityIdByLatitudeAndLongitude;
public class GetCivilEntityIdByLatitudeAndLongitudeQuery : IRequest<ServiceResultDTO>
{
  public decimal Latitude { get; set; }
  public decimal Longitude { get; set; }
  public int? CarrierId { get; set; }
}
public class GetCivilEntityIdByLatitudeAndLongitudeQueryHandler : RequestHandlerBase<GetCivilEntityIdByLatitudeAndLongitudeQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetCivilEntityIdByLatitudeAndLongitudeQueryHandler(ICountryRepository countryRepository,IServiceProvider serviceProvider, ILogger<GetCivilEntityIdByLatitudeAndLongitudeQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCivilEntityIdByLatitudeAndLongitudeQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    { 
      dynamic res = await _countryRepository.GetCivilEntityIdByLatitudeAndLongitude(request.Latitude,request.Longitude,request.CarrierId);
        
      serviceResult = new ServiceResultDTO(res);
      return serviceResult;
    }
    catch (Exception ex)
    {
      _ = ex.Message;
      serviceResult.CreateError("InvalidLatLng",new string[] {"Invalid lat lng"});
      throw;
    }
  }
}
public class GetCivilEntityIdByLatitudeAndLongitudeQueryValidator : AbstractValidator<GetCivilEntityIdByLatitudeAndLongitudeQuery>
{
  public GetCivilEntityIdByLatitudeAndLongitudeQueryValidator()
  {
    RuleFor(x => x.Latitude).NotEmpty().NotNull();
    RuleFor(x => x.Longitude).NotEmpty().NotNull();
  } 
}
