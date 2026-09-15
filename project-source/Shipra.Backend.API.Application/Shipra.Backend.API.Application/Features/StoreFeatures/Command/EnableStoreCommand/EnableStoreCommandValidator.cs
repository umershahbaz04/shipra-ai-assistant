using FluentValidation;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.EnableStoreCommand;
public class EnableStoreCommandValidator : AbstractValidator<EnableStoreCommand>
{
  public EnableStoreCommandValidator()
  {
    RuleFor(v => v.StoreId).NotNull().NotEmpty().GreaterThan(0);
  }
}
