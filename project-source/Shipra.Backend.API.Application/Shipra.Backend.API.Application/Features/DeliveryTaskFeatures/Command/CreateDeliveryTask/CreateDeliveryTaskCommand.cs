using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.CreateDeliveryTask;
public class CreateDeliveryTaskCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}
