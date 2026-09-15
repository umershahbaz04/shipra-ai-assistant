using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ExpenseFeatures.Command.CreateExpenseCategory;
public class CreateExpenseCategoryCommandValidator : AbstractValidator<CreateExpenseCategoryCommand>
{
  public CreateExpenseCategoryCommandValidator()
  {
    RuleFor(v => v.ExpenseName).NotNull().NotEmpty();
  }
}
