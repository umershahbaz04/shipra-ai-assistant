using FluentValidation;

namespace Shipra.Backend.API.Application.Features.CarrierFeatures.Commands.UploadCarrierImage;
public class UploadCarrierImageCommandValidator : AbstractValidator<UploadCarrierImageCommand>
{
  public UploadCarrierImageCommandValidator()
  {
    RuleFor(x => x.File).NotNull().NotEmpty();
  }
}
