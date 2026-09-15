using System.Dynamic;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Request;
using Shipra.Backend.API.Core.Enum;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrderPaymentLinks;
public class GetAllOrderPaymentLinksQuery : IRequest<ServiceResultDTO>
{
  public FilterModelDTO? FilterModel { get; set; }
  public int? PaymentLinkStatusId { get; set; }
  public bool? IsForPayoutRequest { get; set; }
}
public class GetAllOrderPaymentLinksQueryHandler : RequestHandlerBase<GetAllOrderPaymentLinksQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetAllOrderPaymentLinksQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetAllOrderPaymentLinksQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetAllOrderPaymentLinksQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();

    try
    {
      var filter = request.FilterModel!;

      if (request.IsForPayoutRequest.GetValueOrDefault())
      {
        request.PaymentLinkStatusId = (int)EnumPaymentStatus.Paid;
      }
      var trackingPageUrl = _configuration.GetValue<string>("TrackingPageUrl");
      DateTime? scehdualFrom = null;
      DateTime? scehdualTo = null;
      if (request.IsForPayoutRequest.GetValueOrDefault())
      {
        scehdualFrom = filter.CreatedFrom;
        scehdualTo = filter.CreatedTo;
      }
      else
      {
        filter.CreatedFrom = null;
        filter.CreatedTo = null;
      }
      var dataList = await _orderRepository.GetAllOrderPaymentLinks(filter.CreatedFrom, filter.CreatedTo, filter.Start, filter.Length, filter.Search!, filter.SortCol, filter.SortDir!, _currentUser.ClientIdStr!, request.PaymentLinkStatusId,request.IsForPayoutRequest,trackingPageUrl!, scehdualFrom, scehdualTo);

      dynamic result = new ExpandoObject();
      int? totalCount = 0; 
      if (dataList.ToList().Count > 0)
      {
        var firstRecord = dataList.FirstOrDefault();
        totalCount = firstRecord?.TotalCount;
      }
      result.TotalCount = totalCount;
      result.list = dataList;

      serviceResult = new ServiceResultDTO(result);
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
