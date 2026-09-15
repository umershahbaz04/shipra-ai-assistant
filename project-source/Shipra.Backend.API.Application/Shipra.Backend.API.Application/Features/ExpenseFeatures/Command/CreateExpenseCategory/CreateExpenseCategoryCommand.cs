using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.ExpenseFeatures.Command.CreateExpenseCategory;
public class CreateExpenseCategoryCommand : IRequest<ServiceResultDTO>
{
  public string? ExpenseName { get; set; }
}
