using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetClientByKey;
public class GetClientByKeyQuery : IRequest<ServiceResultDTO>
{
  public string? TenantUsername { get; set; }
  public string? PublicKey { get; set; }
  public string? ClientId { get; set; }
}
