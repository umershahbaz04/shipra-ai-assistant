using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ShipmentFeatures.Query.GetOrderInfoByOrderNoForPopUp;
public class GetOrderInfoByOrderNoForPopUpQueryValidator : AbstractValidator<GetOrderInfoByOrderNoForPopUpQuery>
{
  public GetOrderInfoByOrderNoForPopUpQueryValidator()
  {
    RuleFor(v => v.OrderNo).NotNull().NotEmpty();
  }
}
