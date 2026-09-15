using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ReturnReportFeature.Query.GetOrderDetailByUplaodReturnReportFile;
public class GetOrderDetailByUplaodReturnReportFileValidator : AbstractValidator<GetOrderDetailByUplaodReturnReportFileQuery>
{
  public GetOrderDetailByUplaodReturnReportFileValidator()
  {
    RuleFor(e => e.CarrierId).NotNull().NotEmpty();
  }
}
