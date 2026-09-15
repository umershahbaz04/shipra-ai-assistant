using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderDraft;
public class GetAllOrderDraftsQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllOrderDraftQueryHandler : RequestHandlerBase<GetAllOrderDraftsQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllOrderDraftQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllOrderDraftQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrderDraftsQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var orderDraft = await _orderRepository.GetAllDraftOrders(_currentUser.ClientId!);

      var data = orderDraft.Select(x => new
      {
        x.OrderDraftId,
        x.OrderNo,
        x.OrderInfo,
        x.OrderTypeId,
        x.CreatedOn
      }).ToList();
      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
