using FluentValidation;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Commands.UploadClientImage;

public class UploadClientImageCommandValidator : AbstractValidator<UploadClientImageCommand>
{ 
  public UploadClientImageCommandValidator()
  {
    RuleFor(x => x.File).NotNull().NotEmpty(); 
  }

}
