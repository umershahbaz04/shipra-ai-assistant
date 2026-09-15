using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDriverLatestDeliveryNoteToday;
public class GetDriverLatestDeliveryNoteTodayQuery : IRequest<ServiceResultDTO>
{
  public string DriverId { get; set; } = string.Empty;
}
