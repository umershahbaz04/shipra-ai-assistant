using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetMyCarrierShipmentsByReturnReportNo;
public class GetMyCarrierShipmentsByReturnReportIdQuery : IRequest<ServiceResultDTO>
{
  public string? CarrierRRId { get; set; }
}
