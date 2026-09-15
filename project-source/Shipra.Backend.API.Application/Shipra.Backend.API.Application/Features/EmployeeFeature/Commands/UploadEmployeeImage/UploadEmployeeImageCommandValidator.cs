using FluentValidation;

namespace Shipra.Backend.API.Application.Features.EmployeeFeature.Commands.UploadEmployeeImage;
public class UploadEmployeeImageCommandValidator : AbstractValidator<UploadEmployeeImageCommand>
{
  public UploadEmployeeImageCommandValidator()
  {
    RuleFor(x => x.File).NotNull().NotEmpty();
  }
}
