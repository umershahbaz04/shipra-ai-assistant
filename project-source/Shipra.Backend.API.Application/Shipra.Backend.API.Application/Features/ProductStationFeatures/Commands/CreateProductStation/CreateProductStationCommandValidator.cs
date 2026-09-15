using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ProductStationtFeatures.Commands.CreateProductStation;
public class CreateProductStationCommandValidator : AbstractValidator<CreateProductStationCommand>
{
  public CreateProductStationCommandValidator()
  {
    RuleFor(v => v.Name).NotNull().NotEmpty(); 
  }
}
