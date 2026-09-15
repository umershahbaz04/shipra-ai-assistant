using FluentValidation;

namespace Shipra.Backend.API.Application.Features.DriverFeatures.Command.DeleteDriver;
public class DeleteDriverCommandValidator : AbstractValidator<DeleteDriverCommand>
{
  public DeleteDriverCommandValidator()
  {
    RuleFor(v => v.DriverId).NotEmpty().NotNull();
  }
}
