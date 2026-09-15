using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.CreateProductStation;
public class CreateProductStationCommand : IRequest<ServiceResultDTO>
{
  public bool? IsDefault { get; set; }
  public string? Name { get; set; }
}
