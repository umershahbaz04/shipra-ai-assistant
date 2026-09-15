using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderForDriverById;
public class GetOrderForDriverByIdQuery : IRequest<ServiceResultDTO>
{
  public string? OrderId { get; set; }
}
