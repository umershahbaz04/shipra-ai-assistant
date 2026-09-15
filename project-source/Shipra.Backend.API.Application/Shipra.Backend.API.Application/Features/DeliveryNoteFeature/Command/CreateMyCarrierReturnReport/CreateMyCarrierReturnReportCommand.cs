using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CreateMyCarrierReturnReport;
public class CreateMyCarrierReturnReportCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}
