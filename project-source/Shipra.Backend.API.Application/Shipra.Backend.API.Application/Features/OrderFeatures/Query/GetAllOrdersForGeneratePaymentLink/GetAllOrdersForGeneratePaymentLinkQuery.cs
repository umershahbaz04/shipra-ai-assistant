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
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersForGeneratePaymentLink;
public class GetAllOrdersForGeneratePaymentLinkQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; } 
}
public class GetAllOrdersForGeneratePaymentLinkQueryHandler : RequestHandlerBase<GetAllOrdersForGeneratePaymentLinkQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllOrdersForGeneratePaymentLinkQueryHandler(IOrderRepository orderRepository,IServiceProvider serviceProvider, ILogger<GetAllOrdersForGeneratePaymentLinkQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrdersForGeneratePaymentLinkQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!; 
      dynamic data = await _orderRepository.GetAllOrdersForGeneratePaymentLink(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!,_currentUser.ClientIdStr!);

      serviceResult = new ServiceResultDTO(data);
      serviceResult.CreateSuccessResponse();
      return serviceResult;

    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }

  }
}
