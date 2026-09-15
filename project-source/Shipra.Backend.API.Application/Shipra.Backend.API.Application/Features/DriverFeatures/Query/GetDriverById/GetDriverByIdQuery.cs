using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Query.GetDriverById;
public class GetDriverByIdQuery : IRequest<ServiceResultDTO>
{
  public string? DriverId { get; set; }
}
