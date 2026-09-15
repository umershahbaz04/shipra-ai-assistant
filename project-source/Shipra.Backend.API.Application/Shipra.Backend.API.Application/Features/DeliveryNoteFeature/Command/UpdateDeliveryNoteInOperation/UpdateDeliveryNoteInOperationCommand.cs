using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.UpdateDeliveryNoteInOperation;
public class UpdateDeliveryNoteInOperationCommand : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteDetailId { get; set; }
}
