using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Command.CreateOrderTrackingHistory;
public class CreateOrderTrackingHistoryCommand : CommandBase, IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
  public string? TrackingStatusComments { get; set; }

}
