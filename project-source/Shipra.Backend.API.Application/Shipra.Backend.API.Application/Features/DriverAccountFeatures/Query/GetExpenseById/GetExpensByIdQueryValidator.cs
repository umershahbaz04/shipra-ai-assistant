using FluentValidation;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetDriverExpenseById;

namespace Shipra.Backend.API.Application.Features.DriverAccountFeatures.Query.GetExpenseById;
public class GetExpensByIdQueryValidator : AbstractValidator<GetExpenseByIdQuery>
{
  public GetExpensByIdQueryValidator()
  {
    RuleFor(v => v.ExpenseId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
