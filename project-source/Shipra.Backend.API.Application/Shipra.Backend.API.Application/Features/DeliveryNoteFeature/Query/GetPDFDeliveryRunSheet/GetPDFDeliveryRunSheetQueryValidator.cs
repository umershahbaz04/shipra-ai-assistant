using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetPDFRunSheet;
public class GetPDFDeliveryRunSheetQueryValidator : AbstractValidator<GetPDFDeliveryRunSheetQuery>
{
  public GetPDFDeliveryRunSheetQueryValidator()
  {
    RuleFor(v => v.DeliveryNoteId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
