using FluentValidation;
using Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Query.GetOrderTrackingHistoryByOrderId;

namespace Shipra.Backend.API.Application.Features.OrderTrackingHistoryFeatures.Query.GetOrderTrackingHistoryByOrderNo;
public class GetOrderTrackingHistoryByOrderNoQueryValidator : AbstractValidator<GetOrderTrackingHistoryByOrderNoQuery>
{
  public GetOrderTrackingHistoryByOrderNoQueryValidator()
  {
    RuleFor(v => v.OrderNo).NotNull().NotEmpty();
  }
}
