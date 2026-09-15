using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderForDriverById;
public class GetOrderForDriverByIdQueryValidator : AbstractValidator<GetOrderForDriverByIdQuery>
{
  public GetOrderForDriverByIdQueryValidator()
  {
    RuleFor(v => v.OrderId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
