using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllCarrierLocationsByCarrier;
public class GetAllCarrierLocationsByCarrierQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
}
public class GetAllCarrierLocationsByCarrierQueryHandler : RequestHandlerBase<GetAllCarrierLocationsByCarrierQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllCarrierLocationsByCarrierQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetAllCarrierLocationsByCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCarrierLocationsByCarrierQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var oCarrier = await _carrierRepository.GetCarrierById(request.CarrierId);

      var carrierLocation = await _carrierRepository.GetAllCarrierLocationsByCarrier(oCarrier!);
      serviceResult = new ServiceResultDTO(carrierLocation); 
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetAllCarrierLocationsByCarrierQueryValidator : AbstractValidator<GetAllCarrierLocationsByCarrierQuery>
{
  public GetAllCarrierLocationsByCarrierQueryValidator()
  {
    RuleFor(x => x.CarrierId).NotEmpty().NotNull().GreaterThan(0);
  }
}
