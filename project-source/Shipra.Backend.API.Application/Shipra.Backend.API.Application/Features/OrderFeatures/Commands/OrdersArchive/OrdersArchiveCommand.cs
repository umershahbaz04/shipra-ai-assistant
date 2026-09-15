using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetCountryCityRegionIdByName;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.OrdersArchive;
public class OrdersArchiveCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
}
public class OrdersArchiveCommandHandler : RequestHandlerBase<OrdersArchiveCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public OrdersArchiveCommandHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<OrdersArchiveCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(OrdersArchiveCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var orders = await _orderRepository.GetOrdersByOrderNos(
          request.OrderNo!,
          _currentUser.ClientId!
      );

      if (orders is null || !orders.Any())
        throw new EntityNotFoundException("Order", request.OrderNo!);

      List<object> archivedOrdersResult = new();
      var archiveNumber = OrderArchive.GenerateRandomArchiveNo();
      foreach (var order in orders)
      {
        var orderItems = await _orderRepository.GetOrderItemsByOrderId(order.OrderId);

        var combined = new
        {
          Order = order,
          OrderItems = orderItems
        };

        string json = System.Text.Json.JsonSerializer.Serialize(combined);

        var archive = OrderArchive.Create(order.OrderId!, json, _currentUser.EmployeeId!, _currentUser.ClientId!, archiveNumber);

        var archiveSaved = await _orderRepository.ArchiveOrderAsync(archive);

        if (archiveSaved)
        {
          var deleted = await _orderRepository.DeleteOrder(order);
          foreach (var item in orderItems)
          {
            var itemDeleted = await _orderRepository.DeleteOrderItem(item);
          }
        }

        archivedOrdersResult.Add(new
        {
          OrderId = order.OrderId,
          Archived = true,
          Deleted = true
        });
      }

      return new ServiceResultDTO(archivedOrdersResult);
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      return serviceResult;
    }
  }


}
