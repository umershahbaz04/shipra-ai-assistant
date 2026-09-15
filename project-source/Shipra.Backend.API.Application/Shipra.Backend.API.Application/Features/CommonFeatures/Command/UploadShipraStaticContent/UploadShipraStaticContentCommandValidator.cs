using FluentValidation;

namespace Shipra.Backend.API.Application.Features.CommonFeatures.Command.UploadShipraStaticContent;
public class UploadShipraStaticContentCommandValidator : AbstractValidator<UploadShipraStaticContentCommand>
{
  public UploadShipraStaticContentCommandValidator()
  {
    RuleFor(x => x.File).NotNull().NotEmpty();
  }
}
