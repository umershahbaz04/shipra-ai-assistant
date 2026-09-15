using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.UpdateActiveCarrier;
public class UpdateActiveCarrierCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public int ActiveCarrierId { get; set; }
  public int CarrierId { get; set; } 
  public string? CarrierAlias { get; set; }
  public bool? Active { get; set; }
  public CarrierConfigModel? Config { get; set; }
}
public class UpdateActiveCarrierCommandHandler : RequestHandlerBase<UpdateActiveCarrierCommand, ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  private readonly ICarrierRepository _carrierRepository;

  public UpdateActiveCarrierCommandHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<UpdateActiveCarrierCommandHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTOWithTypeModel<BaseResponseDto>> HandleRequest(UpdateActiveCarrierCommand request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTOWithTypeModel<BaseResponseDto>();

    try
    {
      var activeCarrier = await _carrierRepository.GetActiveCarrierByActiveCarrierId(request.ActiveCarrierId, _currentUser.ClientId!);
      if (activeCarrier == null)
      {
        throw new EntityNotFoundException("ActiveCarrier", request.ActiveCarrierId);
      }

      var config = JsonConvert.SerializeObject(request.Config!); 
      activeCarrier.UpdateActiveCarrier(request.CarrierId, _currentUser.ClientId!, config, request.Active,request.CarrierAlias,activeCarrier.UserName, _currentUser.EmployeeId!);

      var oActiveCarrier = await _carrierRepository.UpdateActiveCarrier(activeCarrier);
      if (oActiveCarrier is not null)
      {
        serviceResult.IsSuccess = true;
      }
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
