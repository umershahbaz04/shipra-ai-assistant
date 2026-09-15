using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetCarrierWithServiceAndLocationByCarrierId;
public class GetCarrierWithServiceAndLocationByCarrierIdQuery : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
}
public class GetCarrierWithServiceAndLocationByCarrierIdQueryHandler : RequestHandlerBase<GetCarrierWithServiceAndLocationByCarrierIdQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetCarrierWithServiceAndLocationByCarrierIdQueryHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<GetCarrierWithServiceAndLocationByCarrierIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetCarrierWithServiceAndLocationByCarrierIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {  
      var data = await _carrierRepository.GetCarrierWithServiceAndLocationByCarrierId(request.CarrierId); 
      serviceResult = new ServiceResultDTO(data!);
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
