using FluentValidation;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.RevertDeliveryTask;
public class RevertDeliveryTaskByOrderNosCommandValidator : AbstractValidator<RevertDeliveryTaskByOrderNosCommand>
{
  public RevertDeliveryTaskByOrderNosCommandValidator()
  {
    RuleFor(v => v.OrderNos).NotNull().NotEmpty();
  }
}
