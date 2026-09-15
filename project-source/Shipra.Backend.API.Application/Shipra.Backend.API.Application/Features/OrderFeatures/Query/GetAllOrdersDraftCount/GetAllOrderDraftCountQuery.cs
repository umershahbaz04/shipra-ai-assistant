using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderDraft;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersDraftCount;
public class GetAllOrderDraftCountQuery : IRequest<ServiceResultDTO>
{
}
public class GetAllOrderDraftCountQueryHandler : RequestHandlerBase<GetAllOrderDraftCountQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllOrderDraftCountQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllOrderDraftCountQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrderDraftCountQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var orderDraft = await _orderRepository.GetAllDraftOrdersCount(_currentUser.ClientId);
      serviceResult = new ServiceResultDTO(orderDraft);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
