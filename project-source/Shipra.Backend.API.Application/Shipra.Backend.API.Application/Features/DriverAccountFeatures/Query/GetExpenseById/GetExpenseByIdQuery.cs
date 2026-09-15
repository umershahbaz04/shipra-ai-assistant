using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverExpenseById;
public class GetExpenseByIdQuery : IRequest<ServiceResultDTO>
{
  public string? ExpenseId { get; set; }
}
