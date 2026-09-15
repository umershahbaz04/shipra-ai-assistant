using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteDetailForDebrief;
public class GetDeliveryNoteDetailForDebriefQueryValidator : AbstractValidator<GetDeliveryNoteDetailForDebriefQuery>
{
  public GetDeliveryNoteDetailForDebriefQueryValidator()
  {
    RuleFor(v => v.DeliveryNoteId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
