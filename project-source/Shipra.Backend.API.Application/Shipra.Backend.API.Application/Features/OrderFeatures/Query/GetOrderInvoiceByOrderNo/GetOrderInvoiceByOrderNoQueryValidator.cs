using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.GetOrderInvoiceByOrderNo;
public class GetOrderInvoiceByOrderNoQueryValidator : AbstractValidator<GetOrderInvoiceByOrderNoQuery>
{
  public GetOrderInvoiceByOrderNoQueryValidator()
  {
    RuleFor(v => v.OrderNos).NotNull().NotEmpty();
  }
}
