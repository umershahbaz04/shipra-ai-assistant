using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.DeleteActiveCarrier;
public class DeleteActiveCarrierCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public int ActiveCarrierId { get; set; }
}
public class DeleteActiveCarrierCommandHandler : RequestHandlerBase<DeleteActiveCarrierCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly ICarrierRepository _carrierRepository;

  public DeleteActiveCarrierCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<DeleteActiveCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(DeleteActiveCarrierCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();
    try
    {
      var activeCarrier = await _carrierRepository.GetActiveCarrierByActiveCarrierId(request.ActiveCarrierId, _currentUser.ClientId!);
      if (activeCarrier == null)
      {
        throw new EntityNotFoundException("ActiveCarrier", request.ActiveCarrierId);
      }
      activeCarrier.DeActiveCarrier(_currentUser.EmployeeId);
      await _carrierRepository.UpdateActiveCarrier(activeCarrier);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
