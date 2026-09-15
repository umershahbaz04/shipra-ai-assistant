using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.MergeDeliveryNotes;

public class MergeDeliveryNotesCommand : IRequest<ServiceResultDTO>
{
  public List<string> SourceDeliveryNoteIds { get; set; } = new();
  public string TargetDeliveryNoteId { get; set; } = string.Empty;
  public DateTime? CreatedDate { get; set; }
}
