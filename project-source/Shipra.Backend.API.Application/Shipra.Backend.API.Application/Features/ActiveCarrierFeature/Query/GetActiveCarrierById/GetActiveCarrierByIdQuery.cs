using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetActiveCarrierById;
public class GetActiveCarrierByIdQuery : IRequest<ServiceResultDTO>
{
  public int ActiveCarrierId { get; set; }
}
public class GetActiveCarrierByIdQueryHandler : RequestHandlerBase<GetActiveCarrierByIdQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetActiveCarrierByIdQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetActiveCarrierByIdQuery> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetActiveCarrierByIdQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var oActiveCarrier = await _carrierRepository.GetActiveCarrierById(request.ActiveCarrierId, _currentUser.ClientId!);
      if (oActiveCarrier is not null)
      {
        var result = new BaseResponseDto()
        {
          Data = oActiveCarrier,
          Message = NotificationConstants.Success
        };
        serviceResult = new ServiceResultDTO(result);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        return serviceResult;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.Error));
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
