using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetWayBill4X6ByOrderNo;
public class GetWayBill4X6ByOrderNosQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNos { get; set; }
}
