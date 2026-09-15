using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.OrderAggregate.Dto;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.AdvanceSearchOrders;

public class AdvanceSearchOrdersQuery : IRequest<ServiceResultDTO>
{
  public string? SearchType { get; set; }
  public string? SearchQuery { get; set; }
  public int Start { get; set; } = 0;
  public int Length { get; set; } = 10;
}

public class AdvanceSearchOrdersQueryHandler : RequestHandlerBase<AdvanceSearchOrdersQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public AdvanceSearchOrdersQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<AdvanceSearchOrdersQueryHandler> logger)
    : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(AdvanceSearchOrdersQuery request, CancellationToken cancellationToken)
  {
    var serviceResult = new ServiceResultDTO();
    try
    {
      var tenantId = _currentUser.ClientId?.Value.ToString();
      if (string.IsNullOrEmpty(tenantId))
      {
        dynamic emptyResult = new ExpandoObject();
        emptyResult.TotalCount = 0;
        emptyResult.list = new List<AdvanceSearchOrderDto>();
        serviceResult = new ServiceResultDTO(emptyResult);
        serviceResult.CreateSuccessResponse();
        return serviceResult;
      }

      var list = await _orderRepository.AdvanceSearchOrders(request.SearchType, request.SearchQuery, request.Start, request.Length, tenantId);
      int totalCount = list.Count > 0 ? (int)list[0].TotalCount : 0;

      dynamic result = new ExpandoObject();
      result.TotalCount = totalCount;
      result.list = list;

      serviceResult = new ServiceResultDTO(result);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      return serviceResult;
    }
  }
}
