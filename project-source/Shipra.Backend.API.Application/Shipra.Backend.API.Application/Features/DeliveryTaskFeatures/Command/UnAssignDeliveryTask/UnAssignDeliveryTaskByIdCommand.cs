using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.DeleteDeliveryTask;
public class UnAssignDeliveryTaskByIdCommand : IRequest<ServiceResultDTO>
{
  public string? DeliveryTaskId { get; set; }
}
