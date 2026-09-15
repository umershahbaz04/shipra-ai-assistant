using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderStatusReport;
public class GetAllOrderStatusReportQuery : IRequest<ServiceResultDTO>
{
  public DateTime? Date { get; set; }
  public int CarrierId { get; set; } = 0;
}
public class GetAllOrderStatusReportQueryHandler : RequestHandlerBase<GetAllOrderStatusReportQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllOrderStatusReportQueryHandler(IOrderRepository orderRepository,IServiceProvider serviceProvider, ILogger<GetAllOrderStatusReportQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrderStatusReportQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var order = await _orderRepository.GetAllOrderStatusReport(request.Date, _currentUser.ClientIdStr!,request.CarrierId);  
      serviceResult = new ServiceResultDTO(order);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
