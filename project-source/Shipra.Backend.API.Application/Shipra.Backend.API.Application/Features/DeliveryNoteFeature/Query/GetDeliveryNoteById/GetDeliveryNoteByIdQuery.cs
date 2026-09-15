using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteById;
public class GetDeliveryNoteByIdQuery : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteId { get; set; }
}
