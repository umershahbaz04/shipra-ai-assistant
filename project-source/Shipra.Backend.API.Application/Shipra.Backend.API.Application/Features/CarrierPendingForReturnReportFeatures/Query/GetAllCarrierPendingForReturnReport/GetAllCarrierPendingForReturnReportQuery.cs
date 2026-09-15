using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CarrierPendingForReturnReportFeatures.Query.GetAllCarrierPendingForReturnReport;
public class GetAllCarrierPendingForReturnReportQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllCarrierPendingForReturnReportQueryHandler : RequestHandlerBase<GetAllCarrierPendingForReturnReportQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllCarrierPendingForReturnReportQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllCarrierPendingForReturnReportQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllCarrierPendingForReturnReportQuery request, CancellationToken cancellationToken)
  {
    var serviceResultDTO = new ServiceResultDTO();
    try
    {
      var filter = request?.FilterModel!;
      var data = await _orderRepository.GetAllCarrierPendingForReturnShipments(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search, filter.SortCol, filter.SortDir, _currentUser.ClientIdStr!);

      serviceResultDTO = new ServiceResultDTO(data);

      return serviceResultDTO;
    }
    catch (Exception ex)
    {
      serviceResultDTO.CreateErrorResponse(ex);
      throw;
    }
  }
}

