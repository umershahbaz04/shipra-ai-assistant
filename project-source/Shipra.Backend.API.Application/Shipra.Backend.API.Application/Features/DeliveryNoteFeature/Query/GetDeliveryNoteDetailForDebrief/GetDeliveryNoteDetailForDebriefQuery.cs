using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteDetailForDebrief;
public class GetDeliveryNoteDetailForDebriefQuery : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteId { get; set; }
}
