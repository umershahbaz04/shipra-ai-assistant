using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.DeleteExpense;
public class DeleteExpenseCommand : IRequest<ServiceResultDTOWithTypeModel<BaseResponseDto>>
{
  public string? ExpenseId { get; set; }
}
