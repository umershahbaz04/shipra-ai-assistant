using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.UncompleteDeliveryNote;
public class UncompleteDeliveryNoteCommand : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteId { get; set; }
}
