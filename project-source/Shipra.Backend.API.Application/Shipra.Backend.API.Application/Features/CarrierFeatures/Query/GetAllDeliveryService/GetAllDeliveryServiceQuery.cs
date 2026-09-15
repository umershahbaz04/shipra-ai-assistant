using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllDeliveryService;
public class GetAllDeliveryServiceQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllDeliveryServiceQueryHandler : RequestHandlerBase<GetAllDeliveryServiceQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllDeliveryServiceQueryHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<GetAllDeliveryServiceQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllDeliveryServiceQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _carrierRepository.GetAllDeliveryService(); 
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
