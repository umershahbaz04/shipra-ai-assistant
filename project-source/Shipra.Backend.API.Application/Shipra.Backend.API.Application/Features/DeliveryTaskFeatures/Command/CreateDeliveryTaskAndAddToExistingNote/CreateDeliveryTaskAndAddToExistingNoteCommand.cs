using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.BatchOutScanDeliveryTask;
public class CreateDeliveryTaskAndAddToExistingNoteCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
  public DateTime? AssigningDate { get; set; }
  public string? DriverId { get; set; }
  public bool? AddToExisting { get; set; }
  public string? NoteNo { get; set; }
  public bool? IsTransfer { get; set; }
}
