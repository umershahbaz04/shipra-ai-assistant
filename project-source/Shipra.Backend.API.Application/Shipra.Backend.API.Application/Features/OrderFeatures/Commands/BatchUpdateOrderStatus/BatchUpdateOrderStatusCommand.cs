using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.BatchUpdateOrderStatus;
public class BatchUpdateOrderStatusCommand : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
  public int CarrierStatusId { get; set; }
  public string? Comments { get; set; }
  public bool? FromDeliveryTaskScreen { get; set; }
}
