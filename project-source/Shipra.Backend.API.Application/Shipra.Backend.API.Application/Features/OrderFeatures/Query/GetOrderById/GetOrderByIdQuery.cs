using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderById;
public class GetOrderByIdQuery : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
}
