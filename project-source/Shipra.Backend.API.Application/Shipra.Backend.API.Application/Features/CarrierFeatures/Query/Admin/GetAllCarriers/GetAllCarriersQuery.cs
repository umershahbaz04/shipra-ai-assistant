using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Query.Admin.GetAllCarriers;
public class GetAllCarriersQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllCarriersQueryHandler : RequestHandlerBase<GetAllCarriersQuery, ServiceResultDTO>
{
  private readonly ICarrierRepository _carrierRepository;

  public GetAllCarriersQueryHandler(ICarrierRepository carrierRepository, IServiceProvider serviceProvider, ILogger<GetAllCarriersQueryHandler> logger) : base(serviceProvider, logger)
  {
    _carrierRepository = carrierRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCarriersQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var filter = request.FilterModel!;
      dynamic data = await _carrierRepository.GetAllCarriers(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!,_currentUser.ClientIdStr!);

      //var mapperDto = _mapper.Map<List<CarrierResponseDTO>>(data);
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
