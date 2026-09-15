using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Commands.UpdateDriverExpense;

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
  public UpdateExpenseCommandValidator()
  {
    RuleFor(v => v.ExpenseId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(v => v.DriverId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(v => v.ExpenseDate).NotNull().NotEmpty();
    RuleFor(v => v.ExpenseCategoryId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.Amount).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.Details).NotNull().NotEmpty();
  }
}
