using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.DeleteMyCarrierRetunReport;
public class DeleteMyCarrierReturnReportCommand : IRequest<ServiceResultDTO>
{
  public string? CarrierRRId { get; set; } 
}
