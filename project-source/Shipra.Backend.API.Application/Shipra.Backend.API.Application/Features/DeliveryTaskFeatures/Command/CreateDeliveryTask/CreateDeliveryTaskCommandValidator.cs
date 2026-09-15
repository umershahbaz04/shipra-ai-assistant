using FluentValidation;

namespace Shipra.Backend.API.Application.Features.DeliveryTaskFeatures.Command.CreateDeliveryTask;
public class CreateDeliveryTaskCommandValidator : AbstractValidator<CreateDeliveryTaskCommand>
{
  public CreateDeliveryTaskCommandValidator()
  {
    RuleFor(v => v.OrderNos).NotNull().NotEmpty();
  }
}
