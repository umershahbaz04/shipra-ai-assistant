using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UploadProductFile;
public class UploadProductFileCommandValidator : AbstractValidator<UploadProductFileCommand>
{
  public UploadProductFileCommandValidator()
  {
    RuleFor(x => x.File).NotNull().NotEmpty();
  }
}
