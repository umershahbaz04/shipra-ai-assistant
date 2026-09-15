using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Command.DeleteOrderNote;
public class DeleteOrderNoteCommandHandler : RequestHandlerBase<DeleteOrderNoteCommand, ServiceResultDTO>
{
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  public DeleteOrderNoteCommandHandler(IOrderTrackingHistoryRepository historyRepository, IServiceProvider serviceProvider, ILogger<DeleteOrderNoteCommandHandler> logger) : base(serviceProvider, logger)
  {
    _historyRepository = historyRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(DeleteOrderNoteCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      Guid guid;
      var hasGuid = Guid.TryParse(request?.OrderNoteId, out guid);
      if (!hasGuid)
      {
        throw new InvalidIdTypeException(request?.OrderNoteId!);
      }
      var orderNote = await _historyRepository.GetOrderNoteById(new OrderNoteId(guid));
      if (orderNote is null)
      {
        throw new EntityNotFoundException("OrderNote",request!.OrderNoteId!);
      }
      serviceResult.IsSuccess = await _historyRepository.DeleteOrderById(orderNote);
      if (serviceResult.IsSuccess)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto()
        {
          Data = serviceResult.IsSuccess,
          Message = NotificationConstants.Success,
        });
        return serviceResult;
      }
      else
      {
        serviceResult.CreateErrorResponse(new Exception(NotificationConstants.InvalidResponse));
        return serviceResult;
      }
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}

