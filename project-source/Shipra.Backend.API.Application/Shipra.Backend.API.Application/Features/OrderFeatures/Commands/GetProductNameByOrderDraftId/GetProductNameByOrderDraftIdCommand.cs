using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrder;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.ProductAggregate;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.GetProductNameByOrderDraftId;
public class GetProductNameByOrderDraftIdCommand : IRequest<ServiceResultDTO>
{
  public long OrderDraftId { get; set; }
}

public class GetProductNameByOrderDraftIdCommandHandler : RequestHandlerBase<GetProductNameByOrderDraftIdCommand, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;
  private readonly IProductRepository _productRepository;

  public GetProductNameByOrderDraftIdCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IServiceProvider serviceProvider, ILogger<GetProductNameByOrderDraftIdCommandHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
    _productRepository = productRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetProductNameByOrderDraftIdCommand request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    var orderDraft = await _orderRepository.GetDraftOrderById(request.OrderDraftId, _currentUser.ClientId!);
    if (orderDraft is null)
    {
      serviceResult.CreateErrorResponse();
      return serviceResult;
    }
    var orderInfo = JsonSerializer.Deserialize<CreateUpdateOrderCommonRequestModel>(orderDraft.OrderInfo!);

    var orderItems = orderInfo?.OrderItems;
    if (orderItems is null || orderItems.Count == 0)
    {
      serviceResult.CreateErrorResponse();
      return serviceResult;
    }

    foreach (var item in orderItems)
    {
      if (string.IsNullOrEmpty(item.ProductId))
        continue;
      var productId = new ProductId(Guid.Parse(item.ProductId));

      var productName = await _productRepository
          .GetProductNameByProductIdAsync(productId);

      if (!string.IsNullOrEmpty(productName))
      {
        item.ProductName = productName;
      }
    }
    orderDraft.OrderInfo = JsonSerializer.Serialize(orderInfo);
    return new ServiceResultDTO(orderDraft);

  }

}
