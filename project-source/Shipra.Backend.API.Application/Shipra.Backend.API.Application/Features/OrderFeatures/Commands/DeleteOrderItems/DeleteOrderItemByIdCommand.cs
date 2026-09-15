using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.DeleteOrderItems;
public class DeleteOrderItemByIdCommand : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
  public string? OrderItemId { get; set; }
}
