using FluentValidation;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Commands.CreateOrderPODFiles;
public class CreateOrderPODFilesCommandValidator : AbstractValidator<CreateOrderPODFilesCommand>
{
  public CreateOrderPODFilesCommandValidator()
  {
    RuleFor(v => v.FilesList).NotNull().NotEmpty().WithMessage("List must contain at least one file.");
    RuleFor(v => v.OrderId).NotNull().NotEmpty();
  }
}
