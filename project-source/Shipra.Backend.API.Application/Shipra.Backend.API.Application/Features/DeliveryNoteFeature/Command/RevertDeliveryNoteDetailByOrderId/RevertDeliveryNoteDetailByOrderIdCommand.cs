using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.RevertDeliveryNote;
public class RevertDeliveryNoteDetailByOrderIdCommand : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
}
