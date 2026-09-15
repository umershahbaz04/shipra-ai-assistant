using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderLastTrackingStatus;
public class GetOrderLastTrackingStatusQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}
