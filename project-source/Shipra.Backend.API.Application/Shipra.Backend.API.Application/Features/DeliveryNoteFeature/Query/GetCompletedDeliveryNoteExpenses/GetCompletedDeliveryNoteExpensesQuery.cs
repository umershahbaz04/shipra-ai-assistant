using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetCompletedDeliveryNoteExpenses;
public class GetCompletedDeliveryNoteExpensesQuery : IRequest<ServiceResultDTO>
{
  public string? DeliveryNoteId { get; set; }
}
