using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.UpdateProductStation;
public class UpdateProductStationCommand : IRequest<ServiceResultDTO>
{
  public int ProductStationId { get; set; }  
  public string? Name { get; set; } 
  public bool? IsDefault { get; set; }
}
