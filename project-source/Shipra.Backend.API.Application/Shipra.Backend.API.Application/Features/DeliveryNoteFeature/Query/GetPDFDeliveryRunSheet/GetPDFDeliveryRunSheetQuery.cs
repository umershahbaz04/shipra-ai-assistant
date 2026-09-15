using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetPDFRunSheet;
public class GetPDFDeliveryRunSheetQuery : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteId { get; set; }
}
