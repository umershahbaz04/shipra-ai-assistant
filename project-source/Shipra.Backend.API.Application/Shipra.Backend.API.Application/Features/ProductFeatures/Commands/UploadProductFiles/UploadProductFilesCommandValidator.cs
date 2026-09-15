using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UploadProductFiles;
public class UploadProductFilesCommandValidator : AbstractValidator<UploadProductFilesCommand>
{
  public UploadProductFilesCommandValidator()
  {
    RuleFor(x => x.Files).Must(x => x != null).WithMessage("Files must contain at least one file."); ;
  }
}
