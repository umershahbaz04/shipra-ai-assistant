using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Command.CompleteDeliveryNote;
public class CompleteDeliveryNoteCommandValidator : AbstractValidator<CompleteDeliveryNoteCommand>
{
  public CompleteDeliveryNoteCommandValidator()
  {
    RuleFor(v => v.DeliveryNoteId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
    RuleFor(x => x.ExpenseList).Must(x => x != null).WithMessage("ExpenseItems list must contain at least one item.");
    RuleForEach(x => x.ExpenseList).SetValidator(x => new CreateExpenseItemValidator());
  }
}
public class CreateExpenseItemValidator : AbstractValidator<ExpenseModel>
{
  public CreateExpenseItemValidator()
  {
    RuleFor(v => v.ExpenseCategoryId).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.Amount).NotNull().NotEmpty().GreaterThan(0);
    RuleFor(v => v.ExpenseDate).NotNull().NotEmpty();
  }
}
