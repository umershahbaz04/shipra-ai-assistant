using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.ClientOrderLabelFeature.Command.DeleteClientOrderLabel;
public class DeleteClientOrderLabelCommand : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
  public string? Label { get; set; }
}

public class DeleteClientOrderLabelCommandHandler : RequestHandlerBase<DeleteClientOrderLabelCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public DeleteClientOrderLabelCommandHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<DeleteClientOrderLabelCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(DeleteClientOrderLabelCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var orderId = new OrderId(new Guid(request.OrderId!));

      var order = await _orderRepository.GetOrderById(orderId, _currentUser.ClientId!);
      if (order is null)
      {
        throw new EntityNotFoundException("Order ", request.OrderId!);
      }
      if (order is not null)//(!order!.TrackingLock.GetValueOrDefault(false))
      {
        // Get all existing labels from DB
        var allClientOrderLabels = await _orderRepository.GetAllClientOrderLabelByOrderId(_currentUser.ClientId!, orderId);
        var labelExist = allClientOrderLabels.FirstOrDefault(x => string.Equals(x.LabelName, request.Label, StringComparison.OrdinalIgnoreCase));

        if (labelExist != null)
        {
          await _orderRepository.DeleteClientOrderLabel(labelExist);
          allClientOrderLabels.Remove(labelExist);
        }

        // Get remaining labels as comma-separated string
        var labelsToAddCsv = string.Join(",", allClientOrderLabels.Select(x => x.LabelName));

        #region update order 
        order.UpdateOrderLabel(labelsToAddCsv, _currentUser.EmployeeId!);
        await _orderRepository.UpdateOrder(order);
        #endregion

        serviceResult = new ServiceResultDTO(new BaseResponseDto { Message = "Labels added successfully" });
      }
      else
      {
        serviceResult.CreateError("OrderLocked", new string[] { "Order already locked" });
      }
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
