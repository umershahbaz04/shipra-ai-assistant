using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.DeleteDriver;
public class DeleteDriverCommand : IRequest<ServiceResultDTO>
{
  public string? DriverId { get; set; }
  public bool IsActive { get; set; }
}
