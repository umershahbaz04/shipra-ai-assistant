using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteDeliveryNote;

public class DeleteDeliveryNoteByIdCommand : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteId { get; set; }
}
