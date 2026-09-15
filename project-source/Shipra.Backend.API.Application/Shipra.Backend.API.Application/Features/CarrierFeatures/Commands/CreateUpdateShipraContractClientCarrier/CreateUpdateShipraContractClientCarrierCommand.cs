using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.CreateUpdateShipraContractClientCarrier;
public class CreateUpdateShipraContractClientCarrierCommand : IRequest<ServiceResultDTO>
{
  public List<CreateUpdateShipraContractClientCarrierRequestModel>? list { get; set; }
}
public class CreateUpdateShipraContractClientCarrierCommandHandler : RequestHandlerBase<CreateUpdateShipraContractClientCarrierCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public CreateUpdateShipraContractClientCarrierCommandHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<CreateUpdateShipraContractClientCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateUpdateShipraContractClientCarrierCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    { 
      // Fetch the existing list from the repository
      var existingCarriers = await _carrierRepository.GetAllShipraContractClientCarriersByClientId(_currentUser.ClientId!);

      // To create: Items in request.list that are not in existingCarriers
      var carriersToCreate = request.list!
          .Where(r => !existingCarriers!.Any(e => e.ShipraContractClientCarrierId == r.ShipraContractClientCarrierId))
          .ToList();

      // To delete: Items in existingCarriers that are not in request.list
      var carriersToDelete = existingCarriers!
          .Where(e => !request.list!.Any(r => r.ShipraContractClientCarrierId == e.ShipraContractClientCarrierId))
          .ToList();

      // Process deletions
      foreach (var carrier in carriersToDelete)
      {
        await _carrierRepository.DeleteShipraContractClientCarrierAsync(carrier);
      }

      // Process creations
      foreach (var carrier in carriersToCreate)
      {
        ShipraContractClientCarrier c = ShipraContractClientCarrier.Create(carrier.ShipraContractCarrierId,_currentUser.ClientId);
        await _carrierRepository.AddShipraContractClientCarrierAsync(c);
      }

      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
