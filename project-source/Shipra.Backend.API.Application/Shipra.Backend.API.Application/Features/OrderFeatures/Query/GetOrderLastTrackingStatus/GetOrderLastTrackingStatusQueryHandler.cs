using System.Dynamic;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Exceptions;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderLastTrackingStatus;


public class GetOrderLastTrackingStatusQueryHandler : RequestHandlerBase<GetOrderLastTrackingStatusQuery, ServiceResultDTO>
{
  private readonly IOrderRepository _orderRepository;

  public GetOrderLastTrackingStatusQueryHandler(IOrderRepository orderRepository, IServiceProvider serviceProvider, ILogger<GetOrderLastTrackingStatusQueryHandler> logger) : base(serviceProvider, logger)
  {
    _orderRepository = orderRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetOrderLastTrackingStatusQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      var orderNos = request.OrderNos!.Split(",");
      List<dynamic> response = new List<dynamic>();
      foreach (var orderNo in orderNos)
      {
        var oOrder = await _orderRepository.GetOrderInfoByOrderNo(orderNo!, _currentUser!.ClientIdStr!);
        if (oOrder is not null)
        {
          response.Add(new { TrackingStatus = oOrder[0].TrackingStatus , Tracking_No = orderNo });
        }
      } 
      if(response.Count > 0)
      {
        serviceResult = new ServiceResultDTO(response);
        serviceResult.CreateSuccessResponse(System.Net.HttpStatusCode.OK); 
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
