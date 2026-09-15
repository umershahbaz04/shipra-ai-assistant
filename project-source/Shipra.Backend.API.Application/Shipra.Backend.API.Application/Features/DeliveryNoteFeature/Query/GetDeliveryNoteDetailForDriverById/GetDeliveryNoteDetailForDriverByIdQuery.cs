using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteDetailForDriverById;
public class GetDeliveryNoteDetailForDriverByIdQuery : IRequest<ServiceResultDTO>
{
    public string? DeliveryNoteId { get; set; }
}
