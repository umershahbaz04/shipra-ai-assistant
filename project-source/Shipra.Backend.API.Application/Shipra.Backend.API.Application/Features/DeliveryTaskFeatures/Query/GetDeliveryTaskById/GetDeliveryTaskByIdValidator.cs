using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Query.GetDeliveryTaskById;
public class GetDeliveryTaskByIdValidator : AbstractValidator<GetDeliveryTaskByIdQuery>
{
  public GetDeliveryTaskByIdValidator()
  {
    RuleFor(v => v.DeliveryTaskId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
