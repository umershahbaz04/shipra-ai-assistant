using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetWayBill4X6ByOrderNo;
public class GetWayBill4X6ByOrderNoQueryValidator : AbstractValidator<GetWayBill4X6ByOrderNosQuery>
{
  public GetWayBill4X6ByOrderNoQueryValidator()
  {
    RuleFor(v => v.OrderNos).NotEmpty().NotNull();
  }
}
