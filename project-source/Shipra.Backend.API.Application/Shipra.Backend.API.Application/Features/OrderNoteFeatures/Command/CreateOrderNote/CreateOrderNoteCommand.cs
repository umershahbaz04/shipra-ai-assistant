using MediatR;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Command.CreateOrderNote;
public class CreateOrderNoteCommand : CommandBase, IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
  public string? NoteDescription { get; set; }
}
