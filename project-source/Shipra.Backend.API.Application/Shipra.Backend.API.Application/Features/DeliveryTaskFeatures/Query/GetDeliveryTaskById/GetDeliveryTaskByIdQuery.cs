using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetDeliveryTaskById;
public class GetDeliveryTaskByIdQuery : IRequest<ServiceResultDTO>
{
  public string? DeliveryTaskId { get; set; }
}
