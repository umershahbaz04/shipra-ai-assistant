using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.DTOs.ProductStationUseCase.Response;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Query.GetAllProductStations;
public class GetAllProductStationsQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; } 
}

public class GetAllProductStationsQueryHandler : RequestHandlerBase<GetAllProductStationsQuery, ServiceResultDTO>
{
  private readonly IProductStationRepository _productStationRepository;

  public GetAllProductStationsQueryHandler(IProductStationRepository productStationRepository, IServiceProvider serviceProvider, ILogger<GetAllProductStationsQueryHandler> logger) : base(serviceProvider, logger)
  {
    _productStationRepository = productStationRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllProductStationsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;

      var data = await _productStationRepository.GetAllProductStations(filter.Start,filter.Length,filter.Search!,filter.CreatedFrom,filter.CreatedTo,_currentUser.ClientIdStr!); 

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
