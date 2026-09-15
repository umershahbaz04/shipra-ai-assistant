using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.CarrierUseCase;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Query.GetAllActiveCarrier;
public class GetAllActiveCarrierQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int CountryId { get; set; }
  public int DeliveryServiceId { get; set; }
}
public class GetAllActiveCarrierQueryHandler : RequestHandlerBase<GetAllActiveCarrierQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllActiveCarrierQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetAllActiveCarrierQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllActiveCarrierQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var data = await _carrierRepository.GetAllActiveCarriers(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,request.CountryId, request.DeliveryServiceId, _currentUser.ClientId!);

      serviceResult = new ServiceResultDTO(data!);
      serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
