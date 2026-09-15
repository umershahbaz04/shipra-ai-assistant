using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderItemByOrderNo;
public class GetOrderItemByOrderNoQueryValidator : AbstractValidator<GetOrderItemByOrderNoQuery>
{
  public GetOrderItemByOrderNoQueryValidator()
  {
    RuleFor(v => v.OrderNo).NotNull().NotEmpty();
  }
}
