using FluentValidation;
using Shipra.Backend.API.Application.Common;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.DeleteDeliveryTask;
public class UnAssignDeliveryTaskByIdCommandValidator : AbstractValidator<UnAssignDeliveryTaskByIdCommand>
{
  public UnAssignDeliveryTaskByIdCommandValidator()
  {
    RuleFor(v => v.DeliveryTaskId).NotNull().NotEmpty().Must(GuidHelper.Validator).WithMessage(GuidHelper.GuidMessage);
  }
}
