using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.CreateClientCarrierTrackingStatus;
public class CreateClientCarrierTrackingStatusCommand : IRequest<ServiceResultDTO>
{
  public string? TrackingStatus { get; set; }
}
public class CreateClientCarrierTrackingStatusCommandHandler : RequestHandlerBase<CreateClientCarrierTrackingStatusCommand, ServiceResultDTO>
{
  private readonly IClientRepository _clientRepository;

  public CreateClientCarrierTrackingStatusCommandHandler(IClientRepository clientRepository,IServiceProvider serviceProvider, ILogger<CreateClientCarrierTrackingStatusCommandHandler> logger) : base(serviceProvider, logger)
  {
    _clientRepository = clientRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateClientCarrierTrackingStatusCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var allClientCarrierStatus = await _clientRepository.GetAllClientCarrierTrackingStatus(_currentUser.ClientId!);
      
      if (!string.IsNullOrEmpty(request.TrackingStatus) && allClientCarrierStatus!.Any(x => x.TrackingStatus!.Trim().Equals(request.TrackingStatus.Trim(), StringComparison.OrdinalIgnoreCase)))
      {
        serviceResult.CreateError("DuplicateStatus", new string[] { "Carrier Tracking Status already exists." });
        return serviceResult;
      }

      int carrierTrackingStatusId = ClientCarrierTrackingStatus.GetNextCarrierTrackingStatus(allClientCarrierStatus);
      ClientCarrierTrackingStatus oClientCarrierTrackingStatus = ClientCarrierTrackingStatus.Create(carrierTrackingStatusId,request.TrackingStatus!, "", _currentUser.ClientId!);
      oClientCarrierTrackingStatus = await _clientRepository.CreateClientCarrierTrackingStatus(oClientCarrierTrackingStatus);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  } 
}
