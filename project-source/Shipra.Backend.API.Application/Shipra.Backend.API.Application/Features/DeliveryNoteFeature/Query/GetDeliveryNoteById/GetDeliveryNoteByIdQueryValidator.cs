using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteById;
public class GetDeliveryNoteByIdQueryValidator : AbstractValidator<GetDeliveryNoteByIdQuery>
{
  public GetDeliveryNoteByIdQueryValidator()
  {
    RuleFor(v => v.DeliveryNoteId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
