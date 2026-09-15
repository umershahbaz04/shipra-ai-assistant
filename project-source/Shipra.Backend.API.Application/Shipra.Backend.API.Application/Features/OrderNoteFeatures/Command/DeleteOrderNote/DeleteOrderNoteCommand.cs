using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Command.DeleteOrderNote;
public class DeleteOrderNoteCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNoteId { get; set; }
}
