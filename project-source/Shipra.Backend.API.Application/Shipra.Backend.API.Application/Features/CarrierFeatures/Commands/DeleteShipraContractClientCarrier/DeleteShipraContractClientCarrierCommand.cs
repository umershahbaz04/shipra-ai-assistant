using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.DeleteShipraContractClientCarrier;
public class DeleteShipraContractClientCarrierCommand : IRequest<ServiceResultDTO>
{
  public int ShipraContractClientCarrierId { get; set; }
}
public class DeleteShipraContractClientCarrierCommandHandler : RequestHandlerBase<DeleteShipraContractClientCarrierCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public DeleteShipraContractClientCarrierCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<DeleteShipraContractClientCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteShipraContractClientCarrierCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var carrier = await _carrierRepository.GetShipraContractClientCarrierById(request.ShipraContractClientCarrierId);
      if (carrier == null)
      {
        throw new EntityNotFoundException("ContractCarrier", request.ShipraContractClientCarrierId);
      } 
      await _carrierRepository.DeleteShipraContractClientCarrierAsync(carrier);
      serviceResult = new ServiceResultDTO(carrier);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;

    }
  }
}
