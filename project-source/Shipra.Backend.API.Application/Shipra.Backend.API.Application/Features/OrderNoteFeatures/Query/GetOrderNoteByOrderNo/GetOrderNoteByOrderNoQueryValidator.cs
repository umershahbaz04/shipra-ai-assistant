using FluentValidation;
using Shipra.Backend.API.Application.Features.OrderNoteFeatures.Query.GetOrderNoteById;

namespace Shipra.Backend.API.Application.Features.OrderNoteFeatures.Query.GetOrderNoteByOrderNo;
public class GetOrderNoteByOrderNoQueryValidator : AbstractValidator<GetOrderNoteByOrderNoQuery>
{
  public GetOrderNoteByOrderNoQueryValidator()
  {
    RuleFor(v => v.OrderNo).NotNull().NotEmpty();
  }
}
