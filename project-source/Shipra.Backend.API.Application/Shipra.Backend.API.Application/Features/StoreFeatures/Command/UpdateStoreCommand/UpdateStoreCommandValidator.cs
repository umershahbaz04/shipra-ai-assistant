using FluentValidation;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.UpdateStoreCommand;
public class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
{
  public UpdateStoreCommandValidator()
  {
    RuleFor(v => v.StoreImage).NotNull().NotEmpty();
    RuleFor(v => v.StoreName).NotNull().NotEmpty(); 
    RuleFor(v => v.CustomerServiceNo).NotNull().NotEmpty(); 
  }
}
