using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetOrderDetailForReturnReport;
public class GetOrderDetailForReturnReportValidator : AbstractValidator<GetOrderDetailForReturnReportQuery>
{
  public GetOrderDetailForReturnReportValidator()
  {
    RuleFor(e => e.OrderNo).NotNull().NotEmpty();
    RuleFor(e => e.TrackingNo).NotNull().NotEmpty();
    RuleFor(e => e.CarrierId).NotNull().NotEmpty();
  }
}
