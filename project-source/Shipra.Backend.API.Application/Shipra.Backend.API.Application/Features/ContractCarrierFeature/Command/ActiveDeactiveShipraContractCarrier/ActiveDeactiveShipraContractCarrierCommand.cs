using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ContractCarrierFeature.Command.ActiveDeactiveShipraContractCarrier;
public class ActiveDeactiveShipraContractCarrierCommand : IRequest<ServiceResultDTO>
{
  public int ShipraContractCarrierId { get; set; }
  public bool? Active { get; set; }
}
public class ActiveDeactiveShipraContractCarrierCommandHandler : RequestHandlerBase<ActiveDeactiveShipraContractCarrierCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public ActiveDeactiveShipraContractCarrierCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<ActiveDeactiveShipraContractCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(ActiveDeactiveShipraContractCarrierCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var shipraContract = await _carrierRepository.GetShipraCarrierContractByCarrierId(request.ShipraContractCarrierId);
      if (shipraContract is null)
      {
        throw new EntityNotFoundException("ShipraContractCarrier ", request!.ShipraContractCarrierId);
      }
      string msg = "Carrier Activated successfully";
      if (request.Active.GetValueOrDefault())
      {
        shipraContract.ActivateCarrier();
      }
      else
      {
        msg = "Carrier De Activated successfully";
        shipraContract.DeActivateCarrier();
      }
      await _carrierRepository.UpdateShipraCarrierContractByCarrierId(shipraContract);

      serviceResult = new ServiceResultDTO(new BaseResponseDto { Data = request.ShipraContractCarrierId, Message = msg });

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

public class ActiveDeactiveShipraContractCarrierCommandValidator : AbstractValidator<ActiveDeactiveShipraContractCarrierCommand>
{
  public ActiveDeactiveShipraContractCarrierCommandValidator()
  {
    RuleFor(x => x.ShipraContractCarrierId).NotEmpty().NotNull().GreaterThan(0);

  }
}
