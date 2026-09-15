using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.UpdateDriver;
public class UpdateDriverCommand : IRequest<ServiceResultDTO>
{
  public string? DriverId { get; set; }
  public string? WorkEmail { get; set; }
  public string? PhoneNo { get; set; }
  public string? AppUsername { get; set; }
  public string? AppPassword { get; set; }
  public string? EmployeeId { get; set; }
}
