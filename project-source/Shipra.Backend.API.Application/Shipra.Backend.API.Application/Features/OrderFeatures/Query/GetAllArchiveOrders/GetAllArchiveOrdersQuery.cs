using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderStatusReport;
using Shipra.Backend.API.Core.ClientAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllArchiveOrders;
public class GetAllArchiveOrdersQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
}
public class GetAllArchiveOrdersQueryHandler : RequestHandlerBase<GetAllArchiveOrdersQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllArchiveOrdersQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllArchiveOrdersQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }
  protected override async Task<ServiceResultDTO> HandleRequest(GetAllArchiveOrdersQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;
      var order = await _orderRepository.GetAllArchveOrders(_currentUser.ClientIdStr!, filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!);
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
