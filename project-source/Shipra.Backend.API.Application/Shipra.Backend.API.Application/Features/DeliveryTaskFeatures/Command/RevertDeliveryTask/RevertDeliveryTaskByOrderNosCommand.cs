using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.RevertDeliveryTask;
public class RevertDeliveryTaskByOrderNosCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}
