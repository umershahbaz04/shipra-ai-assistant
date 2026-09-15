using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetAllOrdersByTenantId;
public class GetAllOrdersByTenantIdQuery : IRequest<ServiceResultDTO>
{
  public string? TenantId { get; set; }
  public DateTime? DateFrom { get; set; }
  public DateTime? DateTo { get; set; }
}
