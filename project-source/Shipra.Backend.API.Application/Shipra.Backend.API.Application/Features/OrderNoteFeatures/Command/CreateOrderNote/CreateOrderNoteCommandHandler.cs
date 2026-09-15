using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Command.CreateOrderNote;
public class CreateOrderNoteCommandHandler : RequestHandlerBase<CreateOrderNoteCommand, ServiceResultDTO>
{
  private readonly IOrderTrackingHistoryRepository _historyRepository;
  private readonly IOrderRepository _orderRepository;
  public CreateOrderNoteCommandHandler(IOrderTrackingHistoryRepository historyRepository, IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<CreateOrderNoteCommandHandler> logger) : base(serviceProvider, logger)
  {
    _historyRepository = historyRepository;
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(CreateOrderNoteCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      Order? result = await _orderRepository.GetOrderByOrderNo(request.OrderNo!, _currentUser.ClientId!);
      if (result == null)
      {
        throw new EntityNotFoundException("OrderNotFound", request.OrderNo!);
      }
      //if (result.TrackingLock.GetValueOrDefault(false))
      //{
      //  serviceResult.CreateError("OrderLocked", new string[] { "Order is locked, you cannot add notes." });
      //  return serviceResult;
      //}
      var response = await _historyRepository.CreateOrderNote(OrderNote.CreateOrderNote(result!.OrderId!, request?.NoteDescription, _currentUser.EmployeeId!));
      if (response.OrderNoteId != null)
      {
        serviceResult = new ServiceResultDTO(new BaseResponseDto()
        {
          Data = response!.OrderNoteId,
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
