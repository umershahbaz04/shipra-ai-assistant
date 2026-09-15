using FluentValidation;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.DeleteStoreCommand;
public class DeleteStoreCommandValidator : AbstractValidator<DeleteStoreCommand>
{
  public DeleteStoreCommandValidator()
  {
    RuleFor(v => v.StoreId).NotNull().NotEmpty().GreaterThan(0);
  }
}
