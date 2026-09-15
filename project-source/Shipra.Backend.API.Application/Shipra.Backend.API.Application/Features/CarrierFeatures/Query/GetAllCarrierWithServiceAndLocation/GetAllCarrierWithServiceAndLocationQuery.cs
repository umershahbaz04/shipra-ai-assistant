using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.GetAllCarrierWithServiceAndLocation;
public class GetAllCarrierWithServiceAndLocationQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int CountryId { get; set; }
  public int DeliveryServiceId { get; set; }
}
public class GetAllCarrierWithServiceAndLocationQueryHandler : RequestHandlerBase<GetAllCarrierWithServiceAndLocationQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllCarrierWithServiceAndLocationQueryHandler(ICarrierRepository carrierRepository,IServiceProvider serviceProvider, ILogger<GetAllCarrierWithServiceAndLocationQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCarrierWithServiceAndLocationQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;

      var data = await _carrierRepository.GetAllCarrierWithServiceAndLocation(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,request.CountryId,request.DeliveryServiceId,_currentUser.ClientIdStr!);

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
