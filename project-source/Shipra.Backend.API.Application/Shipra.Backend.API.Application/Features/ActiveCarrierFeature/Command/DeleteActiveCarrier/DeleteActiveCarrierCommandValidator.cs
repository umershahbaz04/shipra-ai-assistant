using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ActiveCarrierFeature.Command.DeleteActiveCarrier;
public class DeleteActiveCarrierCommandValidator : AbstractValidator<DeleteActiveCarrierCommand>
{
  public DeleteActiveCarrierCommandValidator()
  {
    RuleFor(v => v.ActiveCarrierId).NotNull().NotEmpty().GreaterThan(0);
  }
}
