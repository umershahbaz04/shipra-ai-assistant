using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Query.GetOrderNoteById;
public class GetOrderNoteByOrderNoQuery : IRequest<ServiceResultDTO>
{
  public string? OrderNo { get; set; }
}
