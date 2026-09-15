using FluentValidation;

namespace Shipra.Backend.API.Application.Features.StoreFeatures.Command.UploadStoreImage;
public class UploadStoreImageCommandValidator : AbstractValidator<UploadStoreImageCommand>
{
  private const int MaxFileSizeInKb = 50;
  private const int MaxImageWidth = 711;
  private const int MaxImageHeight = 449;
  public UploadStoreImageCommandValidator()
  {
    RuleFor(x => x.File).NotNull().NotEmpty().Must(file => ImageValidationUtils.IsFileSizeValid(file!, MaxFileSizeInKb))
                .WithMessage($"File must be ≤ {MaxFileSizeInKb} KB.").
                Must(file => ImageValidationUtils.IsSupportedContentType(file!))
                .WithMessage("Only JPEG and PNG images are allowed.")
               .Must(file => ImageValidationUtils.HasValidDimensions(file!, MaxImageWidth, MaxImageHeight))
                .WithMessage($"Image dimensions must be ≤ {MaxImageWidth}x{MaxImageHeight} pixels.");
  }
}
