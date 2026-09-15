using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Query.GetOrderTrackingHistoryByOrderId;
public class GetOrderTrackingHistoryByOrderNoQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
  public bool? IsCarrierRefreshTracking { get; set; }
}
