using FluentValidation;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.CreateExpense;

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
  public CreateExpenseCommandValidator()
  {
    RuleFor(v => v.ExpenseCategoryId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.ExpenseDate).NotNull().NotEmpty();
    RuleFor(v => v.Amount).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.Details).NotNull().NotEmpty();
    RuleFor(v => v.DriverId).NotNull().NotEmpty();
  }
}
