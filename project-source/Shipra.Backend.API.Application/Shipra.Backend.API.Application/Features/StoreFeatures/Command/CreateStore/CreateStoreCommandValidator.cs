using FluentValidation;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.CreateStore;
public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
  public CreateStoreCommandValidator()
  {
    RuleFor(v => v.StoreName).NotNull().NotEmpty(); 
    RuleFor(v => v.CustomerServiceNo).NotNull().NotEmpty();

  }
}
