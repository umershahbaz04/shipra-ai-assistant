using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ProductStationTransferFeatures.Query.GetTotalNoOfProductStationTransferQuery;
public class GetAllTypeCountProductStationTransferQuery : IRequest<ServiceResultDTO>
{
}
public class GetTotalNoOfProductStationTransferQueryHandler : RequestHandlerBase<GetAllTypeCountProductStationTransferQuery, ServiceResultDTO>
{
  private readonly IProductStationTransferRepository _productStationTransferRepository;
  public GetTotalNoOfProductStationTransferQueryHandler(IProductStationTransferRepository productStationTransferRepository, IServiceProvider serviceProvider, ILogger<GetTotalNoOfProductStationTransferQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productStationTransferRepository = productStationTransferRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllTypeCountProductStationTransferQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var data = await _productStationTransferRepository.GetAllTypeCountProductStationTransfer(_currentUser.ClientIdStr!);
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
