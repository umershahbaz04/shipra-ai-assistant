using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Query.GetOrderNoteById;
public class GetOrderNoteByOrderNoQueryHandler : RequestHandlerBase<GetOrderNoteByOrderNoQuery, ServiceResultDTO>
{
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  public GetOrderNoteByOrderNoQueryHandler(IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<GetOrderNoteByOrderNoQueryHandler> logger) : base(serviceProvider, logger)
  {
    _historyRepository = historyRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderNoteByOrderNoQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {

      var response = await _historyRepository.GetOrderNoteByOrderNo(request!.OrderNo!.ToString(), _currentUser.ClientId!.Value.ToString());
      if (response is null)
      {
        throw new EntityNotFoundException("OrderNote ", new OrderId(new Guid(request!.OrderNo!)));
      }
      serviceResult = new ServiceResultDTO(response);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
