using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Commands.CreateCarrierReturnReport;
public class CreateCarrierReturnReportCommand : IRequest<ServiceResultDTO>
{
  public int CarrierId { get; set; }
  public string? TrackingNos { get; set; }
}
