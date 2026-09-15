using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.DeleteExpense;
public class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
{
  public DeleteExpenseCommandValidator()
  {
    RuleFor(v => v.ExpenseId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
