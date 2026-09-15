using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ContractCarrierFeature.Query.GetShipraContractByShipraContractCarrierId;
public class GetShipraContractByShipraContractCarrierIdQuery : IRequest<ServiceResultDTO>
{
  public int ShipraContractCarrierId { get; set; }
}
public class GetShipraContractByShipraContractCarrierIdQueryHandler : RequestHandlerBase<GetShipraContractByShipraContractCarrierIdQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetShipraContractByShipraContractCarrierIdQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetShipraContractByShipraContractCarrierIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetShipraContractByShipraContractCarrierIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    { 
      var shipraContract = await _carrierRepository.GetShipraContractByShipraContractCarrierId(request.ShipraContractCarrierId);
      if (shipraContract is null)
      {
        throw new EntityNotFoundException("ShipraContractCarrier ", request!.ShipraContractCarrierId); 
      } 

      serviceResult = new ServiceResultDTO(shipraContract);

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetShipraContractByShipraContractCarrierIdValidator : AbstractValidator<GetShipraContractByShipraContractCarrierIdQuery>
{
  public GetShipraContractByShipraContractCarrierIdValidator()
  {
    RuleFor(x => x.ShipraContractCarrierId).NotEmpty().NotNull().GreaterThan(0);
  } 
}
