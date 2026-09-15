 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CarrierAggregate;
using Shipra.Backend.API.Core.EmployeeAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.deletepickLocation;

public class DeletePickupLocationCommand : IRequest<ServiceResultDTO>
{
  public int ActiveCarrierPickupLocationId { get; set; }
}

public class DeletePickupLocationCommandHandler : RequestHandlerBase<DeletePickupLocationCommand, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public DeletePickupLocationCommandHandler(
      ICarrierRepository carrierRepository,
      IServiceProvider serviceProvider,
      ILogger<DeletePickupLocationCommandHandler> logger
  ) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeletePickupLocationCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    { 
      // Get the pickup location by ID
      var pickupLocation = await _carrierRepository.GetActiveCarrierLocationbyId(request.ActiveCarrierPickupLocationId ,_currentUser.ClientId!);

      if (pickupLocation == null)
      {
        throw new EntityNotFoundException("Pickup Location", request.ActiveCarrierPickupLocationId);
      }

      var isDeleted = await _carrierRepository.deletepickLocation(pickupLocation); // Pass the actual object

      if (isDeleted)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto
        {
          Message = NotificationConstants.Success
        });
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.Error));
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
