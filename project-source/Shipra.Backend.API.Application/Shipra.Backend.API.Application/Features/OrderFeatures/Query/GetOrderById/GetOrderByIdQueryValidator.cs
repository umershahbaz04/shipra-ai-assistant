using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderById;
public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
  public GetOrderByIdQueryValidator()
  {
    RuleFor(v => v.OrderId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
