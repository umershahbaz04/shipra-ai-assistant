using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierById;
public class GetCarrierByIdQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; } 
}
public class GetCarrierByIdQueryHandler : RequestHandlerBase<GetCarrierByIdQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetCarrierByIdQueryHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<GetCarrierByIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCarrierByIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var oCarrier = await _carrierRepository.GetCarrierById(request.CarrierId);
      if (oCarrier is not null)
      {
        var result = new BaseResponseDto()
        {
          Data = oCarrier,
          Message = NotificationConstants.Success
        };
        serviceResult = new ServiceResultDTO(result);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
        return serviceResult;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.ErrorEntityNotFound));
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
