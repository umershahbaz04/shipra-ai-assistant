using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.RevertDeliveryTask;
public class SendSmsCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
}
