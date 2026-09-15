using FluentValidation;

namespace Shipra.Backend.API.Application.Features.DeliveryNoteFeature.Query.GetDeliveryNoteDetailForDriverById;
public class GetDeliveryNoteDetailForDriverByIdQueryValidator : AbstractValidator<GetDeliveryNoteDetailForDriverByIdQuery>
{
  public GetDeliveryNoteDetailForDriverByIdQueryValidator()
  {
    RuleFor(v => v.DeliveryNoteId).NotNull().NotEmpty();
  }
}
